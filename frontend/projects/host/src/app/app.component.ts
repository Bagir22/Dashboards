import { Component, OnInit, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  async ngOnInit() {
    try {
      await import('dashboards-mft/web-components');
    } catch (err) {
      console.error(err);
    }
  }
}
