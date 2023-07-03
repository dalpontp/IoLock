import { HttpClient, HttpParams, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Building } from '../models/building';
import { Room } from '../models/room';
import { Log } from '../models/log';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class LogsService {

  constructor(private http: HttpClient) { }

  getLogs() : Observable<Log[]> {
    return this.http.get<Log[]>('https://localhost:7117/api/Logs?bearer=');
  }

  getUserLogs(email: string) : Observable<Log[]> {
    return this.http.get<Log[]>(`https://localhost:7117/api/Logs/Users/${email}`);
  }

  addUserLog(email: string, room: Room, building: Building) : Observable<number> {
    return this.http.post<number>(`https://localhost:7117/api/Logs/Users${email}?&room=${room.room}&building=${building.building}`, null);
  }
}
