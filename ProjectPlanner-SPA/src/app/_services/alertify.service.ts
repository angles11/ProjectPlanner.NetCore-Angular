import { Injectable } from '@angular/core';
import * as alertify from 'alertifyjs';

@Injectable({
  providedIn: 'root',
})
export class AlertifyService {
  constructor() {
    alertify.set('notifier', 'position', 'top-center');
  }

  confirm(title: string, message: string, okCallback: () => any, cancelCallback: () => any) {
    alertify.confirm(
      title,
      message,
      (e: any) => {
        if (e) {
          okCallback();
        } else {
        }
      },
      null
    );
  }

  success(message: string) {
    alertify.success(message);
  }

  error(message: string) {
    alertify.error(message);
  }

  warning(message: string) {
    alertify.warning(message);
  }

  message(message: string) {
    alertify.message(message);
  }
}
