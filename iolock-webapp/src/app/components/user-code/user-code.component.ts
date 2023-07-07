import { Component, OnInit, isDevMode  } from '@angular/core';
import { Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { KeycloakService } from 'keycloak-angular';
import { KeycloakProfile } from 'keycloak-js';
import { AccessService } from 'src/app/services/access.service';
import { AccessRequest } from 'src/app/models/access_request';
import { UsersService } from 'src/app/services/users.service';

@Component({
  selector: 'app-user-code',
  templateUrl: './user-code.component.html',
  styleUrls: ['./user-code.component.css']
})
export class UserCodeComponent implements OnInit {
  public isLogged: boolean = false;
  public userProfile: KeycloakProfile | null = null;
  public userToken: string = '';
  public accessPassword: number = 0;

  constructor(private userService : UsersService, private accessService: AccessService, private readonly keycloak: KeycloakService, private route: Router) { }

  public async ngOnInit() {
    this.isLogged = await this.keycloak.isLoggedIn();

    if(this.isLogged) {
      this.userProfile = await this.keycloak.loadUserProfile();
      this.userToken = await this.keycloak.getToken();
      this.userService.NewUserCheck().subscribe();
    } else {
      this.route.navigate([`/prelogin`]);
    }
  }

  public logout(){
    this.keycloak.logout();
  }

  onSubmit(form: NgForm) {
    // this.keycloak.getToken().then(value => this.userToken = value);
    const accessRequest : AccessRequest = {
      code: form.value.name,
      email: this.userProfile!.email!
    }

    // this.buildingsService.getBuildings().subscribe({
    //   next: buildings => {
    //     this.buildingsList = buildings;
    //   }
    // })

    this.accessService.getAccessPassword(accessRequest).subscribe((response: any) => {
      console.log(response)
      this.accessPassword = response == null ? 0 : response;
    });
  }
}
