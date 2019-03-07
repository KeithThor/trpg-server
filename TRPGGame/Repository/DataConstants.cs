using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace TRPGGame.Repository
{
    public static class DataConstants
    {
        public static string AssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}
