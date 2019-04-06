"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var DamageTypes = /** @class */ (function () {
    function DamageTypes() {
        this.blunt = 0;
        this.sharp = 0;
        this.fire = 0;
        this.frost = 0;
        this.lightning = 0;
        this.earth = 0;
        this.holy = 0;
        this.shadow = 0;
    }
    DamageTypes.asArray = function (damageTypes) {
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
    };
    DamageTypes.fromArray = function (array) {
        var result = new DamageTypes();
        array.forEach(function (val, index) {
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
    };
    return DamageTypes;
}());
exports.DamageTypes = DamageTypes;
//# sourceMappingURL=damage-types.model.js.map