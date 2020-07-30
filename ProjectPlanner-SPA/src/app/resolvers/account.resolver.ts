import { Injectable } from '@angular/core';
import {ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../_services/auth.service';
import { UserService } from '../_services/user.service';
import { User } from '../_models/user';

@Injectable()

export class AccountResolver implements Resolve<User> {

    constructor(private userService: UserService, private authService: AuthService) { }

    resolve(): Observable<User> {
        return this.userService.getUser(this.authService.decodedToken.nameid);
    }
}