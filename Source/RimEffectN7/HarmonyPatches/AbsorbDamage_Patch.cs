namespace RimEffectN7
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using HarmonyLib;
    using JetBrains.Annotations;
    using Mono.Cecil.Cil;
    using Verse;
    using Verse.Noise;

    [HarmonyPatch(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.PreApplyDamage))]
    public static class AbsorbDamage_Patch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        {
            MethodInfo spawnedInfo = AccessTools.PropertyGetter(typeof(Thing), nameof(Pawn.Spawned));
            bool       foundFirst  = false;

            Label label = ilg.DefineLabel();

            foreach (CodeInstruction instruction in instructions)
            {
                if(instruction.Calls(spawnedInfo))
                    if (!foundFirst)
                        foundFirst = true;
                    else
                    {
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AbsorbDamage_Patch), nameof(Absorb)))
                                     {
                                         labels = instruction.ExtractLabels()
                                     };
                        yield return new CodeInstruction(OpCodes.Brfalse_S, label);
                        yield return new CodeInstruction(OpCodes.Ldarg_2);
                        yield return new CodeInstruction(OpCodes.Ldc_I4_1);
                        yield return new CodeInstruction(OpCodes.Stind_I1);
                        yield return new CodeInstruction(OpCodes.Ret);
                        yield return new CodeInstruction(OpCodes.Ldarg_0) {labels = new List<Label> { label }};
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Pawn_HealthTracker), "pawn"));
                    }

                yield return instruction;
            }
        }

        public static bool Absorb(Pawn pawn) => 
            pawn.equipment.Primary?.def.GetModExtension<DeflectExtension>()?.Deflected ?? false;
    }
}
