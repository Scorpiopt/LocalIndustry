using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace LocalIndustry
{
    [HarmonyPatch(typeof(GenRecipe), "MakeRecipeProducts")]
    public static class GenRecipe_MakeRecipeProducts_Patch
    {
        public static IEnumerable<Thing> Postfix(IEnumerable<Thing> __result, RecipeDef recipeDef, Pawn worker)
        {
            bool craftedItem = false;
            foreach (var t in __result)
            {
                Log.Message("Worker: " + worker + " - " + worker.Faction);
                if (worker.Faction == Faction.OfPlayer)
                {
                    craftedItem = true;
                    GameComponent_ColonyItems.Instance.Add(t);
                }
                yield return t;
            }

            if (craftedItem && worker?.needs?.mood?.thoughts?.memories != null)
            {
                worker.needs.mood.thoughts.memories.TryGainMemory(LI_DefOf.LI_CraftedItem);
            }
        }
    }
}
