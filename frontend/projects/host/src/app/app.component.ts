import { Component, OnInit, CUSTOM_ELEMENTS_SCHEMA, ViewEncapsulation, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TuiRoot, TuiButton } from '@taiga-ui/core';
import { AppService } from './app.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, TuiRoot],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AppComponent implements OnInit {
  private readonly appService = inject(AppService);

  public get isAdmin(): boolean {
    return this.appService.isAdmin;
  }

  public get adminUrl(): string {
    return this.appService.metabaseAdminUrl;
  }

  public get metabaseUrl(): string {
    return this.appService.metabaseUrl;
  }

  public ngOnInit(): void {
    const url = this.appService.mftUrl;

    if (url) {
      this.loadRemoteMft(url);
    } else {
      console.error('MFT_URL не определен в окружении');
    }
  }

  private loadRemoteMft(url: string): void {
    const script = document.createElement('script');
    script.src = `${url}/main.js`;
    script.type = 'module';

    script.onerror = () => {
      console.error(`Ошибка загрузки микрофронтенда по адресу: ${script.src}`);
    };

    document.head.appendChild(script);
  }
}
