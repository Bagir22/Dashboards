import { Component, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TuiRoot, TuiButton } from '@taiga-ui/core';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, TuiRoot, TuiButton],
  schemas: [CUSTOM_ELEMENTS_SCHEMA], // Для Web Component
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  metabaseUrl = String(process.env['METABASE_URL'] || '').replace(/"/g, '').replace(/\/$/, '');
  mftUrl = String(process.env['MFT_URL'] || '').replace(/"/g, '').replace(/\/$/, '');

  get metabaseAdminUrl(): string {
    return `${this.metabaseUrl}/admin/`;
  }
}