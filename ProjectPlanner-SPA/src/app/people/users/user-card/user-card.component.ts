import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { ConfirmDialogModel, ConfirmDialogComponent } from 'src/app/_notifications/confirm-dialog/confirm-dialog.component';
import { MySnackBarService } from 'src/app/_notifications/my-snackBar.service';
import { AuthService } from 'src/app/_services/auth.service';
import { UserDialogComponent } from '../../users/user-dialog/user-dialog.component';

@Component({
  selector: 'app-user-card',
  templateUrl: './user-card.component.html',
  styleUrls: ['./user-card.component.css']
})
export class UserCardComponent implements OnInit {
  @Input() user: User;

  requestSent = false;

  constructor(private userService: UserService,
              private dialog: MatDialog,
              private snackBar: MySnackBarService,
              private authService: AuthService) { }

  ngOnInit() {
  }

  addFriend() {
    const message = 'Are you sure you want to add ' + this.user.knownAs + ' to your friends list?';

    const dialogData = new ConfirmDialogModel('Add Friend', message, 'Add');

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: dialogData,
    });

    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult) {
        this.userService.addFriend(this.authService.decodedToken.nameid, this.user.id).subscribe(() => {
          this.requestSent = true;
          this.snackBar.openSnackBar('Friend request Sent', 'success', 2000, 'bottom');
        }, error => {
          this.snackBar.openSnackBar(error, 'error', 5000, 'top');
        });
      }
    });
  }

  cancelRequest() {
    this.userService.deleteFriend(this.authService.decodedToken.nameid, this.user.id).subscribe(() => {
      this.requestSent = false;
      this.snackBar.openSnackBar('Request cancelled', 'info', 2000, 'top');
    }, error => {
      this.snackBar.openSnackBar(error, 'error', 5000, 'top');
    });
  }

  openDialog() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data = this.user;
    dialogConfig.panelClass = 'user-dialog-custom-class';
    this.dialog.open(UserDialogComponent, dialogConfig);
  }

}
