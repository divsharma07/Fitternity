import { Component, Input, OnInit } from '@angular/core';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Member } from 'src/app/_models/member';
import { AdminService } from 'src/app/_services/admin.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-photos-modal',
  templateUrl: './photos-modal.component.html',
  styleUrls: ['./photos-modal.component.css']
})
export class PhotosModalComponent implements OnInit {
  @Input() username : string;
  member: Member;
  galleryOptions!: NgxGalleryOptions[];
  galleryImages!: NgxGalleryImage[];


  constructor(private memberService: MembersService, public bsModalRef: BsModalRef, private adminService: AdminService) { }

  ngOnInit(): void {
    console.log(this.member.photos)
  }

  deletePhoto(photoId: number) {
    this.adminService.deletePhoto(photoId, this.member.username).subscribe(() => {
      this.member.photos = this.member.photos.filter(x => x.id !== photoId);
    })
  }
}
