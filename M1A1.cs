using GHPC;
using GHPC.Camera;
using GHPC.Equipment.Optics;
using GHPC.Player;
using GHPC.State;
using GHPC.Thermals;
using GHPC.Utility;
using GHPC.Vehicle;
using GHPC.Weaponry;
using GHPC.Weapons;
using M1A1Abrams.APS;
using MelonLoader;
using ModUtil;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace M1A1Abrams
{
    public class M1A1 : Module
    {
        public static MelonPreferences_Entry<bool> has_trophy;

        static MelonPreferences_Entry<int> max_trophy;

        static MelonPreferences_Entry<int> trophy_min_engage_caliber;
        static MelonPreferences_Entry<float> trophy_intercept_delay;
        static MelonPreferences_Entry<float> trophy_min_engage_velocity;
        static MelonPreferences_Entry<float> trophy_max_engage_velocity;

        static MelonPreferences_Entry<string> sabot_m1;
        static MelonPreferences_Entry<string> sabot_m1ip;
        static MelonPreferences_Entry<string> heat_m1;
        static MelonPreferences_Entry<string> heat_m1ip;

        static MelonPreferences_Entry<int> flir_gen_m1;
        static MelonPreferences_Entry<int> flir_gen_m1ip;

        static MelonPreferences_Entry<int> m829Count;
        static MelonPreferences_Entry<int> m830Count;

        static MelonPreferences_Entry<bool> rotate_azimuth_m1;
        static MelonPreferences_Entry<bool> rotate_azimuth_m1ip;

        public static MelonPreferences_Entry<bool> m1e1;
        static MelonPreferences_Entry<int> randomChanceNum;
        static MelonPreferences_Entry<bool> randomChance;
        static MelonPreferences_Entry<bool> citv_m1a1;
        static MelonPreferences_Entry<bool> citv_m1e1;

        static MelonPreferences_Entry<bool> du_package_m1;
        static MelonPreferences_Entry<int> du_gen_m1;
        static MelonPreferences_Entry<bool> du_package_m1ip;
        static MelonPreferences_Entry<int> du_gen_m1ip;

        public static MelonPreferences_Entry<bool> externalfrontturretarmor;
        public static MelonPreferences_Entry<bool> tuskfictional;

        public static MelonPreferences_Entry<bool> citv_smooth;

        public static MelonPreferences_Entry<bool> digital_enchancement_m1a1;
        public static MelonPreferences_Entry<bool> digital_enchancement_m1e1;

        public static MelonPreferences_Entry<bool> de_fixed_reticle_size;

        public static MelonPreferences_Entry<bool> crows_m1e1;
        public static MelonPreferences_Entry<bool> crows_m1a1;
        public static MelonPreferences_Entry<bool> crows_raufoss;
        public static MelonPreferences_Entry<bool> crows_alt_placement;

        public static MelonPreferences_Entry<bool> enhanced_systems;

        public static MelonPreferences_Entry<bool> m1_to_m1ip;

        public static MelonPreferences_Entry<bool> original_rack_size;

        static WeaponSystemCodexScriptable gun_m256;
        static WeaponSystemCodexScriptable gun_m256a1;

        //static Material addon1mat;
        //public static ArmorCodexScriptable turretaddon_armor_codex;
        //public static ArmorCodexScriptable turretaddon_armor_codex1;
        //public static ArmorCodexScriptable WEA_rha;

        static GameObject addon_turret;
        static GameObject addon_hull;
        static GameObject addon_turret_l55;

        private static APS.Schema trophy_schema = new APS.Schema();

        public static Vector2Int flir_gen2_res = new Vector2Int(1024, 576);
        public static Vector2Int flir_gen3_res = new Vector2Int(1366, 768);
        public static Vector2Int flir_gen3a1_res = new Vector2Int(1920, 1080);

        public static void Config(MelonPreferences_Category cfg)
        {
            has_trophy = cfg.CreateEntry<bool>("Trophy System", true);
            has_trophy.Comment = "trophy system M1IP debug";

            //max_trophy = cfg.CreateEntry<int>("Max Drozd APS per platoon", -1);
            //max_trophy.Comment = "Limit how many M1IPs in a platoon can have Trophy. Set to -1 to uncap";

            trophy_min_engage_caliber = cfg.CreateEntry<int>("Min Engagement Caliber", 65);
            trophy_min_engage_caliber.Comment = "Minimum caliber a projectile has to be for it to be detected by Trophy";

            trophy_intercept_delay = cfg.CreateEntry<float>("Intercept Cooldown", 1.62f);
            trophy_intercept_delay.Comment = "Cooldown in seconds after Trophy intercepts a projectile";

            trophy_min_engage_velocity = cfg.CreateEntry<float>("Min Engagement Velocity", 40f);
            trophy_min_engage_velocity.Comment = "Minimum velocity a projectile needs to be travelling at for it to be engaged by Trophy";

            trophy_max_engage_velocity = cfg.CreateEntry<float>("Max Engagement Velocity", 1250f);
            trophy_max_engage_velocity.Comment = "Maximum velocity a projectile can travel at for it to be engaged by Trophy";

            m829Count = cfg.CreateEntry<int>("M829", 22);
            m829Count.Description = "How many rounds of M829 (APFSDS), M830 (HEAT) each M1A1 should carry. Maximum of 40 rounds total. Bring in at least one M829 round.";
            m830Count = cfg.CreateEntry<int>("M830", 18);

            original_rack_size = cfg.CreateEntry<bool>("Vanilla Ammo Capacity", false);
            original_rack_size.Description = "Carry a total of 52 rounds instead of 40.";

            sabot_m1 = cfg.CreateEntry<string>("AP Round (M1E1)", "M829A2");
            sabot_m1.Description = "Customize which rounds M1A1s/M1E1s use";
            sabot_m1.Comment = "M827, M829, M829A1, M829A2, M829A3";
            sabot_m1ip = cfg.CreateEntry<string>("AP Round (M1A1)", "M829A4");

            heat_m1 = cfg.CreateEntry<string>("HEAT Round (M1E1)", "M830");
            heat_m1.Comment = "M830, M830A1 (has proximity fuse that can be toggled using middle mouse)";
            heat_m1ip = cfg.CreateEntry<string>("HEAT Round (M1A1)", "M830A1");

            rotate_azimuth_m1 = cfg.CreateEntry<bool>("Rotate Azimuth (M1E1)", true);
            rotate_azimuth_m1.Description = "Horizontal stabilization of sights when applying lead.";
            rotate_azimuth_m1ip = cfg.CreateEntry<bool>("Rotate Azimuth (M1A1)", true);

            flir_gen_m1 = cfg.CreateEntry<int>("FLIR Generation (M1E1)", 2);
            flir_gen_m1.Description = "Settings for the gunner's thermals";
            flir_gen_m1.Comment = "Higher generation = higher resolution (1-3, integer)";
            flir_gen_m1ip = cfg.CreateEntry<int>("FLIR Generation (M1A1)", 3);

            digital_enchancement_m1e1 = cfg.CreateEntry<bool>("Digital Enhancement (M1E1)", false);
            digital_enchancement_m1e1.Comment = "Additional zoom levels for thermals.";
            digital_enchancement_m1a1 = cfg.CreateEntry<bool>("Digital Enhancement (M1A1)", true);
            de_fixed_reticle_size = cfg.CreateEntry<bool>("Fixed Reticle Size", false);
            de_fixed_reticle_size.Comment = "Digitally enhanced zoom levels will not increase the size of the reticle.";

            citv_m1e1 = cfg.CreateEntry<bool>("CITV (M1E1)", false);
            citv_m1e1.Description = "Replaces commander's NVGs with variable-zoom thermals.";
            citv_m1a1 = cfg.CreateEntry<bool>("CITV (M1A1)", true);

            citv_smooth = cfg.CreateEntry<bool>("Smooth CITV Panning", true);
            citv_smooth.Comment = "Makes CITV feel more like a camera.";

            du_package_m1ip = cfg.CreateEntry<bool>("DU Armour (M1A1)", true);
            du_package_m1ip.Description = "DU inserts for the composite turret cheeks: increased KE protection. M1A1 exclusive. Increased weight.";
            du_gen_m1ip = cfg.CreateEntry<int>("DU Generation (M1A1)", 3);
            du_gen_m1ip.Comment = "Higher generation = more KE protection (1-3, integer)";

            externalfrontturretarmor = cfg.CreateEntry<bool>("External Front Turret Armour", true);
            externalfrontturretarmor.Comment = "Adds external armor to the front of the turret. M1IP only debug";

            tuskfictional = cfg.CreateEntry<bool>("TUSK Fictional", true);
            tuskfictional.Comment = "Adds fictional TUSK upgrades to the M1IP Abrams, adds composite armor";

            du_package_m1 = cfg.CreateEntry<bool>("DU Armour (M1E1)", false);
            du_gen_m1 = cfg.CreateEntry<int>("DU Generation (M1E1)", 1);
            du_package_m1.Comment = "Doesn't apply to M1E1, only those converted to M1A1";

            crows_m1e1 = cfg.CreateEntry<bool>("CROWS (M1E1) <DISABLED>", false);
            crows_m1e1.Description = "Remote weapons system equipped with a .50 caliber M2HB; 400 rounds, automatic lead, thermals.";
            crows_m1a1 = cfg.CreateEntry<bool>("CROWS (M1A1) <DISABLED>", false);

            crows_alt_placement = cfg.CreateEntry<bool>("Alternative Position", false);
            crows_alt_placement.Comment = "Moves the CROWS to the right side of the commander instead of directly in front.";

            crows_raufoss = cfg.CreateEntry<bool>("Use Mk 211 Mod 0", false);
            crows_raufoss.Comment = "Loads the CROWS M2HB with high explosive rounds.";

            enhanced_systems = cfg.CreateEntry<bool>("Enhanced Systems", true);
            enhanced_systems.Comment = "Enhances the Abrams systems. Converts to SEPv4.";

            m1e1 = cfg.CreateEntry<bool>("M1E1", false);
            m1e1.Description = "Convert M1s to M1E1s (i.e: they get the 120mm gun).";

            m1_to_m1ip = cfg.CreateEntry<bool>("M1E1 -> M1A1", false);
            m1_to_m1ip.Description = "Convert all M1E1s to M1A1s (will still use M1E1 settings)";

            randomChance = cfg.CreateEntry<bool>("Random", false);
            randomChance.Description = "M1IPs/M1s will have a random chance of being converted to M1A1s/M1E1s.";
            randomChanceNum = cfg.CreateEntry<int>("ConversionChance", 50);
        }

        public static IEnumerator Convert(GameState _)
        {
            foreach (Vehicle vic in M1A1AbramsMod.vics)
            {
                if (vic == null) continue;

                GameObject vic_go = vic.gameObject;

                if (vic_go.GetComponent<AlreadyConverted>() != null) continue;
                if (vic.FriendlyName != "M1IP" && !(m1e1.Value && vic.FriendlyName == "M1 Abrams")) continue;

                int rand = (randomChance.Value) ? UnityEngine.Random.Range(1, 100) : 0;
                if (rand > randomChanceNum.Value) continue;

                WeaponsManager weaponsManager = vic.GetComponent<WeaponsManager>();
                WeaponSystemInfo mainGunInfo = weaponsManager.Weapons[0];
                WeaponSystem mainGun = mainGunInfo.Weapon;
                UsableOptic optic = vic.transform.Find("IPM1_rig/HULL/TURRET/Turret Scripts/GPS/Optic").GetComponent<UsableOptic>();
                UsableOptic night_optic = optic.slot.LinkedNightSight.PairedOptic;
                FireControlSystem fcs = optic.FCS;
                Transform gas = vic.transform.Find("IPM1_rig/HULL/TURRET/GUN/Gun Scripts/Aux sight (GAS)");
                bool is_m1ip = vic.UniqueName == "M1IP Abrams" && vic.GetComponent<PreviouslyM1>() == null;

                vic._friendlyName = (vic.FriendlyName == "M1IP") ? "M1A1" : "M1E1";

                optic.slot.ExclusiveWeapons = new WeaponSystem[] { weaponsManager.Weapons[0].Weapon, weaponsManager.Weapons[1].Weapon };
                night_optic.slot.ExclusiveWeapons = new WeaponSystem[] { weaponsManager.Weapons[0].Weapon, weaponsManager.Weapons[1].Weapon };

                vic_go.AddComponent<MPAT_Switch>();

                GAS.Create(Ammo_120mm.ap[sabot_m1ip.Value].ClipType.MinimalPattern[0], Ammo_120mm.heat[heat_m1ip.Value].ClipType.MinimalPattern[0]);
                GAS.Add(gas, optic.slot.ExclusiveWeapons);

                int flir_gen = is_m1ip ? flir_gen_m1ip.Value : flir_gen_m1.Value;
                if (flir_gen > 1)
                {
                    Vector2Int resolution = flir_gen == 2 ? flir_gen2_res : flir_gen3_res;
                    night_optic.slot.VibrationShakeMultiplier = 0.0f;
                    night_optic.slot.FLIRWidth = resolution.x;
                    night_optic.slot.FLIRHeight = resolution.y;
                    night_optic.slot.FLIRBlitMaterialOverride = Assets.flir_blit_mat_green_no_scan;
                }

                bool has_digital_enhancement = is_m1ip ? digital_enchancement_m1a1.Value : digital_enchancement_m1e1.Value;
                if (has_digital_enhancement)
                {
                    DigitalEnhancement digital_enhance = mainGun.FCS.gameObject.AddComponent<DigitalEnhancement>();
                    digital_enhance.original_blur = night_optic.slot.BaseBlur;
                    digital_enhance.slot = night_optic.slot;
                    digital_enhance.reticle_plane = night_optic.slot.transform.Find("Reticle Mesh/FFP");
                    digital_enhance.Add(2.4f, 0.01f, 0.69f);
                    digital_enhance.Add(1f, 0.02f, 0.29f);
                }

                bool has_du_package = is_m1ip ? du_package_m1ip.Value : du_package_m1.Value;
                int du_gen = is_m1ip ? du_gen_m1ip.Value : du_gen_m1.Value;
                vic_go.GetComponent<Rigidbody>().mass = has_du_package && vic.FriendlyName == "M1A1" ? 62781.3776f : 57152.6386f;
                if (has_du_package && vic._friendlyName == "M1A1")
                {
                    vic._friendlyName += du_gen > 1 ? "HC" : "HA";

                    GameObject turret_cheeks = vic.transform.Find("IPM1_rig/HULL/TURRET").GetComponent<LateFollowTarget>()
                        ._lateFollowers[0].transform.Find("Turret_Armor/cheeks composite arrays").gameObject;

                    VariableArmor var_armour = turret_cheeks.GetComponent<VariableArmor>();
                    var_armour._armorType = DUArmour.du_armor_codexes[du_gen - 1];

                    AarVisual cheek_visual = turret_cheeks.GetComponent<AarVisual>();

                    cheek_visual.AarMaterial = DUArmour.du_aar_mats[du_gen - 1];

                    GameObject turret_side_l = vic.transform.Find("IPM1_rig/HULL/TURRET").GetComponent<LateFollowTarget>()
                        ._lateFollowers[0].transform.Find("Turret_Armor/left side composite array").gameObject;

                    VariableArmor var_armour1 = turret_side_l.GetComponent<VariableArmor>();
                    var_armour1._armorType = DUArmour.du_armor_codexes[du_gen - 1];

                    AarVisual tsl_visual = turret_side_l.GetComponent<AarVisual>();

                    tsl_visual.AarMaterial = DUArmour.du_aar_mats[du_gen - 1];

                    GameObject turret_side_r = vic.transform.Find("IPM1_rig/HULL/TURRET").GetComponent<LateFollowTarget>()
                        ._lateFollowers[0].transform.Find("Turret_Armor/right side composite array").gameObject;

                    VariableArmor var_armour2 = turret_side_r.GetComponent<VariableArmor>();
                    var_armour1._armorType = DUArmour.du_armor_codexes[du_gen - 1];

                    AarVisual tsr_visual = turret_side_r.GetComponent<AarVisual>();

                    tsr_visual.AarMaterial = DUArmour.du_aar_mats[du_gen - 1];

                    GameObject lfp_armor = vic.GetComponent<LateFollowTarget>()
                        ._lateFollowers[0].transform.Find("HULLARMOR/lower front plate composite array").gameObject;

                    VariableArmor var_armour3 = lfp_armor.GetComponent<VariableArmor>();
                    var_armour3._armorType = DUArmour.du_armor_codexes[du_gen - 1];

                    AarVisual lfp_visual = lfp_armor.GetComponent<AarVisual>();

                    lfp_visual.AarMaterial = DUArmour.du_aar_mats[du_gen - 1];

                    //GameObject mantlet_armor = vic_go.transform.Find("ARMORGUN/COMPOSITE ARMOR ARRAY").gameObject;

                    //VariableArmor var_armour4 = mantlet_armor.GetComponent<VariableArmor>();
                    //var_armour4._armorType = DUArmour.nera_armor_codex;

                    //when armor type is set to DU use this
                    //AarVisual mantlet_visual = mantlet_armor.GetComponent<AarVisual>();

                    //mantlet_visul.AarMaterial = DUArmour.du_aar_mats[du_gen = 1];

                }

                bool trophy = is_m1ip ? has_trophy.Value : true;
                if (trophy)
                {
                    GameObject trophyS = GameObject.Instantiate(Assets.trophy_obj, vic.transform.Find("IPM1_rig/HULL/TURRET"));

                    trophyS.transform.SetParent(vic.transform.Find("IPM1_rig/HULL/TURRET")
                        .GetComponent<LateFollowTarget>()._lateFollowers[0].transform, true);
                    trophyS.transform.localPosition = new Vector3(0f, 0f, 0f);

                    GameObject T_ammo = trophyS.transform.Find("AMMO").gameObject;

                    Transform[] radar_colliders = trophyS.transform.Find("COLLIDERS")
                    .GetComponentsInChildren<Transform>().Where(o => o.name != "COLLIDERS").ToArray();
                    foreach (Transform collider in radar_colliders)
                    {
                        collider.gameObject.layer = 7;
                        collider.tag = "Untagged";
                    }

                    Transform[] launchers = new Transform[]
                    {
                        T_ammo.transform.Find("LEFT"),
                        T_ammo.transform.Find("RIGHT"),
                    };

                    Transform colliders_holder = trophyS.transform.Find("COLLIDERS");
                    Transform[] colliders = colliders_holder.GetComponentsInChildren<Transform>().Where(t => t != colliders_holder).ToArray();
                    int[][] assignments = new int[][]
                    {
                        new int[] { 1 },
                        new int[] { 0 },
                    };

                    trophy_schema.min_engage_caliber = trophy_min_engage_caliber.Value;
                    trophy_schema.min_engage_velocity = trophy_min_engage_velocity.Value;
                    trophy_schema.max_engage_velocity = trophy_max_engage_velocity.Value;

                    APS.APS.Add(launchers, colliders, assignments, trophy_schema);
                }

                bool efta = is_m1ip ? externalfrontturretarmor.Value : true;
                if (efta)
                {
                    GameObject turretaddon = GameObject.Instantiate(Assets.addon1_obj, vic.transform.Find("IPM1_rig/HULL/TURRET"));

                    if (turretaddon != null)
                    {


                        GameObject hbLeft = turretaddon.transform.Find("aarCAfwall.000 left").gameObject;
                        if (hbLeft != null && hbLeft.gameObject.TryGetComponent(out VariableArmor vaH_L))
                        {
                            vaH_L.Unit = vic;
                            vaH_L._name = "Left Cheek Front Plating";
                            hbLeft.transform.localScale = new Vector3(100f, 100f, 100f);
                        }

                        GameObject hbRight = turretaddon.transform.Find("aarCAfwall.000 right").gameObject;
                        if (hbRight != null && hbRight.gameObject.TryGetComponent(out VariableArmor vaH_R))
                        {
                            vaH_R.Unit = vic;
                            vaH_R._name = "Right Cheek Front Plating";
                            hbRight.transform.localScale = new Vector3(-100f, -100f, -100f);
                        }

                        GameObject neraLeft = turretaddon.transform.Find("CAarmor.nera 000 left").gameObject;
                        if (neraLeft != null && neraLeft.gameObject.TryGetComponent(out VariableArmor vaN_L))
                        {
                            vaN_L.Unit = vic;
                            vaN_L._name = "Left Non Explosive Reactive Armored Layer";
                            neraLeft.transform.localScale = new Vector3(100f, 100f, 100f);
                        }

                        GameObject neraRight = turretaddon.transform.Find("CAarmor.nera 000 right").gameObject;
                        if (neraRight != null && neraRight.gameObject.TryGetComponent(out VariableArmor vaN_R))
                        {
                            vaN_R.Unit = vic;
                            vaN_R._name = "Right Non Explosive Reactive Armored Layer";
                            neraRight.transform.localScale = new Vector3(-100f, -100f, -100f);
                        }

                        Transform csLeft = turretaddon.transform.Find("CAarmor.cs 000 left");
                        if (csLeft != null && csLeft.gameObject.TryGetComponent(out VariableArmor vaC_L))
                        {
                            vaC_L.Unit = vic;
                            vaC_L._name = "Left Special Composite Layer";
                            csLeft.localScale = new Vector3(100f, 100f, 100f);
                        }

                        Transform csRight = turretaddon.transform.Find("CAarmor.cs 000 right");
                        if (csRight != null && csRight.gameObject.TryGetComponent(out VariableArmor vaC_R))
                        {
                            vaC_R.Unit = vic;
                            vaC_R._name = "Right Special Composite Layer";
                            csRight.localScale = new Vector3(-100f, -100f, -100f);
                        }
                    }

                    turretaddon.transform.SetParent(vic.transform.Find("IPM1_rig/HULL/TURRET").GetComponent<LateFollowTarget>()
                        ._lateFollowers[0].transform, true);
                    turretaddon.transform.localPosition = new Vector3(0f, 0f, 0f);
                    //turretaddon.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

                    vic_go.transform.Find("IPM1_rig/HULL/TURRET/M1 camo net/turret_front_left").gameObject.SetActive(false);
                    vic_go.transform.Find("IPM1_rig/HULL/TURRET/M1 camo net/turret_front_right").gameObject.SetActive(false);

                    }

                bool has_tuskfictional = is_m1ip ? tuskfictional.Value : true;
                if (has_tuskfictional)
                {
                    GameObject tusk = GameObject.Instantiate(Assets.tusk_obj, vic.transform.Find("M1IP_mesh/M1IP_hull"));

                    GameObject tusk_hitboxes = tusk.transform.Find("HITBOX").gameObject;
                    if (tusk_hitboxes != null)
                    {
                        // Front Walls
                        string[] fwallIndices = { "000", "001", "002" };
                        string[] fwallNames = { "Back Composite Encasing", "Middle Composite Encasing", "Front Composite Encasing" };

                        for (int i = 0; i < fwallIndices.Length; i++)
                        {
                            GameObject fwL = tusk_hitboxes.transform.Find($"aarAMAPfwall.{fwallIndices[i]} left").gameObject;
                            if (fwL != null && fwL.gameObject.TryGetComponent(out VariableArmor vaFW_L))
                            {
                                vaFW_L.Unit = vic;
                                vaFW_L._name = $"Left {fwallNames[i]}";
                                fwL.transform.localScale = new Vector3(100f, 100f, 100f);
                            }

                            GameObject fwR = tusk_hitboxes.transform.Find($"aarAMAPfwall.{fwallIndices[i]} right").gameObject;
                            if (fwR != null && fwR.gameObject.TryGetComponent(out VariableArmor vaFW_R))
                            {
                                vaFW_R.Unit = vic;
                                vaFW_R._name = $"Right {fwallNames[i]}";
                                fwR.transform.localScale = new Vector3(-100f, -100f, -100f);
                            }
                        }

                        // Side Walls
                        string[] swallIndices = { "0000", "0001", "0011", "0020", "0021", "0010" };
                        foreach (string idx in swallIndices)
                        {
                            GameObject swL = tusk_hitboxes.transform.Find($"aarAMAPswall.{idx} left").gameObject;
                            if (swL != null && swL.gameObject.TryGetComponent(out VariableArmor vaSW_L))
                            {
                                vaSW_L.Unit = vic;
                                vaSW_L._name = "Left Side Composite Encasing";
                                swL.transform.localScale = new Vector3(100f, 100f, 100f);
                            }

                            GameObject swR = tusk_hitboxes.transform.Find($"aarAMAPswall.{idx} right").gameObject;
                            if (swR != null && swR.gameObject.TryGetComponent(out VariableArmor vaSW_R))
                            {
                                vaSW_R.Unit = vic;
                                vaSW_R._name = "Right Side Composite Encasing";
                                swR.transform.localScale = new Vector3(-100f, -100f, -100f);
                            }
                        }

                        // Backing Plates
                        GameObject bpL = tusk_hitboxes.transform.Find("aarBacking plate.000 left").gameObject;
                        if (bpL != null && bpL.gameObject.TryGetComponent(out VariableArmor vaBP_L))
                        {
                            vaBP_L.Unit = vic;
                            vaBP_L._name = "Backing Plate Left";
                            bpL.transform.localScale = new Vector3(100f, 100f, 100f);
                        }

                        GameObject bpR = tusk_hitboxes.transform.Find("aarBacking plate.000 right").gameObject;
                        if (bpR != null && bpR.gameObject.TryGetComponent(out VariableArmor vaBP_R))
                        {
                            vaBP_R.Unit = vic;
                            vaBP_R._name = "Backing Plate Right";
                            bpR.transform.localScale = new Vector3(-100f, -100f, -100f);
                        }
                    }

                    GameObject tusk_armor = tusk.transform.Find("ARMOR").gameObject;
                    if (tusk_armor != null)
                    {
                        GameObject tusk_nera = tusk_armor.transform.Find("NERA").gameObject;
                        if (tusk_nera != null)
                        {
                            for (int i = 0; i <= 2; i++)
                            {
                                // Armor Blocks
                                GameObject abNL = tusk_nera.transform.Find($"ab00{i} nera.000 left").gameObject;
                                if (abNL != null && abNL.gameObject.TryGetComponent(out VariableArmor vaABN_L))
                                {
                                    vaABN_L.Unit = vic;
                                    vaABN_L._name = "Left Third Layer NERA";
                                    abNL.transform.localScale = new Vector3(100f, 100f, 100f);
                                }

                                GameObject abNR = tusk_nera.transform.Find($"ab00{i} nera.000 right").gameObject;
                                if (abNR != null && abNR.gameObject.TryGetComponent(out VariableArmor vaABN_R))
                                {
                                    vaABN_R.Unit = vic;
                                    vaABN_R._name = "Right Third Layer NERA";
                                    abNR.transform.localScale = new Vector3(-100f, -100f, -100f);
                                }

                                // Flaps
                                GameObject flapNL = tusk_nera.transform.Find($"flap00{i} nera.000 left").gameObject;
                                if (flapNL != null && flapNL.gameObject.TryGetComponent(out VariableArmor vaFN_L))
                                {
                                    vaFN_L.Unit = vic;
                                    vaFN_L._name = "Left Flap NERA";
                                    flapNL.transform.localScale = new Vector3(100f, 100f, 100f);
                                }

                                GameObject flapNR = tusk_nera.transform.Find($"flap00{i} nera.000 right").gameObject;
                                if (flapNR != null && flapNR.gameObject.TryGetComponent(out VariableArmor vaFN_R))
                                {
                                    vaFN_R.Unit = vic;
                                    vaFN_R._name = "Right Flap NERA";
                                    flapNR.transform.localScale = new Vector3(-100f, -100f, -100f);
                                }
                            }

                            // Small Armor Blocks
                            GameObject sabNL = tusk_nera.transform.Find("sab000 nera.000 left").gameObject;
                            if (sabNL != null && sabNL.gameObject.TryGetComponent(out VariableArmor vaSABN_L))
                            {
                                vaSABN_L.Unit = vic;
                                vaSABN_L._name = "Left Small NERA Block";
                                sabNL.transform.localScale = new Vector3(100f, 100f, 100f);
                            }

                            GameObject sabNR = tusk_nera.transform.Find("sab000 nera.000 right").gameObject;
                            if (sabNR != null && sabNR.gameObject.TryGetComponent(out VariableArmor vaSABN_R))
                            {
                                vaSABN_R.Unit = vic;
                                vaSABN_R._name = "Right Small NERA Block";
                                sabNR.transform.localScale = new Vector3(-100f, -100f, -100f);
                            }
                        }

                        Transform tusk_composite = tusk_armor.transform.Find("Composites");
                        if (tusk_composite != null)
                        {
                            for (int i = 0; i <= 2; i++)
                            {
                                Transform csL = tusk_composite.Find($"ab00{i} cs.000 left");
                                if (csL != null && csL.gameObject.TryGetComponent(out VariableArmor vaC_L))
                                {
                                    vaC_L.Unit = vic;
                                    vaC_L._name = "Left First Layer Composite Array";
                                    csL.localScale = new Vector3(100f, 100f, 100f);
                                }

                                Transform csR = tusk_composite.Find($"ab00{i} cs.000 right");
                                if (csR != null && csR.gameObject.TryGetComponent(out VariableArmor vaC_R))
                                {
                                    vaC_R.Unit = vic;
                                    vaC_R._name = "Right First Layer Composite Array";
                                    csR.localScale = new Vector3(-100f, -100f, -100f);
                                }
                            }
                        }

                        Transform tusk_DU = tusk_armor.transform.Find("Depleted Uranium");
                        if (tusk_DU != null)
                        {
                            for (int i = 0; i <= 2; i++)
                            {
                                Transform duL = tusk_DU.Find($"ab00{i} dugen.000 left");
                                if (duL != null && duL.gameObject.TryGetComponent(out VariableArmor vaDU_L))
                                {
                                    vaDU_L.Unit = vic;
                                    vaDU_L._name = "Left Second Layer Depleted Uranium";
                                    duL.localScale = new Vector3(100f, 100f, 100f);
                                }

                                Transform duR = tusk_DU.Find($"ab00{i} dugen.000 right");
                                if (duR != null && duR.gameObject.TryGetComponent(out VariableArmor vaDU_R))
                                {
                                    vaDU_R.Unit = vic;
                                    vaDU_R._name = "Right Second Layer Depleted Uranium";
                                    duR.localScale = new Vector3(-100f, -100f, -100f);
                                }
                            }
                        }
                    }

                    tusk.transform.SetParent(vic.GetComponent<LateFollowTarget>()
                        ._lateFollowers[0].transform, true);
                    tusk.transform.localPosition = new Vector3(0f, 0f, 0f);
                    tusk.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

                }

                bool rotate_azimuth = is_m1ip ? rotate_azimuth_m1ip.Value : rotate_azimuth_m1.Value;
                    if (rotate_azimuth)
                    {
                        optic.RotateAzimuth = true;
                        optic.slot.LinkedNightSight.PairedOptic.RotateAzimuth = true;
                        optic.slot.VibrationShakeMultiplier = 0f;
                        optic.slot.VibrationPreBlur = false;
                        optic.Alignment = OpticAlignment.BoresightStabilized;
                        optic.slot.LinkedNightSight.PairedOptic.Alignment = OpticAlignment.BoresightStabilized;
                    }

                    bool has_citv = is_m1ip ? citv_m1a1.Value : citv_m1e1.Value;
                    if (has_citv)
                    {
                        GameObject c = GameObject.Instantiate(Assets.citv_obj, vic.transform.Find("IPM1_rig/HULL/TURRET"));
                        c.transform.localPosition = new Vector3(-0.6794f, 0.9341f, 0.4348f);
                        c.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

                        c.transform.SetParent(vic.transform.Find("IPM1_rig/HULL/TURRET").GetComponent<LateFollowTarget>()
                            ._lateFollowers[0].transform, true);

                        CITV citv_component = vic.DesignatedCameraSlots[0].LinkedNightSight.gameObject.AddComponent<CITV>();
                        citv_component.model = c;

                        if (vic.GetInstanceID() == PlayerInput.Instance.CurrentPlayerUnit.GetComponent<Vehicle>().GetInstanceID())
                        {
                            CameraManager.Instance.UpdateLightMode(mode: NightVisionType.Thermal);
                            CameraSlot.ActiveInstance.ThisActiveChanged(true);
                        }

                        c.transform.Find("assembly").GetComponent<UniformArmor>().Unit = vic;
                        c.transform.Find("glass").GetComponent<UniformArmor>().Unit = vic;

                        vic._targetSpotterSettings._periscopeFOV = 120f;
                        vic.TargetSpotterSettings._sightDistance = 4500f;
                        vic.TargetSpotterSettings._nightSightDistanceIdeal = 4500f;
                        vic.TargetSpotterSettings._nightSightDistancePassive = 4500f;
                        vic._friendlyName += "+";
                    }

                bool sepv4 = is_m1ip ? enhanced_systems.Value : false;
                if (sepv4)
                {

                    //GameObject idogh = GameObject.Instantiate(Assets.dogh_obj, vic.transform.Find("IPM1_rig/HULL/TURRET"));
                    //idogh.transform.localPosition = new Vector3(0f, -0.445f, -0.25f);
                    ////idogh.transform.localEulerAngles = new Vector3(0f, 0, 0f);

                    //idogh.transform.SetParent(vic.transform.Find("IPM1_rig/HULL/TURRET").GetComponent<LateFollowTarget>()
                    //    ._lateFollowers[0].transform.Find("Turret_Armor/DOGHOUSE"), true);

                    //idogh.transform.Find("idoghouse").GetComponent<UniformArmor>().Unit = vic;

                    //GameObject c = GameObject.Instantiate(Assets.citv_obj, vic.transform.Find("IPM1_rig/HULL/TURRET"));
                    //c.transform.localPosition = new Vector3(-0.6794f, 0.9341f, 0.4348f);
                    //c.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

                    //c.transform.SetParent(vic.transform.Find("IPM1_rig/HULL/TURRET").GetComponent<LateFollowTarget>()
                    //    ._lateFollowers[0].transform, true);

                    //CITV citv_component = vic.DesignatedCameraSlots[0].LinkedNightSight.gameObject.AddComponent<CITV>();
                    //citv_component.model = c;

                    //if (vic.GetInstanceID() == PlayerInput.Instance.CurrentPlayerUnit.GetComponent<Vehicle>().GetInstanceID())
                    //{
                    //    CameraManager.Instance.UpdateLightMode(mode: NightVisionType.Thermal);
                    //    CameraSlot.ActiveInstance.ThisActiveChanged(true);
                    //}

                    //c.transform.Find("assembly").GetComponent<UniformArmor>().Unit = vic;
                    //c.transform.Find("glass").GetComponent<UniformArmor>().Unit = vic;

                    Vector2Int resolutionNO = flir_gen3a1_res;
                    night_optic.slot.VibrationShakeMultiplier = 0.0f;
                    night_optic.slot.FLIRWidth = resolutionNO.x;
                    night_optic.slot.FLIRHeight = resolutionNO.y;
                    night_optic.slot.FLIRBlitMaterialOverride = Assets.flir_blit_mat_green_no_scan;

                    //DigitalEnhancement dev4_no = mainGun.FCS.gameObject.GetComponent<DigitalEnhancement>();
                    //dev4_no.original_blur = night_optic.slot.BaseBlur;
                    //dev4_no.slot = night_optic.slot;
                    //dev4_no.reticle_plane = night_optic.slot.transform.Find("Reticle Mesh/FFP");
                    //dev4_no.Add(2.4f, 0f, 0.69f);
                    //dev4_no.Add(1f, 0f, 0.29f);
                    //dev4_no.Add(0.4f, 0.0089f, 0.09f);

                    //vic._targetSpotterSettings._periscopeFOV = 175f;
                    //vic.TargetSpotterSettings._sightDistance = 11000f;
                    //vic.TargetSpotterSettings._nightSightDistanceIdeal = 10500f;
                    //vic.TargetSpotterSettings._nightSightDistancePassive = 10000f;
                    //vic.TargetSpotterSettings._nightSightDistanceUnaided = 8750f;

                    fcs.SuperelevateWeapon = true;
                    fcs.SuperleadWeapon = true;
                    fcs.SuperelevateFireGating = true;

                }

                if ((vic.FriendlyName == "M1A1HA+" || vic.FriendlyName == "M1A1HC+") && rotate_azimuth)
                    {
                        vic._friendlyName = "M1A2";
                    }

                    if (vic.FriendlyName == "M1A2" && (flir_gen > 2 || crows_m1a1.Value))
                    {
                        vic._friendlyName += " SEP";
                    }

                    if (vic.FriendlyName == "M1A2" && efta)
                    {
                        vic._friendlyName += "C+";
                    }

                    if (vic.FriendlyName == "M1A2 SEP" && efta)
                    {
                        vic._friendlyName = "M1A2C+ SEP";
                    }

                    if (vic.FriendlyName == "M1A2C+ SEP" && has_tuskfictional)
                    {
                        vic._friendlyName += " TUSK III";
                    }

                    mainGunInfo.Name = "120mm gun M256";
                    mainGun.Impulse = 68000;
                    mainGun.CodexEntry = gun_m256;
                    mainGun.WeaponSound.SingleShotEventPaths[0] = "event:/Weapons/canon_125mm-2A46";

                    GameObject dummy_tube = new GameObject("dummy tube");
                    dummy_tube.transform.parent = vic_go.transform.Find("IPM1_rig/HULL/TURRET/GUN");
                    dummy_tube.transform.localScale = new Vector3(0f, 0f, 0f);

                    Transform smr_path = (vic.UniqueName == "M1") ? vic.transform.Find("M1A0_mesh/M1A0 turret") : vic.transform.Find("M1IP_mesh/M1IP_turret");
                    int gun_recoil_idx = (vic.UniqueName == "M1") ? 13 : 9;
                    SkinnedMeshRenderer smr = smr_path.GetComponent<SkinnedMeshRenderer>();
                    Transform[] bones = smr.bones;
                    bones[gun_recoil_idx] = dummy_tube.transform;
                    smr.bones = bones;

                    GameObject gunTube = vic_go.transform.Find("IPM1_rig/HULL/TURRET/GUN/gun_recoil").gameObject;
                    gunTube.transform.Find("GUN/Gun Breech.001").GetComponent<MeshRenderer>().enabled = false;

                    GameObject _m256_obj = GameObject.Instantiate(Assets.m256_obj, gunTube.transform);
                    _m256_obj.transform.localPosition = new Vector3(0f, 0.0064f, -1.9416f);
       
                    Transform muzzleFlashes = mainGun.MuzzleEffects[1].transform;
                    muzzleFlashes.GetChild(1).transform.localScale = new Vector3(1.3f, 1.3f, 1f);
                    muzzleFlashes.GetChild(2).transform.localScale = new Vector3(1.3f, 1.3f, 1f);
                    muzzleFlashes.GetChild(4).transform.localScale = new Vector3(1.3f, 1.3f, 1f);

                // convert ammo
                string ap_idx = is_m1ip ? sabot_m1ip.Value : sabot_m1.Value;
                    string heat_idx = is_m1ip ? heat_m1ip.Value : heat_m1.Value;
                    AmmoClipCodexScriptable sabotClipCodex = Ammo_120mm.ap[ap_idx];
                    AmmoClipCodexScriptable heatClipCodex = Ammo_120mm.heat[heat_idx];

                    LoadoutManager loadoutManager = vic.GetComponent<LoadoutManager>();
                    loadoutManager.TotalAmmoCounts = new int[] { m829Count.Value, m830Count.Value };
                    loadoutManager.LoadedAmmoList.AmmoClips = new AmmoClipCodexScriptable[] { sabotClipCodex, heatClipCodex };

                    if (!original_rack_size.Value)
                    {
                        loadoutManager._totalAmmoCount = 40;

                        for (int i = 0; i <= 2; i++)
                        {
                            GHPC.Weapons.AmmoRack rack = loadoutManager.RackLoadouts[i].Rack;
                            rack.ClipCapacity = i == 2 ? 4 : 18;
                            Util.EmptyRack(rack);
                        }
                    }

                    loadoutManager.SpawnCurrentLoadout();
                    mainGun.Feed.AmmoTypeInBreech = null;
                    mainGun.Feed.Start();
                    loadoutManager.RegisterAllBallistics();

                    vic_go.transform.Find("IPM1_rig/HULL/TURRET/GUN/turret_gun").gameObject.SetActive(false);

                    if (vic.UniqueName == "M1IP Abrams")
                    {
                        vic_go.transform.Find("IPM1_rig/HULL/TURRET/M1 camo net/turret_gun").gameObject.SetActive(false);
                    }

                    vic_go.AddComponent<AlreadyConverted>();
                }

                yield break;
            }
        


        public static void Init()
        {            
            if (gun_m256 == null)
            {
                gun_m256 = ScriptableObject.CreateInstance<WeaponSystemCodexScriptable>();
                gun_m256.name = "gun_m256";
                gun_m256.CaliberMm = 120;
                gun_m256.FriendlyName = "120mm Gun M256";
                gun_m256.Type = WeaponSystemCodexScriptable.WeaponType.LargeCannon;
            }

            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(Convert), GameStatePriority.Medium);
        }
    }
}