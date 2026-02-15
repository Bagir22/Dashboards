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
import { AppService, Dashboard, MetabaseAuth } from './app.service';

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
  @Input('metabase-url') public metabaseUrl: string = '';

  private readonly appService = inject(AppService);
  private readonly sanitizer = inject(DomSanitizer);
  private readonly cdr = inject(ChangeDetectorRef);

  public dashboards: Dashboard[] = [];
  public activeIndex: number = 0;
  public safeUrl?: SafeResourceUrl;
  public isLoading: boolean = true;

  public ngOnInit(): void {
    void this.initialize();
  }

  public onTabClick(index: number): void {
    this.activeIndex = index;
    this.updateIframe();
  }

  private async initialize(): Promise<void> {
    const rawBaseUrl = this.metabaseUrl || (typeof METABASE_URL !== 'undefined' ? METABASE_URL : '');
    const baseUrl = this.appService.cleanUrl(rawBaseUrl);

    if (!baseUrl) {
      console.error('METABASE_URL не определен');
      this.isLoading = false;
      return;
    }

    const auth: MetabaseAuth = {
      username: (typeof METABASE_USER !== 'undefined' ? METABASE_USER : '').replace(/"/g, ''),
      password: (typeof METABASE_PASS !== 'undefined' ? METABASE_PASS : '').replace(/"/g, '')
    };

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
    const baseUrl = this.appService.cleanUrl(rawBaseUrl);

    if (active?.mftid && baseUrl) {
      const url = `${baseUrl}/public/dashboard/${active.mftid}`;
      this.safeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);
      this.cdr.detectChanges();
    }
  }
}
