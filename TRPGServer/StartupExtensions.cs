using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using TRPGGame;
using TRPGGame.Entities.Data;
using TRPGGame.Managers;
using TRPGGame.Repository;
using TRPGGame.Services;
using TRPGServer.Hubs;
using TRPGServer.Services;

namespace TRPGServer
{
    public static class StartupExtensions
    {
        /// <summary>
        /// Adds authentication to services.
        /// </summary>
        /// <param name="services"></param>
        public static void AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        var keyBuilder = new SymmetricKeyBuilder(configuration);
                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateIssuerSigningKey = true,
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.FromMinutes(1),

                            ValidIssuer = "localhost",
                            ValidAudience = "players",
                            IssuerSigningKey = keyBuilder.GetSymmetricKey()
                        };

                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var authToken = context.Request.Query["access_token"];
                                
                                var path = context.HttpContext.Request.Path;
                                if (!string.IsNullOrEmpty(authToken) &&
                                    (path.StartsWithSegments("/hubs")))
                                {
                                    // Read the token out of the query string
                                    context.Token = authToken;
                                }
                                return Task.CompletedTask;
                            }
                        };
                    });

        }

        /// <summary>
        /// Adds all the scopes and bindings for dependency injection.
        /// </summary>
        /// <param name="services"></param>
        public static void AddInjections(this IServiceCollection services)
        {
            services.AddSingleton<SymmetricKeyBuilder>();
            services.AddSingleton<TokenBuilder>();
            services.AddSingleton<UserBuilder>();
            services.AddSingleton<JwtSecurityTokenHandler>();
            services.AddSingleton<MapListenerContainer>();

            services.AddSingleton<GameDataHandler>();
            services.AddTransient<MapDataHandler>();

            var bootstrapper = new Bootstrapper();
            services.AddSingleton(bootstrapper);
            services.AddSingleton(bootstrapper.GetInstance<Game>());
            services.AddSingleton(bootstrapper.GetInstance<IWorldState>());
            services.AddSingleton(bootstrapper.GetInstance<WorldEntityManager>());
            services.AddSingleton(bootstrapper.GetInstance<ICombatEntityManager>());
            services.AddSingleton(bootstrapper.GetInstance<IFormationManager>());
            services.AddSingleton(bootstrapper.GetInstance<IRepository<CharacterBase>>());
            services.AddSingleton(bootstrapper.GetInstance<IRepository<CharacterHair>>());
            services.AddTransient(typeof(CombatEntityFactory), (provider) =>
            {
                return bootstrapper.GetInstance<CombatEntityFactory>();
            });
        }

        /// <summary>
        /// Adds the game server instance and starts all game related services.
        /// </summary>
        /// <param name="app"></param>
        public static void UseGameServer(this IApplicationBuilder app)
        {
            var lifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            var game = app.ApplicationServices.GetRequiredService<Game>();
            game.CancellationToken = lifetime.ApplicationStopped;
            Task.Factory.StartNew(game.StartGame);
            app.ApplicationServices.GetRequiredService<GameDataHandler>();

            AddMapEntityListeners(app);
        }

        /// <summary>
        /// Adds MapEntityListeners to each map of the game.
        /// </summary>
        /// <param name="app"></param>
        private static void AddMapEntityListeners(IApplicationBuilder app)
        {
            var worldState = app.ApplicationServices.GetRequiredService<IWorldState>();
            var listenerContainer = app.ApplicationServices.GetRequiredService<MapListenerContainer>();
            var worldEntityManager = app.ApplicationServices.GetRequiredService<WorldEntityManager>();
            var hubContext = app.ApplicationServices.GetRequiredService<IHubContext<WorldEntityHub>>();
            var listeners = new List<MapEntityListener>();
            foreach (var mapManager in worldState.MapManagers.Values)
            {
                var listener = new MapEntityListener(mapManager, hubContext, worldEntityManager);
                listeners.Add(listener);
            }
            listenerContainer.Listeners = listeners;
        }
    }
}
