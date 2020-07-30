import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/_services/auth.service';
import { MySnackBarService } from 'src/app/_notifications/my-snackBar.service';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.css']
})
export class ConfirmEmailComponent implements OnInit {

  token: string;
  email: string;

  constructor(private route: ActivatedRoute, private authService: AuthService,
              private snackBar: MySnackBarService, private router: Router) { }

  ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      this.token = params.token;
      this.email = params.email;
    });

    if (this.token && this.email) {
      this.authService.confirmEmail(this.email, this.token).subscribe(() => {
        this.snackBar.openSnackBar('Your email has been confirmed. Please login', 'success', 5000, 'bottom');
        setTimeout(() => {
          this.router.navigate(['login']);
        }, 3000);
      }, error => {
        this.snackBar.openSnackBar(error, 'error', 5000);
      });
    }

    else {
      this.router.navigate(['']);
    }

  }
}
