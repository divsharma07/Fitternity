import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Habit } from '../_models/habit';
import { HabitPair } from '../_models/habitPair';
import { UserParams } from '../_models/userParams';
import { User } from '../_models/users';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class HabitService {
  user: User;
  baseUrl = environment.apiUrl + "habits";
  userParams: UserParams;

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(user);
    })
  }

  addHabitActivity(username: string, habitName: string) {
    return this.http.post(this.baseUrl + "/action/" + username, {"name": habitName});
  }

  addUserHabit() {
   // return this.http.post<>(this.baseUrl + "/pair/" + username, {});
  }

  addHabitPair(username: string, habitName: string) {
    return this.http.post<HabitPair>(this.baseUrl + "/" + username, {"name": habitName});
  }

  getAllHabitPairs(username: string) {
    return this.http.get<HabitPair[]>(this.baseUrl + "/pair/" + username);
  }

  getUserHabits(username: string) {
    return this.http.get<Habit[]>(this.baseUrl + "/" + username);
  }
}
