import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../models/user';
import { RoomBuilding } from '../models/room_building';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  constructor(private http: HttpClient) { }

  getUsers() : Observable<User[]> {
    var a = this.http.get<User[]>(`${environment.apiUrl}/Users`);
    a.subscribe(x => console.log(x));
    return a;
  }

  getUsersByEmail(email: string) : Observable<User[]> {
    return this.http.get<User[]>(`${environment.apiUrl}/Users/${email}`);
  }
  getUserAvailableRooms(email: string) : Observable<RoomBuilding[]> {
    return this.http.get<RoomBuilding[]>(`${environment.apiUrl}/Users/${email}/Rooms`);
  }
}
