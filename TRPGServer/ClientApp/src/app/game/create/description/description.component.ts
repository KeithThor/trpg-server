import { Component, Input } from "@angular/core";

@Component({
  selector: "game-description-component",
  templateUrl: "./description.component.html"
})
export class DescriptionComponent {
  @Input() hoveredComponentName: string;
}
