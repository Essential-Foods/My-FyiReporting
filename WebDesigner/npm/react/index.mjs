import {
  forwardRef,
  useRef,
  useImperativeHandle,
  useEffect,
  createElement,
} from 'react';

const DEFAULT_SCRIPT =
  '/_content/Majorsilence.Reporting.WebDesigner/rdl-designer/designer.js';

const _pending = new Map();

function _ensureScript(src) {
  if (!src || typeof document === 'undefined') return;
  if (_pending.has(src)) return;
  const existing = document.querySelector(`script[data-rdl-designer="${src}"]`);
  if (existing) { _pending.set(src, Promise.resolve()); return; }
  const p = new Promise((resolve) => {
    const el = document.createElement('script');
    el.type = 'module';
    el.src = src;
    el.dataset.rdlDesigner = src;
    el.onload = resolve;
    el.onerror = resolve;
    document.head.appendChild(el);
  });
  _pending.set(src, p);
}

export const ReportDesigner = forwardRef(function ReportDesigner(props, ref) {
  const {
    previewEndpoint,
    saveEndpoint,
    loadEndpoint,
    initialRdl,
    scriptSrc,
    style,
    className,
    ...rest
  } = props;

  const elRef = useRef(null);
  const resolvedSrc = scriptSrc !== undefined ? scriptSrc : DEFAULT_SCRIPT;

  useImperativeHandle(ref, () => ({
    getRdl() {
      return elRef.current ? elRef.current.getRdl() : '';
    },
    loadRdl(xml) {
      if (elRef.current) elRef.current.loadRdl(xml);
    },
  }), []);

  useEffect(() => {
    _ensureScript(resolvedSrc);
  }, [resolvedSrc]);

  useEffect(() => {
    if (!initialRdl) return;
    if (typeof customElements === 'undefined') return;
    customElements.whenDefined('report-designer').then(() => {
      if (elRef.current) elRef.current.loadRdl(initialRdl);
    });
  }, [initialRdl]);

  const elementProps = Object.assign({}, rest, {
    ref: elRef,
    'preview-endpoint': previewEndpoint,
    'save-endpoint': saveEndpoint != null ? saveEndpoint : '',
    'load-endpoint': loadEndpoint != null ? loadEndpoint : '',
    style: style != null ? style : { display: 'block', height: '700px' },
  });
  if (className) elementProps['class'] = className;

  return createElement('report-designer', elementProps);
});

ReportDesigner.displayName = 'ReportDesigner';
