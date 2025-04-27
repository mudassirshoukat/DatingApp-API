import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from 'src/environments/environment'; // Adjust to your environment
import { ToastrService } from 'ngx-toastr';
import { MessageService } from './message.service'; // Adjust to your service for handling messages

@Injectable({
  providedIn: 'root'
})
export class SimpleHubService {
  private hubUrl = environment.samplehubUrl;
  private hubConnection?: HubConnection;

  constructor(private toast: ToastrService, private messageService: MessageService) { }

  // Create and start the SignalR connection
  createHubConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'simplehub')  // URL to your backend hub
      .withAutomaticReconnect()  // Optional: Automatically reconnect if the connection drops
      .build();

    // Start the connection
    this.hubConnection.start()
      .catch(error => console.log('Error starting SignalR connection: ', error));

    // Handle events from the SignalR server

    // Listen to the 'GetConversations' event
    this.hubConnection.on('GetConversations', (message: string) => {
      this.toast.info(message); // Show toast notification
      console.log(message); // Log the message if necessary
    });

    // Listen to the 'ReceivePing' event
    this.hubConnection.on('ReceivePing', (message: string) => {
      this.toast.info(message); // Show toast notification
      console.log(message); // Log the message if necessary
    });

 
  }

  // Stop the connection
  stopHubConnection() {
    this.hubConnection?.stop()
      .catch(error => console.log('Error stopping SignalR connection: ', error));
  }

  // Optional: Ping the server
  pingServer() {
    if (this.hubConnection) {
      this.hubConnection.send('Ping')
        .catch(error => console.log('Error sending Ping: ', error));
    }
  }
}
