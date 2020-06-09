import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { Project } from '../_models/project';
import { ProjectsService } from '../_services/projects.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';

@Injectable()
export class ProjectsListResolver implements Resolve<Project[]> {
  constructor(private projectsService: ProjectsService, private router: Router, private authService: AuthService) {}

  resolve(): Observable<Project[]> {
    return this.projectsService.getProjects(this.authService.decodedToken.nameid).pipe(
      catchError((error) => {
        console.log(error);
        this.router.navigate(['/login']);
        return of(null);
      })
    );
  }
}
