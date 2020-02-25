import { Component, OnInit } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { products } from '@model/product';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.scss']
})
export class ProductComponent implements OnInit {
  public gridData: any[] = products;
  
  constructor(private cookieService: CookieService) { }

  ngOnInit() {
  }

}
