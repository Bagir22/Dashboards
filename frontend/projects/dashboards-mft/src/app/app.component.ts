import { Component, ViewEncapsulation, OnInit, inject } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboards-mft',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="mft-container">
      <h2 style="color: #ff4081;">Metabase Statistics</h2>

      <div class="iframe-wrapper" *ngIf="safeUrl; else loading">
        <iframe
          [src]="safeUrl"
          width="100%"
          height="500"
          allowtransparency>
        </iframe>
      </div>

      <ng-template #loading>
      </ng-template>
    </div>
  `,
  styles: [`
    .mft-container { font-family: sans-serif; padding: 10px; }
    .iframe-wrapper { border: 2px solid #eee; border-radius: 8px; overflow: hidden; }
  `],
  encapsulation: ViewEncapsulation.None
})
export class AppComponent implements OnInit {
  private sanitizer = inject(DomSanitizer);
  safeUrl?: SafeResourceUrl;

  ngOnInit() {
    const baseUrl = process.env['METABASE_URL'];
    const publicId = process.env['METABASE_DASHBOARD_ID'];
    const rawUrl = `${baseUrl}/public/dashboard/${publicId}`;

    this.safeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(rawUrl);
  }

}
