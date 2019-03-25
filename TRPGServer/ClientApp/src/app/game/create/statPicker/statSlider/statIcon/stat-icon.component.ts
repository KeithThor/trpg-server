import { Component, Input } from "@angular/core";

@Component({
  selector: "game-stat-icon",
  templateUrl: "./stat-icon.component.html"
})
export class StatIconComponent {
  @Input() position: number;
  @Input() getImgSrcFunc: (position: number) => string;
  @Input() clickHandler: (position: number) => void;

  public getIconUri(): string {
    return this.getImgSrcFunc(this.position);
  }

  public onClick(): void {
    console.log(this.position);
    this.clickHandler(this.position);
  }
}
