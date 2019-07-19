import { Component, OnInit, ViewChild, ElementRef, HostListener } from "@angular/core";
import { Message } from "../model/message.model";
import { MessageTypeConstants } from "../../constants";
import { ChatService } from "../services/chat.service";
import { setTimeout } from "timers";

/**Component responsible for displaying and allowing the user to send messages from and to the server. */
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
  @ViewChild("messageBox") private messageBox: ElementRef;
  public receivedMessages: Message[];
  public message: Message;
  public isTextboxFocused: boolean = false;

  /**
   * Called on keydown event.
   *
   * If the textbox is focused and enter is pressed, will send the currently stored message and set the focus back
   * to the textbox.
   * @param event
   */
  @HostListener('document: keydown', ['$event'])
  public onKey(event: KeyboardEvent): void {
    if (this.isTextboxFocused) {
      if (event.key === "Enter") {
        this.sendMessageAsync();
        this.messageBox.nativeElement.focus();
      }
    }
  }

  /**
   * Called when a message is received from the server.
   *
   * Adds the message to the receivedMessages array and keeps the scroll bar at the bottom if it is currently at the bottom.
   * @param message The message received from the server.
   */
  private receiveMessage(message: Message): void {
    let isScrolledToBottom: boolean = this.container.nativeElement.scrollHeight - this.container.nativeElement.clientHeight
                                      <= this.container.nativeElement.scrollTop + 1;

    this.receivedMessages.push(message);
    if (this.receivedMessages.length > 100) this.receivedMessages.shift();

    if (isScrolledToBottom) {
      setTimeout(() => {
        this.container.nativeElement.scrollTop = this.container.nativeElement.scrollHeight
      }, 0);
    }
  }

  /**Sends the currently stored message to the server. */
  public sendMessageAsync(): void {
    this.chatService.sendMessageAsync(this.message);
    this.message.content = "";
  }
}
