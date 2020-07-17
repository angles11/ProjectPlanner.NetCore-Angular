import { Injectable } from '@angular/core';
import { ProjectService } from '../_services/project.service';
import { Router, ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { Project } from '../_models/project';
import { Observable } from 'rxjs';
import { AuthService } from '../_services/auth.service';
import { catchError } from 'rxjs/operators';

@Injectable()

export class ProjectDetailResolver implements Resolve<Project> {

    constructor(private projectService: ProjectService, private authService: AuthService) { }

    resolve(route: ActivatedRouteSnapshot): Observable<Project> {
        return this.projectService.getProject(this.authService.decodedToken.nameid, route.params.id);
    }
}

