import { Component, Input, Output, EventEmitter } from "@angular/core";
import { CombatEntity } from "../model/combat-entity.model";
import { CommandTypesConstants } from "../gameplay-constants.static";
import { Category } from "../model/category.model";
import { Ability } from "../model/ability.model";

/**A component that allows the user to choose an Ability from a CombatEntity. Contains components
 * that help the user visualize the Ability being selected.*/
@Component({
  selector: "game-combat-panel",
  templateUrl: "./combat-panel.component.html",
  styleUrls: ["./combat-panel.component.css"]
})
export class CombatPanelComponent {
  public activeCommand: string;
  public activeCategory: Category;

  private _activeEntity: CombatEntity;
  public get activeEntity(): CombatEntity {
    return this._activeEntity;
  }
  @Input() public set activeEntity(value: CombatEntity) {
    this.resetState();
    this._activeEntity = value;
  }

  private activeEntityId: number;
  @Input() activeEntityPosition: number;
  @Input() flipGrid: boolean;
  public activeAbility: Ability;
  @Output() onSelectAbility: EventEmitter<SelectedAbilityData> = new EventEmitter();
  @Output() onDefend: EventEmitter<void> = new EventEmitter();
  @Output() onFlee: EventEmitter<void> = new EventEmitter();
  @Output() onResetState: EventEmitter<void> = new EventEmitter();

  public inActionPanel: boolean;
  public inCategoryPanel: boolean;
  public hoveredCommand: string;
  public hoveredCategory: Category;
  public hoveredAbility: Ability;
  private isUsingItem: boolean;

  public getCommands(): string[] {
    return CommandTypesConstants.asArray;
  }

  /**Resets the state of the CombatPanel and emits the onResetState event. */
  private resetState(): void {
    this.reset();

    this.onResetState.emit();
  }

  /**Resets the state of the CombatPanel. */
  public reset(): void {
    this.activeAbility = null;
    this.activeCategory = null;
    this.activeCommand = null;
    this.hoveredAbility = null;
    this.hoveredCategory = null
    this.inActionPanel = false;
    this.inCategoryPanel = false;
  }

  /**Returns an array of Abilities that belong to the currently active entity filtered by
   * the currently active Category and command.*/
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
      case CommandTypesConstants.defend:
        this.onDefend.emit();
        break;
      case CommandTypesConstants.flee:
        this.onFlee.emit();
        break;
      default:
        return null;
    }
  }

  /**Gets all the unique Categories that belong to the currently active entity for the currently
   active command.*/
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

  /**
   * Given an array of Abilities, returns an array containing the unique Categories of those Abilities.
   * @param abilities The Abilities to return the Categories of.
   */
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

  /**
   * Whenever a command is clicked, reset the state of the CombatPanel and set the active
   * command to the clicked command.
   * @param command The name of the clicked command.
   */
  public onCommandClick(command: string): void {
    if (command != null) {
      if (command === this.activeCommand) {
        this.resetState();
      }
      else {
        this.resetState();

        this.activeCommand = command;
        this.inCategoryPanel = true;
      }
    }
  }

  /**
   * When an AbilityPanel button is clicked, set the active Ability to the Ability that belongs to that
   * AbilityPanel button.
   * @param ability The ability that was clicked.
   */
  public onAbilityClick(ability: Ability): void {
    if (ability != null) {
      if (ability === this.activeAbility) {
        this.activeAbility = null;
        this.onSelectAbility.emit(null);
      }
      else {
        this.activeAbility = ability;
        let sAbility = this.createSelectedAbilityData();
        this.onSelectAbility.emit(sAbility);
      }
    }
  }

  /**Creates a new instance of a SelectedAbilityData object from class variables. */
  private createSelectedAbilityData(): SelectedAbilityData {
    let data = new SelectedAbilityData();
    data.ability = this.activeAbility;
    data.isUsingItem = this.isUsingItem;
    return data;
  }

  /**
   * When a CategoryPanel button is clicked, set the active Category to the Category that belongs to that
   * CategoryPanel button.
   * @param category The Category that was clicked.
   */
  public onCategoryClick(category: Category): void {
    if (category != null) {
      this.activeCategory = category;
      this.inActionPanel = true;
    }
  }

  /**
   * Sets the hoveredCommand to the command the user's mouse entered.
   * @param command The command to set the hoveredCommand to.
   */
  public onMouseEnterCommand(command: string): void {
    if (command != null) {
      this.hoveredCommand = command;
    }
  }

  /**
   * Sets the hoveredAbility to the Ability the user's mouse entered.
   * @param ability The ability to set the hoveredAbility to.
   */
  public onMouseEnterAbility(ability: Ability): void {
    if (ability != null) {
      this.hoveredAbility = ability;
    }
  }

  /**
   * Sets the hoveredCategory to the Category the user's mouse entered.
   * @param category The Category to set the hoveredCategory to.
   */
  public onMouseEnterCategory(category: Category): void {
    if (category != null) {
      this.hoveredCategory = category;
    }
  }

  /**
   * Sets the hoveredCommand to null whenever the user's mouse leaves the command.
   * @param command The command that the user's mouse left.
   */
  public onMouseLeaveCommand(command: string): void {
    this.hoveredCommand = null;
  }

  /**
   * Sets the hoveredAbility to null whenever the user's mouse leaves the Ability.
   * @param command The Ability that the user's mouse left.
   */
  public onMouseLeaveAbility(ability: Ability): void {
    this.hoveredAbility = null;
  }

  /**
   * Sets the hoveredCategory to null whenever the user's mouse leaves the Category.
   * @param command The Category that the user's mouse left.
   */
  public onMouseLeaveCategory(category: Category): void {
    this.hoveredCategory = null;
  }
}

/**Data object containing information about the selected ability. */
export class SelectedAbilityData {
  public isUsingItem: boolean;
  public ability: Ability;
}
