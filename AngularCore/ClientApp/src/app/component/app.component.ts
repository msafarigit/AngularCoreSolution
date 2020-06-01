import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { APP_CONFIG, AppConfig } from 'app/app.config';
import { SignalRService } from '@service/signal-r.service';

import { loadScript, setDefaultOptions } from 'esri-loader';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  host: {
    '[class]': '"row h-100"'
  },
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'app';

  constructor(@Inject(APP_CONFIG) private config: AppConfig, public signalRService: SignalRService, private http: HttpClient) {
    setDefaultOptions({ url: this.config.arcBaseUrl });

    loadScript({
      dojoConfig: {
        baseUrl: `${this.config.arcBaseUrl}/dojo`,
        async: true
      }
    });
  }

  ngOnInit() {
    this.signalRService.startConnection();
    this.signalRService.addTransferChartDataListener();
    this.startHttpRequest();
  }

  private startHttpRequest = () => {
    this.http.get('http://localhost:21904/api/chart').subscribe(res => {
        console.log(res);
      }, error => console.error(error));
  }
}
