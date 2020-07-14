
import { Resolve } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { ProjectService } from '../_services/project.service';

@Injectable()
export class ProjectsResolver implements Resolve<any> {
    constructor(private projectsService: ProjectService, private authService: AuthService){}


    resolve(): Observable<any>{
        return this.projectsService.getProjects(this.authService.decodedToken.nameid).pipe(
            catchError((error) => {
                return of(null);
            })
        );
    }
}
