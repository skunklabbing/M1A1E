using GHPC.Equipment;
using UnityEngine;
using System.IO;
using MelonLoader.Utils;
using ModUtil;

namespace M1A1Abrams
{
    public class DUArmour : Module
    {
        static Material gen1_du_aar_mat;
        static ArmorCodexScriptable gen1_du_armor_codex;

        static Material gen2_du_aar_mat;
        static ArmorCodexScriptable gen2_du_armor_codex;

        public static Material gen3_du_aar_mat;
        public static ArmorCodexScriptable gen3_du_armor_codex;

        public static ArmorCodexScriptable nera_armor_codex;
        public static ArmorCodexScriptable composite_armor_codex;
        public static ArmorCodexScriptable aar_hitbox_armor_codex;

        public static Material[] du_aar_mats;
        public static ArmorCodexScriptable[] du_armor_codexes;
        //public static ArmorCodexScriptable[] hitbox_armor_codex;
        //public static ArmorCodexScriptable[] special_armor_codexes;

        public override void LoadStaticAssets()
        {
            AssetBundle mats = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/", "du_mats"));
            gen1_du_aar_mat = mats.LoadAsset<Material>("gen1.mat");
            gen1_du_aar_mat.hideFlags = HideFlags.DontUnloadUnusedAsset;

            gen2_du_aar_mat = mats.LoadAsset<Material>("gen2.mat");
            gen2_du_aar_mat.hideFlags = HideFlags.DontUnloadUnusedAsset;

            gen3_du_aar_mat = mats.LoadAsset<Material>("gen3.mat");
            gen3_du_aar_mat.hideFlags = HideFlags.DontUnloadUnusedAsset;

            gen1_du_armor_codex = ScriptableObject.CreateInstance<ArmorCodexScriptable>();
            gen1_du_armor_codex.name = "Gen 1 Abrams DU composite";

            gen2_du_armor_codex = ScriptableObject.CreateInstance<ArmorCodexScriptable>();
            gen2_du_armor_codex.name = "Gen 2 Abrams DU composite";

            gen3_du_armor_codex = ScriptableObject.CreateInstance<ArmorCodexScriptable>();
            gen3_du_armor_codex.name = "Gen 3 Abrams DU composite";

            nera_armor_codex = ScriptableObject.CreateInstance<ArmorCodexScriptable>();
            nera_armor_codex.name = "Non-Explosive Reactive Armor";

            composite_armor_codex = ScriptableObject.CreateInstance<ArmorCodexScriptable>();
            composite_armor_codex.name = "Special Composite Armor";

            aar_hitbox_armor_codex = ScriptableObject.CreateInstance<ArmorCodexScriptable>();
            aar_hitbox_armor_codex.name = "Generic Armor";

            ArmorType gen1 = new ArmorType();
            gen1.Name = "special armor";
            gen1.CanRicochet = true;
            gen1.CanShatterLongRods = true;
            gen1.NormalizesHits = true;
            gen1.ThicknessSource = ArmorType.RhaSource.Multipliers;
            gen1.SpallAngleMultiplier = 1;
            gen1.SpallPowerMultiplier = 0.80f;
            gen1.RhaeMultiplierCe = 1.4f;
            gen1.RhaeMultiplierKe = 0.75f;
            gen1_du_armor_codex.ArmorType = gen1;

            ArmorType gen2 = new ArmorType();
            gen2.Name = "special armor";
            gen2.CanRicochet = true;
            gen2.CanShatterLongRods = true;
            gen2.NormalizesHits = true;
            gen2.ThicknessSource = ArmorType.RhaSource.Multipliers;
            gen2.SpallAngleMultiplier = 1;
            gen2.SpallPowerMultiplier = 0.80f;
            gen2.RhaeMultiplierCe = 1.55f;
            gen2.RhaeMultiplierKe = 0.85f;
            gen2_du_armor_codex.ArmorType = gen2;

            ArmorType gen3 = new ArmorType();
            gen3.Name = "special armor";
            gen3.CanRicochet = true;
            gen3.CanShatterLongRods = true;
            gen3.NormalizesHits = true;
            gen3.ThicknessSource = ArmorType.RhaSource.Multipliers;
            gen3.SpallAngleMultiplier = 1;
            gen3.SpallPowerMultiplier = 0.80f;
            gen3.RhaeMultiplierCe = 1.75f;
            gen3.RhaeMultiplierKe = 0.96f;
            gen3_du_armor_codex.ArmorType = gen3;

            ArmorType nera = new ArmorType();
            nera.Name = "nera armor";
            nera.CanRicochet = true;
            nera.CanShatterLongRods = true;
            nera.NormalizesHits = true;
            nera.ThicknessSource = ArmorType.RhaSource.Multipliers;
            nera.SpallAngleMultiplier = 0.49f;
            nera.SpallPowerMultiplier = 0.25f;
            nera.RhaeMultiplierCe = 1.98f;
            nera.RhaeMultiplierKe = 0.85f;
            nera_armor_codex.ArmorType = nera;

            ArmorType cmp = new ArmorType();
            cmp.Name = "composite armor";
            cmp.CanRicochet = true;
            cmp.CanShatterLongRods = true;
            cmp.NormalizesHits = true;
            cmp.ThicknessSource = ArmorType.RhaSource.Multipliers;
            cmp.SpallAngleMultiplier = 0.58f;
            cmp.SpallPowerMultiplier = 0.82f;
            cmp.RhaeMultiplierCe = 2.1f;
            cmp.RhaeMultiplierKe = 1f;
            composite_armor_codex.ArmorType = cmp;

            ArmorType gena = new ArmorType();
            gena.Name = "generic armor";
            gena.CanRicochet = true;
            gena.CanShatterLongRods = true;
            gena.NormalizesHits = true;
            gena.ThicknessSource = ArmorType.RhaSource.Multipliers;
            gena.SpallAngleMultiplier = 1f;
            gena.SpallPowerMultiplier = 1f;
            gena.RhaeMultiplierCe = 0.5f;
            gena.RhaeMultiplierKe = 0.5f;
            aar_hitbox_armor_codex.ArmorType = gena;

            du_aar_mats = new Material[] { gen1_du_aar_mat, gen2_du_aar_mat, gen3_du_aar_mat };
            du_armor_codexes = new ArmorCodexScriptable[] { gen1_du_armor_codex, gen2_du_armor_codex, gen3_du_armor_codex, };
            //hitbox_armor_codex = new ArmorCodexScriptable[] { aar_hitbox_armor_codex };
            //special_armor_codexes = new ArmorCodexScriptable[] { nera_armor_codex, composite_armor_codex };
        }
    }
}
