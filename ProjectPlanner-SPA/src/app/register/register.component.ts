import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormGroupDirective } from '@angular/forms';
import { AuthService } from '../_services/auth.service';
import { Router } from '@angular/router';
import { User } from '../_models/user';
import { MySnackBarService } from '../_notifications/my-snackBar.service';
import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { AppDateAdapter, APP_DATE_FORMATS } from '../_helpers/format-datepicker';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @ViewChild(FormGroupDirective) formGroupDirective: FormGroupDirective;

  hidePwd = true;
  hideCPwd = true;
  registerForm: FormGroup;
  maxDate: Date;
  user: User;
  imageSrc: string;
  selectedPhoto: File;
  dateOfBirth: string;

  registrationSuccessful = false;

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

  onFileSelect(event): void {
    if (event.target.files && event.target.files[0]) {
      const file = event.target.files[0];
      const reader = new FileReader();

      reader.readAsDataURL(file);
      reader.onload = (e: any) => {
        this.imageSrc = e.target.result;
      };
      this.selectedPhoto = file;
    }
  }
  register() {
    if (this.registerForm.valid) {

      const formData: FormData = new FormData();

      Object.entries(this.registerForm.value).forEach(
        ([key, value]: any[]) => {
          if (key === 'dateOfBirth') {
            this.dateOfBirth = (new Date(value)).toUTCString();
          } else {
            formData.set(key, value);
          }
        });

      // https://stackoverflow.com/questions/29287311/how-to-append-datetime-value-to-formdata-and-receive-it-in-controller//
      formData.append('dateOfBirth', this.dateOfBirth);
      formData.append('photo', this.selectedPhoto);

      this.authService.register(formData).subscribe(() => {
        this.snackBar.openSnackBar('Registered successfully', 'success', 200, 'bottom');
      }, error => {
          this.snackBar.openSnackBar(error, 'error', 5000);
      }, () => {
          this.registrationSuccessful = true;
          this.formGroupDirective.resetForm();
      });
    }
  }
}
