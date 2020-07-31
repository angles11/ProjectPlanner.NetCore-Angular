import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Project } from 'src/app/_models/project';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AuthService } from 'src/app/_services/auth.service';
import { TodoService } from 'src/app/_services/todo.service';
import { MySnackBarService } from 'src/app/_notifications/my-snackBar.service';
import { Animations } from 'src/app/_helpers/animations';
import { Todo } from 'src/app/_models/todo';
import { ProjectService } from 'src/app/_services/project.service';

@Component({
  selector: 'app-project-detail',
  templateUrl: './project-detail.component.html',
  styleUrls: ['./project-detail.component.css'],
  animations: [Animations.inOutAnimation]
})
export class ProjectDetailComponent implements OnInit {

  project: Project;
  panelOpenState = false;
  newTodoForm: FormGroup;
  minDate: Date;

  constructor(private route: ActivatedRoute, private fb: FormBuilder,
              public authService: AuthService, public todoService: TodoService,
              private snackBar: MySnackBarService) { }

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.project = data.project;
    });

    this.createNewTodoForm();
    this.minDate = new Date();
  }

  createNewTodoForm() {
    this.newTodoForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(20)]],
      shortDescription: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(50)]],
      longDescription: ['', [Validators.required, Validators.minLength(20), Validators.maxLength(300)]],
      estimatedDate: [null, Validators.required]
    });
  }

  create() {
    if (this.newTodoForm.valid) {
      this.todoService.createTodo(this.authService.decodedToken.nameid, this.project.id, this.newTodoForm.value)
        .subscribe((response: Todo) => {
          this.snackBar.openSnackBar('Todo added successfully', 'success', 5000);
          console.log(response);
        this.project.todos.push(response);
        this.panelOpenState = false;
      }, error => {
        this.snackBar.openSnackBar(error, 'error', 5000);
      });
    }
  }
  toggleForm() {
    this.panelOpenState = !this.panelOpenState;
  }

  onDeleted(event: number) {
    const index = this.project.todos.findIndex(x => x.id === event);
    this.project.todos.splice(index, 1);
  }

}
