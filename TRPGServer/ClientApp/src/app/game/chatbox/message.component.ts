import { Component, Input } from "@angular/core";
import { Message } from "../model/message.model";
import { MessageTypeConstants } from "../../constants";

@Component({
  selector: 'chatbox-message-component',
  templateUrl: './message.component.html',
})
export class MessageComponent {
  @Input() message: Message;

  usernameStyles() {
    let _this = this;
    return { 'color': _this.message.isUserAdmin ? 'blue' : 'black' }
  }

  messageTypeStyles() {
    let _this = this;
    return { 'color': _this.getMessageTypeColor.bind(_this)() }
  }

  getMessageTypeColor(): string {
    if (this.message.messageType === MessageTypeConstants.local) return "grey";
    if (this.message.messageType === MessageTypeConstants.global) return "blue";
    if (this.message.messageType === MessageTypeConstants.private) return "purple";
    return "black";
  }
}
