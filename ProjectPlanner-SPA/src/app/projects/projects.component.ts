import { Component, OnInit } from '@angular/core';
import { ProjectsService } from '../_services/projects.service';
import { AuthService } from '../_services/auth.service';
import { ActivatedRoute, Data } from '@angular/router';
import { Project } from '../_models/project';
import { User } from '../_models/user';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker/public_api';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.css'],
})
export class ProjectsComponent implements OnInit {
  projects: Project[];
  newProject: Project;
  user: User;
  newProjectForm: FormGroup;
  bsConfig: Partial<BsDatepickerConfig>;
  isCollapsed = true;
  alert: any = {};

  constructor(
    private projectsService: ProjectsService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.route.data.subscribe((data: Data) => {
      this.projects = data.projects;
    });

    this.user = this.authService.currentUser;
    this.bsConfig = {
      containerClass: 'theme-dark-blue',
      isAnimated: true,
      minDate: new Date(),
    };
    this.createNewProjectForm();
  }

  createProject() {
    if (this.newProjectForm.valid) {
      this.newProject = Object.assign({}, this.newProjectForm.value);
      this.projectsService.createProject(this.authService.decodedToken.nameid, this.newProject).subscribe(
        (project: Project) => {
          this.projects.unshift(project);
          this.newProjectForm.reset();
          this.isCollapsed = true;
          this.alertify.success('Project ' + this.newProject.title + ' created successfully');
        },
        (error) => {
          this.alertify.error(error);
        }
      );
    }
  }
  createNewProjectForm() {
    this.newProjectForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      estimatedDate: [null, Validators.required],
    });
  }

  removeProject(id: number) {
    const index = this.projects.findIndex((p) => p.id === id);
    this.projects.splice(index, 1);
  }
}
