import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Building } from '../models/building';
import { Room } from '../models/room';
import { Log } from '../models/log';

@Injectable({
  providedIn: 'root'
})
export class LogsService {

  constructor(private http: HttpClient) { }

  getLogs(bearer: string) : Observable<Log[]> {
    return this.http.get<Log[]>('https://localhost:7117/api/Logs?bearer=' + bearer);
  }

  getUserLogs(bearer: string, email: string) : Observable<Log[]> {
    return this.http.get<Log[]>(`https://localhost:7117/api/Logs/Users/${email}?bearer=${bearer}`);
  }

  addUserLog(bearer: string, email: string, room: Room, building: Building) : Observable<number> {
    return this.http.post<number>(`https://localhost:7117/api/Logs/Users${email}?bearer=${bearer}&room=${room.room}&building=${building.building}`, null);
  }
}
