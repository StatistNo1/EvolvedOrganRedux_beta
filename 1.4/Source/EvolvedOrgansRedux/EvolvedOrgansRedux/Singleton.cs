using System.Collections.Generic;
using System.Linq;
using Verse;

namespace EvolvedOrgansRedux {
    public sealed class Singleton {
        private static readonly Singleton instance = new();
        public Settings Settings { get; set; } = LoadedModManager.GetMod<EvolvedOrgansReduxSettings>().GetSettings<Settings>();
        public Dictionary<BodyPartTagDef, int> BodyPartTagsToRecalculate { get; set; } = new();
        public List<Finished_EVOR_Research_AddGroupsAndTags> Test { get; set; } = new();
        public List<BodyPartDef> BodyPartDefsThatNeedsGroupsAndTagsReset { get; set; } = new() {
            DefOf.EVOR_AdditionalArm,
            DefOf.EVOR_AdditionalClavicle,
            DefOf.EVOR_AdditionalEar,
            DefOf.EVOR_AdditionalEye,
            DefOf.EVOR_AdditionalFinger,
            DefOf.EVOR_AdditionalHand,
            DefOf.EVOR_AdditionalHumerus,
            DefOf.EVOR_AdditionalRadius,
            DefOf.LowerShoulder
        };
        public List<string> ForbiddenMods { get; set; } = new() {
                "Android tiers",
                "Android tiers - TX Series",
                "Android Tiers Reforged"
            };
        public string NameOfThisMod { get; set; }
        public List<RecipeDef> AddAdditionalShouldersToRecipedef { get; set; } = new();
        public List<RecipeDef> AddAdditionalEarToRecipedef { get; set; } = new();
        public List<RecipeDef> AddAdditionalEyeToRecipedef { get; set; } = new();
        public List<RecipeDef> AddLeftchestcavityToRecipedef { get; set; } = new();
        public List<RecipeDef> AddRightchestcavityToRecipedef { get; set; } = new();
        static Singleton() {}
        private Singleton() {}
        public static Singleton Instance { get { return instance; } }
        public void FillListsOfBodyPartTagsToRecalculate() {
            BodyPartTagsToRecalculate.Clear();
            AddBodyPartTagDefToDictionary(RimWorld.BodyPartTagDefOf.ManipulationLimbCore);
            AddBodyPartTagDefToDictionary(RimWorld.BodyPartTagDefOf.ManipulationLimbSegment);
            AddBodyPartTagDefToDictionary(RimWorld.BodyPartTagDefOf.ManipulationLimbDigit);
            AddBodyPartTagDefToDictionary(RimWorld.BodyPartTagDefOf.MovingLimbCore);
            AddBodyPartTagDefToDictionary(RimWorld.BodyPartTagDefOf.SightSource);
            AddBodyPartTagDefToDictionary(RimWorld.BodyPartTagDefOf.HearingSource);
        }
        public void AddBodyPartTagDefToDictionary(BodyPartTagDef bodyPartTagDef) {
            BodyPartTagsToRecalculate.Add(bodyPartTagDef, DefDatabase<BodyDef>.GetNamed("Human").AllParts.
                Where(e => e.def.tags.Contains(bodyPartTagDef) &&
                e.def.modContentPack == Settings.Mod.Content)
                .Count());
        }
    }
}
