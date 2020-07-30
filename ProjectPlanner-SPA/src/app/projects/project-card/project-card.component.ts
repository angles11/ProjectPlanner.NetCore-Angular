import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { Project } from 'src/app/_models/project';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { ProjectService } from 'src/app/_services/project.service';
import { MySnackBarService } from 'src/app/_notifications/my-snackBar.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogModel, ConfirmDialogComponent } from 'src/app/_notifications/confirm-dialog/confirm-dialog.component';
import { Animations } from 'src/app/_helpers/animations';
import { trigger, transition, style, animate } from '@angular/animations';

@Component({
  selector: 'app-project-card',
  templateUrl: './project-card.component.html',
  styleUrls: ['./project-card.component.css'],
  animations: [
    Animations.inOutAnimation,
    Animations.expandAnimation
  ]

})
export class ProjectCardComponent implements OnInit {

  @Input() project: Project;
  @Input() friends: User[];
  @Input() index: number;

  @Output() deletedEvent = new EventEmitter<any>();

  progressBarValue = 0;

  panelContentOpenState = false;
  filteredFriends: User[];
  panelOpenState = false;
  editProjectForm: FormGroup;

  constructor(public authService: AuthService, private projectService: ProjectService,
              private snackBar: MySnackBarService, private fb: FormBuilder,
              private dialog: MatDialog) { }

  ngOnInit() {
    this.filterFriends();
    this.updateProgressBar();
  }

  filterFriends() {
    this.filteredFriends = this.friends.filter(f => !this.project.collaborators.find(c => c.id === f.id));
  }

  updateProgressBar() {
    setInterval(() => {
      if (this.progressBarValue < this.project.completedPercentage) {
        console.log(this.progressBarValue);
        this.progressBarValue++;
      }
    }, 10);
  }
  addProjectCollaborator(friendId: string) {
    this.projectService.addCollaborator(
      this.authService.decodedToken.nameid, this.project.id, friendId
    ).subscribe(() => {
      const index = this.filteredFriends.findIndex(f => f.id === friendId);
      this.project.collaborators.push(this.filteredFriends[index]);
      this.filteredFriends.splice(index, 1);
    }, error => {
      this.snackBar.openSnackBar(error, 'error', 5000);
    });
  }

   togglePanelContent() {
     this.panelContentOpenState = !this.panelContentOpenState;
     this.panelOpenState = false;
  }

  removeCollaborator(collaboratorId: string) {
    this.projectService.deleteCollaborator(this.authService.decodedToken.nameid, this.project.id, collaboratorId).subscribe(() => {
      this.snackBar.openSnackBar('Collaborator removed successfully', 'success', 2000);
      this.filteredFriends.push(this.project.collaborators.find(c => c.id === collaboratorId));
      this.project.collaborators = this.project.collaborators.filter(c => c.id !== collaboratorId);
    }, error => {
      this.snackBar.openSnackBar(error, 'error', 5000);
    });
  }

  deleteProject() {
    const message = 'Are you sure you want to delete ' + this.project.title;
    const dialogData = new ConfirmDialogModel('Delete Project', message, 'Delete');
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: dialogData
    });

    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult) {
        this.projectService.deleteProject(this.authService.decodedToken.nameid, this.project.id).subscribe(() => {
          this.deletedEvent.emit(this.project.id);
        }, error => {
          this.snackBar.openSnackBar(error, 'error', 5000);
        });
      }
    });
  }



  createEditProjectForm() {
    this.editProjectForm = this.fb.group({
      title: [this.project.title, Validators.required],
      shortDescription: [this.project.shortDescription, Validators.required],
      longDescription: [this.project.longDescription, Validators.required],
      estimatedDate: [this.project.estimatedDate, Validators.required]
    });
  }
  toggleForm() {
    this.panelOpenState = !this.panelOpenState;
    if (this.panelOpenState) {
      this.createEditProjectForm();
    }
  }

  editProject() {
    if (this.editProjectForm.valid) {
      this.projectService.editProject(this.authService.decodedToken.nameid, this.project.id, this.editProjectForm.value).subscribe(() => {
        this.project = Object.assign(this.project, this.editProjectForm.value, this.project.modified = new Date());
        this.snackBar.openSnackBar('Project edited successfully', 'success', 2000);
        this.editProjectForm.reset();
        this.panelOpenState = false;
      }, error => {
        this.snackBar.openSnackBar(error, 'error', 5000);
      });
    }
  }
}
