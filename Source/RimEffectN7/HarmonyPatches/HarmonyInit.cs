using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimEffectN7
{
    using HarmonyLib;
    using Verse;

    [StaticConstructorOnStartup]
    internal static class HarmonyInit
    {
        static HarmonyInit()
        {
            new Harmony("OskarPotocki.RimEffectN7").PatchAll();
        }
    }
}
