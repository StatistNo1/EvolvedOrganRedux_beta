using System.Collections.Generic;
using Verse;

namespace EvolvedOrgansRedux {
    [StaticConstructorOnStartup]
    public static class Initializer {
        static Initializer() {
           //Singleton.Instance.Settings.Mod.Content.
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
                Find.LetterStack.ReceiveLetter(label, text, RimWorld.LetterDefOf.NeutralEvent);
            }
            if (Settings.ShowResearchMessageAtNewGame)
                Find.LetterStack.ReceiveLetter("ShowResearchMessageAtNewGameLetterName".Translate(),
                "ShowResearchMessageAtNewGameLetterContent".Translate(),
                RimWorld.LetterDefOf.NeutralEvent);
        }
        public override void FinalizeInit() {
            base.FinalizeInit();
            if (!Settings.ImportantMessage20320905) {
                Find.LetterStack.ReceiveLetter("EvolvedOrgansRedux info",
                    "I have removed the setting 'CombatibilitySwitchEORVersionMidSave'. This setting was only there for someone that transitioned from the original EVOR version to this one. It's been a few years now since my version is the only one up to date, so everyone should have disabled that setting by now. If you get problems with your implants switching positions or stuff like that, please let me know on the Steam page.",
                    RimWorld.LetterDefOf.NeutralEvent);
                Settings.ImportantMessage20320905 = true;
                LoadedModManager.GetMod<EvolvedOrgansReduxSettings>().WriteSettings();

            }
        }
        //Any BodyParts that have been added to RecipeDefs or Tags/Groups that have been added to BodyParts have to be manually reset.
        private void ResetBodyDefsAndResearchDefsOnNewGame() {
            BodyDef bodyDef = DefDatabase<BodyDef>.GetNamed("Human");
            List<RecipeDef> list = new();
            foreach (BodyPartDef bodyPartDef in Singleton.Instance.BodyPartDefsThatNeedsGroupsAndTagsReset) {
                bodyPartDef.tags.Clear();
                foreach (BodyPartTagDef bodyPartTagDef in Singleton.Instance.BodyPartTagsToRecalculate.Keys)
                    if (bodyDef.cachedPartsByTag.ContainsKey(bodyPartTagDef))
                        bodyDef.cachedPartsByTag[bodyPartTagDef].RemoveAll(x => x.def == bodyPartDef);
                foreach (BodyPartRecord bpr in bodyDef.GetPartsWithDef(bodyPartDef))
                    bpr.groups.Clear();
            }
            foreach (Finished_EVOR_Research_AddGroupsAndTags r in Singleton.Instance.Test)
                r.AlreadyApplied = false;

        }
    }
}