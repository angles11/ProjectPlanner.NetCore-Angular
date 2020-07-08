import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Friend } from 'src/app/_models/friend';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { MySnackBarService } from 'src/app/_notifications/my-snackBar.service';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { ConfirmDialogModel, ConfirmDialogComponent } from 'src/app/_notifications/confirm-dialog/confirm-dialog.component';
import { UserDialogComponent } from '../../users/user-dialog/user-dialog.component';


@Component({
  selector: 'app-friend-card',
  templateUrl: './friend-card.component.html',
  styleUrls: ['./friend-card.component.css']
})
export class FriendCardComponent implements OnInit {

  @Input() friend: Friend;
  @Input() type: string;

  @Output() changedEvent = new EventEmitter<any>();

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private snackBar: MySnackBarService,
    private dialog: MatDialog) { }

  ngOnInit() {
  }

  acceptUser() {
    this.userService.acceptFriend(this.authService.decodedToken.nameid, this.friend.userFriend.id).subscribe(() => {
      this.snackBar.openSnackBar('User accepted successfully', 'success', 2000);

      const event = {
        id: this.friend.userFriend.id,
        status: 'Accepted',
      };
      this.changedEvent.emit(event);

    }, error => {
      this.snackBar.openSnackBar(error, 'error', 5000);
    });
  }

  declineUser() {
    this.userService.deleteFriend(this.authService.decodedToken.nameid, this.friend.userFriend.id).subscribe(() => {
      this.snackBar.openSnackBar('Friend request declined successfully', 'success', 2000);
      const event = {
        id: this.friend.userFriend.id,
        status: 'Declined'
      };
      this.changedEvent.emit(event);
    }, error => {
      this.snackBar.openSnackBar(error, 'error', 5000);
    });
  }

  unfriend() {
    const message = 'Are you sure yo want to remove ' + this.friend.userFriend.knownAs + ' from your friends list?';

    const dialogData = new ConfirmDialogModel('Delete Friend', message, 'Delete');

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: dialogData,
    });

    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult) {
        this.userService.deleteFriend(this.authService.decodedToken.nameid, this.friend.userFriend.id).subscribe(() => {
          this.snackBar.openSnackBar('Friend deleted successfully', 'success', 2000);
          const event = {
            id: this.friend.userFriend.id,
            status: 'Deleted'
          };
          this.changedEvent.emit(event);
        }, error => {
          this.snackBar.openSnackBar(error, 'error', 5000);
        });
      }
    });
  }

  cancelRequest() {
    const message = 'Are you sure you want to cancel your friend request to ' + this.friend.userFriend.knownAs + ' ?';

    const dialogData = new ConfirmDialogModel('Cancel Friend Request', message, 'Confirm');

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: dialogData,
    });

    dialogRef.afterClosed().subscribe(dialogResult => {
      this.userService.deleteFriend(this.authService.decodedToken.nameid, this.friend.userFriend.id).subscribe(() => {
        this.snackBar.openSnackBar('Request cancelled successfully', 'success', 10000);
        const event = {
          id: this.friend.userFriend.id,
          status: 'Cancelled'
        };
        this.changedEvent.emit(event);
      }, error => {
        this.snackBar.openSnackBar(error, 'error', 5000);
      });
    });
  }

  openDialog() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data = this.friend.userFriend;
    dialogConfig.panelClass = 'user-dialog-custom-class';
    this.dialog.open(UserDialogComponent, dialogConfig);
  }

}
