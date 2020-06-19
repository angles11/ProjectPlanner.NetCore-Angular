/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { MySnackBarService } from './my-snackBar.service';

describe('Service: MySnackBar', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [MySnackBarService]
    });
  });

  it('should ...', inject([MySnackBarService], (service: MySnackBarService) => {
    expect(service).toBeTruthy();
  }));
});
