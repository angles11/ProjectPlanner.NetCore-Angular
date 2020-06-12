import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  baseUrl: any = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getFriends(userId: string) {
    return this.http.get(this.baseUrl + 'user/' + userId + '/friends');
  }

  getUsers(userId: string, searchTerm?: string): Observable<User[]> {
    const options = searchTerm
      ? {
          params: new HttpParams().set('searchTerm', searchTerm),
        }
      : {};

    return this.http.get<User[]>(this.baseUrl + 'user/' + userId + '/users', options);
  }
}
