export class DamageTypes {
  public blunt: number = 0;
  public sharp: number = 0;
  public fire: number = 0;
  public frost: number = 0;
  public lightning: number = 0;
  public earth: number = 0;
  public holy: number = 0;
  public shadow: number = 0;

  public static asArray(damageTypes: DamageTypes): number[] {
    return [
      damageTypes.blunt,
      damageTypes.sharp,
      damageTypes.fire,
      damageTypes.frost,
      damageTypes.lightning,
      damageTypes.earth,
      damageTypes.holy,
      damageTypes.shadow
    ];
  }

  public static fromArray(array: number[]): DamageTypes {
    let result = new DamageTypes();
    array.forEach((val, index) => {
      switch (index) {
        case 0:
          result.blunt = val;
          break;
        case 1:
          result.sharp = val;
          break;
        case 2:
          result.fire = val;
          break;
        case 3:
          result.frost = val;
          break;
        case 4:
          result.lightning = val;
          break;
        case 5:
          result.earth = val;
          break;
        case 6:
          result.holy = val;
          break;
        case 7:
          result.shadow = val;
          break;
        default:
          break;
      }
    });
    return result;
  }
}
