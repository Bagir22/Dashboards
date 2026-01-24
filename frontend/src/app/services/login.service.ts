import { Injectable } from '@angular/core';
import { LoginDTO } from '../models/login.dto';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})

export class LoginService {
  login(data: LoginDTO): Observable<{ token: string }> {
    console.log('Logging in with:', data);

    return of({ token: 'token' });
  }

  logout(): void {
    console.log('Logout');
  }
}
