import { Component, OnInit, Input } from '@angular/core';
import { Todo } from 'src/app/_models/todo';
import { TodoService } from 'src/app/_services/todo.service';
import { AuthService } from 'src/app/_services/auth.service';
import { MySnackBarService } from 'src/app/_notifications/my-snackBar.service';
import { transition, animate, style, trigger } from '@angular/animations';
import { TodoMessage } from 'src/app/_models/todoMessage';
import { MatDialogConfig, MatDialog } from '@angular/material/dialog';
import { MessagesDialogComponent } from '../messages-dialog/messages-dialog.component';

@Component({
  selector: 'app-todo-card',
  templateUrl: './todo-card.component.html',
  styleUrls: ['./todo-card.component.css'],
  animations: [
    trigger(
      'inOutAnimation',
      [
        transition(':enter', [
          style({ transform: 'translateX(-100%)', opacity: 0 }),
          animate('500ms', style({ transform: 'translateX(0)', opacity: 1 }))
        ]),
        transition(':leave', [
          style({ transform: 'translateX(0)', opacity: 1 }),
          animate('500ms', style({ transform: 'translateX(-100%)', opacity: 0 }))
        ]
        )
      ]
    )
  ]
})
export class TodoCardComponent implements OnInit {

  @Input() todo: Todo;
  @Input() index: number;

  status: string;
  panelOpenState = false;
  lastMessage: TodoMessage;
  newMessage: string;

  constructor(private todoService: TodoService, private authService: AuthService,
              private snackBar: MySnackBarService, private dialog: MatDialog) { }

  ngOnInit() {
    this.getLastMessage();
  }

  changeStatus(event: any) {
    this.todoService.changeStatus(this.authService.decodedToken.nameid,
      this.todo.projectId, this.todo.id, this.todo.status).subscribe(() => {
      }, error => {
        this.snackBar.openSnackBar(error, 'error', 5000);
      });
  }

  toggleForm() {
    this.panelOpenState = !this.panelOpenState;
  }

  getLastMessage() {
    this.lastMessage = this.todo.messages[this.todo.messages.length - 1];
  }
  onAddMessage() {
    this.todoService.addMessage(
      this.authService.decodedToken.nameid,
      this.todo.projectId, this.todo.id, this.newMessage)
      .subscribe((response: TodoMessage) => {
        this.panelOpenState = false;
        this.todo.messages.push(response);
        this.getLastMessage();
      }, error => {
        this.snackBar.openSnackBar(error, 'error', 5000);
      });
  }

  openDialog() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data = {messages: this.todo.messages, userId: this.authService.decodedToken.nameid};
    this.dialog.open(MessagesDialogComponent, dialogConfig);
  }
}
