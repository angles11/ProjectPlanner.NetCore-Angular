import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MySnackBarComponent } from '../_notifications/my-snackBar/my-snackBar.component';


@Injectable({
  providedIn: 'root'
})
export class MySnackBarService {
  constructor(private snackBar: MatSnackBar) {}
  public openSnackBar(message, type, duration?, verticalPosition?, horizontalPosition?) {
    const snackType = type !== undefined ? type : 'success';

    this.snackBar.openFromComponent(MySnackBarComponent, {
      duration: duration || 4000,
      horizontalPosition: horizontalPosition || 'center',
      verticalPosition: verticalPosition ||  'top',
      data: { message, snackType, snackBar: this.snackBar}
    });
  }
}
