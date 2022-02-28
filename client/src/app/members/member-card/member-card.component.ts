import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Habit } from 'src/app/_models/habit';
import { Member } from 'src/app/_models/member';
import { HabitService } from 'src/app/_services/habit.service';
import { MembersService } from 'src/app/_services/members.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() member: Member | undefined; 
  memberHabits : Habit[];

  constructor(private membersService: MembersService, 
    private toastr: ToastrService, public presence: PresenceService, private habitService: HabitService) { }

  ngOnInit(): void {
    this.habitService.getUserHabits(this.member.username).subscribe(response => {
      this.memberHabits = response;
    });
  }

  addLike(member: Member) {
    this.membersService.addLike(member.username).subscribe(() =>{
      this.toastr.success('You have liked ' + member.knownAs);
    })
  }
}