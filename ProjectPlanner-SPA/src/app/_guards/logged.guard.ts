import { Injectable } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { Router, CanActivate } from '@angular/router';


@Injectable({
    providedIn: 'root',
})

export class LoggedGuard implements CanActivate {
    constructor(private authService: AuthService, private router: Router) { }

    canActivate(): boolean {
        if (this.authService.loggedIn()) {
            this.router.navigate(['/projects']);
            return false;
        }

        return true;
    }
}
