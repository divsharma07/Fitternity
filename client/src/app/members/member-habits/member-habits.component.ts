import { Component, ElementRef, Input, OnInit, Renderer2 } from '@angular/core';
import { Habit } from 'src/app/_models/habit';
import { HabitPair } from 'src/app/_models/habitPair';
import { HabitService } from 'src/app/_services/habit.service';
import * as confetti from 'canvas-confetti';
import { ToastrService } from 'ngx-toastr';
import { User } from 'src/app/_models/users';
import { AccountService } from 'src/app/_services/account.service';
import { take } from 'rxjs/operators';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-habits',
  templateUrl: './member-habits.component.html',
  styleUrls: ['./member-habits.component.css']
})
export class MemberHabitsComponent implements OnInit {
  @Input() username: string;
  memberHabits: Habit[];
  memberHabitPair: HabitPair[];
  public clicked = false;
  myCanvas: HTMLCanvasElement;
  user: User;

  constructor(private habitService: HabitService, private renderer2: Renderer2, private memberService: MembersService,
    private elementRef: ElementRef, private toastr: ToastrService, private accountService: AccountService) { }

  ngOnInit(): void {
    this.habitService.getUserHabits(this.username).subscribe(response => {
      this.memberHabits = response;
    });
    this.habitService.getAllHabitPairs(this.username).subscribe(response => {
      this.memberHabitPair = [...response];
    })
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
    })
  }

  addHabitPair(habitName: string) {
    this.habitService.addHabitPair(this.username, habitName).subscribe(response => {
      this.memberHabitPair.push(response);
    })
  }

  addHabitActivity(username: string, habitName: string) {
    this.habitService.addHabitActivity(username, habitName).subscribe(response => {
      this.toastr.success('You finished your ' + habitName + ' task today');
      this.congratulate();
    }, error => {
      this.toastr.error('You have already performed the action today')
    });
  }

  congratulate() {
    if(this.myCanvas == null) {
      this.myCanvas = document.createElement('canvas');
      this.myCanvas.width = 1000;
      this.myCanvas.height = 700;
      this.renderer2.appendChild(this.elementRef.nativeElement, this.myCanvas); 
    }
 
    const myConfetti = confetti.create(this.myCanvas, {
      resize: true, // will fit all screen sizes
    });
 
    myConfetti({particleCount: 150, spread:100});
  }

  random(min: number, max: number) {
    return Math.random() * (max - min) + min;
  }
}
