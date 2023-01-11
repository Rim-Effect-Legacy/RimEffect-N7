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
