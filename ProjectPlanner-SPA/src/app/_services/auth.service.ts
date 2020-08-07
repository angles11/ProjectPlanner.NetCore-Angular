import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiUrl + 'auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;

constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(user.user));
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.currentUser = user.user;
        }
      })
    );
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'register', model);
  }

  confirmEmail(email: string, token: string) {
    return this.http.post(this.baseUrl + 'confirm'  , {token, email});
  }

  forgotPassword(email: string) {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    const json = JSON.stringify(email);
    return this.http.post(this.baseUrl + 'forgotPassword', json, {headers});
  }

  resetPassword(email: string, token: string, password: string) {
    return this.http.patch(this.baseUrl + 'resetPassword', { token, email, password });
  }
}
