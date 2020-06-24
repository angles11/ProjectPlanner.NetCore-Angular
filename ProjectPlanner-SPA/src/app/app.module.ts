import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { JwtModule } from '@auth0/angular-jwt';
import { ErrorInterceptorProvider } from './_services/error.inteceptor';

import { FlexLayoutModule } from '@angular/flex-layout';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatRadioModule } from '@angular/material/radio';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';


import { ProjectsComponent } from './projects/projects.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { NavComponent } from './nav/nav.component';
import { appRoutes } from './routes.routing';
import { MySnackBarComponent } from './_notifications/my-snackBar/my-snackBar.component';
import { MatNativeDateModule, DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { PeopleComponent } from './people/people.component';
import { FriendsComponent } from './people/friends/friends.component';
import { UsersComponent } from './people/users/users.component';
import { APP_DATE_FORMATS, AppDateAdapter } from './_helpers/format-datepicker';



export function tokenGetter() {
   return localStorage.getItem('token');
}


@NgModule({
   declarations: [
      AppComponent,
      ProjectsComponent,
      LoginComponent,
      RegisterComponent,
      NavComponent,
      MySnackBarComponent,
      PeopleComponent,
      FriendsComponent,
      UsersComponent
   ],
   imports: [
      BrowserModule,
      HttpClientModule,
      FormsModule,
      ReactiveFormsModule,
      FlexLayoutModule,
      BrowserAnimationsModule,
      RouterModule.forRoot(appRoutes),
      MatToolbarModule,
      MatButtonModule,
      MatSidenavModule,
      MatIconModule,
      MatListModule,
      MatMenuModule,
      MatFormFieldModule,
      MatInputModule,
      MatDividerModule,
      MatSnackBarModule,
      MatRadioModule,
      MatDatepickerModule,
      MatNativeDateModule,
      MatSelectModule,
      MatTabsModule,
      JwtModule.forRoot({
         config: {
            tokenGetter,
            whitelistedDomains: ['localhost:5000'],
            blacklistedRoutes: ['localhost:5000/api/auth'],
         },
      }),
   ],
   providers: [
      MatDatepickerModule,
      ErrorInterceptorProvider,
      { provide: DateAdapter, useClass: AppDateAdapter },
      { provide: MAT_DATE_FORMATS, useValue: APP_DATE_FORMATS }
   ],
   bootstrap: [
      AppComponent
   ],
   entryComponents: [
      MySnackBarComponent
   ]
})
export class AppModule { }
