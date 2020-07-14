import { NativeDateAdapter } from '@angular/material/core';
import { MatDateFormats } from '@angular/material/core';


export class AppDateAdapter extends NativeDateAdapter {
     format(date: Date, displayFormat: Object): string {
       if (displayFormat == "input") {
           const day = date.getDate();
           const month = date.getMonth() + 1;
           const year = date.getFullYear();
           return this._to2digit(day) + '/' + this._to2digit(month) + '/' + year;
       } else {
           return date.toDateString();
       }
   }

   private _to2digit(n: number) {
       return ('00' + n).slice(-2);
   }
}
export const APP_DATE_FORMATS: MatDateFormats = {
   parse: {
       dateInput: {month: 'short', year: 'numeric', day: 'numeric'}
   },
   display: {
       // dateInput: { month: 'short', year: 'numeric', day: 'numeric' },
       dateInput: 'input',
       monthYearLabel: {year: 'numeric', month: 'short'},
       dateA11yLabel: {year: 'numeric', month: 'long', day: 'numeric'},
       monthYearA11yLabel: {year: 'numeric', month: 'long'},
   }
};
