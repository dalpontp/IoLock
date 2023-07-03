import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../models/user';
import { RoomBuilding } from '../models/room_building';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  constructor(private http: HttpClient) { }

  getUsers() : Observable<User[]> {
    return this.http.get<User[]>('https://localhost:7117/api/Users');
  }

  getUsersByEmail(email: string) : Observable<User[]> {
    return this.http.get<User[]>(`https://localhost:7117/api/Users/${email}`);
  }
  getUserAvailableRooms(email: string) : Observable<RoomBuilding[]> {
    return this.http.get<RoomBuilding[]>(`https://localhost:7117/api/Users/${email}/Rooms`);
  }
}
