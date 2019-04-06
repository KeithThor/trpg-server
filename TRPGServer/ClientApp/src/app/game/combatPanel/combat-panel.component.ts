import { Component, Input, PACKAGE_ROOT_URL } from "@angular/core";
import { CombatEntity } from "../model/combat-entity.model";
import { CommandTypesConstants } from "../gameplay-constants.static";
import { Category } from "../model/category.model";
import { Ability } from "../model/ability.model";

@Component({
  selector: "game-combat-panel",
  templateUrl: "./combat-panel.component.html",
  styleUrls: ["./combat-panel.component.css"]
})
export class CombatPanelComponent {
  public activeCommand: string;
  public activeCategory: Category;
  @Input() activeEntity: CombatEntity;
  public activeAbility: Ability;

  public inActionPanel: boolean;
  public inCategoryPanel: boolean;
  
  public hoveredCommand: string;
  public hoveredCategory: Category;
  public hoveredAbility: Ability;

  public getCommands(): string[] {
    return CommandTypesConstants.asArray;
  }

  public getAbilities(): Ability[] {
    if (this.activeEntity == null || this.activeCommand == null) return null;
    if (this.activeCategory == null) return this.activeEntity.abilities;

    // Todo: Add case for items
    switch (this.activeCommand) {
      case CommandTypesConstants.attack:
        return this.activeEntity.abilities.filter(ability => {
          return !ability.isSkill
            && !ability.isSpell
            && ability.category.id === this.activeCategory.id;
        });
      case CommandTypesConstants.spells:
        return this.activeEntity.abilities.filter(ability => {
          return ability.isSpell
            && ability.category.id === this.activeCategory.id;
        });
      case CommandTypesConstants.skills:
        return this.activeEntity.abilities.filter(ability => {
          return ability.isSkill
            && ability.category.id === this.activeCategory.id;
        });
      default:
        return null;
    }
  }

  public getCategories(): Category[] {
    if (this.activeEntity == null) return null;
    // Todo: Add case for items
    switch (this.activeCommand) {
      case CommandTypesConstants.attack:
        let attacks = this.activeEntity.abilities.filter(ability => !ability.isSkill && !ability.isSpell);
        return this.abilitiesToCategories(attacks);
      case CommandTypesConstants.spells:
        let spells = this.activeEntity.abilities.filter(ability => ability.isSpell);
        return this.abilitiesToCategories(spells);
      case CommandTypesConstants.skills:
        let skills = this.activeEntity.abilities.filter(ability => ability.isSkill);
        return this.abilitiesToCategories(skills);
      default:
        return null;
    }
  }

  private abilitiesToCategories(abilities: Ability[]): Category[] {
    let categories: Category[] = [];
    abilities.forEach(ability => {
      // Get unique categories
      if (categories.findIndex(category => category.id === ability.category.id) === -1) {
        categories.push(ability.category);
      }
    });

    return categories;
  }

  public onCommandClick(command: string): void {
    if (command != null) {
      if (command === this.activeCommand) {
        this.activeCommand = null;
        this.inActionPanel = false;
        this.inCategoryPanel = false;
      }
      else {
        this.activeCommand = command;
        this.inCategoryPanel = true;
        this.inActionPanel = false;
      }
    }
  }

  public onAbilityClick(ability: Ability): void {
    if (ability != null) {
      if (ability === this.activeAbility) {
        this.activeAbility = null;
      }
      else {
        this.activeAbility = ability;
      }
    }
  }

  public onCategoryClick(category: Category): void {
    if (category != null) {
      this.activeCategory = category;
      this.inActionPanel = true;
    }
  }

  public onMouseEnterCommand(command: string): void {
    if (command != null) {
      this.hoveredCommand = command;
    }
  }

  public onMouseEnterAbility(ability: Ability): void {
    if (ability != null) {
      this.hoveredAbility = ability;
    }
  }

  public onMouseEnterCategory(category: Category): void {
    if (category != null) {
      this.hoveredCategory = category;
    }
  }

  public onMouseLeaveCommand(command: string): void {
    this.hoveredCommand = null;
  }

  public onMouseLeaveAbility(ability: Ability): void {
    this.hoveredAbility = null;
  }
  public onMouseLeaveCategory(category: Category): void {
    this.hoveredCategory = null;
  }
}
