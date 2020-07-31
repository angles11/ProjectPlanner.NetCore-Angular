import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Todo } from 'src/app/_models/todo';
import { TodoService } from 'src/app/_services/todo.service';
import { AuthService } from 'src/app/_services/auth.service';
import { MySnackBarService } from 'src/app/_notifications/my-snackBar.service';
import { TodoMessage } from 'src/app/_models/todoMessage';
import { MatDialogConfig, MatDialog } from '@angular/material/dialog';
import { MessagesDialogComponent } from '../messages-dialog/messages-dialog.component';
import { Animations } from 'src/app/_helpers/animations';

@Component({
  selector: 'app-todo-card',
  templateUrl: './todo-card.component.html',
  styleUrls: ['./todo-card.component.css'],
  animations: [
   Animations.inOutAnimation
  ]
})
export class TodoCardComponent implements OnInit {

  @Input() todo: Todo;
  @Input() index: number;
  @Input() ownerId: string;

  @Output() deletedEvent = new EventEmitter<number>();

  status: string;
  panelOpenState = false;
  lastMessage: TodoMessage;
  newMessage: string;

  constructor(private todoService: TodoService, public authService: AuthService,
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
        console.log(response);
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

  deleteTodo() {
    this.todoService.deleteTodo(this.authService.decodedToken.nameid, this.todo.projectId, this.todo.id).subscribe(() => {
      this.snackBar.openSnackBar('Todo deleted successfully', 'success', 3000);
      this.deletedEvent.emit(this.todo.id);
    }, error => {
        this.snackBar.openSnackBar(error, 'error', 5000);
    });
  }
}
