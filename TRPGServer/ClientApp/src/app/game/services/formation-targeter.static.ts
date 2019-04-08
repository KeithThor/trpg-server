import { FormationConstants } from "../model/formation.model";

export class FormationTargeter {
  /**
   * Translates a given targets array from one point to another, removing any positions
   * that logically don't belong there.
   * @param initialCenter The initial center of position for the targets to translate.
   * @param targets The target positions to translate to another position.
   * @param newCenter The new center of position to translate the targets to.
   */
  public static translate(initialCenter: number, targets: number[], newCenter: number): number[] {
    let newTargets: number[] = targets.slice();
    let change = initialCenter - newCenter;
    let rowDifference = this.getRowDifference(initialCenter, newCenter);
    let columnDifference = this.getColumnDifference(initialCenter, newCenter);

    // Remove positions depending on their change in rows
    let removePositions: number[] = [];
    newTargets.forEach((val, index) => {
      let valRowDiff = this.getRowDifference(val, val - change);
      let rowPosition = this.getRow(val) - valRowDiff;

      // Remove any positions that go out of bounds of row size
      if (rowPosition < 1 || rowPosition > FormationConstants.maxRows) removePositions.push(index);
      // If there is a misalignment in the change in rows of this position and the center, remove this position
      else if (rowDifference != valRowDiff) removePositions.push(index);
    });

    removePositions.reverse().forEach(i => newTargets.splice(i, 1));

    // Remove positions depending on their change in columns
    removePositions = []
    newTargets.forEach((val, index) => {
      let valColDiff = this.getColumnDifference(val, val - change);
      let columnPosition = this.getColumn(val) - valColDiff;
      // Remove any positions that go out of bounds of column size
      if (columnPosition < 1 || columnPosition > FormationConstants.maxColumns) removePositions.push(index);
      // If there is a misalignment in the chance in columns of this position and the center, remove this position
      else if (columnDifference != valColDiff) removePositions.push(index);
    });

    removePositions.reverse().forEach(i => newTargets.splice(i, 1));
    newTargets.forEach((val, index) => {
      newTargets[index] = val - change;
    });

    return newTargets;
  }

  /**
   * Given the starting center position and the new center position, gets the change
   * in rows that would occur if the center position was translated to the new
   * position.
   * @param initialCenter The number representing the position of the initial center of targets
   * position for an ability.
   * @param newCenter The number representing the position to translate the center of targets position to.
   */
  private static getRowDifference(initialCenter: number, newCenter: number): number {
    let initialRow = this.getRow(initialCenter);
    let newRow = this.getRow(newCenter);

    return initialRow - newRow;
  }

  /**
   * Gets the row number that a given position exists in.
   * @param position The position to get the row number for.
   */
  private static getRow(position: number): number {
    // - 1 to keep every third number from being a new row
    return Math.floor((position - 1) / FormationConstants.maxColumns + 1);
  }

  /**
   * Given the starting center position and the new center position, gets the change
   * in columns that would occur if the center position was translated to the new
   * position.
   * @param initialCenter The number representing the position of the initial center of targets
   * position for an ability.
   * @param newCenter The number representing the position to translate the center of targets position to.
   */
  private static getColumnDifference(initialCenter: number, newCenter: number): number {
    let initalColumn = this.getColumn(initialCenter);
    let newColumn = this.getColumn(newCenter);

    return initalColumn - newColumn;
  }

  /**
   * Gets the column number that a given position exists in.
   * @param position The position to get the row number for.
   */
  private static getColumn(position: number): number {
    return (position - 1) % FormationConstants.maxRows + 1;
  }
}
