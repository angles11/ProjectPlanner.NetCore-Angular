import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ProjectsComponent } from './projects/projects.component';
import { AuthGuard } from './_guards/auth.guard';
import { PeopleComponent } from './people/people.component';
import { FriendsComponent } from './people/friends/friends.component';
import { UsersComponent } from './people/users/users.component';

export const appRoutes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: '', component: ProjectsComponent, pathMatch: 'full', canActivate: [AuthGuard] },
  { path: 'projects', component: ProjectsComponent, canActivate: [AuthGuard] },
  {
    path: 'people', component: PeopleComponent, canActivate: [AuthGuard], children: [
      {
        path: '', component: FriendsComponent
      },
      {
      path: 'friends', component: FriendsComponent
      },
      {
        path: 'users', component: UsersComponent
      }
  ]}

];
