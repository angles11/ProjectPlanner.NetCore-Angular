import { Friend } from '../_models/friend';
import { Resolve } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { UserService } from '../_services/user.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Injectable } from '@angular/core';

@Injectable()
export class FriendsResolver implements Resolve<Friend[]> {
    constructor(private userService: UserService, private authService: AuthService){}

    resolve(): Observable<Friend[]>{
        return this.userService.getFriends(this.authService.decodedToken.nameid).pipe(
            catchError((error) => {
                return of(null);
            })
        );
    }
}
