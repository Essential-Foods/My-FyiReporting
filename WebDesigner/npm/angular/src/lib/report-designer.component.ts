import {
  Component,
  ElementRef,
  Input,
  NgZone,
  OnChanges,
  OnInit,
  SimpleChanges,
  ViewChild,
  CUSTOM_ELEMENTS_SCHEMA,
} from '@angular/core';

const DEFAULT_SCRIPT =
  '/_content/Majorsilence.Reporting.WebDesigner/rdl-designer/designer.js';

// Module-level dedup: only inject the script tag once per page.
const _loaded = new Set<string>();

function ensureScript(src: string): void {
  if (!src || typeof document === 'undefined') return;
  if (_loaded.has(src)) return;
  if (document.querySelector(`script[data-rdl-designer="${src}"]`)) {
    _loaded.add(src);
    return;
  }
  const el = document.createElement('script');
  el.type = 'module';
  el.src = src;
  el.dataset['rdlDesigner'] = src;
  document.head.appendChild(el);
  _loaded.add(src);
}

/** Minimal type for the <report-designer> DOM element methods. */
interface ReportDesignerElement extends HTMLElement {
  getRdl(): string;
  loadRdl(xml: string): void;
}

/**
 * Angular standalone wrapper around the `<report-designer>` Web Component.
 *
 * Usage (Angular 14+):
 * ```ts
 * // app.module.ts  (or standalone component imports)
 * import { RdlDesignerModule } from '@majorsilence/report-designer-angular';
 * @NgModule({ imports: [RdlDesignerModule], ... })
 * export class AppModule {}
 * ```
 *
 * Template:
 * ```html
 * <rdl-designer #designer
 *   previewEndpoint="/rdl-designer/preview"
 *   saveEndpoint="/rdl-designer/save"
 *   loadEndpoint="/rdl-designer/load"
 *   style="display:block;height:700px">
 * </rdl-designer>
 * ```
 *
 * Component class:
 * ```ts
 * @ViewChild('designer') designer!: RdlDesignerComponent;
 *
 * getXml() { return this.designer.getRdl(); }
 * ```
 */
@Component({
  selector: 'rdl-designer',
  standalone: true,
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  template: `<report-designer #el
    [attr.preview-endpoint]="previewEndpoint"
    [attr.save-endpoint]="saveEndpoint ?? ''"
    [attr.load-endpoint]="loadEndpoint ?? ''"
    style="display:block;width:100%;height:100%">
  </report-designer>`,
  styles: [':host { display: block; }'],
})
export class RdlDesignerComponent implements OnInit, OnChanges {
  /** POST endpoint that renders RDL to an HTML preview page. */
  @Input() previewEndpoint = '/rdl-designer/preview';

  /** POST endpoint that saves the named RDL file. */
  @Input() saveEndpoint?: string;

  /** GET endpoint that loads a named RDL file. */
  @Input() loadEndpoint?: string;

  /**
   * Optional RDL XML to load on first render.
   * Subsequent changes call `loadRdl()` automatically.
   */
  @Input() initialRdl?: string;

  /**
   * URL of designer.js.  Defaults to the ASP.NET Core static-files path.
   * Set to empty string to skip automatic script loading.
   */
  @Input() scriptSrc = DEFAULT_SCRIPT;

  @ViewChild('el') private _el!: ElementRef<ReportDesignerElement>;

  constructor(private _zone: NgZone) {}

  ngOnInit(): void {
    if (this.scriptSrc) ensureScript(this.scriptSrc);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['initialRdl']?.currentValue) {
      this._whenDefined(() => this.loadRdl(changes['initialRdl'].currentValue));
    }
  }

  /** Returns the current report as RDL XML. */
  getRdl(): string {
    return this._el?.nativeElement?.getRdl() ?? '';
  }

  /** Replaces the current report with the supplied RDL XML. */
  loadRdl(xml: string): void {
    this._el?.nativeElement?.loadRdl(xml);
  }

  private _whenDefined(fn: () => void): void {
    if (typeof customElements === 'undefined') return;
    customElements.whenDefined('report-designer').then(() => {
      this._zone.run(fn);
    });
  }
}
