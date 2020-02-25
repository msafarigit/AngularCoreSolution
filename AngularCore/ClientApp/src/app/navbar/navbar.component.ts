import { Component, OnInit, Inject } from '@angular/core';
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

  constructor(@Inject(APP_CONFIG) private config: AppConfig) { }

  ngOnInit() {
    this.title = this.config.title;
    this.version = this.config.version;
  }

  signout(): void {
    this.navbarCollapsed = true;
    // '~/Authentication/LogOut'
  }
}
