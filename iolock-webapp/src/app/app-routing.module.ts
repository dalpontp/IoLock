import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { UserCodeComponent } from './components/user-code/user-code.component';
import { AdministrationComponent } from './components/administration/administration.component';
import { PreloginComponent } from './components/prelogin/prelogin.component';

// const routes : Routes =  [
//   { path: '', component: HomeComponent}
// ];

const routes: Routes = [
  {path: '', component: HomeComponent, children: [
    {path: '', redirectTo: 'prelogin', pathMatch: 'full'},
    {path: 'prelogin', component: PreloginComponent},
    {path: 'code', component: UserCodeComponent},
    {path: 'administration', component: AdministrationComponent}
  ]},
]

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
