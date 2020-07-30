import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ProjectsComponent } from './projects/projects.component';
import { AuthGuard } from './_guards/auth.guard';
import { PeopleComponent } from './people/people.component';
import { FriendsComponent } from './people/friends/friends.component';
import { UsersComponent } from './people/users/users.component';
import { FriendsResolver } from './resolvers/friends.resolver';
import { UsersResolver } from './resolvers/users.resolver';
import { ProjectsListResolver } from './resolvers/projects-list.resolver';
import { ProjectDetailComponent } from './projects/project-detail/project-detail.component';
import { ProjectDetailResolver } from './resolvers/project-detail.resolver';
import { ConfirmEmailComponent } from './register/confirm-email/confirm-email.component';
import { AccountComponent } from './account/account.component';
import { AccountResolver } from './resolvers/account.resolver';

export const appRoutes: Routes = [
  { path: 'login', component: LoginComponent, data: { animation: 'LoginPage' }},
  { path: 'register', component: RegisterComponent, data: { animation: 'RegisterPage' } },
  {path: 'account', component: AccountComponent, canActivate:[AuthGuard], resolve: {user: AccountResolver} },
  {path: 'register/ConfirmEmail', component: ConfirmEmailComponent},
  {
    path: '', component: ProjectsComponent, pathMatch: 'full', canActivate: [AuthGuard],  data: {animation: 'ProjectsPage'},
    resolve: { projects: ProjectsListResolver, friends: FriendsResolver }
  },
  {
    path: 'projects', component: ProjectsComponent, canActivate: [AuthGuard], data: {animation: 'ProjectsPage'},
    resolve: { projects: ProjectsListResolver, friends: FriendsResolver }
  },
  {
    path: 'projects/:id', component: ProjectDetailComponent, canActivate: [AuthGuard], data: {animation: 'ProjectPage'},
    resolve: {project: ProjectDetailResolver}
  },
  {
    path: 'people', component: PeopleComponent, canActivate: [AuthGuard], data: {animation: 'PeoplePage'},
    children: [
      {
        path: '', redirectTo: 'friends', pathMatch: 'full'
      },
      {
        path: 'friends', component: FriendsComponent, data: {animation: 'FriendsPage'}, resolve: { friends: FriendsResolver }
      },
      {
        path: 'users', component: UsersComponent, data: {animation: 'UsersPage'}, resolve: { users: UsersResolver }
      }
    ]
  }

];
