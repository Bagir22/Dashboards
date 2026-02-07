import { Component, OnInit, CUSTOM_ELEMENTS_SCHEMA, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TuiRoot, TuiButton } from '@taiga-ui/core';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, TuiRoot, TuiButton],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AppComponent implements OnInit {
  metabaseUrl = String(process.env['METABASE_URL'] || '').replace(/"/g, '').replace(/\/$/, '');
  mftUrl = String(process.env['MFT_URL'] || '').replace(/"/g, '').replace(/\/$/, '');
  
  get metabaseAdminUrl(): string {
    return `${this.metabaseUrl}/admin/`;
  }

  ngOnInit() {
    if (this.mftUrl) {
      this.loadRemoteMft();
    } else {
      console.error('MFT_URL не определен в окружении');
    }
  }

  private loadRemoteMft() {
    const script = document.createElement('script');
    script.src = `${this.mftUrl}/main.js`;
    script.type = 'module';

    script.onerror = () => {
      console.error(`Ошибка загрузки микрофронтенда по адресу: ${script.src}`);
    };

    document.head.appendChild(script);
  }
}
