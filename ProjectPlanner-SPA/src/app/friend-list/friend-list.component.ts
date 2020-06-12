import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';

@Component({
  selector: 'app-friend-list',
  templateUrl: './friend-list.component.html',
  styleUrls: ['./friend-list.component.css'],
})
export class FriendListComponent implements OnInit {
  pendingFriends: User[];
  acceptedFriends: User[];
  blockedFriends: User[];
  users: User[];
  searchTerm: string;

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private alertify: AlertifyService,
    private userService: UserService
  ) {}

  ngOnInit() {
    this.route.data.subscribe(
      (data) => {
        this.pendingFriends = data.friends.pendingFriends;
        this.acceptedFriends = data.friends.acceptedFriends;
        this.blockedFriends = data.friends.blockedFriends;
        this.users = data.users;
        console.log(this.users);
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  getUsers() {
    this.userService.getUsers(this.authService.decodedToken.nameid, this.searchTerm).subscribe(
      (data) => {
        this.users = data;
        console.log(data);
        console.log(this.searchTerm);
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }
}
