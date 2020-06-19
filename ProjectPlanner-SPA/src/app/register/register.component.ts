import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../_services/auth.service';
import { Router } from '@angular/router';
import { User } from '../_models/user';
import { MySnackBarService } from '../_notifications/my-snackBar.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  hidePwd = true;
  hideCPwd = true;
  registerForm: FormGroup;
  maxDate: Date;
  user: User;

  constructor(private authService: AuthService, private fb: FormBuilder, private router: Router, private snackBar: MySnackBarService) { }

  ngOnInit() {
    this.maxDate = new Date();
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      gender: ['male', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      dateOfBirth: [null, Validators.required],
      country: ['', Validators.required],
      position: ['', Validators.required],
      employer: ['', Validators.required],
      experience: ['', Validators.required],
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

  register() {
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);
      this.authService.register(this.user).subscribe(() => {
        this.snackBar.openSnackBar('Registered successfully', 'success', 2000, 'bottom');
      }, error => {
          this.snackBar.openSnackBar(error, 'error', 5000);
      }, () => {
          this.authService.login(this.user).subscribe(() => {
            this.router.navigate(['/projects']);
          }, error => {
              console.log(error);
          })
      })
    }
  }
}
