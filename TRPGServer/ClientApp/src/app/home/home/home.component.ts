import { Component, OnInit } from '@angular/core';
import { LocalStorageConstants } from '../../constants';
import { HubConnectionBuilder, HubConnection } from '@aspnet/signalr';
import { AccountService } from '../../shared/services/account.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  constructor(private account: AccountService) {

  }
  connection: HubConnection;
  ngOnInit(): void {
    this.connection = new HubConnectionBuilder().withUrl("/hubs/gamedata").build();
    this.connection.on("updateGameState", (data) => this.updateGameState(data));
    this.connection.on("receiveMessage", (data) => this.receiveMessage(data));
    this.connection.start();
    console.log(this.gameData);
  }
  message: string;
  gameData: string[] = [];
  getUsername(): string {
    return localStorage.getItem(LocalStorageConstants.username);
  }

  isLoggedIn(): boolean {
    return this.account.isAuthenticated();
  }

  logOut(): void {
    return this.account.logout();
  }

  updateGameState(data: string): void {
    this.gameData.push(data);
  }

  receiveMessage(message: string): void {
    this.gameData.push(message);
  }

  sendMessage(): void {
    this.connection.send("SendMessage", this.message);
    this.message = "";
  }
}
