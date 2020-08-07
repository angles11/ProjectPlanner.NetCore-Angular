import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/_services/auth.service';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { MySnackBarComponent } from 'src/app/_notifications/my-snackBar/my-snackBar.component';
import { MySnackBarService } from 'src/app/_notifications/my-snackBar.service';

@Component({
  selector: 'app-recover-password',
  templateUrl: './recover-password.component.html',
  styleUrls: ['./recover-password.component.css']
})
export class RecoverPasswordComponent implements OnInit {

  token: string;
  email: string;
  hidePwd = true;
  hideCPwd = true;
  resetPasswordForm: FormGroup;
  registrationSuccessful = false;

  constructor(private route: ActivatedRoute, private authService: AuthService,
              private router: Router, private fb: FormBuilder, private snackBar: MySnackBarService) { }

  ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      this.token = params.token;
      this.email = params.email;
    });

    if (!this.token || !this.email) {
      this.router.navigate(['']);
    }

    this.createResetPasswordForm();
  }

   createResetPasswordForm() {
    this.resetPasswordForm = this.fb.group({
      password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(25)]],
      confirmPassword: ['', Validators.required]
    },
      { validator: this.passwordMatchValidator });
  }

  passwordMatchValidator(g: FormGroup) {
    const password = g.get('password').value;
    const confirmPassword = g.get('confirmPassword').value;
    if (password !== confirmPassword) {
      g.get('confirmPassword').setErrors({ passwordMismatch: true });
    }
    else {
      return null;
    }
  }

  resetPassword() {
    if (this.resetPasswordForm.valid) {
      const formValue = this.resetPasswordForm.value;
      this.authService.resetPassword(this.email, this.token, formValue.password).subscribe(() => {
        this.registrationSuccessful = true;
      }, error => {
          this.snackBar.openSnackBar(error, 'error', 5000);
      });
    }
  }

}
