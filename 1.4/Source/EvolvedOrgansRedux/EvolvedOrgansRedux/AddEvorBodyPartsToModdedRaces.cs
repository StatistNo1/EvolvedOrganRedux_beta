using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace EvolvedOrgansRedux {
    [StaticConstructorOnStartup]
    public static class AddEvorBodyPartsToModdedRaces {
        static AddEvorBodyPartsToModdedRaces() {
            bool meatRecipeWasNotRemovedForCompatibilityReasons = DefDatabase<RecipeDef>.GetNamedSilentFail("EVOR_Production_Protein_Humie") != null;
            //Go through all the ThingDefs to find out which humanoid races exist
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs) {
                //If the race is Humanoid but not the base race of this game
                if (def.race?.Humanlike == true && !def.defName.Equals("Human") &&
                    (def.modContentPack == null || !Singleton.Instance.ForbiddenMods.Contains(def.modContentPack.Name))) {
                    try {
                        AddBodyParts(def.race);
                        if (meatRecipeWasNotRemovedForCompatibilityReasons) {
                            AddMeatToRecipesFromModdedRace(def.race);
                            if (Prefs.LogVerbose)
                                Log.Message("EvolvedOrgansRedux: Meat of " + def.defName + " was added to the protein recipe.");
                        }
                    } catch (System.Exception e) {
                        Log.Error(
                            "EvolvedOrgansRedux : AddDefsForEachModdedRace" +
                            "\nProblematic race: " + def.defName +
                            "\nError: " + e);
                    }
                }
            }
        }
        private static void AddMeatToRecipesFromModdedRace(RaceProperties raceProperties) {
            if (!ModLister.HasActiveModWithName("Questionable Ethics Enhanced")) {
                DefDatabase<RecipeDef>.GetNamed("EVOR_Production_Protein_Humie").fixedIngredientFilter.SetAllow(raceProperties.meatDef, true);
                DefDatabase<RecipeDef>.GetNamed("EVOR_Production_Protein_Humie").ClearCachedData();
                DefDatabase<RecipeDef>.GetNamed("EVOR_Production_Protein_Humie").ResolveReferences();
                DefDatabase<RecipeDef>.GetNamed("EVOR_Production_Protein_Humie_Bulk").fixedIngredientFilter.SetAllow(raceProperties.meatDef, true);
                DefDatabase<RecipeDef>.GetNamed("EVOR_Production_Protein_Humie_Bulk").ClearCachedData();
                DefDatabase<RecipeDef>.GetNamed("EVOR_Production_Protein_Humie_Bulk").ResolveReferences();
            }
        }
        private static void AddBodyParts(RaceProperties raceProperties) {
            Singleton.Instance.FillListsOfBodyPartTagsToRecalculate();
            foreach (BodyPartRecord bodyPartRecord in DefDatabase<BodyDef>.GetNamed("Human").AllParts
                .Where(e => e.def.modContentPack == Singleton.Instance.Settings.Mod.Content)) {
                bool alreadyHasSimilarPart = raceProperties.body.AllParts.Exists(e => e.def.defName.ToLower().Contains(bodyPartRecord.def.defName.ToLower()));
                if (alreadyHasSimilarPart) {
                    List<BodyPartRecord> similarBodyParts = raceProperties.body.AllParts.Where(e => e.def.defName.ToLower().Contains(bodyPartRecord.def.defName.ToLower())).ToList();
                    foreach (BodyPartRecord bpr in similarBodyParts) {
                        if (bpr.def.tags == null) bpr.def.tags = new();
                        if (bpr.def.tags.Contains(BodyPartTagDefOf.MovingLimbSegment)) bpr.def.tags.Remove(BodyPartTagDefOf.MovingLimbSegment);
                        if (bpr.def.tags.Contains(BodyPartTagDefOf.ManipulationLimbSegment)) bpr.def.tags.Remove(BodyPartTagDefOf.ManipulationLimbSegment);

                        foreach (BodyPartTagDef tag in bodyPartRecord.def.tags) {
                            if (!bpr.def.tags.Contains(tag))
                                bpr.def.tags.Add(tag);
                        }
                    }
                } else if (bodyPartRecord.parent.def == BodyPartDefOf.Torso || bodyPartRecord.parent.def == BodyPartDefOf.Head) {
                    BodyPartRecord parent = raceProperties.body.AllParts.Find(e => e.Label == bodyPartRecord.parent.Label);
                    AddParts(bodyPartRecord, raceProperties, parent);
                }
            }
            DefDatabase<BodyDef>.GetNamed(raceProperties.body.defName).AllParts.Clear();
            DefDatabase<BodyDef>.GetNamed(raceProperties.body.defName).ResolveReferences();
        }
        private static void AddParts(BodyPartRecord bodyPartRecord, RaceProperties raceProperties, BodyPartRecord parent) {
            BodyPartRecord newBodyPart = CopyBodyPartRecord(bodyPartRecord, raceProperties, parent);
            parent.parts.Add(newBodyPart);
            if (bodyPartRecord.GetDirectChildParts().Count() > 0)
                foreach (BodyPartRecord bpr in bodyPartRecord.GetDirectChildParts())
                    AddParts(bpr, raceProperties, newBodyPart);
        }
        private static BodyPartRecord CopyBodyPartRecord(BodyPartRecord bpr, RaceProperties raceProperties, BodyPartRecord parent) {
            return new BodyPartRecord {
                body = raceProperties.body,
                coverage = bpr.coverage,
                coverageAbs = bpr.coverageAbs,
                coverageAbsWithChildren = bpr.coverageAbsWithChildren,
                customLabel = bpr.customLabel,
                def = bpr.def,
                depth = bpr.depth,
                groups = bpr.groups,
                height = bpr.height,
                parent = parent,
                untranslatedCustomLabel = bpr.untranslatedCustomLabel
            };
        }
    }
}
