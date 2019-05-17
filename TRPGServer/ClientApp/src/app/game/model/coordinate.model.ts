/**Object representing a point in a two dimensional plane. */
export class Coordinate {
  public constructor(
    fields?: {
      positionX?: number,
      positionY?: number
    }) {
    if (fields != null) {
      this.positionX = (fields.positionX != null) ? fields.positionX : 0;
      this.positionY = (fields.positionY != null) ? fields.positionY : 0;
    }
  }

  public positionX: number;
  public positionY: number;
}
