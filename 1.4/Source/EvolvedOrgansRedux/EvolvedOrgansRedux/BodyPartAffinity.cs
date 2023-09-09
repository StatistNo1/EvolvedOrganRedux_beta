namespace EvolvedOrgansRedux {
    public class BodyPartAffinity {
        enum BodyPart {
            None,
            Shoulder,
            Lung,
            Heart
        }
        System.Collections.Generic.List<Verse.RecipeDef> RemoveImplantRecipesToAdd = new System.Collections.Generic.List<Verse.RecipeDef>();
        public BodyPartAffinity() {
            string newBodyPart = "";
            Verse.RecipeDef recipe = null;
            string modOfRecipe = "";
            BodyPart bodypart;
            //Put the new RecipeDefs first into this list and add them later
            foreach (Verse.RecipeDef rd in Verse.DefDatabase<Verse.RecipeDef>.AllDefs) {
                try {
                    modOfRecipe = rd.modContentPack.Name;
                } catch {
                    modOfRecipe = "The mod can't be determined.";
                }
                newBodyPart = "Something went wrong -> Step 1";
                try {
                    newBodyPart = "Something went wrong -> Step 2";
                    if (rd.appliedOnFixedBodyParts.Contains(DefOf.Shoulder))
                        bodypart = BodyPart.Shoulder;
                    else if (rd.appliedOnFixedBodyParts.Contains(DefOf.Lung))
                        bodypart = BodyPart.Lung;
                    else if (rd.appliedOnFixedBodyParts.Contains(DefOf.Heart))
                        bodypart = BodyPart.Heart;
                    else
                        bodypart = BodyPart.None;

                    //Natural body parts can't be added. It's just how the game is coded.
                    if (bodypart != BodyPart.None &&
                        rd.workerClass == typeof(RimWorld.Recipe_InstallArtificialBodyPart) &&
                        rd.modContentPack.Name != Singleton.Instance.NameOfThisMod &&
                        rd.recipeUsers != null &&
                        rd.IsSurgery &&
                        rd.defaultIngredientFilter.AnyAllowedDef != null &&
                        rd.appliedOnFixedBodyParts.Count == 1 &&
                        !rd.defaultIngredientFilter.AnyAllowedDef.thingCategories.Contains(Verse.DefDatabase<Verse.ThingCategoryDef>.GetNamed("BodyPartsNatural"))) {
                        recipe = rd;
                        float originalPartEfficiency = rd.addsHediff.addedPartProps.partEfficiency;
                        newBodyPart = "Something went wrong -> Step 3";
                        switch (bodypart) {
                            case BodyPart.Shoulder:
                                newBodyPart = "LowerShoulder";
                                Singleton.Instance.AddLowershouldersToRecipedef.Add(rd);
                                makeNewUninstallRecipe(rd);
                                break;
                            case BodyPart.Lung:
                                newBodyPart = "BodyCavity1";
                                //Add all other lungs to the RecipeDef, except the natural one.
                                Singleton.Instance.AddLeftchestcavityToRecipedef.Add(rd);
                                //Gives them the Breathing stat so they actually do something else than just filling a hole.
                                if (rd.addsHediff.stages == null)
                                    rd.addsHediff.stages = new() { new Verse.HediffStage() };
                                rd.addsHediff.stages[0].capMods.Add(AddCapMods(RimWorld.PawnCapacityDefOf.Breathing, originalPartEfficiency));
                                //Reduces the effience by the amount of offset to get it back the base stats.
                                originalPartEfficiency *= 0.9f;
                                break;
                            case BodyPart.Heart:
                                newBodyPart = "BodyCavity2";
                                //Add all other hearts to the RecipeDef, except the natural one.
                                Singleton.Instance.AddRightchestcavityToRecipedef.Add(rd);
                                //Gives them the BloodPunping stat so they actually do something else than just filling a hole.
                                if (rd.addsHediff.stages == null)
                                    rd.addsHediff.stages = new() { new Verse.HediffStage() };
                                rd.addsHediff.stages[0].capMods.Add(AddCapMods(RimWorld.PawnCapacityDefOf.BloodPumping, originalPartEfficiency));
                                //Reduces the effience by the amount of offset to get it back the base stats.
                                rd.addsHediff.addedPartProps.partEfficiency *= 0.9f;
                                break;
                        }
                    }
                } catch (System.Exception e) {
                    string errorMessage = "";
                    errorMessage += "BodyPartAffinity Exception";
                    errorMessage += "\nFailed to copy recipe: " + recipe.defName;
                    errorMessage += "\nRecipe is part of mod:: " + modOfRecipe;
                    errorMessage += "\nAffected body part: " + newBodyPart;
                    errorMessage += "\nCurrently applied to these BodyParts: ";
                    foreach (Verse.BodyPartDef bpd in recipe.appliedOnFixedBodyParts) {
                        errorMessage += "\n" + bpd.defName;
                    }
                    errorMessage += "\nException: " + e;
                    Verse.Log.Error(errorMessage);
                }
            }
            foreach (Verse.RecipeDef recipeDef in RemoveImplantRecipesToAdd) {
                Verse.DefDatabase<Verse.RecipeDef>.Add(recipeDef);
            }
            Verse.DefDatabase<Verse.RecipeDef>.ResolveAllReferences();
        }

        private Verse.PawnCapacityModifier AddCapMods( Verse.PawnCapacityDef pcd, float partEfficiency) {
            Verse.PawnCapacityModifier pcm = new ();
            pcm.capacity = pcd;
            pcm.offset = partEfficiency * 0.1f;
            return pcm;
        }
        private void makeNewUninstallRecipe(Verse.RecipeDef rd) {
            Verse.RecipeDef template = Verse.DefDatabase<Verse.RecipeDef>.GetNamed("EVOR_SurgeryRemove_Appendage_ExtradextrousArm");
            Verse.RecipeDef recipeDef = new Verse.RecipeDef();
            recipeDef.defName = "EVOR_SurgeryRemove_" + rd.defName;
            try {
                recipeDef.label = "Remove " + rd.descriptionHyperlinks.Find(e => e.def.GetType() == typeof(Verse.ThingDef)).def.label;
            } catch {
                recipeDef.label = "Remove";
            }
            recipeDef.description = rd.description;
            recipeDef.jobString = rd.jobString;
            recipeDef.effectWorking = template.effectWorking;
            recipeDef.soundWorking = template.soundWorking;
            recipeDef.workSpeedStat = template.workSpeedStat;
            recipeDef.workSkill = template.workSkill;
            recipeDef.workSkillLearnFactor = template.workSkillLearnFactor;
            recipeDef.workAmount = template.workAmount;
            recipeDef.isViolation = template.isViolation;
            recipeDef.skillRequirements = template.skillRequirements;
            recipeDef.recipeUsers = rd.recipeUsers;
            recipeDef.ingredients = template.ingredients;
            recipeDef.fixedIngredientFilter = template.fixedIngredientFilter;
            recipeDef.workerClass = template.workerClass;
            recipeDef.descriptionHyperlinks = rd.descriptionHyperlinks;
            recipeDef.removesHediff = rd.addsHediff;
            RemoveImplantRecipesToAdd.Add(recipeDef);
        }
    }
}
