import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TodoMessage } from 'src/app/_models/todoMessage';

@Component({
  selector: 'app-messages-dialog',
  templateUrl: './messages-dialog.component.html',
  styleUrls: ['./messages-dialog.component.css'],
})
export class MessagesDialogComponent implements OnInit {

  messages: TodoMessage[];
  userId: string;

  constructor(private dialogRef: MatDialogRef<MessagesDialogComponent>,
              @Inject(MAT_DIALOG_DATA) public data: any) {
    this.messages = data.messages;
    this.userId = data.userId;
    }

  ngOnInit() {

  }

  onDismiss() {
    this.dialogRef.close();
  }
}
