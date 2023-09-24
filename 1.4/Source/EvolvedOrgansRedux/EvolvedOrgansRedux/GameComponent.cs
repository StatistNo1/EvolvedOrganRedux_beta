﻿using System.Collections.Generic;
using Verse;
using RimWorld;
using System.Linq;

namespace EvolvedOrgansRedux {
    [StaticConstructorOnStartup]
    public static class AddEVORBodyPartsToRecipesIfResearchIsNotRequired {
        static AddEVORBodyPartsToRecipesIfResearchIsNotRequired() {
            if (!Settings.RequireResearchProject) {
                BodyPartAffinity.AddEVORBodyPartsToRecipes(BodyPartDefOf.Eye, DefOf.EVOR_AdditionalEye);
                BodyPartAffinity.AddEVORBodyPartsToRecipes(DefOf.Ear, DefOf.EVOR_AdditionalEar);
                BodyPartAffinity.AddEVORBodyPartsToRecipes(DefOf.Shoulder, DefOf.LowerShoulder);
                BodyPartAffinity.AddEVORBodyPartsToRecipes(DefOf.Finger, DefOf.EVOR_AdditionalFinger);
                BodyPartAffinity.AddEVORBodyPartsToRecipes(BodyPartDefOf.Hand, DefOf.EVOR_AdditionalHand);
                BodyPartAffinity.AddEVORBodyPartsToRecipes(DefOf.Radius, DefOf.EVOR_AdditionalRadius);
                BodyPartAffinity.AddEVORBodyPartsToRecipes(DefOf.Humerus, DefOf.EVOR_AdditionalHumerus);
                BodyPartAffinity.AddEVORBodyPartsToRecipes(BodyPartDefOf.Arm, DefOf.EVOR_AdditionalArm);
                BodyPartAffinity.AddEVORBodyPartsToRecipes(DefOf.Clavicle, DefOf.EVOR_AdditionalClavicle);
                BodyPartAffinity.AddEVORBodyPartsToRecipes(BodyPartDefOf.Lung, DefOf.BodyCavity1);
                BodyPartAffinity.AddEVORBodyPartsToRecipes(BodyPartDefOf.Heart, DefOf.BodyCavity2);
            }
            BodyPartAffinity.AddCapModsToHediffs(BodyPartDefOf.Lung, PawnCapacityDefOf.Breathing, 0.1f);
            BodyPartAffinity.AddCapModsToHediffs(BodyPartDefOf.Heart, PawnCapacityDefOf.BloodPumping, 0.1f);
            Singleton.Instance.FillListsOfBodyPartTagsToRecalculate();

        }
    }
    public class GameComponent : Verse.GameComponent {
        public GameComponent(Game game) { }
        public override void StartedNewGame() {
            base.StartedNewGame();
            if (Settings.RequireResearchProject)
                ResetBodyDefsAndResearchDefsOnNewGame();
            if (Singleton.Instance.Settings.ChoicesOfWorkbenches.Count > 1 && Settings.ChosenWorkbench == Singleton.Instance.Settings.ChoicesOfWorkbenches[0]) {
                string label = "EvolvedOrgansRedux";
                string text = "ChoicesOfWorkbenchesPartOneSingle".Translate(Singleton.Instance.Settings.ChoicesOfWorkbenches[1]);
                //ToDo
                //if (Singleton.Instance.Settings.ChoicesOfWorkbenches.Count > 2 && Settings.ChosenWorkbench == Singleton.Instance.Settings.ChoicesOfWorkbenches[0]) {
                //    text = "ChoicesOfWorkbenchesPartOneMultiple".Translate();
                //    for (int i = 1; i < Singleton.Instance.Settings.ChoicesOfWorkbenches.Count; i++) {
                //        text += "\n\n" + Singleton.Instance.Settings.ChoicesOfWorkbenches[i];
                //    }
                //    text += "ChoicesOfWorkbenchesPartTwoMultiple".Translate();
                //}
                Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NeutralEvent);
            }
            if (Settings.ShowResearchMessageAtNewGame && Settings.RequireResearchProject)
                Find.LetterStack.ReceiveLetter("ShowResearchMessageAtNewGameLetterName".Translate(),
                    "ShowResearchMessageAtNewGameLetterContent".Translate(),
                    LetterDefOf.NeutralEvent);
        }
        public override void FinalizeInit() {
            base.FinalizeInit();
            if (!Settings.ImportantMessage20320905) {
                Find.LetterStack.ReceiveLetter("EvolvedOrgansReduxinfo".Translate(),
                    "ImportantMessage20320905".Translate(),
                    LetterDefOf.NeutralEvent);
                Settings.ImportantMessage20320905 = true;
            }
            if (!Settings.ImportantMessage20230917) {
                Find.LetterStack.ReceiveLetter("EvolvedOrgansReduxinfo".Translate(),
                    "ImportantMessage20230917".Translate(),
                    LetterDefOf.NeutralEvent);
                Settings.ImportantMessage20230917 = true;
            }
            LoadedModManager.GetMod<EvolvedOrgansReduxSettings>().WriteSettings();
        }
        //Any BodyParts that have been added to RecipeDefs or Tags/Groups that have been added to BodyParts have to be manually reset on new games
        //Else you can research the additional eyes on a playthrough and start a new one and the added groups, tags and stuff still sticks.
        private void ResetBodyDefsAndResearchDefsOnNewGame() {
            BodyDef bodyDef = DefDatabase<BodyDef>.GetNamed("Human");
            List<RecipeDef> list = new();
            foreach (Finished_EVOR_Research_AddGroupsAndTags defModExtension in Singleton.Instance.BodyPartsToReset) {
                BodyPartDef bodyPartDef = defModExtension.BodyPart;
                bodyPartDef.tags.Clear();
                foreach (BodyPartTagDef bodyPartTagDef in Singleton.Instance.BodyPartTagsToRecalculate.Keys)
                    if (bodyDef.cachedPartsByTag.ContainsKey(bodyPartTagDef))
                        bodyDef.cachedPartsByTag[bodyPartTagDef].RemoveAll(x => x.def == bodyPartDef);
                foreach (BodyPartRecord bpr in bodyDef.GetPartsWithDef(bodyPartDef))
                    bpr.groups.Clear();
                foreach (RecipeDef recipeDef in DefDatabase<RecipeDef>.AllDefs.Where(x => x.appliedOnFixedBodyParts.Contains(bodyPartDef)))
                    recipeDef.appliedOnFixedBodyParts.Remove(bodyPartDef);
            }
            foreach (Finished_EVOR_Research_AddGroupsAndTags r in Singleton.Instance.BodyPartsToReset)
                r.AlreadyApplied = false;
        }
    }
}