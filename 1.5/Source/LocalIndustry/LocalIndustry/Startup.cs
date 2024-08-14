using HarmonyLib;
using Verse;

namespace LocalIndustry
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup()
        {
            new Harmony("LocalIndustry.Mod").PatchAll();
        }
    }
}
