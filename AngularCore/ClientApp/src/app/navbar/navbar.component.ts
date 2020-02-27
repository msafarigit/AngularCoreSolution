import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';

import { APP_CONFIG, AppConfig } from 'app/app.config';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  host: {
    '[class]': '"w-100"'
  },
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {

  navbarCollapsed = true;
  title: string;
  version: string;

  constructor(private http: HttpClient, @Inject(APP_CONFIG) private config: AppConfig) { }

  ngOnInit() {
    this.title = this.config.title;
    this.version = this.config.version;
  }

  async signout(): Promise<void> {
    try {
      this.navbarCollapsed = true;
      const url = `${this.config.apiEndpoint}Authenticate/LogOut`;
      const promise = await this.http.get<HttpResponse<{}>>(url).toPromise();
    } catch (e) {
      console.log('error', e);
    }
  }
}
