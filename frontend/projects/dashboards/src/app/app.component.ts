import {
  Component,
  Input,
  OnInit,
  inject,
  ChangeDetectorRef,
  ViewEncapsulation
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { TuiTabs } from '@taiga-ui/kit';
import { AppService, Dashboard } from './app.service';

declare const METABASE_URL: string;
declare const METABASE_USER: string;
declare const METABASE_PASS: string;

@Component({
  selector: 'app-dashboards-root',
  standalone: true,
  imports: [CommonModule, TuiTabs],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AppComponent implements OnInit {
  @Input('metabase-url') metabaseUrl: string = '';

  private readonly appService = inject(AppService);
  private readonly sanitizer = inject(DomSanitizer);
  private readonly cdr = inject(ChangeDetectorRef);

  dashboards: Dashboard[] = [];
  activeIndex = 0;
  safeUrl?: SafeResourceUrl;
  isLoading = true;

  ngOnInit(): void {
    void this.initialize();
  }

  public onTabClick(index: number): void {
    this.activeIndex = index;
    this.updateIframe();
  }

  private async initialize(): Promise<void> {
    const rawBaseUrl = this.metabaseUrl || (typeof METABASE_URL !== 'undefined' ? METABASE_URL : '');
    const baseUrl = this.cleanUrl(rawBaseUrl);

    const auth = {
      username: (typeof METABASE_USER !== 'undefined' ? METABASE_USER : '').replace(/"/g, ''),
      password: (typeof METABASE_PASS !== 'undefined' ? METABASE_PASS : '').replace(/"/g, '')
    };

    if (!baseUrl) {
      console.error('Критическая ошибка: METABASE_URL не определен в окружении');
      this.isLoading = false;
      return;
    }

    try {
      this.dashboards = await this.appService.fetchDashboards(baseUrl, auth);
      if (this.dashboards.length > 0) {
        this.updateIframe();
      }
    } catch (e) {
      console.error('Ошибка загрузки дашбордов:', e);
    } finally {
      this.isLoading = false;
      this.cdr.detectChanges();
    }
  }

  private updateIframe(): void {
    const active = this.dashboards[this.activeIndex];
    const rawBaseUrl = this.metabaseUrl || (typeof METABASE_URL !== 'undefined' ? METABASE_URL : '');
    const baseUrl = this.cleanUrl(rawBaseUrl);

    if (active?.mftid && baseUrl) {
      const url = `${baseUrl}/public/dashboard/${active.mftid}`;
      this.safeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);
      this.cdr.detectChanges();
    }
  }

  private cleanUrl(url: any): string {
    if (!url) return '';
    return String(url).replace(/"/g, '').replace(/\/$/, '');
  }
}
