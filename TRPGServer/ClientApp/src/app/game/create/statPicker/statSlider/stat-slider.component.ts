import { Component, Input } from "@angular/core";
import { StatConstants } from "../StatConstants";
import { CharacterStats } from "../../../model/character-stats.model";

@Component({
  selector: "game-stat-slider",
  templateUrl: "./stat-slider.component.html",
  styleUrls: ["./stat-slider.component.css"]
})
export class StatSliderComponent {
  @Input() statName: string;
  @Input() bonusStats: CharacterStats;
  @Input() maxStats: CharacterStats;
  @Input() allocatedStats: CharacterStats;
  @Input() clickHandler: (statName: string, changeAmount: number) => void;
  @Input() mouseEnterHandler: (name: string) => void;
  public maxStatAsArray: number[];

  /**
   * Gets the uri for the icon that represents a stat point depending on which position the
   * icon is in and whether it is affected by a negative or positive bonus.
   * @param position The number that represents the amount of stats the icon represents.
   */
  public getIconUri(position: number): string {
    if (this.isNegativeStat(position)) {
      return "images/dungeon/wall/crystal_wall_red.png";
    }
    else if (position <= this.allocatedStats[this.statName]) {
      return "images/dungeon/wall/crystal_wall_lightgreen.png";
    }
    else if (this.isBonusStat(position)) {
      return "images/dungeon/wall/crystal_wall_lightcyan.png";
    }
    else return "images/dungeon/wall/crystal_wall_lightgray.png";
  }

  /** Constructs the text for this slider component. */
  public getSliderText(): string {
    let statNameCapped = this.statName.charAt(0).toUpperCase() + this.statName.slice(1);
    let statParameters = "(" + this.allocatedStats[this.statName] + " / " + this.maxStats[this.statName] + ")";
    let bonusParameters = "";
    if (this.bonusStats[this.statName] !== 0) {
      if (this.bonusStats[this.statName] > 0) bonusParameters = " +" + this.bonusStats[this.statName];
      else bonusParameters = " " + this.bonusStats[this.statName];
    }
    return statNameCapped + statParameters + bonusParameters;
  }

  /** Returns an array containing the max stat number converted into an array from 1 to the max stat number. */
  public getMaxStatAsArray(): number[] {
    let arr: number[] = [];
    let max: number = this.maxStats[this.statName];

    for (var i = 1; i <= max; i++) {
      arr.push(i);
    }

    this.maxStatAsArray = arr;
    return this.maxStatAsArray;
  }

  /**
   * Handler called whenever any stat icons are clicked by the player.
   * @param position The position of the stat icon clicked by the player.
   */
  public onClick(position: number): void {
    if (this.isNegativeStat(position)) return;
    if (this.isBonusStat(position) && position > this.maxStats[this.statName] - this.bonusStats[this.statName]) return;

    // Allow the player to set stat to 0 if bonus stats allow a stat to still be over stat minimum.
    if (position === 1 && this.allocatedStats[this.statName] === 1 && this.bonusStats[this.statName] > 0) {
      let changeAmount = -1;
      this.clickHandler(this.statName, changeAmount);
    }
    else {
      let changeAmount = position - this.allocatedStats[this.statName];
      this.clickHandler(this.statName, changeAmount);
    }
  }

  /** Handler called whenever the user mouses over this component. */
  public onMouseEnter(): void {
    if (this.mouseEnterHandler != null) {
      this.mouseEnterHandler(this.statName);
    }
  }

  /**
   * Returns true if the given position represents a negative stat that should be non-interactive
   * and rendered with a red crystal.
   * @param position The position to check.
   */
  private isNegativeStat(position: number): boolean {
    return this.bonusStats[this.statName] < 0 &&
      position <= this.allocatedStats[this.statName] &&
      position > this.allocatedStats[this.statName] + this.bonusStats[this.statName]
  }

  /**
   * Returns true if the given position represents a positive stat that should be non-interactive
   * and rendered witha light cyan crystal.
   * @param position The position to check.
   */
  private isBonusStat(position: number): boolean {
    return this.bonusStats[this.statName] > 0 &&
      position <= this.bonusStats[this.statName] + this.allocatedStats[this.statName];
  }
}
