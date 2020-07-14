import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class ProjectService {
  baseUrl: any = environment.apiUrl;

  constructor(private http: HttpClient) { }

  createProject(userId: string, project: any) {
   return this.http.post(this.baseUrl + 'user/' + userId + '/projects', project);
  }

  getProjects(userId: string) {
    return this.http.get(this.baseUrl + 'user/' + userId + '/projects');
  }
}
