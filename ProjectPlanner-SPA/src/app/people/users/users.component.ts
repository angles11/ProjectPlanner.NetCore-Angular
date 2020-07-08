import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { UserService } from 'src/app/_services/user.service';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { PageEvent } from '@angular/material/paginator';
import { Pagination, PaginatedResult } from 'src/app/_models/pagination';
import { MySnackBarService } from 'src/app/_notifications/my-snackBar.service';
import { FormControl } from '@angular/forms';

interface SortingOption {
  value: string;
  viewValue: string;
  viewIcon: string;
}

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class UsersComponent implements OnInit {

  users: User[];
  numbers: number[] = [1, 2, 3, 4, 5, 6];
  pageSizeOptions: number[] = [10, 25, 50, 100];
  pageEvent: PageEvent;
  pagination: Pagination;
  searchTerm = '';
  userParams: any = {};
  gender: string;
  sortingOptions: SortingOption[] = [
    {
      value: 'nameAscending',
      viewValue: 'Name',
      viewIcon: 'arrow_drop_up'
    },
    {
      value: 'nameDescending',
      viewValue: 'Name',
      viewIcon: 'arrow_drop_downa'
    },
    {
      value: 'positionAscending',
      viewValue: 'Position',
      viewIcon: 'arrow_drop_up'
    },
    {
      value: 'positionDescending',
      viewValue: 'Position',
      viewIcon: 'arrow_drop_down'
    },
    {
      value: 'employerAscending',
      viewValue: 'Employer',
      viewIcon: 'arrow_drop_up'
    },
    {
      value: 'employerDescending',
      viewValue: 'Employer',
      viewIcon: 'arrow_drop_down'
    },
    {
      value: 'countryAscending',
      viewValue: 'Country',
      viewIcon: 'arrow_drop_up'
    },
    {
      value: 'countryDescending',
      viewValue: 'Country',
      viewIcon: 'arrow_drop_down'
    }];

  sortingOption: SortingOption;

  constructor(private route: ActivatedRoute,
              private userService: UserService,
              private authService: AuthService,
              private snackBar: MySnackBarService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.users = data.users.result;
      this.pagination = data.users.pagination;
    });
    this.userParams.orderBy = 'nameAscending';
    this.userParams.gender = 'all';
    this.gender = 'all';
  }

  pageChanged(event?: PageEvent) {
    this.pagination.pageIndex = event.pageIndex;
    this.pagination.pageSize = event.pageSize;
    this.loadUsers();
    return event;
  }

  loadUsers() {
    this.userService.getUsers(
      this.authService.decodedToken.nameid,
      this.pagination.pageIndex,
      this.pagination.pageSize,
      this.searchTerm,
      this.userParams).subscribe((response: PaginatedResult<User[]>) => {
        this.users = response.result;
        this.pagination = response.pagination;
      }, error => {
        this.snackBar.openSnackBar(error, 'error', 4000);
      });
  }

  orderByChange() {
    this.userParams.orderBy = this.sortingOption.value;
    this.loadUsers();
  }

  genderChange() {
    this.userParams.gender = this.gender;
    this.loadUsers();
  }

}

