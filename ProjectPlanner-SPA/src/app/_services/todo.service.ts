import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Todo } from '../_models/todo';

@Injectable({
  providedIn: 'root'
})
export class TodoService {

  baseUrl: any = environment.apiUrl;

  constructor(private http: HttpClient) { }

  createTodo(userId: string, projectId: number, todo: any) {
    return this.http.post(this.baseUrl + userId + '/projects/' + projectId + '/todo', todo);
  }

  editTodo(userId: string, projectId: number, todoId: number, todo: Todo) {

    const jsonString = JSON.stringify(status);
    return this.http.put(this.baseUrl + userId + '/projects/' + projectId + '/todo/' + todoId, todo);
  }

  changeStatus(userId: string, projectId: number, todoId: number, status: string) {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    const json = JSON.stringify(status);
    return this.http.patch(this.baseUrl + userId + '/projects/' + projectId + '/todo/' + todoId, json, {headers});
  }

  addMessage(userId: string, projectId: number, todoId: number, message: string) {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    const json = JSON.stringify(message);

    return this.http.post(this.baseUrl + userId + '/projects/' + projectId + '/todo/' + todoId, json, {headers});
  }

  deleteTodo(userId: string, projectId: number, todoId: number) {
    return this.http.delete(this.baseUrl + userId + '/projects/' + projectId + '/todo/' + todoId);
  }

}
