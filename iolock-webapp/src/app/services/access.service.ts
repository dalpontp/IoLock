import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccessPassword } from '../models/access_password';
import { AccessRequest } from '../models/access_request';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class AccessService {

  constructor(private http: HttpClient) { }

  getAccessPassword(accessRequest: AccessRequest) : Observable<AccessPassword> {
    return this.http.post<AccessPassword>(`${environment.apiUrl}/Access`, accessRequest);
  }

  revokeAccessPermission(room: string, building: string, email: string) : Observable<number>{
    return this.http.delete<number>(`${environment.apiUrl}/Users/${email}/Rooms/${room}/Building/${building}`);
  }

  giveAccessPermission(room: string, building: string, email: string) : Observable<number>{
    return this.http.post<number>(`${environment.apiUrl}/Users/${email}/Rooms/${room}/Building/${building}`, null);
  }
}
