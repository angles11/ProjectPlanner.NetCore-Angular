import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { UserService } from 'src/app/_services/user.service';
import { Friend } from 'src/app/_models/friend';
import { MySnackBarService } from 'src/app/_notifications/my-snackBar.service';
export { };

@Component({
  selector: 'app-friends',
  templateUrl: './friends.component.html',
  styleUrls: ['./friends.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class FriendsComponent implements OnInit {
  friends: Friend[];
  acceptedFriends: Friend[] = [];
  friendRequestsReceived: Friend[] = [];
  friendRequestsSent: Friend[] = [];
  blockedFriends: Friend[] = [];
  numbers = [1, 2, 3, 4, 5, 6];

  constructor(private authService: AuthService, private userService: UserService,
    private route: ActivatedRoute, private snackBar: MySnackBarService) { }

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.friends = data.friends;
    }, error => {
      this.snackBar.openSnackBar(error, 'error', 5000);
    });
    this.mapFriends(this.friends);
  }

  mapFriends(friends: Friend[]) {
    friends.forEach((friend) => {
      if (friend.status === 1) {
        this.acceptedFriends.push(friend);
      }
      else if (friend.status === 0) {
        if (friend.sentById === friend.userFriend.id) {
          this.friendRequestsReceived.push(friend);
        }
        else {
          this.friendRequestsSent.push(friend);
        }
      }
      else {
        this.blockedFriends.push(friend);
      }
    });
  }

  onChangedStatus(event: any) {
    let index: number;
    switch (event.status) {
      case 'Accepted':
        index = this.findIndex(this.friendRequestsReceived, event.id);
        if (index !== -1) {
          const user = this.friendRequestsReceived.splice(index, 1)[0];
          this.acceptedFriends.push(user);
          this.acceptedFriends.sort((a, b) => a.userFriend.knownAs.localeCompare(b.userFriend.knownAs));
        }
        break;
      case 'Declined':
        index = this.findIndex(this.friendRequestsReceived, event.id);
        if (index !== -1) {
          this.friendRequestsReceived.splice(index, 1);
        }
        break;
      case 'Deleted':
        index = this.findIndex(this.acceptedFriends, event.id);
        if (index !== -1) {
          this.acceptedFriends.splice(index, 1);
        }
        break;
      case 'Cancelled':
        index = this.findIndex(this.friendRequestsSent, event.id);
        if (index !== -1) {
          this.friendRequestsSent.splice(index, 1);
        }
        break;
    }
  }

  findIndex(array: Friend[], id: string): number {
    return array.findIndex(x => x.userFriend.id === id);
  }

}
