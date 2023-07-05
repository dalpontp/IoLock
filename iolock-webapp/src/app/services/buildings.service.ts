import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Building } from '../models/building';
import { Room } from '../models/room';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class BuildingsService {

  constructor(private http: HttpClient) { }

  getBuildings() : Observable<Building[]> {
    return this.http.get<Building[]>(`${environment.apiUrl}/Buildings`);
  }

  getBuildingRooms(building: Building) : Observable<Room[]> {
    return this.http.get<Room[]>(`${environment.apiUrl}/Buildings/${building.building}/Rooms`);
  }
}
