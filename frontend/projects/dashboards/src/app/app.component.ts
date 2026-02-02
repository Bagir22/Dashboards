import { Component, ViewEncapsulation, OnInit, OnChanges, Input, inject } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboards',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
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

  ngOnChanges() {
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
