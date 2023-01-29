using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace LocalIndustry
{
    [HarmonyPatch(typeof(ScenPart_StartingThing_Defined), nameof(ScenPart_StartingThing_Defined.PlayerStartingThings))]
    public static class ScenPart_StartingThing_Defined_PlayerStartingThings_Patch
    {
        public static IEnumerable<Thing> Postfix(IEnumerable<Thing> __result)
        {
            foreach (var t in __result)
            {
                GameComponent_ColonyItems.Instance.Add(t);
                yield return t;
            }
        }
    }
}
