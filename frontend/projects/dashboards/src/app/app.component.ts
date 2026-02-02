import { Component, ViewEncapsulation, OnInit, OnChanges, Input, inject } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboards',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container">
      <div class="iframe-wrapper" *ngIf="safeUrl; else loading">
        <iframe
          [src]="safeUrl"
          width="100%"
          height="600"
          frameborder="0"
          allowtransparency>
        </iframe>
      </div>
      <ng-template #loading>
        <p>Настройка дашборда...</p>
      </ng-template>
    </div>
  `,
  styles: [`
    .container { font-family: sans-serif; }
    .iframe-wrapper { border: 1px solid #ddd; border-radius: 8px; overflow: hidden; background: #fff; }
    iframe { display: block; }
  `],
  encapsulation: ViewEncapsulation.None
})
export class AppComponent implements OnInit, OnChanges {
  @Input('dashboard-id') dashboardId: string = '';
  @Input('metabase-url') metabaseUrl: string = '';

  private sanitizer = inject(DomSanitizer);
  safeUrl?: SafeResourceUrl;

  ngOnInit() {
    this.updateUrl();
  }

  ngOnChanges(changes: any) {
    this.updateUrl();
  }

  private updateUrl() {
    const baseUrl = this.metabaseUrl || process.env['METABASE_URL'];
    const publicId = this.dashboardId || process.env['METABASE_DASHBOARD_ID'];

    if (baseUrl && publicId) {
      const rawUrl = `${baseUrl}/public/dashboard/${publicId}`;
      this.safeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(rawUrl);
    }
  }
}
