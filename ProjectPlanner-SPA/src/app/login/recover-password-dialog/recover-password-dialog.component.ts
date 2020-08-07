import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { MySnackBarService } from 'src/app/_notifications/my-snackBar.service';

@Component({
  selector: 'app-recover-password-dialog',
  templateUrl: './recover-password-dialog.component.html',
  styleUrls: ['./recover-password-dialog.component.css']
})
export class RecoverPasswordDialogComponent implements OnInit {

  userEmail: string;
  recoverSuccessful = false;
  constructor(private authService: AuthService, private snackBar: MySnackBarService) { }

  ngOnInit() {
  }

  recoverPassword() {
    this.authService.forgotPassword(this.userEmail).subscribe(() => {
      this.recoverSuccessful = true;
    }, error => {
        this.snackBar.openSnackBar(error, 'error', 5000);
    });
  }
}
