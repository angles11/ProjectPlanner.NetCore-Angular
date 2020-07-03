import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  baseUrl: any = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getFriends(userId: string) {
    return this.http.get(this.baseUrl + 'user/' + userId + '/friends');
  }

  getUsers(userId: string, pageIndex?, itemsPerPage?, searchTerm?, userParams?): Observable<PaginatedResult<User[]>>{

    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();

    let params = new HttpParams();

    if (pageIndex != null && itemsPerPage != null) {
      params = params.append('pageIndex', pageIndex);
      params = params.append('pageSize', itemsPerPage);
    }

    if (searchTerm != null) {
      params = params.append('searchTerm', searchTerm);
    }

    if (userParams != null) {
      params = params.append('orderBy', userParams.orderBy);
      params = params.append('gender', userParams.gender);
    }
    return this.http.get<User[]>(this.baseUrl + 'user/' + userId + '/users', { observe: 'response', params })
      .pipe(
        map(response => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }
          return paginatedResult;
        })
      );
  }

  addFriend(userId: string, recipientId: string) {
    return this.http.post(this.baseUrl + 'user/' + userId + '/friends/' + recipientId, {});
  }

  deleteFriend(userId: string, userId2: string) {
    return this.http.delete(this.baseUrl + 'user/' + userId + '/friends/' + userId2);
  }
}
