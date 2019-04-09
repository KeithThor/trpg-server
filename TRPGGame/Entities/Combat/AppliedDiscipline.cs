using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities.Combat
{
    public class AppliedDiscipline
    {
        public Discipline BaseDiscipline { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
    }
}
