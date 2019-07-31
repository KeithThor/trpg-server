using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TRPGGame.Entities;
using TRPGGame.Services;
using Xunit;

namespace TRPGGame.Test.Services
{
    public class FormationTargeterTest
    {

        [Theory]
        [MemberData(nameof(GetSingleTargetData))]
        public void GetTargets_WithSingleTarget_ReturnsNotNull(Ability ability, int target, Formation formation)
        {
            var entities = FormationTargeter.GetTargets(ability, target, formation);

            Assert.NotEmpty(entities);
        }

        [Theory]
        [MemberData(nameof(GetSingleNoTargetData))]
        public void GetTargets_WithNoTarget_ReturnsNull(Ability ability, int target, Formation formation)
        {
            var entities = FormationTargeter.GetTargets(ability, target, formation);

            Assert.Empty(entities);
        }

        [Theory]
        [MemberData(nameof(GetMultipleTargetsData))]
        public void GetTargets_WithMultipleTargets_ReturnsTargets(Ability ability,
                                                                  int target,
                                                                  Formation formation,
                                                                  IEnumerable<CombatEntity> expectedTargets)
        {
            var entities = FormationTargeter.GetTargets(ability, target, formation);

            // Contains all of the same items
            var expectations = !expectedTargets.Except(entities).Any() && expectedTargets.Count() == entities.Count();

            Assert.True(expectations);
        }

        [Theory]
        [MemberData(nameof(GetMultipleNoTargetsData))]
        public void GetTargets_WithMultipleNoTargets_ReturnsEmpty(Ability ability,
                                                                  int target,
                                                                  Formation formation)
        {
            var entities = FormationTargeter.GetTargets(ability, target, formation);

            Assert.Empty(entities);
        }

        [Theory]
        [MemberData(nameof(GetBlockedTargetsData))]
        public void GetTargets_WithBlockedTargets_ReturnsEmpty(Ability ability,
                                                               int target,
                                                               Formation formation)
        {
            var entities = FormationTargeter.GetTargets(ability, target, formation);

            Assert.Empty(entities);
        }

        [Theory]
        [MemberData(nameof(GetStaticTargetsData))]
        public void GetTargets_WithStaticTargets_ReturnsExpected(Ability ability,
                                                                 int target,
                                                                 Formation formation,
                                                                 IEnumerable<CombatEntity> expectedTargets)
        {
            var entities = FormationTargeter.GetTargets(ability, target, formation);

            // Contains all of the same items
            var expectations = !expectedTargets.Except(entities).Any() && expectedTargets.Count() == entities.Count();

            Assert.True(expectations);
        }

        [Theory]
        [MemberData(nameof(GetPointBlankTargetsData))]
        public void GetTargets_WithPointBlankTargets_ReturnsExpected(Ability ability,
                                                                     int target,
                                                                     Formation formation,
                                                                     IEnumerable<CombatEntity> expectedTargets)
        {
            var entities = FormationTargeter.GetTargets(ability, target, formation);

            // Contains all of the same items
            var expectations = !expectedTargets.Except(entities).Any() && expectedTargets.Count() == entities.Count();

            Assert.True(expectations);
        }

        [Theory]
        [MemberData(nameof(GetOutOfBoundsData))]
        public void GetTargets_WithOutOfBounds_ReturnsExpected(Ability ability,
                                                               int target,
                                                               Formation formation,
                                                               IEnumerable<CombatEntity> expectedTargets)
        {
            var entities = FormationTargeter.GetTargets(ability, target, formation);

            // Contains all of the same items
            var expectations = !expectedTargets.Except(entities).Any() && expectedTargets.Count() == entities.Count();

            Assert.True(expectations);
        }

        /// <summary>
        /// Creates a Formation with no CombatEntities.
        /// </summary>
        private static Formation CreateEmptyFormation()
        {
            var fixture = new Fixture();
            var formation = fixture.Build<Formation>()
                                   .Without(form => form.Positions)
                                   .Create();

            formation.Positions = new CombatEntity[GameplayConstants.MaxFormationColumns][];
            for (int i = 0; i < formation.Positions.Length; i++)
            {
                formation.Positions[i] = new CombatEntity[3];
            }

            return formation;
        }

        /// <summary>
        /// Creates a Formation filled with CombatEntities.
        /// </summary>
        private static Formation CreatePopulatedFormation()
        {
            var fixture = new Fixture();
            var formation = fixture.Build<Formation>()
                                   .Without(form => form.Positions)
                                   .Create();

            formation.Positions = new CombatEntity[GameplayConstants.MaxFormationColumns][];
            for (int i = 0; i < formation.Positions.Length; i++)
            {
                formation.Positions[i] = new CombatEntity[]
                {
                    new CombatEntity(),
                    new CombatEntity(),
                    new CombatEntity(),
                };
            }

            return formation;
        }

        /// <summary>
        /// Creates a CombatEntity with health that is used to test ability blocking.
        /// </summary>
        private static CombatEntity CreateHealthyCombatEntity()
        {
            return new CombatEntity
            {
                Resources = new ResourceStats
                {
                    CurrentHealth = 100
                }
            };
        }

        public static IEnumerable<object[]> GetSingleTargetData()
        {
            var data = new List<object[]>();
            var fixture = new Fixture();

            // Create single-target ability
            var ability = fixture.Create<Ability>();
            ability.Targets = new List<int> { 5 };
            ability.CenterOfTargets = 5;
            ability.CanTargetBeBlocked = false;
            ability.IsPositionStatic = false;
            ability.IsPointBlank = false;

            // Test middle of the Formation
            var formationOne = CreateEmptyFormation();
            formationOne.Positions[1][1] = new CombatEntity();

            data.Add(new object[]
            {
                ability,
                5,
                formationOne
            });

            // Test bottom left corner of the Formation
            var formationTwo = CreateEmptyFormation();
            formationTwo.Positions[2][0] = new CombatEntity();

            data.Add(new object[]
            {
                ability,
                7,
                formationTwo
            });

            // Test top right corner of the Formation
            var formationThree = CreateEmptyFormation();
            formationThree.Positions[0][2] = new CombatEntity();

            data.Add(new object[]
            {
                ability,
                3,
                formationThree
            });

            // Test bottom middle of the Formation
            var formationFour = CreateEmptyFormation();
            formationFour.Positions[2][1] = new CombatEntity();

            data.Add(new object[]
            {
                ability,
                8,
                formationFour
            });

            return data;
        }

        public static IEnumerable<object[]> GetSingleNoTargetData()
        {
            var data = new List<object[]>();
            var fixture = new Fixture();

            // Create single-target ability
            var ability = fixture.Create<Ability>();
            ability.Targets = new List<int> { 5 };
            ability.CenterOfTargets = 5;
            ability.CanTargetBeBlocked = false;
            ability.IsPositionStatic = false;
            ability.IsPointBlank = false;

            // Test middle of the Formation
            var formationOne = CreatePopulatedFormation();
            formationOne.Positions[1][1] = null;

            data.Add(new object[]
            {
                ability,
                5,
                formationOne
            });

            // Test bottom left corner of the Formation
            var formationTwo = CreatePopulatedFormation();
            formationTwo.Positions[2][0] = null;

            data.Add(new object[]
            {
                ability,
                7,
                formationTwo
            });

            // Test top right corner of the Formation
            var formationThree = CreatePopulatedFormation();
            formationThree.Positions[0][2] = null;

            data.Add(new object[]
            {
                ability,
                3,
                formationThree
            });

            // Test bottom middle of the Formation
            var formationFour = CreatePopulatedFormation();
            formationFour.Positions[2][1] = null;

            data.Add(new object[]
            {
                ability,
                8,
                formationFour
            });

            return data;
        }

        public static IEnumerable<object[]> GetMultipleTargetsData()
        {
            var data = new List<object[]>();
            var fixture = new Fixture();

            // Create row-target ability
            var ability = fixture.Create<Ability>();
            ability.Targets = new List<int> { 4, 5, 6 };
            ability.CenterOfTargets = 4;
            ability.CanTargetBeBlocked = false;
            ability.IsPositionStatic = false;
            ability.IsPointBlank = false;

            // Test middle row of the Formation
            var formationOne = CreateEmptyFormation();
            formationOne.Positions[1][0] = new CombatEntity();
            formationOne.Positions[1][2] = new CombatEntity();

            var expectedOne = new List<CombatEntity>
            {
                formationOne.Positions[1][0],
                formationOne.Positions[1][2]
            };

            data.Add(new object[]
            {
                ability,
                4,
                formationOne,
                expectedOne
            });

            // Test bottom right corner of the Formation
            var formationTwo = CreateEmptyFormation();
            formationTwo.Positions[2][2] = new CombatEntity();
            formationTwo.Positions[0][2] = new CombatEntity();

            var expectedTwo = new List<CombatEntity>
            {
                formationTwo.Positions[2][2]
            };

            data.Add(new object[]
            {
                ability,
                9,
                formationTwo,
                expectedTwo
            });

            // Test top middle corner of the Formation
            var formationThree = CreateEmptyFormation();
            formationThree.Positions[0][1] = new CombatEntity();
            formationThree.Positions[0][2] = new CombatEntity();
            formationThree.Positions[1][1] = new CombatEntity();

            var expectedThree = new List<CombatEntity>
            {
                formationThree.Positions[0][1],
                formationThree.Positions[0][2]
            };

            data.Add(new object[]
            {
                ability,
                2,
                formationThree,
                expectedThree
            });

            // Test bottom left of the Formation
            var formationFour = CreateEmptyFormation();
            formationFour.Positions[2][1] = new CombatEntity();
            formationFour.Positions[2][0] = new CombatEntity();
            formationFour.Positions[1][0] = new CombatEntity();

            var expectedFour = new List<CombatEntity>
            {
                formationFour.Positions[2][0],
                formationFour.Positions[2][1]
            };

            data.Add(new object[]
            {
                ability,
                7,
                formationFour,
                expectedFour
            });

            return data;
        }

        public static IEnumerable<object[]> GetMultipleNoTargetsData()
        {
            var data = new List<object[]>();
            var fixture = new Fixture();

            // Create row-target ability
            var ability = fixture.Create<Ability>();
            ability.Targets = new List<int> { 4, 5, 6 };
            ability.CenterOfTargets = 4;
            ability.CanTargetBeBlocked = false;
            ability.IsPositionStatic = false;
            ability.IsPointBlank = false;

            // Test middle row of the Formation
            var formationOne = CreateEmptyFormation();
            formationOne.Positions[0][0] = new CombatEntity();
            formationOne.Positions[2][2] = new CombatEntity();

            data.Add(new object[]
            {
                ability,
                4,
                formationOne
            });

            // Test bottom right corner of the Formation
            var formationTwo = CreateEmptyFormation();
            formationTwo.Positions[1][2] = new CombatEntity();
            formationTwo.Positions[0][2] = new CombatEntity();

            data.Add(new object[]
            {
                ability,
                9,
                formationTwo
            });

            // Test top middle corner of the Formation
            var formationThree = CreateEmptyFormation();
            formationThree.Positions[0][0] = new CombatEntity();
            formationThree.Positions[1][1] = new CombatEntity();

            data.Add(new object[]
            {
                ability,
                2,
                formationThree
            });

            // Test bottom left of the Formation
            var formationFour = CreateEmptyFormation();
            formationFour.Positions[0][0] = new CombatEntity();
            formationFour.Positions[1][1] = new CombatEntity();
            formationFour.Positions[1][0] = new CombatEntity();

            data.Add(new object[]
            {
                ability,
                7,
                formationFour
            });

            return data;
        }

        public static IEnumerable<object[]> GetBlockedTargetsData()
        {
            var data = new List<object[]>();
            var fixture = new Fixture();

            // Create row-target ability
            var ability = fixture.Create<Ability>();
            ability.Targets = new List<int> { 4, 5, 6 };
            ability.CenterOfTargets = 4;
            ability.CanTargetBeBlocked = true;
            ability.IsPositionStatic = false;
            ability.IsPointBlank = false;

            // Test middle top of the Formation
            var formationOne = CreateEmptyFormation();
            formationOne.Positions[0][0] = CreateHealthyCombatEntity();
            formationOne.Positions[0][1] = CreateHealthyCombatEntity();
            formationOne.Positions[2][2] = CreateHealthyCombatEntity();

            data.Add(new object[]
            {
                ability,
                2,
                formationOne
            });

            // Test bottom right corner of the Formation
            var formationTwo = CreateEmptyFormation();
            formationTwo.Positions[2][2] = CreateHealthyCombatEntity();
            formationTwo.Positions[2][0] = CreateHealthyCombatEntity();

            data.Add(new object[]
            {
                ability,
                9,
                formationTwo
            });

            // Test middle of the Formation
            var formationThree = CreateEmptyFormation();
            formationThree.Positions[1][0] = CreateHealthyCombatEntity();
            formationThree.Positions[1][1] = CreateHealthyCombatEntity();

            data.Add(new object[]
            {
                ability,
                5,
                formationThree
            });

            // Test middle right of the Formation
            var formationFour = CreateEmptyFormation();
            formationFour.Positions[1][2] = CreateHealthyCombatEntity();
            formationFour.Positions[1][1] = CreateHealthyCombatEntity();
            formationFour.Positions[1][0] = CreateHealthyCombatEntity();

            data.Add(new object[]
            {
                ability,
                6,
                formationFour
            });

            return data;
        }

        public static IEnumerable<object[]> GetStaticTargetsData()
        {
            var data = new List<object[]>();
            var fixture = new Fixture();

            // Create row-target ability
            var ability = fixture.Create<Ability>();
            ability.Targets = new List<int> { 1, 3, 5, 7, 9 };
            ability.CenterOfTargets = 5;
            ability.CanTargetBeBlocked = false;
            ability.IsPositionStatic = true;
            ability.IsPointBlank = false;

            // Test center of Formation
            var formationOne = CreateEmptyFormation();
            formationOne.Positions[0][0] = new CombatEntity();
            formationOne.Positions[0][1] = new CombatEntity();
            formationOne.Positions[2][2] = new CombatEntity();

            var expectedOne = new List<CombatEntity>
            {
                formationOne.Positions[0][0],
                formationOne.Positions[2][2]
            };

            data.Add(new object[]
            {
                ability,
                2,
                formationOne,
                expectedOne
            });

            // Test bottom right corner of the Formation
            var formationTwo = CreateEmptyFormation();
            formationTwo.Positions[0][0] = new CombatEntity();
            formationTwo.Positions[0][1] = new CombatEntity();
            formationTwo.Positions[1][1] = new CombatEntity();

            var expectedTwo = new List<CombatEntity>
            {
                formationTwo.Positions[0][0],
                formationTwo.Positions[1][1]
            };

            data.Add(new object[]
            {
                ability,
                9,
                formationTwo,
                expectedTwo
            });

            // Test left middle of the Formation
            var formationThree = CreateEmptyFormation();
            formationThree.Positions[1][0] = new CombatEntity();
            formationThree.Positions[0][1] = new CombatEntity();
            formationThree.Positions[2][1] = new CombatEntity();

            var expectedThree = new List<CombatEntity>();

            data.Add(new object[]
            {
                ability,
                4,
                formationThree,
                expectedThree
            });

            // Test middle right of the Formation
            var formationFour = CreateEmptyFormation();
            formationThree.Positions[1][2] = new CombatEntity();
            formationThree.Positions[1][0] = new CombatEntity();
            formationThree.Positions[2][1] = new CombatEntity();

            var expectedFour = new List<CombatEntity>();

            data.Add(new object[]
            {
                ability,
                6,
                formationFour,
                expectedFour
            });

            return data;
        }

        public static IEnumerable<object[]> GetPointBlankTargetsData()
        {
            var data = new List<object[]>();
            var fixture = new Fixture();

            // Create row-target ability
            var ability = fixture.Create<Ability>();
            ability.Targets = new List<int> { 4, 5, 6 };
            ability.CenterOfTargets = 4;
            ability.CanTargetBeBlocked = false;
            ability.IsPositionStatic = false;
            ability.IsPointBlank = true;

            // Test middle row of the Formation
            var formationOne = CreateEmptyFormation();
            formationOne.Positions[1][0] = new CombatEntity();
            formationOne.Positions[1][2] = new CombatEntity();

            var expectedOne = new List<CombatEntity>
            {
                formationOne.Positions[1][0],
                formationOne.Positions[1][2]
            };

            data.Add(new object[]
            {
                ability,
                4,
                formationOne,
                expectedOne
            });

            // Test bottom right corner of the Formation
            var formationTwo = CreateEmptyFormation();
            formationTwo.Positions[2][2] = new CombatEntity();
            formationTwo.Positions[0][2] = new CombatEntity();

            var expectedTwo = new List<CombatEntity>
            {
                formationTwo.Positions[2][2]
            };

            data.Add(new object[]
            {
                ability,
                9,
                formationTwo,
                expectedTwo
            });

            // Test top middle corner of the Formation
            var formationThree = CreateEmptyFormation();
            formationThree.Positions[0][1] = new CombatEntity();
            formationThree.Positions[0][2] = new CombatEntity();
            formationThree.Positions[1][1] = new CombatEntity();

            var expectedThree = new List<CombatEntity>
            {
                formationThree.Positions[0][1],
                formationThree.Positions[0][2]
            };

            data.Add(new object[]
            {
                ability,
                2,
                formationThree,
                expectedThree
            });

            // Test bottom left of the Formation
            var formationFour = CreateEmptyFormation();
            formationFour.Positions[2][1] = new CombatEntity();
            formationFour.Positions[2][0] = new CombatEntity();
            formationFour.Positions[1][0] = new CombatEntity();

            var expectedFour = new List<CombatEntity>
            {
                formationFour.Positions[2][0],
                formationFour.Positions[2][1]
            };

            data.Add(new object[]
            {
                ability,
                7,
                formationFour,
                expectedFour
            });

            return data;
        }

        public static IEnumerable<object[]> GetOutOfBoundsData()
        {
            var data = new List<object[]>();
            var fixture = new Fixture();

            // Create T-target ability
            var ability = fixture.Create<Ability>();
            ability.Targets = new List<int> { 2, 4, 5, 8 };
            ability.CenterOfTargets = 4;
            ability.CanTargetBeBlocked = false;
            ability.IsPositionStatic = false;
            ability.IsPointBlank = false;

            // Test middle row of the Formation
            var formationOne = CreateEmptyFormation();
            formationOne.Positions[0][1] = new CombatEntity();
            formationOne.Positions[1][2] = new CombatEntity();
            formationOne.Positions[1][0] = new CombatEntity();
            formationOne.Positions[2][1] = new CombatEntity();

            var expectedOne = new List<CombatEntity>
            {
                formationOne.Positions[0][1],
                formationOne.Positions[1][0],
                formationOne.Positions[2][1]
            };

            data.Add(new object[]
            {
                ability,
                4,
                formationOne,
                expectedOne
            });

            // Test top left corner
            var formationTwo = CreateEmptyFormation();
            formationTwo.Positions[0][0] = new CombatEntity();
            formationTwo.Positions[0][1] = new CombatEntity();
            formationTwo.Positions[1][1] = new CombatEntity();
            formationTwo.Positions[2][1] = new CombatEntity();

            var expectedTwo = new List<CombatEntity>
            {
                formationTwo.Positions[0][0],
                formationTwo.Positions[0][1],
                formationTwo.Positions[1][1]
            };

            data.Add(new object[]
            {
                ability,
                1,
                formationTwo,
                expectedTwo
            });

            // Test bottom right corner of the Formation
            var formationThree = CreateEmptyFormation();
            formationThree.Positions[1][0] = new CombatEntity();
            formationThree.Positions[1][2] = new CombatEntity();
            formationThree.Positions[2][1] = new CombatEntity();
            formationThree.Positions[2][2] = new CombatEntity();

            var expectedThree = new List<CombatEntity>
            {
                formationThree.Positions[2][2]
            };

            data.Add(new object[]
            {
                ability,
                9,
                formationThree,
                expectedThree
            });

            // Test middle right of the Formation
            var formationFour = CreatePopulatedFormation();

            var expectedFour = new List<CombatEntity>
            {
                formationFour.Positions[1][2]
            };

            data.Add(new object[]
            {
                ability,
                6,
                formationFour,
                expectedFour
            });

            return data;
        }
    }
}
