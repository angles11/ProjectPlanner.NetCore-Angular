import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Project } from '../_models/project';

@Injectable({
  providedIn: 'root',
})
export class ProjectsService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getProjects(userId: string) {
    return this.http.get(this.baseUrl + 'user/' + userId + '/projects');
  }

  createProject(userId: string, project: Project) {
    return this.http.post(this.baseUrl + 'user/' + userId + '/projects', project);
  }

  deleteProject(userId: string, projectId: number) {
    return this.http.delete(this.baseUrl + 'user/' + userId + '/projects/' + projectId);
  }
}
