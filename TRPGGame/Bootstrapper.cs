using Autofac;
using System;
using TRPGGame.Data;
using TRPGGame.Entities;
using TRPGGame.Entities.Data;
using TRPGGame.Managers;
using TRPGGame.Managers.Combat;
using TRPGGame.Managers.Combat.Interfaces;
using TRPGGame.Repository;
using TRPGGame.Services;

namespace TRPGGame
{
    /// <summary>
    /// Class responsible for resolving type instances for the project.
    /// </summary>
    public class Bootstrapper
    {
        private readonly IContainer _container;

        public Bootstrapper()
        {
            var containerBuilder = new ContainerBuilder();
            RegisterComponents(containerBuilder);
            _container = containerBuilder.Build();
        }

        /// <summary>
        /// Registers all the types in this solution.
        /// </summary>
        /// <param name="containerBuilder"></param>
        private void RegisterComponents(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<Game>().SingleInstance();
            containerBuilder.RegisterType<WorldState>().As<IWorldState>().SingleInstance();
            containerBuilder.RegisterType<MapRepository>().As<IRepository<Map>>();
            containerBuilder.RegisterType<MapTileRepository>().As<IRepository<MapTile>>();
            containerBuilder.RegisterType<CharacterBaseRepository>().As<IRepository<CharacterBase>>();
            containerBuilder.RegisterType<CharacterHairRepository>().As<IRepository<CharacterHair>>();
            containerBuilder.RegisterType<WorldEntityDbContext>();
            containerBuilder.RegisterType<WorldEntityManager>().SingleInstance();
            containerBuilder.RegisterType<CombatEntityManager>()
                            .As<CombatEntityManager>()
                            .As<ICombatEntityManager>()
                            .SingleInstance();
            containerBuilder.RegisterType<FormationManager>().As<IFormationManager>().SingleInstance();
            containerBuilder.RegisterType<FormationFactory>().As<IFormationFactory>();
            containerBuilder.RegisterType<CombatEntityFactory>().As<ICombatEntityFactory>();
            containerBuilder.RegisterType<WorldEntityFactory>().As<IWorldEntityFactory>().SingleInstance();
            containerBuilder.RegisterType<CategoryRepository>().As<IRepository<Category>>();
            containerBuilder.RegisterType<StatusEffectRepository>().As<IRepository<StatusEffect>>();
            containerBuilder.RegisterType<AbilityRepository>().As<IRepository<Ability>>();
            containerBuilder.RegisterType<ItemTypeRepository>().As<IRepository<ItemType>>();
            containerBuilder.RegisterType<ItemRepository>().As<IRepository<Item>>();
            containerBuilder.RegisterType<AiFormationRepository>().As<IRepository<AiFormationTemplate>>();
            containerBuilder.RegisterType<ClassTemplateRepository>()
                            .As<IRepository<ClassTemplate>>()
                            .As<IRepository<IReadOnlyClassTemplate>>();
            containerBuilder.RegisterType<AiEntityBaseRepository>()
                            .As<IRepository<AiEntityBase>>();
            containerBuilder.RegisterType<EquipmentManager>().As<IEquipmentManager>();
            containerBuilder.RegisterType<StatusEffectManager>().As<IStatusEffectManager>();
            containerBuilder.RegisterType<StateManager>().As<IStateManager>().SingleInstance();
            containerBuilder.RegisterType<AbilityManager>().As<IAbilityManager>();
            containerBuilder.RegisterType<PlayerEntityManagerFactory>();
            containerBuilder.RegisterType<BattleManagerFactory>();
            containerBuilder.RegisterType<AiEntityManagerFactory>();
            containerBuilder.RegisterType<CombatAi>().As<ICombatAi>();
        }

        /// <summary>
        /// Gets an instance of a type that exists within the solution.
        /// <para>Throws an exception if no types are found.</para>
        /// </summary>
        /// <typeparam name="T">The type of the instance to return.</typeparam>
        /// <returns></returns>
        public T GetInstance<T>()
        {
            bool success = _container.TryResolve(out T instance);
            if (!success) throw new Exception($"Failed to get instance of type {instance.GetType()}");

            return instance;
        }
    }
}
