import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { UserService } from '../_services/user.service';
import { ActivatedRoute } from '@angular/router';
import { User } from '../_models/user';
import { AuthService } from '../_services/auth.service';
import { MySnackBarService } from '../_notifications/my-snackBar.service';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css']
})
export class AccountComponent implements OnInit {

  hidePwd = true;
  hideCPwd = true;
  accountForm: FormGroup;
  passwordForm: FormGroup;
  maxDate: Date;
  user: User;
  imageSrc: string;
  selectedPhoto: File;

  constructor(private userService: UserService, private fb: FormBuilder,
              private route: ActivatedRoute, private authService: AuthService,
              private snackBar: MySnackBarService) { }

  ngOnInit(): void {
    this.route.data.subscribe((data) => {
      this.user = data.user;
      console.log(this.user);
    });
    this.createAccountForm();
    this.createPasswordForm();
    this.maxDate = new Date();
    this.imageSrc = this.user.photoUrl;
  }

  createAccountForm() {
    this.accountForm = this.fb.group({
      knownAs: [this.user.knownAs, [Validators.required, Validators.minLength(8), Validators.maxLength(15)]],
      gender: [this.user.gender, Validators.required],
      dateOfBirth: [this.user.dateOfBirth, Validators.required],
      country: [this.user.country, Validators.required],
      position: [this.user.position, Validators.required],
      employer: [this.user.employer, Validators.required],
      experience: [this.user.experience, Validators.required]
    });
  }

  createPasswordForm() {
    this.passwordForm = this.fb.group({
      password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(25)]],
      newPassword: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(25)]],
      confirmNewPassword: ['', Validators.required]
    },
      { validator: this.passwordMatchValidator });
  }

  passwordMatchValidator(g: FormGroup) {
    const newPassword = g.get('newPassword').value;
    const confirmNewPassword = g.get('confirmNewPassword').value;
    if (newPassword !== confirmNewPassword) {
      g.get('confirmNewPassword').setErrors({ passwordMismatch: true });
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
  editAccount() {
    if (this.accountForm.valid) {
      this.user = Object.assign(this.authService.currentUser, this.accountForm.value);
      this.userService.editAccount(this.authService.decodedToken.nameid, this.user).subscribe(() => {
        this.snackBar.openSnackBar('Account edited successfully', 'success', 3000);
      }, error => {
        this.snackBar.openSnackBar(error, 'error', 5000);
      }, () => {
          console.log(this.user);
          this.authService.currentUser = this.user;
          this.updateUser();
      });
    }
  }

  changePhoto() {

    const formData: FormData = new FormData();
    formData.append('photo', this.selectedPhoto);
    console.log(formData);
    this.userService.changePhoto(this.authService.decodedToken.nameid, formData).subscribe(() => {
      this.snackBar.openSnackBar('Profile picture changed successfully', 'success', 3000);
    }, error => {
      this.snackBar.openSnackBar(error, 'error', 5000);
    }, () => {
        this.authService.currentUser.photoUrl = this.imageSrc;
        this.updateUser();
    });
  }

  changePassword() {
    if (this.passwordForm.valid) {
      this.userService.changePassword(this.authService.decodedToken.nameid, this.passwordForm.value).subscribe(() => {
        this.snackBar.openSnackBar('Password changed successfully', 'success', 3000);
      }, error => {
        this.snackBar.openSnackBar(error, 'error', 5000);
      });
    }
  }

  updateUser() {
    localStorage.setItem(
            'user',
            JSON.stringify(this.authService.currentUser)
          );
  }
}
