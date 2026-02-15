
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AppService {
  public readonly metabaseUrl: string = String(process.env['METABASE_URL'] || '')
    .replace(/"/g, '')
    .replace(/\/$/, '');

  public readonly mftUrl: string = String(process.env['MFT_URL'] || '')
    .replace(/"/g, '')
    .replace(/\/$/, '');

  public get isAdmin(): boolean {
    return localStorage.getItem('isAdmin') === 'true';
  }

  public get metabaseAdminUrl(): string {
    return `${this.metabaseUrl}/admin/`;
  }
}
