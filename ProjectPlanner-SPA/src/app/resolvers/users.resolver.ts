import { Resolve } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { UserService } from '../_services/user.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { User } from '../_models/user';
import { Injectable } from '@angular/core';

@Injectable()
export class UsersResolver implements Resolve<User[]> {
    pageIndex = 0;
    pageSize = 10;
    userParams = {
        orderBy: 'nameAscending',
        gender: 'all'
    };
    constructor(private userService: UserService, private authService: AuthService){}

    resolve(): Observable<User[]>{
        return this.userService.getUsers(this.authService.decodedToken.nameid, this.pageIndex, this.pageSize, null, this.userParams).pipe(
            catchError((error) => {
                return of(null);
            })
        );
    }
}
