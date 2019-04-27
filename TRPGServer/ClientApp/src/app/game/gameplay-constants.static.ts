export class GameplayConstants {
  public static readonly aiId: string = "6a89b96c-f3fc-4cc9-a80a-b45354189405";
}

export class CommandTypesConstants {
  public static readonly attack: string = "Attack";
  public static readonly defend: string = "Defend";
  public static readonly spells: string = "Spells";
  public static readonly skills: string = "Skills";
  public static readonly items: string = "Items";
  public static readonly flee: string = "Flee";

  public static asArray: string[] = [
    "Attack",
    "Defend",
    "Spells",
    "Skills",
    "Items",
    "Flee"
  ];
}
