import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { KeycloakService } from 'keycloak-angular';
import { KeycloakProfile } from 'keycloak-js';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  public isLogged: boolean = false;
  public userProfile: KeycloakProfile | null = null;

  constructor(private readonly keycloak: KeycloakService, private route: Router) { }

  public async ngOnInit() {
    this.isLogged = await this.keycloak.isLoggedIn();

    type userRoles = Array<{id: number, text: string}>;

    if(this.isLogged) {
      this.userProfile = await this.keycloak.loadUserProfile();
      this.route.navigate([`/code`]);
    } else {
      this.route.navigate([`/prelogin`]);
    }
  }

  public login() {
    this.keycloak.login();
  }

  public logout(){
    this.keycloak.logout();
  }

}
