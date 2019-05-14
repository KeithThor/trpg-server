import { Component, Input, Output, EventEmitter } from "@angular/core";

/**Component that displays a confirmation prompt to the user. Does not disable other inputs. */
@Component({
  selector: "shared-confirmation-prompt",
  templateUrl: "./confirmation-prompt.component.html",
  styleUrls: ["./confirmation-prompt.component.css"]
})
export class ConfirmationPromptComponent {
  @Input() title: string;
  @Input() message: string;
  @Input() optionOne: string;
  @Input() optionTwo: string;

  @Output() onOptionOneClicked: EventEmitter<void> = new EventEmitter();
  @Output() onOptionTwoClicked: EventEmitter<void> = new EventEmitter();
}
