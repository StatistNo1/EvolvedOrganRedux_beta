using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace EvolvedOrgansRedux {
    public class Finished_EVOR_Research_CopyBodyPartsFromRecipeToRecipe : ResearchMod {
        public BodyPartDef BodyPartsToCopyFrom;
        public BodyPartDef BodyPartsToCopyTo;
        public PawnCapacityDef PawnCapacityDefToAdd;
        public float CapacityDefModifier = 1f;
        private bool AlreadyApplied = false;
        public override void Apply() {
            if (!AlreadyApplied && Settings.RequireResearchProject) {
                BodyPartAffinity.AddEVORBodyPartsToRecipes(BodyPartsToCopyFrom, BodyPartsToCopyTo, PawnCapacityDefToAdd, CapacityDefModifier);
                AlreadyApplied = true;
            }
        }
    }
    public class Finished_EVOR_Research_AddBodyPartsToEVORRecipe : ResearchMod {
        public List<BodyPartDef> EligibleBodyParts;
        public List<RecipeDef> RecipeDefsToModify;
        private bool AlreadyApplied = false;
        public override void Apply() {
            if (!AlreadyApplied && Settings.RequireResearchProject) {
                foreach (RecipeDef recipeDefToModify in RecipeDefsToModify)
                    foreach (BodyPartDef bodyPartToAdd in EligibleBodyParts)
                        //Makes sure that bodyparts that fulfil the same purpose are also added to the Recipe. Like tails from other mods
                        foreach (BodyPartDef td in DefDatabase < BodyPartDef>.AllDefs.Where(e => e.defName.Contains(bodyPartToAdd.defName)))
                            recipeDefToModify.appliedOnFixedBodyParts.Add(td);
                AlreadyApplied = true;
            }
        }
    }
    public class Finished_EVOR_Research_AddGroupsAndTags : ResearchMod {
        public BodyPartDef BodyPart;
        public List<BodyPartTagDef> TagsToAdd;
        public List<BodyPartGroupDef> GroupsToAdd;
        public bool AlreadyApplied = false;
        public override void Apply() {
            if (!AlreadyApplied && Settings.RequireResearchProject) {
                Singleton.Instance.Test.Add(this);
                BodyPart.tags.AddRange(TagsToAdd);
                BodyDef bodyDef = DefDatabase<BodyDef>.GetNamed("Human");
                List<BodyPartRecord> PartsWithDefs = bodyDef.GetPartsWithDef(BodyPart);
                foreach (BodyPartTagDef tag in TagsToAdd)
                    if (bodyDef.cachedPartsByTag.ContainsKey(tag))
                        bodyDef.cachedPartsByTag[tag].AddRange(PartsWithDefs);
                foreach (BodyPartRecord bpr in PartsWithDefs)
                    bpr.groups.AddRange(GroupsToAdd);
                Singleton.Instance.FillListsOfBodyPartTagsToRecalculate();
                AlreadyApplied = true;
            }
        }
    }
}
