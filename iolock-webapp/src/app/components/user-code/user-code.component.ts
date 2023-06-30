import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { KeycloakService } from 'keycloak-angular';
import { KeycloakProfile } from 'keycloak-js';
import { AccessService } from 'src/app/services/access.service';
import { AccessRequest } from 'src/app/models/access_request';

@Component({
  selector: 'app-user-code',
  templateUrl: './user-code.component.html',
  styleUrls: ['./user-code.component.css']
})
export class UserCodeComponent {
  public isLogged: boolean = false;
  public userProfile: KeycloakProfile | null = null;
  public userToken: string = '';
  public accessPassword: number = -1;

  constructor(private accessService: AccessService, private readonly keycloak: KeycloakService, private route: Router) { }

  public async ngOnInit() {
    this.isLogged = await this.keycloak.isLoggedIn();

    type userRoles = Array<{id: number, text: string}>;

    if(this.isLogged) {
      this.userProfile = await this.keycloak.loadUserProfile();
      this.userToken = await this.keycloak.getToken();
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
      bearer: this.userToken
    }
    this.accessService.getAccessPassword(accessRequest).subscribe((response: any) => {
      this.accessPassword = response == null ? 0 : response;
    });
  }
}
