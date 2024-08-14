using Verse;

namespace LocalIndustry
{
    public class SpecialThingFilterWorker_ColonistItems : SpecialThingFilterWorker
    {
        public override bool Matches(Thing t)
        {
            return GameComponent_ColonyItems.Instance.ColonyCanUseIt(t);
        }
    }
}
