import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ProductComponent } from './product/product.component';

const routes: Routes = [
  { path: '', redirectTo: 'productSearch', pathMatch: 'full' },
  { path: 'productSearch', component: ProductComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BaseInfoRoutingModule { }
