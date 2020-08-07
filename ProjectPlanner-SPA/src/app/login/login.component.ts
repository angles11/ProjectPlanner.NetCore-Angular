import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { Router } from '@angular/router';
import { MySnackBarService } from '../_notifications/my-snackBar.service';
import { MatDialog } from '@angular/material/dialog';
import { RecoverPasswordDialogComponent } from './recover-password-dialog/recover-password-dialog.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  hide = true;
  model: any = {};

  constructor(private authService: AuthService, private router: Router, private snackBar: MySnackBarService, public dialog: MatDialog) { }

  ngOnInit() {
  }

  login() {
    this.authService.login(this.model).subscribe(() => {
      this.snackBar.openSnackBar('Welcome! ' + this.authService.currentUser.knownAs, 'success', 3000, 'bottom');
      this.router.navigate(['/projects']);
    }, error => {
      this.snackBar.openSnackBar(error, 'error', 3000);
    });
  }

  openForgotPasswordDialog() {
    const dialogRef = this.dialog.open(RecoverPasswordDialogComponent);
  }
}
