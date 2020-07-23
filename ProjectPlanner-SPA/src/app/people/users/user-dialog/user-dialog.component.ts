import { Component, OnInit, Inject } from '@angular/core';
import { User } from 'src/app/_models/user';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-user-dialog',
  templateUrl: './user-dialog.component.html',
  styleUrls: ['./user-dialog.component.css']
})
export class UserDialogComponent implements OnInit {

  user: User;

  constructor(private dialogRef: MatDialogRef<UserDialogComponent>,
              @Inject(MAT_DIALOG_DATA) public data: User) {
    this.user = data;
  }
  ngOnInit() {
  }

  onDismiss() {
    this.dialogRef.close();
  }
}
