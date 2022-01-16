import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { UserParams } from '../_models/userParams';
import { User } from '../_models/users';
import { MembersService } from './members.service';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient, private memberService: MembersService) { }

  getUsersWithRoles() {
    return this.http.get<Partial<User[]>>(this.baseUrl + 'admin/users-with-roles');
  }

  updateUserRoles(username: string, roles: string[]) {
    return this.http.post(this.baseUrl + 'admin/edit-roles/' + username + '?roles=' + roles, {})
  }

  getMembers(gender: string) {
    return this.http.get<Member[]>(this.baseUrl + 'users?gender=' + gender);
  }

  deletePhoto(photoId: number, userId: string) {
    return this.http.delete(this.baseUrl + 'admin/delete-photo/' + userId + "/" + photoId);
  }
}
