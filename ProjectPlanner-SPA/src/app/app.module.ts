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
import { MatCardModule } from '@angular/material/card';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatDialogModule } from '@angular/material/dialog';


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
import { FriendsResolver } from './resolvers/friends.resolver';
import { UsersResolver } from './resolvers/users.resolver';
import { UserCardComponent } from './people/users/user-card/user-card.component';
import { FriendCardComponent } from './people/friends/friend-card/friend-card.component';
import { ConfirmDialogComponent } from './_notifications/confirm-dialog/confirm-dialog.component';
import { UserDialogComponent } from './people/user-dialog/user-dialog.component';




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
      UsersComponent,
      UserCardComponent,
      FriendCardComponent,
      ConfirmDialogComponent,
      UserDialogComponent
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
      MatCardModule,
      MatPaginatorModule,
      MatGridListModule,
      MatDialogModule,
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
      { provide: MAT_DATE_FORMATS, useValue: APP_DATE_FORMATS },
      FriendsResolver,
      UsersResolver
   ],
   bootstrap: [
      AppComponent
   ],
   entryComponents: [
      MySnackBarComponent,
      ConfirmDialogComponent,
      UserDialogComponent
   ]
})
export class AppModule { }
