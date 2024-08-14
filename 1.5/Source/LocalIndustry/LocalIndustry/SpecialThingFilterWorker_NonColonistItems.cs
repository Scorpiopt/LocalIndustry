using Verse;

namespace LocalIndustry
{
    public class SpecialThingFilterWorker_NonColonistItems : SpecialThingFilterWorker
    {
        public override bool Matches(Thing t)
        {
            return GameComponent_ColonyItems.Instance.ColonyCanUseIt(t) is false;
        }
    }
}
