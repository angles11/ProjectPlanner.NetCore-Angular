import { Component, OnInit } from '@angular/core';
import { transition, trigger, style, animate } from '@angular/animations';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../_services/auth.service';
import { ProjectService } from '../_services/project.service';
import { MySnackBarService } from '../_notifications/my-snackBar.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.css'],
  animations: [
    trigger(
      'inOutAnimation',
      [
        transition(
          ':enter',
          [
            style({ opacity: 0 }),
            animate('1s ease-out',
              style({ opacity: 1 }))
          ]
        ),
        transition(
          ':leave',
          [
            style({ opacity: 1 }),
            animate('1s ease-in',
              style({ opacity: 0 }))
          ]
        )
      ]
    )
  ]
})
export class ProjectsComponent implements OnInit {

  panelOpenState = false;
  newProjectForm: FormGroup;
  projects: any;

  constructor(private authService: AuthService, private fb: FormBuilder,
    private projectService: ProjectService, private snackBar: MySnackBarService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.projects = data.projects;
      console.log(this.projects);
    }, error => {
        console.log(error);
    })
    this.createNewProjectForm();

  }

  createNewProjectForm() {
    this.newProjectForm = this.fb.group({
      title: ['', Validators.required],
      shortDescription: ['', Validators.required],
      longDescription: ['', Validators.required],
      estimatedDate: [null, Validators.required]
    });
  }

  create() {
    if (this.newProjectForm.valid) {
      this.projectService.createProject(this.authService.decodedToken.nameid, this.newProjectForm.value).subscribe(() => {
        this.snackBar.openSnackBar('Project added successfully', 'success', 2000);
        this.newProjectForm.reset();
        this.panelOpenState = false;
      }, error => {
          this.snackBar.openSnackBar(error, 'error', 5000);
      });
    }
  }
  toggleForm() {
    this.panelOpenState = !this.panelOpenState;
  }
}
