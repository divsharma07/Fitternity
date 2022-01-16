import { Component, OnInit } from '@angular/core';
import { NgxGalleryAnimation, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { forkJoin, Observable } from 'rxjs';
import { PhotosModalComponent } from 'src/app/modals/photos-modal/photos-modal.component';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/users';
import { AdminService } from 'src/app/_services/admin.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  members: Member[]
  bsModalRef: BsModalRef;

  constructor(private adminService: AdminService, private modalService : BsModalService) { }

  ngOnInit(): void {
    let males = this.adminService.getMembers("male")
    let females = this.adminService.getMembers("female")
    forkJoin([males, females]).subscribe(((members: any[]) => {
      this.members = members[0].concat(members[1]);
    }))
  }

  openPhotosModal(member: Member) {
    const config = {
      class: 'modal-dialog.centered',
      initialState: {
        member
      }
    }
      this.bsModalRef = this.modalService.show(PhotosModalComponent, config);
    }
  }

