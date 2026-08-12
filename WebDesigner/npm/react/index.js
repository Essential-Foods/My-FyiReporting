'use strict';

const React = require('react');

const DEFAULT_SCRIPT =
  '/_content/Majorsilence.Reporting.WebDesigner/rdl-designer/designer.js';

// Script loading — idempotent, deduped across multiple component instances.
const _pending = new Map(); // src → Promise

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
    el.onerror = resolve; // don't block render on script error
    document.head.appendChild(el);
  });
  _pending.set(src, p);
}

/**
 * React wrapper around the <report-designer> Web Component.
 *
 * Usage:
 *   import { ReportDesigner } from '@majorsilence/report-designer-react';
 *
 *   const ref = React.useRef();
 *   <ReportDesigner
 *     ref={ref}
 *     previewEndpoint="/rdl-designer/preview"
 *     saveEndpoint="/rdl-designer/save"
 *     loadEndpoint="/rdl-designer/load"
 *     style={{ display: 'block', height: '700px' }}
 *   />
 *   // later: ref.current.getRdl()  /  ref.current.loadRdl(xml)
 *
 * scriptSrc defaults to the ASP.NET Core static-files path emitted by the
 * Majorsilence.Reporting.WebDesigner NuGet package.  Override it if you serve
 * designer.js from a different location.
 */
const ReportDesigner = React.forwardRef(function ReportDesigner(props, ref) {
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

  const elRef = React.useRef(null);
  const resolvedSrc = scriptSrc !== undefined ? scriptSrc : DEFAULT_SCRIPT;

  // Expose getRdl / loadRdl via the forwarded ref.
  React.useImperativeHandle(ref, () => ({
    getRdl() {
      return elRef.current ? elRef.current.getRdl() : '';
    },
    loadRdl(xml) {
      if (elRef.current) elRef.current.loadRdl(xml);
    },
  }), []);

  // Load designer.js on first mount.
  React.useEffect(() => {
    _ensureScript(resolvedSrc);
  }, [resolvedSrc]);

  // Apply initialRdl after the custom element upgrades.
  React.useEffect(() => {
    if (!initialRdl) return;
    if (typeof customElements === 'undefined') return;
    customElements.whenDefined('report-designer').then(() => {
      if (elRef.current) elRef.current.loadRdl(initialRdl);
    });
  }, [initialRdl]);

  // React 16-18: pass 'class' (not className) and kebab-case attributes for
  // custom elements.  React 19 handles this automatically.
  const elementProps = Object.assign({}, rest, {
    ref: elRef,
    'preview-endpoint': previewEndpoint,
    'save-endpoint': saveEndpoint != null ? saveEndpoint : '',
    'load-endpoint': loadEndpoint != null ? loadEndpoint : '',
    style: style != null ? style : { display: 'block', height: '700px' },
  });
  if (className) elementProps['class'] = className;

  return React.createElement('report-designer', elementProps);
});

ReportDesigner.displayName = 'ReportDesigner';

module.exports = { ReportDesigner };
