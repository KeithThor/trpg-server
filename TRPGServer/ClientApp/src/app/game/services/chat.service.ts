import { Injectable } from "@angular/core";
import { HubConnection, HubConnectionBuilder } from "@aspnet/signalr";
import { LocalStorageConstants } from "../../constants";
import { Message } from "../model/message.model";

/** Service responsible for sending and receiving player-to-player messages from the server. */
@Injectable()
export class ChatService {
  private connection: HubConnection;
  private receiveMessageCallbacks: { (message: Message): void; }[];

  /** Initializes this service asynchronously. */
  public async initializeAsync(): Promise<void> {
    this.receiveMessageCallbacks = [];
    this.connection = new HubConnectionBuilder()
      .withUrl("/hubs/chat", {
        accessTokenFactory: () => sessionStorage.getItem(LocalStorageConstants.authToken)
      })
      .build();
    this.connection.on("receiveMessage", (data) => this.receiveMessage(data));
    await this.connection.start()
      .catch((err) => console.log(err))
      .then(() => {
        this.connection.send("AddToMap");
      });
  }

  /**
   * Registers a callback with this service that is called whenever a message is received from the server.
   * @param callback The function that will be called whenever a message is received from the server.
   */
  public onReceiveMessage(callback: (message: Message) => void): void {
    this.receiveMessageCallbacks.push(callback);
  }

  /**
   * Calls each registered callback with the message that is received from the server.
   * @param message The message that was received from the server.
   */
  private receiveMessage(message: Message): void {
    this.receiveMessageCallbacks.forEach(callback => callback(message));
  }

  /**
   * Sends a message to the server asynchronously. The returned promise resolves when the message is sent.
   * @param message A message object containing the message type and message contents.
   */
  public sendMessageAsync(message: Message): Promise<void> {
    return this.connection.send("SendMessage", message);
  }

  /**
   * Sends a message to the server to change the map group of the player to the map the player is currently on.
   * @param oldMapId The id of the map the player was previously on.
   */
  public changeMapGroupAsync(oldMapId: number): Promise<void> {
    return this.connection.send("ChangeMapGroup", oldMapId);
  }

  /** Ends the connection with the server asynchronously. */
  public endConnectionAsync(): Promise<void> {
    console.log("Chatbox connection terminated");
    return this.connection.stop();
  }
}
