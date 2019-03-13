import { Component, OnInit, ViewChild, ElementRef } from "@angular/core";
import { Message } from "../model/message.model";
import { MessageTypeConstants } from "../../constants";
import { ChatService } from "../services/chat.service";

@Component({
  selector: 'game-chatbox-component',
  templateUrl: './chatbox.component.html',
  styleUrls: ['./chatbox.component.css']
})
export class ChatboxComponent implements OnInit {
  constructor(private chatService: ChatService) {
    this.receivedMessages = [];
    this.message = new Message();
    this.message.messageType = MessageTypeConstants.local;
  }

  ngOnInit(): void {
    this.chatService.onReceiveMessage(this.receiveMessage.bind(this));
  }
  @ViewChild("container") private container: ElementRef;
  private receivedMessages: Message[];
  private message: Message;

  private receiveMessage(message: Message): void {
    this.receivedMessages.push(message);
    this.container.nativeElement.scrollTop = this.container.nativeElement.scrollHeight;
  }

  public sendMessageAsync(): void {
    this.chatService.sendMessageAsync(this.message);
    this.message.content = "";
  }
}
