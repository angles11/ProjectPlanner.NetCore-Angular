
import { Resolve } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { ProjectService } from '../_services/project.service';
import { Project } from '../_models/project';

@Injectable()
export class ProjectsListResolver implements Resolve<Project[]> {
    constructor(private projectsService: ProjectService, private authService: AuthService){}


    resolve(): Observable<Project[]>{
        return this.projectsService.getProjects(this.authService.decodedToken.nameid).pipe(
            catchError((error) => {
                return of(null);
            })
        );
    }
}
