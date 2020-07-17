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

export const appRoutes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: '', component: ProjectsComponent, pathMatch: 'full', canActivate: [AuthGuard],
    resolve: { projects: ProjectsListResolver, friends: FriendsResolver }
  },
  {
    path: 'projects', component: ProjectsComponent, canActivate: [AuthGuard],
    resolve: { projects: ProjectsListResolver, friends: FriendsResolver }
  },
  {
    path: 'projects/:id', component: ProjectDetailComponent, canActivate: [AuthGuard],
    resolve: {project: ProjectDetailResolver}
  },
  {
    path: 'people', component: PeopleComponent, canActivate: [AuthGuard],
    children: [
      {
        path: '', redirectTo: 'friends', pathMatch: 'full'
      },
      {
        path: 'friends', component: FriendsComponent, resolve: { friends: FriendsResolver }
      },
      {
        path: 'users', component: UsersComponent, resolve: { users: UsersResolver }
      }
    ]
  }

];
