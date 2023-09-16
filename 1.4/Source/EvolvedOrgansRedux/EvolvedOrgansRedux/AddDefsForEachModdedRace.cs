using System.Linq;

namespace EvolvedOrgansRedux {
    [Verse.StaticConstructorOnStartup]
    public static class AddDefsForEachModdedRace {
        static AddDefsForEachModdedRace() {
            bool meatRecipeWasNotRemovedForCompatibilityReasons = Verse.DefDatabase<Verse.RecipeDef>.GetNamedSilentFail("EVOR_Production_Protein_Humie") != null;
            //Go through all the ThingDefs to find out which humanoid races exist
            foreach (Verse.ThingDef def in Verse.DefDatabase<Verse.ThingDef>.AllDefs) {
                //If the race is Humanoid but not the base race of this game
                if (def.race?.Humanlike == true && !def.defName.Equals("Human") && (def.modContentPack == null || !Singleton.Instance.ForbiddenMods.Contains(def.modContentPack.Name))) { //Human stuff will just be solved with the XMLs.
                    try {
                        AddBodyParts(def.race);
                        if (meatRecipeWasNotRemovedForCompatibilityReasons) {
                            new AddMeatToRecipesFromModdedRace(def.race);
                            if (Verse.Prefs.LogVerbose)
                                Verse.Log.Message("EvolvedOrgansRedux: Meat of " + def.defName + " was added to the protein recipe.");
                        }
                    } catch (System.Exception e) {
                        Verse.Log.Error(
                            "EvolvedOrgansRedux : AddDefsForEachModdedRace" +
                            "\nProblematic race: " + def.defName +
                            "\nError: " + e
                            );
                    }
                }
            }
        }
        public static void AddBodyParts(Verse.RaceProperties raceProperties) {
            //Get the BodyPartRecord that has been added via XML to the human body
            //CoreParts for Shoulders, tail, back bodycavity
            System.Collections.Generic.List<Verse.BodyPartRecord> CorePartsToAdd =
                new(Verse.DefDatabase<Verse.BodyDef>.GetNamed("Human").corePart.parts
                .Where(e => e.def.modContentPack == Singleton.Instance.Settings.Mod.Content));
            //Headparts for Eyes and ears
            System.Collections.Generic.List<Verse.BodyPartRecord> HeadPartsToAdd =
                new(Verse.DefDatabase<Verse.BodyDef>.GetNamed("Human").corePart.parts
                .Find(e => e.def == RimWorld.BodyPartDefOf.Neck).parts
                .Find(e => e.def == RimWorld.BodyPartDefOf.Head).parts
                .Where(e => e.def.modContentPack == Singleton.Instance.Settings.Mod.Content));
            //Add the BodyParts this mod adds to the other race's body, if a similar body part already exists, change the tags.
            foreach (Verse.BodyPartRecord bpr in CorePartsToAdd)
                if (!raceProperties.body.AllParts.Exists(e => e.def.defName.ToLower().Contains(bpr.def.defName.ToLower())))
                    raceProperties.body.corePart.parts.Add(CopyBodyPartRecord(bpr, raceProperties));
                else
                    foreach (Verse.BodyPartTagDef bptd in bpr.def.tags)
                        if (!raceProperties.body.AllParts.Find(e => e.def.defName.ToLower().Contains(bpr.def.defName.ToLower())).def.tags.Contains(bptd))
                            raceProperties.body.AllParts.Find(e => e.def.defName.ToLower().Contains(bpr.def.defName.ToLower())).def.tags.Add(bptd);
            foreach (Verse.BodyPartRecord bpr in HeadPartsToAdd)
                if (!raceProperties.body.AllParts.Exists(e => e.def.defName.ToLower().Contains(bpr.def.defName.ToLower())))
                    raceProperties.body.corePart.parts.Add(CopyBodyPartRecord(bpr, raceProperties));
                else
                    foreach (Verse.BodyPartTagDef bptd in bpr.def.tags)
                        if (!raceProperties.body.AllParts.Find(e => e.def.defName.ToLower().Contains(bpr.def.defName.ToLower())).def.tags.Contains(bptd))
                            raceProperties.body.AllParts.Find(e => e.def.defName.ToLower().Contains(bpr.def.defName.ToLower())).def.tags.Add(bptd);
            Verse.DefDatabase<Verse.BodyDef>.GetNamed(raceProperties.body.defName).AllParts.Clear();
            Verse.DefDatabase<Verse.BodyDef>.GetNamed(raceProperties.body.defName).ResolveReferences();
        }
        private static Verse.BodyPartRecord CopyBodyPartRecord(Verse.BodyPartRecord bpr, Verse.RaceProperties raceProperties) {
            return new Verse.BodyPartRecord {
                body = raceProperties.body,
                coverage = bpr.coverage,
                coverageAbs = bpr.coverageAbs,
                coverageAbsWithChildren = bpr.coverageAbsWithChildren,
                customLabel = bpr.customLabel,
                def = bpr.def,
                depth = bpr.depth,
                groups = bpr.groups,
                height = bpr.height,
                parent = Verse.DefDatabase<Verse.BodyDef>.GetNamed(raceProperties.body.defName).corePart,
                untranslatedCustomLabel = bpr.untranslatedCustomLabel
            };
        }
    }
}
