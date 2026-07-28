using GHPC;
using GHPC.AI;
using GHPC.Camera;
using GHPC.Equipment;
using GHPC.Thermals;
using GHPC.Utility;
using GHPC.Vehicle;
using GHPC.Weaponry;
using GHPC.Weapons;
using MelonLoader;
using MelonLoader.Utils;
using ModUtil;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace M1A1Abrams
{
    internal class Assets : Module
    {
        public static AmmoType ammo_m833;
        public static AmmoType ammo_m456;

        public static GameObject flir_post;
        public static Material flir_blit_mat_green;
        public static Material flir_blit_mat_green_no_scan;

        public static GameObject citv_monitor;

        public static GameObject citv_obj;
        public static GameObject m256_obj;
        public static GameObject addon1_obj;
        public static GameObject tusk_obj;
        public static GameObject dogh_obj;

        public static GameObject trophy_obj;

        public static GameObject avis_obj;

        //public static ArmorCodexScriptable gen1_nerax_codex;



        public override void UnloadDynamicAssets()
        {
            Material.DestroyImmediate(flir_blit_mat_green_no_scan);
        }

        public override void LoadDynamicAssets()
        {
            if (!AssetUtil.VehicleInMission("_M1IP (variant)") && !AssetUtil.VehicleInMission("_M1 (variant)")) return;

            Vehicle vic;
            if (AssetUtil.VehicleInMission("_M1IP (variant)"))
            {
                vic = AssetUtil.LoadVanillaVehicle("M1IP");
            }
            else
            {
                vic = AssetUtil.LoadVanillaVehicle("M1");
            }

            Transform flir = vic.GetComponentInChildren<FireControlSystem>(true).transform.Find("FLIR");

            flir_post = flir.Find("FLIR Post Processing - Green").gameObject;
            flir_blit_mat_green = flir.GetComponent<CameraSlot>().FLIRBlitMaterialOverride;
            flir_blit_mat_green_no_scan = new Material(Shader.Find("Blit (FLIR)/Blit Simple"));
            flir_blit_mat_green_no_scan.CopyPropertiesFromMaterial(flir_blit_mat_green);
            flir_blit_mat_green_no_scan.SetTexture("_PixelCookie", null);
        }

        public override void LoadStaticAssets()
        {
            ammo_m833 = Resources.FindObjectsOfTypeAll<AmmoCodexScriptable>().Where(o => o.name == "ammo_M833").First().AmmoType;
            ammo_m456 = Resources.FindObjectsOfTypeAll<AmmoCodexScriptable>().Where(o => o.name == "ammo_M456").First().AmmoType;

            var citv_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/", "citv_monitor"));
            citv_monitor = citv_bundle.LoadAsset<GameObject>("CITV MONITOR");
            citv_monitor.hideFlags = HideFlags.DontUnloadUnusedAsset;

            var citv_obj_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/", "citv"));
            citv_obj = citv_obj_bundle.LoadAsset<GameObject>("citv.prefab");
            citv_obj.hideFlags = HideFlags.DontUnloadUnusedAsset;
            citv_obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            GameObject assem = citv_obj.transform.Find("assembly").gameObject;
            GameObject glass = citv_obj.transform.Find("glass").gameObject;

            assem.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (FLIR)");
            glass.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (FLIR)");
            citv_obj.AddComponent<HeatSource>().heat = 0.5f;

            GameObject assem_armour = assem.transform.Find("ARMOUR").gameObject;
            GameObject glass_armour = glass.transform.Find("ARMOUR").gameObject;

            assem_armour.tag = "Penetrable";
            glass_armour.tag = "Penetrable";
            assem_armour.layer = 8;
            glass_armour.layer = 8;

            UniformArmor assem_u_armour = assem.AddComponent<UniformArmor>();
            UniformArmor glass_u_armour = glass.AddComponent<UniformArmor>();
            assem_u_armour.PrimarySabotRha = 40f;
            assem_u_armour.PrimaryHeatRha = 40f;

            glass_u_armour.PrimarySabotRha = 5f;
            glass_u_armour.PrimaryHeatRha = 5f;

            assem_u_armour._name = "CITV";
            glass_u_armour._name = "CITV glass";

            var m256_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/", "m256"));
            m256_obj = m256_bundle.LoadAsset<GameObject>("m256.prefab");
            m256_obj.hideFlags = HideFlags.DontUnloadUnusedAsset;
            m256_obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            m256_obj.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (FLIR)");
            m256_obj.AddComponent<HeatSource>().heat = 0.5f;

            var trophy_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/", "m1ip_trophy"));
            trophy_obj = trophy_bundle.LoadAsset<GameObject>("trophysystem.prefab");
            trophy_obj.hideFlags = HideFlags.DontUnloadUnusedAsset;
            trophy_obj.transform.localScale = new Vector3(1f, 1f, 1f);
            //trophy_obj.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (FLIR)");
            //trophy_obj.AddComponent<HeatSource>().heat = 0.5f;

            GameObject LtCollider = trophy_obj.transform.Find("COLLIDERS/LEFT").gameObject;
            GameObject RtCollider = trophy_obj.transform.Find("COLLIDERS/RIGHT").gameObject;

            LtCollider.gameObject.layer = 7;
            LtCollider.tag = "Untagged";

            RtCollider.gameObject.layer = 7;
            RtCollider.tag = "Untagged";

            // armor

            var extarmor_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/", "capplique"));
            addon1_obj = extarmor_bundle.LoadAsset<GameObject>("capplique.prefab");
            addon1_obj.hideFlags = HideFlags.DontUnloadUnusedAsset;
            addon1_obj.transform.localScale = new Vector3(1f, 1f, 1f);

            // --- Put this inside Assets.cs ---
            if (addon1_obj != null)
            {

                GameObject caVisual_L = addon1_obj.transform.Find("cApplique.000 left").gameObject;
                if (caVisual_L != null)
                {
                    MeshRenderer caMRl = caVisual_L.GetComponent<MeshRenderer>() ?? caVisual_L.gameObject.AddComponent<MeshRenderer>();
                    if (caVisual_L.GetComponent<MeshRenderer>() == null) caVisual_L.AddComponent<MeshRenderer>();
                    caMRl.material.shader = Shader.Find("Standard (FLIR)");

                    HeatSource caHSl = caVisual_L.GetComponent<HeatSource>() ?? caVisual_L.gameObject.AddComponent<HeatSource>();
                    if (caVisual_L.GetComponent<HeatSource>() == null) caVisual_L.AddComponent<HeatSource>();
                    caHSl.heat = 0.5f;
                }

                GameObject caVisual_R = addon1_obj.transform.Find("cApplique.000 right").gameObject;
                if (caVisual_L != null)
                {
                    MeshRenderer caMRr = caVisual_R.GetComponent<MeshRenderer>() ?? caVisual_R.gameObject.AddComponent<MeshRenderer>();
                    if (caVisual_R.GetComponent<MeshRenderer>() == null) caVisual_R.AddComponent<MeshRenderer>();
                    caMRr.material.shader = Shader.Find("Standard (FLIR)");

                    HeatSource caHSr = caVisual_R.GetComponent<HeatSource>() ?? caVisual_R.gameObject.AddComponent<HeatSource>();
                    if (caVisual_R.GetComponent<HeatSource>() == null) caVisual_R.AddComponent<HeatSource>();
                    caHSr.heat = 0.5f;
                }


                // 1. HITBOX SETUP
                GameObject hbL = addon1_obj.transform.Find("aarCAfwall.000 left").gameObject;
                if (hbL != null)
                {
                    VariableArmor va = hbL.gameObject.GetComponent<VariableArmor>() ?? hbL.gameObject.AddComponent<VariableArmor>();
                    if (hbL.gameObject.GetComponent<AarVisual>() == null) hbL.gameObject.AddComponent<AarVisual>();
                    hbL.gameObject.tag = "Penetrable";
                    hbL.gameObject.layer = 8;
                    if (va != null) va._armorType = DUArmour.aar_hitbox_armor_codex;
                }

                GameObject hbR = addon1_obj.transform.Find("aarCAfwall.000 right").gameObject;
                if (hbR != null)
                {
                    VariableArmor va = hbR.gameObject.GetComponent<VariableArmor>() ?? hbR.gameObject.AddComponent<VariableArmor>();
                    if (hbR.gameObject.GetComponent<AarVisual>() == null) hbR.gameObject.AddComponent<AarVisual>();
                    hbR.gameObject.tag = "Penetrable";
                    hbR.gameObject.layer = 8;
                    if (va != null) va._armorType = DUArmour.aar_hitbox_armor_codex;
                }

                // 2. NERA SETUP
                GameObject neraL = addon1_obj.transform.Find("CAarmor.nera 000 left").gameObject;
                if (neraL != null)
                {
                    VariableArmor va = neraL.gameObject.GetComponent<VariableArmor>() ?? neraL.gameObject.AddComponent<VariableArmor>();
                    if (neraL.gameObject.GetComponent<AarVisual>() == null) neraL.gameObject.AddComponent<AarVisual>();
                    neraL.gameObject.tag = "Penetrable";
                    neraL.gameObject.layer = 8;
                    if (va != null) va._armorType = DUArmour.nera_armor_codex;
                }

                GameObject neraR = addon1_obj.transform.Find("CAarmor.nera 000 right").gameObject;
                if (neraR != null)
                {
                    VariableArmor va = neraR.gameObject.GetComponent<VariableArmor>() ?? neraR.gameObject.AddComponent<VariableArmor>();
                    if (neraR.gameObject.GetComponent<AarVisual>() == null) neraR.gameObject.AddComponent<AarVisual>();
                    neraR.gameObject.tag = "Penetrable";
                    neraR.gameObject.layer = 8;
                    if (va != null) va._armorType = DUArmour.nera_armor_codex;
                }

                // 3. SPECIAL COMPOSITE SETUP
                Transform csL = addon1_obj.transform.Find("CAarmor.cs 000 left");
                if (csL != null)
                {
                    VariableArmor va = csL.gameObject.GetComponent<VariableArmor>() ?? csL.gameObject.AddComponent<VariableArmor>();
                    if (csL.gameObject.GetComponent<AarVisual>() == null) csL.gameObject.AddComponent<AarVisual>();
                    csL.gameObject.tag = "Penetrable";
                    csL.gameObject.layer = 8;
                    if (va != null) va._armorType = DUArmour.composite_armor_codex;
                }

                Transform csR = addon1_obj.transform.Find("CAarmor.cs 000 right");
                if (csR != null)
                {
                    VariableArmor va = csR.gameObject.GetComponent<VariableArmor>() ?? csR.gameObject.AddComponent<VariableArmor>();
                    if (csR.gameObject.GetComponent<AarVisual>() == null) csR.gameObject.AddComponent<AarVisual>();
                    csR.gameObject.tag = "Penetrable";
                    csR.gameObject.layer = 8;
                    if (va != null) va._armorType = DUArmour.composite_armor_codex;
                }
            }

            var dhbundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/", "doghouse"));
            dogh_obj = dhbundle.LoadAsset<GameObject>("idogh.prefab");
            dogh_obj.hideFlags = HideFlags.DontUnloadUnusedAsset;
            dogh_obj.transform.localScale = new Vector3(1f, 1f, 1f);

            GameObject idoghouse = dogh_obj.transform.Find("idoghouse").gameObject;
            idoghouse.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard (FLIR)");
            idoghouse.AddComponent<HeatSource>().heat = 0.5f;
            idoghouse.GetComponent<MeshCollider>().convex = false;

            GameObject idogarmor = idoghouse.transform.Find("ARMOR").gameObject;
            idogarmor.transform.localEulerAngles = new Vector3(90f, 0, 0);
            idogarmor.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

            idogarmor.tag = "Penetrable";
            idogarmor.layer = 8;

            AarVisual idogAV = idogarmor.AddComponent<AarVisual>();

            UniformArmor idogUarmor = idoghouse.AddComponent<UniformArmor>();

            idogUarmor._name = "Doghouse Armor";
            idogUarmor.PrimaryHeatRha = 15f;
            idogUarmor.PrimarySabotRha = 15f;



            var tuskbundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/m1a1assets/", "tusk"));
            tusk_obj = tuskbundle.LoadAsset<GameObject>("tusk_iii.prefab");
            tusk_obj.hideFlags = HideFlags.DontUnloadUnusedAsset;
            tusk_obj.transform.localScale = new Vector3(1f, 1f, 1f);


            if (tusk_obj != null)
            {
                // ==========================================
                // 1. VISUAL SETUP
                // ==========================================
                Transform tusk_visual = tusk_obj.transform.Find("VISUAL");
                if (tusk_visual != null)
                {
                    string[] visualNames = {
                    "armor block.000 left", "armor block.000 right",
                    "armor block.001 left", "armor block.001 right",
                    "armor block.002 left", "armor block.002 right",
                    "backing plate left", "backing plate right",
                    "boltgroup.000 left", "boltgroup.000 right",
                    "boltgroup.001 left", "boltgroup.001 right",
                    "flap.000 left", "flap.000 right",
                    "flap.001 left", "flap.001 right",
                    "flap.002 left", "flap.002 right",
                    "flap.003 left", "flap.003 right",
                    "small armor block.000 left", "small armor block.000 right"
                };

                    foreach (string vName in visualNames)
                    {
                        Transform vObj = tusk_visual.Find(vName);
                        if (vObj != null)
                        {
                            MeshRenderer mr = vObj.gameObject.GetComponent<MeshRenderer>() ?? vObj.gameObject.AddComponent<MeshRenderer>();
                            if (mr != null) mr.material.shader = Shader.Find("Standard (FLIR)");

                            HeatSource hs = vObj.gameObject.GetComponent<HeatSource>() ?? vObj.gameObject.AddComponent<HeatSource>();
                            if (hs != null) hs.heat = 0.5f;
                        }
                    }
                }

                // ==========================================
                // 2. HITBOX SETUP
                // ==========================================
                Transform tusk_hitboxes = tusk_obj.transform.Find("HITBOX");
                if (tusk_hitboxes != null)
                {
                    string[] hitboxNames = {
                    "aarAMAPfwall.000 left", "aarAMAPfwall.000 right",
                    "aarAMAPfwall.001 left", "aarAMAPfwall.001 right",
                    "aarAMAPfwall.002 left", "aarAMAPfwall.002 right",
                    "aarAMAPswall.0000 left", "aarAMAPswall.0000 right",
                    "aarAMAPswall.0001 left", "aarAMAPswall.0001 right",
                    "aarAMAPswall.0011 left", "aarAMAPswall.0011 right",
                    "aarAMAPswall.0020 left", "aarAMAPswall.0020 right",
                    "aarAMAPswall.0021 left", "aarAMAPswall.0021 right",
                    "aarAMAPswall.0010 left", "aarAMAPswall.0010 right",
                    "aarBacking plate.000 left", "aarBacking plate.000 right"
                };

                    foreach (string hbName in hitboxNames)
                    {
                        Transform hbObj = tusk_hitboxes.Find(hbName);
                        if (hbObj != null)
                        {
                            VariableArmor va = hbObj.gameObject.GetComponent<VariableArmor>() ?? hbObj.gameObject.AddComponent<VariableArmor>();
                            if (hbObj.gameObject.GetComponent<AarVisual>() == null) hbObj.gameObject.AddComponent<AarVisual>();
                            hbObj.gameObject.tag = "Penetrable";
                            hbObj.gameObject.layer = 8;
                            if (va != null) va._armorType = DUArmour.aar_hitbox_armor_codex;
                        }
                    }
                }

                // ==========================================
                // 3. ARMOR SETUP
                // ==========================================
                Transform tusk_armor = tusk_obj.transform.Find("ARMOR");
                if (tusk_armor != null)
                {
                    // NERA SETUP
                    Transform tusk_nera = tusk_armor.Find("NERA");
                    if (tusk_nera != null)
                    {
                        string[] neraNames = {
                        "ab000 nera.000 left", "ab000 nera.000 right",
                        "ab001 nera.000 left", "ab001 nera.000 right",
                        "ab002 nera.000 left", "ab002 nera.000 right",
                        "sab000 nera.000 left", "sab000 nera.000 right",
                        "flap000 nera.000 left", "flap000 nera.000 right",
                        "flap001 nera.000 left", "flap001 nera.000 right",
                        "flap002 nera.000 left", "flap002 nera.000 right"
                    };

                        foreach (string neraName in neraNames)
                        {
                            Transform neraObj = tusk_nera.Find(neraName);
                            if (neraObj != null)
                            {
                                VariableArmor va = neraObj.gameObject.GetComponent<VariableArmor>() ?? neraObj.gameObject.AddComponent<VariableArmor>();
                                if (neraObj.gameObject.GetComponent<AarVisual>() == null) neraObj.gameObject.AddComponent<AarVisual>();
                                neraObj.gameObject.tag = "Penetrable";
                                neraObj.gameObject.layer = 8;
                                if (va != null) va._armorType = DUArmour.nera_armor_codex;
                            }
                        }
                    }

                    // COMPOSITE SETUP
                    Transform tusk_composite = tusk_armor.Find("Composites");
                    if (tusk_composite != null)
                    {
                        string[] compNames = {
                        "ab000 cs.000 left", "ab000 cs.000 right",
                        "ab001 cs.000 left", "ab001 cs.000 right",
                        "ab002 cs.000 left", "ab002 cs.000 right"
                    };

                        foreach (string compName in compNames)
                        {
                            Transform compObj = tusk_composite.Find(compName);
                            if (compObj != null)
                            {
                                VariableArmor va = compObj.gameObject.GetComponent<VariableArmor>() ?? compObj.gameObject.AddComponent<VariableArmor>();
                                if (compObj.gameObject.GetComponent<AarVisual>() == null) compObj.gameObject.AddComponent<AarVisual>();
                                compObj.gameObject.tag = "Penetrable";
                                compObj.gameObject.layer = 8;
                                if (va != null) va._armorType = DUArmour.composite_armor_codex;
                            }
                        }
                    }

                    // DEPLETED URANIUM SETUP
                    Transform tusk_DU = tusk_armor.Find("Depleted Uranium");
                    if (tusk_DU != null)
                    {
                        string[] duNames = {
                        "ab000 dugen.000 left", "ab000 dugen.000 right",
                        "ab001 dugen.000 left", "ab001 dugen.000 right",
                        "ab002 dugen.000 left", "ab002 dugen.000 right"
                    };

                        foreach (string duName in duNames)
                        {
                            Transform duObj = tusk_DU.Find(duName);
                            if (duObj != null)
                            {
                                VariableArmor va = duObj.gameObject.GetComponent<VariableArmor>() ?? duObj.gameObject.AddComponent<VariableArmor>();

                                AarVisual av = duObj.gameObject.GetComponent<AarVisual>();
                                if (av == null)
                                {
                                    av = duObj.gameObject.AddComponent<AarVisual>();
                                    av.AarMaterial = DUArmour.gen3_du_aar_mat;
                                }
                                else
                                {
                                    av.AarMaterial = DUArmour.gen3_du_aar_mat;
                                }

                                duObj.gameObject.tag = "Penetrable";
                                duObj.gameObject.layer = 8;
                                if (va != null) va._armorType = DUArmour.gen3_du_armor_codex;
                            }
                        }
                    }
                }
            }
        }
    }
}


