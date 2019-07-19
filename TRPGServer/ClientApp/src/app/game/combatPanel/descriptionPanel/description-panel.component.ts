import { Component, Input, PACKAGE_ROOT_URL } from "@angular/core";
import { Category } from "../../model/category.model";
import { Ability } from "../../model/ability.model";
import { CombatEntity } from "../../model/combat-entity.model";
import { DamageCalculator } from "../../services/damage-calculator.static";
import { DamageTypes } from "../../model/damage-types.model";
import { Coordinate } from "../../model/coordinate.model";

@Component({
  selector: "game-description-panel",
  templateUrl: "./description-panel.component.html",
  styleUrls: [
    "./description-panel.component.css",
    "../../global-game-styles.css"
  ]
})
export class DescriptionPanelComponent {
  @Input() activeCommand: string;
  @Input() activeCategory: Category;
  @Input() activeAbility: Ability;
  @Input() activeEntity: CombatEntity;
  @Input() activeEntityPosition: number;
  @Input() hoveredCommand: string;
  @Input() hoveredCategory: Category;
  @Input() hoveredAbility: Ability;
  @Input() flipGrid: boolean;

  private tempAbilityId: number;
  private tempEntityId: number;
  private cachedDamageTypes: DamageTypes;

  public getActiveObjectName(): string {
    if (this.activeAbility != null || this.hoveredAbility != null) return "ability";
    if (this.activeCategory != null || this.hoveredCategory != null) return "category";
    return "command";
  }

  public notNullOrZero(value: number): boolean {
    return (value != null && value !== 0);
  }

  public getDisplayEntity(): CombatEntity {
    if (this.activeEntity == null) return null;
    //if (this.getDisplayAbility() != null ||
    //  this.getDisplayCategory() != null ||
    //  this.getDisplayCommand() != null) {
    //  return this.activeEntity;
    //}
    else return this.activeEntity;
  }

  public getDisplayCommand(): string {
    if (this.hoveredCommand != null) return this.hoveredCommand;
    else return this.activeCommand;
  }

  public getDisplayCategory(): Category {
    if (this.hoveredCategory != null) return this.hoveredCategory;
    else return this.activeCategory;
  }

  public getDisplayAbility(): Ability {
    if (this.hoveredAbility != null) return this.hoveredAbility;
    else return this.activeAbility;
  }

  public getDamage(): DamageTypes {
    let displayAbility = this.getDisplayAbility();
    if (this.activeEntity == null || displayAbility == null) return new DamageTypes();
    if (this.tempAbilityId !== displayAbility.id || this.tempEntityId !== this.activeEntity.id) {
      this.tempEntityId = this.activeEntity.id;
      this.tempAbilityId = displayAbility.id;
      this.cachedDamageTypes = DamageCalculator.getAbilityDamage(this.activeEntity, displayAbility);
    }
    return this.cachedDamageTypes;
  }

  public getHealing(): number {
    if (this.getDisplayAbility() == null && this.getDisplayEntity() == null) return 0;
    return DamageCalculator.getAbilityHeal(this.getDisplayEntity(), this.getDisplayAbility());
  }
}
