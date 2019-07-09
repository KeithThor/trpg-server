import { Component, Input, Output, EventEmitter } from "@angular/core";
import { setTimeout } from "timers";

@Component({
  selector: "game-error-panel",
  templateUrl: "./error-panel.component.html",
  styleUrls: ["./error-panel.component.css"]
})
export class ErrorPanelComponent {
  private _errorMessage: string;

  @Input() set errorMessage(val: string) {
    this._errorMessage = val;
    setTimeout(this.resetErrorMessage.bind(this), 3000);
  }
  get errorMessage(): string { return this._errorMessage; }

  @Output() errorMessageChange: EventEmitter<string> = new EventEmitter();

  /**Resets the error message to prevent the error from being displayed forever. */
  private resetErrorMessage(): void {
    this._errorMessage = null;
    this.errorMessageChange.emit(null);
  }
}
