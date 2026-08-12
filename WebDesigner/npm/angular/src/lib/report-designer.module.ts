import { NgModule } from '@angular/core';
import { RdlDesignerComponent } from './report-designer.component';

/**
 * Import this module to use <rdl-designer> in your Angular application.
 *
 * ```ts
 * import { RdlDesignerModule } from '@majorsilence/report-designer-angular';
 *
 * @NgModule({ imports: [RdlDesignerModule] })
 * export class AppModule {}
 * ```
 */
@NgModule({
  imports: [RdlDesignerComponent],
  exports: [RdlDesignerComponent],
})
export class RdlDesignerModule {}
