import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { KeycloakService } from 'keycloak-angular';
import { UsersService } from 'src/app/services/users.service';
import { User } from '../../models/user'
import { RoomBuilding } from '../../models/room_building'


import { MatTableDataSource } from '@angular/material/table';
import { FormControl } from '@angular/forms';
import { BuildingsService } from 'src/app/services/buildings.service';
import { Building } from 'src/app/models/building';
import { Room } from 'src/app/models/room';
import { AccessService } from 'src/app/services/access.service';
import { Log } from 'src/app/models/log';
import { LogsService } from 'src/app/services/logs.service';

@Component({
  selector: 'app-administration',
  templateUrl: './administration.component.html',
  styleUrls: ['./administration.component.css']
})
export class AdministrationComponent implements OnInit {
  building: Building | undefined;
  selected: boolean = false
  public isLogged: boolean = false;
  public userToken: string = '';
  displayedUserColumns: string[] = ['givenName', 'familyName', 'email', 'preferredUsername', 'emailVerified'];
  displayedRoomsColumns: string[] = ['room', 'building', 'delete'];
  displayedLogsColumns: string[] = ['userGivenName', 'userFamilyName', 'userEmail', 'room', 'building', 'accessDatetime'];
  users: MatTableDataSource<User> = new MatTableDataSource();
  rooms: MatTableDataSource<RoomBuilding> = new MatTableDataSource();
  logs: MatTableDataSource<Log> = new MatTableDataSource();
  userLogs: MatTableDataSource<Log> = new MatTableDataSource();
  buildings = new FormControl('');

  buildingsList: Building[] = [];
  roomsList: Room[] = [];

  constructor(private userService : UsersService,
    private buildingsService : BuildingsService,
    private readonly keycloak: KeycloakService,
    private route: Router,
    private activatedRoute: ActivatedRoute,
    private accessService: AccessService,
    private logsService: LogsService
  ) { }

  public async ngOnInit() {
    this.selected = !this.activatedRoute.snapshot.paramMap.get('email') ? false : true
    this.isLogged = await this.keycloak.isLoggedIn();

    if(this.isLogged) {
      this.userToken = await this.keycloak.getToken();
      this.userService.getUsers(this.userToken);

      if(!this.selected) {
        this.userService.getUsers(this.userToken).subscribe({
          next: users => {
            this.users = new MatTableDataSource(users);
          },
          error: err => console.error(err)
        })

        this.logsService.getLogs(this.userToken).subscribe({
          next: logs => {
            this.logs = new MatTableDataSource(logs);
          },
          error: err => console.error(err)
        });
      } else {
        this.buildingsService.getBuildings(this.userToken).subscribe({
          next: buildings => {
            this.buildingsList = buildings;
          }
        })

        this.userService.getUserAvailableRooms(this.userToken, this.activatedRoute.snapshot.paramMap.get('email')!).subscribe({
          next: rooms => {
            this.rooms = new MatTableDataSource(rooms);
          },
          error: err => console.error(err)
        })

        this.logsService.getUserLogs(this.userToken, this.activatedRoute.snapshot.paramMap.get('email')!).subscribe({
          next: userLogs => {
            console.log(userLogs)
            this.userLogs = new MatTableDataSource(userLogs);
          },
          error: err => console.error(err)
        });
      }
    } else {
      this.route.navigate([`/prelogin`]);
    }
  }

  public logout(){
    this.keycloak.logout();
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.users.filter = filterValue.trim().toLowerCase();
  }

  applyFilterOnLogs(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.logs.filter = filterValue.trim().toLowerCase();
  }

  applyFilterOnUserLogs(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.userLogs.filter = filterValue.trim().toLowerCase();
  }

  buildingSelected(building: Building) {
    this.building = building;
    this.buildingsService.getBuildingRooms(this.userToken, building).subscribe({
      next: rooms => {
        this.roomsList = rooms;
      }
    })
  }

  deletePermission(element: RoomBuilding) {
    this.accessService.revokeAccessPermission(this.userToken, element.room, element.building, this.activatedRoute.snapshot.paramMap.get('email')!)
    .subscribe({
      next: res => {
        if (res){
          const data  = this.rooms.data;
          const idx = data.findIndex(x=>x.building == element.building && x.room == element.room);
          data.splice(idx,1);
          this.rooms.data = data;
        }
        console.log(res)
      }
    })
    // this.route.navigate([`/users`], );
  }
  roomSelected (room: Room) {
    console.log(this.building?.building)
    console.log(room.room)
    if(this.building?.building != null || this.building?.building != undefined) {
      this.accessService.giveAccessPermission(this.userToken, room.room.trim(), this.building!.building.trim(), this.activatedRoute.snapshot.paramMap.get('email')!)
      .subscribe({
        next: res => {
          console.log(res)
        }
      });
    }
    this.route.navigate([`/users`]);
  }

}
