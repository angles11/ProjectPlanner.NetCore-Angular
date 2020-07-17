import { Component, OnInit } from '@angular/core';
import { transition, trigger, style, animate } from '@angular/animations';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../_services/auth.service';
import { ProjectService } from '../_services/project.service';
import { MySnackBarService } from '../_notifications/my-snackBar.service';
import { ActivatedRoute } from '@angular/router';
import { Project } from '../_models/project';
import { Friend } from '../_models/friend';
import { User } from '../_models/user';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.css'],
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
export class ProjectsComponent implements OnInit {

  panelOpenState = false;
  newProjectForm: FormGroup;
  projects: Project[];
  acceptedFriends: User[] = [];
  searchTerm = '';

  constructor(private authService: AuthService, private fb: FormBuilder,
              private projectService: ProjectService, private snackBar: MySnackBarService,
              private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.projects = data.projects;
      this.filterAcceptedFriends(data.friends);
    }, error => {
        console.log(error);
    });
    this.createNewProjectForm();

  }

  filterAcceptedFriends(friends: Friend[]) {
    friends.forEach(friend => {
      if (friend.status === 1) {
        this.acceptedFriends.push(friend.userFriend);
      }
    });
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
      }, () => {
        this.loadProjects();
      });
    }
  }
  toggleForm() {
    this.panelOpenState = !this.panelOpenState;
  }

  loadProjects() {
    this.projectService.getProjects(this.authService.decodedToken.nameid, this.searchTerm).subscribe((response: Project[]) => {
      this.projects = response;
    }, error => {
      console.log(error);
    });
  }

  onDeleted(event: number) {
    this.projects = this.projects.filter(p => p.id !== event);
  }
}
