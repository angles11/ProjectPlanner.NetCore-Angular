import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Project } from 'src/app/_models/project';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { trigger, transition, style, animate } from '@angular/animations';
import { AuthService } from 'src/app/_services/auth.service';
import { TodoService } from 'src/app/_services/todo.service';

@Component({
  selector: 'app-project-detail',
  templateUrl: './project-detail.component.html',
  styleUrls: ['./project-detail.component.css'],
   animations: [
    trigger(
      'inOutAnimation',
      [
        transition(':enter', [
          style({transform: 'translateX(-100%)', opacity: 0}),
          animate('500ms', style({transform: 'translateX(0)', opacity: 1}))
        ]),
        transition(':leave', [
          style({transform: 'translateX(0)', opacity: 1}),
          animate('500ms', style({transform: 'translateX(-100%)', opacity: 0}))
          ]
        )
      ]
    )
  ]
})
export class ProjectDetailComponent implements OnInit {

  project: Project;
  panelOpenState = false;
  newTodoForm: FormGroup;
  constructor(private route: ActivatedRoute, private fb: FormBuilder, public authService: AuthService, public todoService: TodoService) { }

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.project = data.project;
    });

    this.createNewTodoForm();
  }

    createNewTodoForm() {
    this.newTodoForm = this.fb.group({
      title: ['', Validators.required],
      shortDescription: ['', Validators.required],
      longDescription: ['', Validators.required],
      estimatedDate: [null, Validators.required]
    });
  }

  create() {
    if (this.newTodoForm.valid) {
      this.todoService.createTodo(this.authService.decodedToken.nameid, this.project.id, this.newTodoForm.value).subscribe(() => {
        console.log('success');
      }, error => {
        console.log(error);
      });
    }
  }
   toggleForm() {
    this.panelOpenState = !this.panelOpenState;
  }


}
