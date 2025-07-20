using HarmonyLib;
using System.Collections.Generic;
using Verse;
using RimWorld;
using System.Reflection.Emit;
using System.Reflection;

namespace FishingWorkType
{

    [StaticConstructorOnStartup]
    static class FishingWorkType
    {
        public static WorkTypeDef Fishing;
        public static Harmony harmony;
        static FishingWorkType()
        {
            // Initialize the Fishing WorkTypeDef
            var harmony = new Harmony("net.cadmonkey.rimworld.mod.FishingWorkType");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(Zone_Fishing), "GetFloatMenuOptions", MethodType.Enumerator)]
        public static class Zone_Fishing_GetFloatMenuOptions_MoveNext_Patch
        {
            // Patch the menu options to not reject fishing if a pawn cannot hunt as they are now seperate
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                foreach (var instruction in instructions)
                {
                    // Replace IL_03b0: ldsfld class Verse.WorkTypeDef RimWorld.WorkTypeDefOf::Hunting
                    // with IL_03b0: ldsfld class Verse.WorkTypeDef FishingWorkType.FishingWorkType::Fishing
                    // A pawn would never be disallowed to fish currently, but that could be added in the future if desired
                    if (instruction.opcode == OpCodes.Ldsfld)
                    {
                        var field = instruction.operand as FieldInfo;
                        if (field != null && field.Name == "Hunting")
                        {
                            yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(FishingWorkType), nameof(Fishing)));
                            continue;
                        }
                    }
                    yield return instruction;
                }
            }
        }
    }
}
