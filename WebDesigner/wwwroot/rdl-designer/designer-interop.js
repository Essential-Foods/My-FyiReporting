// designer-interop.js — Blazor JS interop bridge for <report-designer>
// Loaded lazily via IJSRuntime.InvokeAsync("import", "...").
// All exports accept an ElementReference (Blazor marshals it as a DOM element).

let _ensurePromise = null;

/** Ensure the designer Web Component script is loaded and registered. */
async function ensureDesigner() {
  if (_ensurePromise) return _ensurePromise;

  if (customElements.get('report-designer')) {
    _ensurePromise = Promise.resolve();
    return _ensurePromise;
  }

  _ensurePromise = new Promise((resolve, reject) => {
    const existing = document.querySelector(
      'script[src*="rdl-designer/designer.js"]');
    if (existing) {
      // Script tag exists but custom element may not be defined yet.
      customElements.whenDefined('report-designer').then(resolve, reject);
      return;
    }

    const script = document.createElement('script');
    script.type  = 'module';
    script.src   = '/_content/Majorsilence.Reporting.WebDesigner/rdl-designer/designer.js';
    script.onload  = () => customElements.whenDefined('report-designer').then(resolve, reject);
    script.onerror = reject;
    document.head.appendChild(script);
  });

  return _ensurePromise;
}

/**
 * Load RDL XML into the designer element.
 * @param {Element} el - ElementReference from Blazor
 * @param {string} rdlXml
 */
export async function loadRdl(el, rdlXml) {
  await ensureDesigner();
  el?.loadRdl(rdlXml);
}

/**
 * Retrieve the current RDL XML from the designer element.
 * @param {Element} el - ElementReference from Blazor
 * @returns {string}
 */
export async function getRdl(el) {
  await ensureDesigner();
  return el?.getRdl() ?? '';
}
