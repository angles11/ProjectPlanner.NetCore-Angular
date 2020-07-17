import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Project } from '../_models/project';

@Injectable({
  providedIn: 'root',
})
export class ProjectService {
  baseUrl: any = environment.apiUrl;

  constructor(private http: HttpClient) { }

  createProject(userId: string, project: any) {
    return this.http.post(this.baseUrl + userId + '/projects', project);
  }

  getProjects(userId: string, searchTerm?) {

    let params = new HttpParams();

    if (searchTerm != null) {
      params = params.set('searchTerm', searchTerm);
    }

    return this.http.get(this.baseUrl + userId + '/projects', { params });
  }

  addCollaborator(userId: string, projectId: number, friendId: string) {
    return this.http.post(this.baseUrl + userId + '/projects/' + projectId + '/collaborators/' + friendId, {});
  }

  deleteCollaborator(userId: string, projectId: number, collaboratorId: string) {
    return this.http.delete(this.baseUrl + userId + '/projects/' + projectId + '/collaborators/' + collaboratorId);
  }

  deleteProject(userId: string, projectId: number) {
    return this.http.delete(this.baseUrl + userId + '/projects/' + projectId);
  }

  editProject(userId: string, projectId: number, project: any) {
    return this.http.put(this.baseUrl  + userId + '/projects/' + projectId, project);
  }

  getProject(userId: string, projectId: number): Observable<Project> {
    return this.http.get<Project>(this.baseUrl + userId + '/projects/' + projectId);
  }
}
