namespace EvolvedOrgansRedux {
    [RimWorld.DefOf]
    public static class DefOf {
        public static Verse.BodyPartDef Heart;
        public static Verse.BodyPartDef Lung;
        public static Verse.BodyPartDef Shoulder;
        public static Verse.BodyPartDef Eye;
        public static Verse.BodyPartDef Ear;
        public static Verse.BodyPartDef LowerShoulder;
        public static Verse.BodyPartDef EVOR_AdditionalClavicle;
        public static Verse.BodyPartDef EVOR_AdditionalArm;
        public static Verse.BodyPartDef EVOR_AdditionalHumerus;
        public static Verse.BodyPartDef EVOR_AdditionalRadius;
        public static Verse.BodyPartDef EVOR_AdditionalHand;
        public static Verse.BodyPartDef EVOR_AdditionalFinger;
        public static Verse.BodyPartDef EVOR_AdditionalEye;
        public static Verse.BodyPartDef EVOR_AdditionalEar;
        public static Verse.BodyPartDef Back;
        public static Verse.BodyPartDef Tail;
        public static Verse.BodyPartDef BodyCavity1; //Left
        public static Verse.BodyPartDef BodyCavity2; //Right
        public static Verse.BodyPartDef BodyCavityA; //Abdominal
        static DefOf() {
            RimWorld.DefOfHelper.EnsureInitializedInCtor(typeof(RimWorld.BodyPartDefOf));
        }
    }
}
