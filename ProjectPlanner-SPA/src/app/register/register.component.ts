import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../_services/auth.service';
import { Router } from '@angular/router';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker/public_api';
import { User } from '../_models/user';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  bsConfig: Partial<BsDatepickerConfig>;
  user: User;

  constructor(private authService: AuthService, private fb: FormBuilder, private router: Router) {}

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-dark-blue',
      isAnimated: true,
      maxDate: new Date(),
    };
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.fb.group(
      {
        gender: ['male', Validators.required],
        username: ['', Validators.required],
        knownAs: ['', Validators.required],
        dateOfBirth: [null, Validators.required],
        email: ['', Validators.required],
        country: ['', Validators.required],
        position: ['', Validators.required],
        employer: ['', Validators.required],
        experience: ['', Validators.required],
        password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(25)]],
        confirmPassword: ['', Validators.required],
      },
      { validator: this.passwordMatchValidator }
    );
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : { mismatch: true };
  }

  register() {
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);
      this.authService.register(this.user).subscribe(
        () => {
          console.log('success');
        },
        (error) => {
          console.log(error);
        },
        () => {
          this.authService.login(this.user).subscribe(() => {
            this.router.navigate(['/projects']);
            console.log(this.user);
          });
        }
      );
    }
  }
}
