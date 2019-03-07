using System;
using Autofac;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Repository;
using TRPGGame.Entities;
using TRPGGame.Data;
using TRPGGame.Services;

namespace TRPGGame
{
    /// <summary>
    /// Class responsible for resolving type instances for the solution.
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
            containerBuilder.RegisterType<WorldEntityDbContext>();
            containerBuilder.RegisterType<WorldEntityManager>().SingleInstance();
            containerBuilder.RegisterType<WorldEntityFactory>();
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
