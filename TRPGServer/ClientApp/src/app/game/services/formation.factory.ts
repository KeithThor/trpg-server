import { Injectable } from "@angular/core";
import { Formation, FormationConstants } from "../model/formation.model";
import { CombatEntity } from "../model/combat-entity.model";

/**Factory responsible for creating and copying Formation objects. */
@Injectable()
export class FormationFactory {
  /** Creates and returns a new empty Formation. */
  public createFormation(): Formation {
    let formation = new Formation();
    let positions: CombatEntity[][] = [];
    for (var i = 0; i < FormationConstants.maxRows; i++) {
      var arr: CombatEntity[] = [];
      for (var j = 0; j < FormationConstants.maxColumns; j++) {
        arr.push(null);
      }
      positions.push(arr);
    }

    formation.positions = positions;
    return formation;
  }

  /**
   * Returns a shallow copy of the given Formation.
   * @param formation The combat Formation to create a shallow copy of.
   */
  public copyFormation(formation: Formation): Formation {
    let newFormation = new Formation();
    newFormation.id = formation.id;
    newFormation.leaderId = formation.leaderId;
    newFormation.name = formation.name;
    let positions: CombatEntity[][] = [];

    formation.positions.forEach(row => {
      let arr: CombatEntity[] = [];
      row.forEach(entity => {
        arr.push(entity);
      });
      positions.push(arr);
    });
    newFormation.positions = positions;

    return newFormation;
  }
}
