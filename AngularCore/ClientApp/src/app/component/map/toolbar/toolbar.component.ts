import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-maptoolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.scss']
})
export class ToolbarComponent implements OnInit {

  public clickMessage: string;

  public splitButtonData: Array<any> = [
    {
      text: 'Keep Text Only',
      icon: 'paste-plain-text',
      click: () => this.onItemClick('Keep Text Only splitbutton button clicked')
    },
    {
      text: 'Paste as HTML',
      icon: 'paste-as-html',
      click: () => this.onItemClick('Paste as HTML splitbutton button clicked')
    },
    {
      text: 'Paste Markdown',
      icon: 'paste-markdown',
      click: () => this.onItemClick('Paste Markdown splitbutton button clicked')
    }
  ];

  public dropdownButtonData: Array<any> = [
    {
      text: 'Keep Text Only',
      icon: 'paste-plain-text',
      click: () => this.onItemClick('Keep Text Only dropdownbutton button clicked')
    },
    {
      text: 'Paste as HTML',
      icon: 'paste-as-html',
      click: () => this.onItemClick('Paste as HTML dropdownbutton button clicked')
    },
    {
      text: 'Paste Markdown',
      icon: 'paste-markdown',
      click: () => this.onItemClick('Paste Markdown dropdownbutton button clicked')
    }
  ];

  constructor() { }

  ngOnInit() {
  }

  public onItemClick(message: string): void {
    console.log(message);
    this.clickMessage = message;
  }

}
