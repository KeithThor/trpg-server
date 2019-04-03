/** Generic static class containing methods to handle manipulating two dimensional arrays. */
export class TwoDArray<T> {
  /**
   * Returns the first object that satisfies the provided predicate.
   * @param array The two dimensional array to find the object in.
   * @param predicate The function used to find the desired object.
   */
  public static find<T>(array: T[][], predicate: (object: T) => boolean): T {
    if (array == null) return null;
    if (predicate == null) return null;

    let foundObj: T = null;
    let found = false;
    let i = 0;
    while (!found && i < array.length) {
      let j = 0;
      if (array[i] != null) {
        while (!found && j < array[i].length) {
          if (array[i][j] != null && predicate(array[i][j])) {
            foundObj = array[i][j];
            found = true;
          }
          j++;
        }
      }
      i++;
    }
    if (found) return foundObj;
    else return null;
  }

  /**
   * Calls a callback function for each element of a two dimensional array.
   * @param array The two dimensional array to loop through.
   * @param callbackfunc The callback function to call for each element of the array.
   * @param thisArg Optional parameter to substitute for the this parameter of the callback function.
   */
  public static forEach<T>(array: T[][],
    callbackfunc: (value: T, indexOne: number, indexTwo: number, array: T[][]) => void,
    thisArg?: any): void {
    if (array == null) return;
    if (callbackfunc == null) return;

    array.forEach((row, xIndex) => {
      if (row == null) return;
      row.forEach((obj, yIndex) => {
        if (thisArg == null) callbackfunc(obj, xIndex, yIndex, array);
        else callbackfunc.apply(thisArg, [obj, xIndex, yIndex, array]);
      });
    });
  }

  /**
   * Returns an array containing unique objects stored in the two dimensional array.
   * @param array The array to find unique objects from.
   */
  public static getUnique<T>(array: T[][]): T[] {
    let unique: T[] = [];
    if (array == null) return null;

    array.forEach((row) => {
      if (row == null) return;
      row.forEach((obj) => {
        if (obj == null) return;
        if (!unique.some(val => val === obj)) {
          unique.push(obj);
        }
      });
    });

    return unique;
  }

  /**
   * Gets the indeces of an object that satisfies a given predicate.
   *
   * The first index in the array is the row index.
   * The second index in the array is the column index.
   * 
   * array[firstIndex][secondIndex]
   * @param array
   * @param predicate
   */
  public static findIndex<T>(array: T[][], predicate: (object: T) => boolean): number[] {
    if (array == null) return null;
    if (predicate == null) return null;

    let indeces: number[] = [];
    let found = false;
    let i = 0;
    while (!found && i < array.length) {
      if (array[i] != null) {
        let j = 0;
        while (!found && j < array[i].length) {
          if (array[i][j] != null && predicate(array[i][j])) {
            indeces.push(i);
            indeces.push(j);
            found = true;
          }
          j++;
        }
      }
      i++;
    }

    if (found) return indeces;
    else return null;
  }
}
