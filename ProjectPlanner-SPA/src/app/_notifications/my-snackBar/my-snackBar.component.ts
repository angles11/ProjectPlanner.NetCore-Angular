import { Component, OnInit, Inject } from '@angular/core';
import { MAT_SNACK_BAR_DATA } from '@angular/material/snack-bar';

@Component({
  selector: 'app-my-snackBar',
  templateUrl: './my-snackBar.component.html',
  styleUrls: ['./my-snackBar.component.css']
})
export class MySnackBarComponent implements OnInit {
  icon: string;
  class: string;

  constructor(@Inject(MAT_SNACK_BAR_DATA) public data: any) {
  }

  ngOnInit() {
    switch (this.data.snackType) {
      case 'success':
        this.icon = 'done';
        this.class = 'success';
        break;
      case 'error':
        this.icon = 'error';
        this.class = 'error';
        break;
      case 'warn':
        this.icon = 'warning';
        this.class = 'warning';
        break;
      case 'info':
        this.icon = 'info';
        this.class = 'info';
        break;
    }
  }
}
