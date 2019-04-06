import { Component, Input } from "@angular/core";

@Component({
  selector: "game-icon-display",
  templateUrl: "./icon-display.component.html",
  styleUrls: ["./icon-display.component.css"]
})
export class IconDisplayComponent {
  @Input() iconUris: string[];
}
