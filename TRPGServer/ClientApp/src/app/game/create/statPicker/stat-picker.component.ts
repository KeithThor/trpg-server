import { Component, Input, Output, EventEmitter } from "@angular/core";
import { CharacterStats } from "../../model/character-stats.model";
import { StatConstants } from "./StatConstants";
import { CharacterBase } from "../../model/character.model";

@Component({
  selector: "game-stat-picker",
  templateUrl: "./stat-picker.component.html",
  styleUrls: ["./stat-picker.component.css"]
})
export class StatPickerComponent {
  public freePoints: number = 0;
  @Output() statsChange = new EventEmitter<CharacterStats>();
  @Input() stats: CharacterStats;
  @Input() base: CharacterBase;
  public isPristine: boolean = true;
  public self = this;

  /** Returns true if the component is in a valid state to send data. */
  public isValid(): boolean {
    if (this.freePoints !== 0) return false;
    let foundInvalid: boolean = false;

    Object.keys(this.stats).forEach((statName) => {
      if (this.stats[statName] <= 0 || this.stats[statName] > this.base.maxStats[statName])
        foundInvalid = true;
    });

    return !foundInvalid;
  }

  public getStatNames(): string[] {
    return Object.keys(this.stats);
  }

  /**
   * Handler called whenever a StatSlider icon is clicked. Allocates or refunds points depending on whether
   * the change amount was positive or negative.
   * @param statName The name of the stat to change.
   * @param changeAmount The amount of change being made to the stat.
   */
  public onStatSliderClicked(statName: string, changeAmount: number) {
    if (changeAmount === 0) return;
    if (this.freePoints === 0 && changeAmount > 0) return;

    let minStat = (this.base.bonusStats[statName] < 0) ? Math.abs(this.base.bonusStats[statName]) : StatConstants.MinStat;
    let maxStat = this.base.maxStats[statName];
    let statWithBonus = this.stats[statName] + this.base.bonusStats[statName];
    this.isPristine = false;

    // Reducing stat amount below min value with negative bonus points
    if (changeAmount < 0 && this.base.bonusStats[statName] < 0 && this.stats[statName] + changeAmount <= minStat) {
      let change = this.stats[statName] - minStat;
      this.stats[statName] = minStat;
      this.freePoints += change;

      this.statsChange.emit(this.stats);
    }
    // Reducing stat amount below min value with a bonus point or none
    else if (changeAmount < 0 && this.base.bonusStats[statName] >= 0 && statWithBonus + changeAmount <= minStat) {
      let change = statWithBonus - minStat;
      this.stats[statName] -= change;
      this.freePoints += change;

      this.statsChange.emit(this.stats);
    }
    // Increasing stat amount beyond amount of free points, use all free points but don't go beyond
    else if (changeAmount > 0 && this.freePoints < changeAmount) {
      this.stats[statName] += this.freePoints;
      this.freePoints = 0;

      // Too many points were dumped into stat, refund until increased stat is at max
      if (this.stats[statName] + this.base.bonusStats[statName] > maxStat) {
        this.freePoints += this.stats[statName] + this.base.bonusStats[statName] - maxStat;
        this.stats[statName] = maxStat - this.base.bonusStats[statName];
      }

      this.statsChange.emit(this.stats);
    }
    // Normal behavior
    else {
      this.stats[statName] += changeAmount;
      this.freePoints -= changeAmount;

      // Too many points were dumped into stat, refund until increased stat is at max
      if (this.stats[statName] + this.base.bonusStats[statName] > maxStat) {
        this.freePoints += this.stats[statName] + this.base.bonusStats[statName] - maxStat;
        this.stats[statName] = maxStat - this.base.bonusStats[statName];
      }

      this.statsChange.emit(this.stats);
    }
  }

  /** Converts free points into a number array from 1 to the amount of free points the character has. */
  public getFreePointsArray(): number[] {
    let arr: number[] = [];
    for (var i = 1; i <= this.freePoints; i++) {
      arr.push(i);
    }

    return arr;
  }
}
