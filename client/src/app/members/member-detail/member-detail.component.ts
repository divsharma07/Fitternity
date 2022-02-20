import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions, NgxGalleryModule } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/message';
import { PresenceService } from 'src/app/_services/presence.service';
import { AccountService } from 'src/app/_services/account.service';
import { take } from 'rxjs/operators';
import { User } from 'src/app/_models/users';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  galleryOptions!: NgxGalleryOptions[];
  galleryImages!: NgxGalleryImage[];
  member: Member | undefined;
  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent;
  activeTab: TabDirective;
  messages: Message[] = []
  user: User

  constructor(private memberService: MembersService, private route: ActivatedRoute,
     private messageService: MessageService, public presenceService: PresenceService,
     private accountService: AccountService, private router: Router) { 
       this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
       this.router.routeReuseStrategy.shouldReuseRoute = () => false;
     }
 

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.member = data.member;
    })
    this.route.queryParams.subscribe(params => {
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);
    })
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ]
    this.galleryImages = this.memberService.getImages(this.member)
  }

  loadMessages() {
    this.messageService.getMessageThread(this.member.username).subscribe(messages => {
      this.messages = messages;
    })
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.messages.length === 0) {
      this.messageService.createHubConnection(this.user, this.member.username);
      this.loadMessages();
    } else {
      this.messageService.stopHubConnection();
    }
  }

  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }
}
