import { Injectable, NgZone } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { UserModel } from '../_Models/UserModel';
import { BehaviorSubject, take } from 'rxjs';
import { MessageResponseModel } from '../_Models/MessageResponseModel';
import { MessageService } from './message.service';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {

  hubUrl = environment.hubUrl
  private hubConnection?: HubConnection;
  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUserSource.asObservable();

  constructor(private toast: ToastrService, private messageService: MessageService) { }



  createHubConnection(user: UserModel) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', { accessTokenFactory: () => user.Token })
      .withStatefulReconnect()
      .build();
    this.hubConnection.start().catch(error => console.log(error))

    this.hubConnection.on('UserIsOnline', newOnlineUser => {

      this.onlineUsers$.pipe(take(1)).subscribe({
        next: onlineUsers => this.onlineUserSource.next([...onlineUsers, newOnlineUser])
      })

    })

    this.hubConnection.on('UserIsOffline', leaveUser => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: OnlineUsers => this.onlineUserSource.next(OnlineUsers.filter(x => x !== leaveUser))
      })
    })


    this.hubConnection.on("GetOnlineUsers", response => {
      if (response && response.Result) {
        const users = response.Result; // Extract the usernames from the "result" property
        this.onlineUserSource.next(users);

      }
    })


    this.hubConnection.on("NewMessageNotification", ({ Message, SenderKnownAs }: { Message: MessageResponseModel, SenderKnownAs: string }) => {

      this.toast.info("New Message From " + SenderKnownAs)
      this.messageService.pushNewUnreadMessage(Message);

    })

    this.hubConnection.on("GetUnReadMessages", ( messages: MessageResponseModel[]) => {

     this.messageService.setUnReadMessages(messages);

    })

  }


  stopHubConnection() {
    this.hubConnection?.stop().catch(error => console.log(error))
  }


}
