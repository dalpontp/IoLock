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
  public userRoles: string[] = [];

  constructor(private readonly keycloak: KeycloakService, private route: Router) { }

  public async ngOnInit() {
    this.isLogged = await this.keycloak.isLoggedIn();
    this.userRoles = await this.keycloak.getUserRoles();

    type userRoles = Array<{id: number, text: string}>;

    if(this.isLogged) {
      this.userProfile = await this.keycloak.loadUserProfile();
      this.userRoles.includes('app-admin') ? this.route.navigate([`/users`]) : this.route.navigate([`/code`]);
    } else {
      console.log('is not logged')
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
