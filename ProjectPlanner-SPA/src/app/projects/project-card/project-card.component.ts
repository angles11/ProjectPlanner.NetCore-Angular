import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Project } from 'src/app/_models/project';
import { AuthService } from 'src/app/_services/auth.service';
import { ProjectsService } from 'src/app/_services/projects.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-project-card',
  templateUrl: './project-card.component.html',
  styleUrls: ['./project-card.component.css'],
})
export class ProjectCardComponent implements OnInit {
  @Input() project: Project;
  @Output() removeProject = new EventEmitter<number>();

  constructor(
    private authService: AuthService,
    private projectsService: ProjectsService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {}

  deleteProject(id: number) {
    this.alertify.confirm(
      'Delete project',
      'Are you sure you want to delete this project?',
      () => {
        this.projectsService.deleteProject(this.authService.decodedToken.nameid, id).subscribe(
          () => {
            this.removeProject.emit(id);
            this.alertify.success('Project ' + this.project.title + ' deleted successfully');
          },
          (error) => {
            this.alertify.error(error);
          }
        );
      },
      null
    );
  }
}
