using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TRPGGame.Repository;

namespace TRPGGame
{
    /// <summary>
    /// Class containing database-related configuration data.
    /// </summary>
    public class DataConfig
    {
        public DataConfig()
        {
            using (var reader = new StreamReader(DataConstants.AssemblyLocation + "gamedataconfig.json"))
            {
                JContainer configArray = JsonConvert.DeserializeObject<JContainer>(reader.ReadToEnd());
                foreach (var item in configArray)
                {
                    DbConnectionString = item["connectionString"]?.ToObject<string>();
                }
            }
        }

        /// <summary>
        /// Contains the connection string for the database.
        /// </summary>
        public readonly string DbConnectionString;
    }
}
