import * as React from 'react';

/** Public API exposed via a forwarded ref. */
export interface ReportDesignerHandle {
  /** Returns the current report as RDL XML. */
  getRdl(): string;
  /** Replaces the current report with the supplied RDL XML. */
  loadRdl(xml: string): void;
}

export interface ReportDesignerProps
  extends React.HTMLAttributes<HTMLElement> {
  /**
   * POST endpoint that renders RDL XML to an HTML preview page.
   * @example "/rdl-designer/preview"
   */
  previewEndpoint: string;

  /**
   * POST endpoint that saves the named RDL file.
   * Required for the Save… toolbar button to work.
   * @example "/rdl-designer/save"
   */
  saveEndpoint?: string;

  /**
   * GET endpoint that loads a named RDL file.
   * Required for the Load… toolbar button to work.
   * @example "/rdl-designer/load"
   */
  loadEndpoint?: string;

  /**
   * Optional RDL XML to load when the component first mounts.
   * Changes to this prop after mount are applied via `loadRdl`.
   */
  initialRdl?: string;

  /**
   * URL of the designer.js Web Component script to load.
   * Defaults to the ASP.NET Core static-files path:
   * `/_content/Majorsilence.Reporting.WebDesigner/rdl-designer/designer.js`
   *
   * Set to `null` if you have already loaded the script elsewhere.
   */
  scriptSrc?: string | null;
}

/**
 * React wrapper around the `<report-designer>` Web Component.
 *
 * @example
 * ```tsx
 * const ref = React.useRef<ReportDesignerHandle>(null);
 *
 * <ReportDesigner
 *   ref={ref}
 *   previewEndpoint="/rdl-designer/preview"
 *   saveEndpoint="/rdl-designer/save"
 *   loadEndpoint="/rdl-designer/load"
 *   style={{ display: 'block', height: '700px' }}
 * />
 *
 * // later
 * const xml = ref.current?.getRdl();
 * ref.current?.loadRdl(xml);
 * ```
 */
export declare const ReportDesigner: React.ForwardRefExoticComponent<
  ReportDesignerProps & React.RefAttributes<ReportDesignerHandle>
>;
