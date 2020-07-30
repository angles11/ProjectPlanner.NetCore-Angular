import { Component, OnInit } from '@angular/core';
import {slideInAnimation } from '../_helpers/animations';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-people',
  templateUrl: './people.component.html',
  styleUrls: ['./people.component.css'],
  animations: [slideInAnimation]
})
export class PeopleComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

  prepareRoute(outlet: RouterOutlet) {
    return outlet && outlet.activatedRouteData && outlet.activatedRouteData.animation;
  }
}
