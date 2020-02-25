import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RTL } from '@progress/kendo-angular-l10n';
import { GridModule } from '@progress/kendo-angular-grid';

import { BaseInfoRoutingModule } from './base-info-routing.module';
import { ProductComponent } from './product/product.component';

@NgModule({
  declarations: [
    ProductComponent
  ],
  imports: [
    CommonModule,
    GridModule,
    BaseInfoRoutingModule
  ],
  providers: [{ provide: RTL, useValue: true }]
})
export class BaseInfoModule { }
