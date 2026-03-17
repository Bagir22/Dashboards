import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AppService {
  
  public get isAdmin(): boolean {
    return localStorage.getItem('isAdmin') === 'true';
  }

  public getBaseUrl(inputUrl: string, globalVar: any): string {
    const raw = inputUrl || (typeof globalVar !== 'undefined' ? globalVar : '');
    return String(raw).replace(/"/g, '').replace(/\/$/, '');
  }

  public getAdminUrl(baseUrl: string): string {
    return `${baseUrl}/admin/`;
  }
}
