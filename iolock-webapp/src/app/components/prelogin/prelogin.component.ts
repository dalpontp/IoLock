import { Component } from '@angular/core';

@Component({
  selector: 'app-prelogin',
  templateUrl: './prelogin.component.html',
  styleUrls: ['./prelogin.component.css']
})
export class PreloginComponent {
  // public async ngOnInit() {
  //   this.isLogged = await this.keycloak.isLoggedIn();

  //   type userRoles = Array<{id: number, text: string}>;

  //   if(this.isLogged) {
  //     this.userProfile = await this.keycloak.loadUserProfile();
  //   }
  // }
}
