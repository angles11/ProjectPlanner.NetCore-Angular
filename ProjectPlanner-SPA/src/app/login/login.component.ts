import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  model: any = {};
  constructor(private authService: AuthService, private router: Router, private alertify: AlertifyService) {}

  ngOnInit() {}

  login() {
    this.authService.login(this.model).subscribe(
      () => {
        this.router.navigate(['/projects']);
        this.alertify.success('Logging in');
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }
}
