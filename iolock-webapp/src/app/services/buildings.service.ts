import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Building } from '../models/building';
import { Room } from '../models/room';

@Injectable({
  providedIn: 'root'
})
export class BuildingsService {

  constructor(private http: HttpClient) { }

  getBuildings(bearer: string) : Observable<Building[]> {
    return this.http.get<Building[]>('https://localhost:7117/api/Buildings?bearer=' + bearer);
  }

  getBuildingRooms(bearer: string, building: Building) : Observable<Room[]> {
    return this.http.get<Room[]>(`https://localhost:7117/api/Buildings/${building.building}/Rooms?bearer=${bearer}`);
  }
}
