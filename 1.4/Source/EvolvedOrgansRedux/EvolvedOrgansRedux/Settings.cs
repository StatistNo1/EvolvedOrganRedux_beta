namespace EvolvedOrgansRedux {
    public class Settings : Verse.ModSettings {
        public static bool BodyPartAffinity = true;
        public static string ChosenWorkbench = "Evolved Organs Redux";
        public static bool ImportantMessage20320905 = false;
        public static int AmountOfArms = 4;
        public System.Collections.Generic.List<string> ChoicesOfWorkbenches = new() { "Evolved Organs Redux" };
        public System.Collections.Generic.List<int> ChoicesForAmountOfArms = new () { 2, 4, 6, 8 };
        public override void ExposeData() {
            Verse.Scribe_Values.Look(ref BodyPartAffinity, "BodyPartAffinity", defaultValue: true);
            Verse.Scribe_Values.Look(ref ChosenWorkbench, "ChosenWorkbench", defaultValue: "Evolved Organs Redux");
            Verse.Scribe_Values.Look(ref ImportantMessage20320905, "RemovedSetting-CombatibilitySwitchEORVersionMidSave", defaultValue: false, true);
            Verse.Scribe_Values.Look(ref AmountOfArms, "AmountOfArms", defaultValue: 4);
            base.ExposeData();
        }
        public Settings() {
            if (ChoicesOfWorkbenches.Contains(ChosenWorkbench))
                ChosenWorkbench = ChoicesOfWorkbenches[0];
        }
    }

    public class EvolvedOrgansReduxSettings : Verse.Mod {
        readonly Settings settings;
        public EvolvedOrgansReduxSettings(Verse.ModContentPack content) : base(content) {
            this.settings = GetSettings<Settings>();
        }
        public override void DoSettingsWindowContents(UnityEngine.Rect inRect) {
            Verse.Listing_Standard listingStandard = new Verse.Listing_Standard();
            listingStandard.Begin(inRect);
            AddSetting("Check this option if you want to put archotech arms on lower shoulders or additional non-Evor lungs into your body.\nThe game has to be restarted for this change to take effect.", listingStandard);
            listingStandard.CheckboxLabeled("BodyPartAffinity", ref Settings.BodyPartAffinity);
            AddSetting("Check of which mod the organ workbench should be used to create EvolvedOrgansRedux implants.\nThe game has to be restarted for this change to take effect.", listingStandard);
            for (int i = 0; i < settings.ChoicesOfWorkbenches.Count; i++) {
                if (listingStandard.RadioButton(settings.ChoicesOfWorkbenches[i], Settings.ChosenWorkbench == settings.ChoicesOfWorkbenches[i], tabIn: 30f)) {
                    Settings.ChosenWorkbench = settings.ChoicesOfWorkbenches[i];
                }
            }
            AddSetting("Choose up to how many arms your pawns should have.", listingStandard);
            for (int i = 0; i < settings.ChoicesForAmountOfArms.Count; i++) {
                if (listingStandard.RadioButton(settings.ChoicesForAmountOfArms[i].ToString(), Settings.AmountOfArms == settings.ChoicesForAmountOfArms[i], tabIn: 30f)) {
                    Settings.AmountOfArms = settings.ChoicesForAmountOfArms[i];
                }
            }
            listingStandard.End();
        }
        private void AddSetting(string label, Verse.Listing_Standard listingStandard) {
            listingStandard.GapLine();
            listingStandard.Label(label);
            listingStandard.Gap();
        }
        public override string SettingsCategory() {
            return "EvolvedOrgansRedux";
        }
    }
}