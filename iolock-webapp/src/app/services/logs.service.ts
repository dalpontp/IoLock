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
    return this.http.get<Log[]>(`${environment.apiUrl}/Logs?bearer=`);
  }

  getUserLogs(email: string) : Observable<Log[]> {
    return this.http.get<Log[]>(`${environment.apiUrl}/Logs/Users/${email}`);
  }

  addUserLog(email: string, room: Room, building: Building) : Observable<number> {
    return this.http.post<number>(`${environment.apiUrl}/Logs/Users${email}?&room=${room.room}&building=${building.building}`, null);
  }
}
