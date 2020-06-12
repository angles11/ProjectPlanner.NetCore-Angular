import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';
import { UserService } from '../_services/user.service';
import { User } from '../_models/user';

@Injectable()
export class UserListResolver implements Resolve<any> {
  constructor(private userService: UserService, private authService: AuthService) {}

  resolve(): Observable<[User[]]> {
    return this.userService.getUsers(this.authService.decodedToken.nameid).pipe(
      catchError((error) => {
        return of(null);
      })
    );
  }
}
