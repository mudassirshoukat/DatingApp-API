import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { UserModel } from '../_Models/UserModel';
import { PresenceService } from './presence.service';
import { environment } from 'src/environments/environment';
import { SimpleHubService } from './simple-hub.service';



@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private CurrentUserSource = new BehaviorSubject<UserModel | null>(null)
  CurrentUser$ = this.CurrentUserSource.asObservable()
  private BaseUrl = environment.ApiUrl


  constructor(private http: HttpClient,private presenceService:PresenceService,private simpleHubService:SimpleHubService) { }


  LogIn(model: any) {
    return this.http.post<UserModel>(this.BaseUrl + "account/Login", model).pipe(
      map((user: UserModel) => {
        
        console.log("logIn in accountservice")
         this.SetCurrentUser(user)
      

    
        return user;
      })
    )

  }


  Register(model: any) {
    return this.http.post<UserModel>(this.BaseUrl + "account/Register", model).pipe(
      map(user => {
        if (user) {
          this.CurrentUserSource.next(user);
          localStorage.setItem("user", JSON.stringify(user))
        }
        return user;
      })


    )

  }


  SetCurrentUser(user: UserModel) {
    user.Roles=[]
   const roles= this.getDecodedToken(user.Token)
   Array.isArray(roles) ?user.Roles=roles: user.Roles.push(roles);
    this.CurrentUserSource.next(user);
    localStorage.setItem("user", JSON.stringify(user))

    this.presenceService.createHubConnection(user)
    this.simpleHubService.createHubConnection()

  }



  Logout() {
    localStorage.removeItem("user");
    this.CurrentUserSource.next(null);
    this.presenceService.stopHubConnection();
  }

  IsLogIn(): boolean {
    let check: boolean = false
    this.CurrentUser$.pipe(
      map(user => {
        if (user) check = true;

      })
    ).subscribe()
    return check
  }


  getDecodedToken(token:string){
    return JSON.parse(atob(token.split('.')[1])).role
  }
 

}
