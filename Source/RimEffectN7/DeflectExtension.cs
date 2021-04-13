using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimEffectN7
{
    using Verse;

    public class DeflectExtension : DefModExtension
    {
        public float deflectChance = 0f;

        public bool Deflected => 
            Rand.Chance(this.deflectChance);
    }
}
