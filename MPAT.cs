using System.Collections.Generic;
using UnityEngine;
using GHPC.Weapons;
using HarmonyLib;
using GHPC.Player;
using GHPC.UI.Hud;
using GHPC.Utility;
using GHPC.PhysicsHelpers;
using GHPC;
using GHPC.Effects;
using System.Linq;

namespace M1A1Abrams
{
    public class MPATManager : MonoBehaviour
    {
        public bool ProximityActive { get; set; }
        public int AmmoKeyIdx { get; set; }
        public int AmmoCachedIdx { get; set; }


        private float cd = 0f;
        private WeaponSystem weapon = null;
        private PlayerInput player_manager;
        private int self_id;


        void Awake()
        {
            weapon = GetComponent<WeaponsManager>().Weapons[0].Weapon;
            weapon.Fired += OnFired;
            self_id = gameObject.GetInstanceID();
        }

        void Update()
        {
            cd -= Time.deltaTime;

            if (PlayerInput.Instance.CurrentPlayerUnit.gameObject.GetInstanceID() != self_id) return;

            if (
                InputUtil.MainPlayer.GetButtonDoublePressDown(PlayerInput.ammoKeys[AmmoKeyIdx]) && 
                weapon.CurrentAmmoType.CachedIndex == AmmoCachedIdx
            )
            {
                ProximityActive = !ProximityActive;
            }
        }

        private void OnFired(AmmoType ammo, LiveRound live_round)
        {
            if (ammo.CachedIndex != AmmoCachedIdx || !ProximityActive) return;

            live_round.gameObject.AddComponent<MPAT>();
        }
    }

    public class MPAT : MonoBehaviour
    {
        public bool detonated = false;

        private LiveRound live_round;
        private static float[] ranges = new float[] { 10f, 30f };
        private AmmoType dummy_he;

        void Awake()
        {
            live_round = this.GetComponent<LiveRound>();

            if (dummy_he != null) return;

            dummy_he = new AmmoType();
            dummy_he.DetonateEffect = Resources.FindObjectsOfTypeAll<GameObject>().Where(o => o.name == "HE_explosion").First();
            dummy_he.ImpactEffectDescriptor = new ParticleEffectsManager.ImpactEffectDescriptor()
            {
                HasImpactEffect = true,
                ImpactCategory = ParticleEffectsManager.Category.HighExplosive,
                EffectSize = ParticleEffectsManager.EffectSize.MainGun,
                RicochetType = ParticleEffectsManager.RicochetType.None,
                Flags = ParticleEffectsManager.ImpactModifierFlags.Large,
                MinFilterStrictness = ParticleEffectsManager.FilterStrictness.Low
            };
        }

        void OnDisable()
        {
            Component.Destroy(this);
        }

        void Detonate()
        {
            if (detonated) return;
            detonated = true;
            live_round._terrainHit = false;
            live_round._didTerrainHit = false;
            live_round._fuzeCompleted = true;
            live_round._materialHit = GHPC.Effects.ParticleEffectsManager.SurfaceMaterial.None;
            live_round.createExplosion(hitSurface: false, 0f, Vector3.zero, 0.03f);
            live_round.Detonate();

            ParticleEffectsManager.Instance.CreateImpactEffectOfType(dummy_he,
                ParticleEffectsManager.FusedStatus.Fuzed, ParticleEffectsManager.SurfaceMaterial.None, false, transform.position);

            for (int i = 0; i < 25; i++)
            {
                GHPC.Weapons.LiveRound frag;
                frag = LiveRoundMarshaller.Instance.GetRoundOfVisualType(LiveRoundMarshaller.LiveRoundVisualType.Spall)
                    .GetComponent<GHPC.Weapons.LiveRound>();
                frag.Info = Ammo_120mm.m830a1_forward_frag;
                frag.CurrentSpeed = 600f;
                frag.MaxSpeed = 600f;
                frag.IsSpall = false;
                frag.Shooter = live_round.Shooter;
                frag.transform.position = live_round.transform.position;
                frag.transform.forward = Quaternion.Euler(
                    UnityEngine.Random.Range(-5f, 5f),
                    UnityEngine.Random.Range(-5f, 5f),
                    UnityEngine.Random.Range(-5f, 5f)
                ) * live_round.transform.forward;
                frag.Init(live_round, null);
                frag.name = "mpat forward frag " + i;
            }
        }

        void Update()
        {
            Vector3 pos = this.transform.position;
            Vector3[] dirs = new Vector3[]
            {
                this.transform.forward,
                Vector3.down
            };

            for (int i = 0; i < dirs.Length; i++)
            {
                RaycastHit hit;
                Ray ray = new Ray(pos, dirs[i]);
                if (
                    RaycastColliderUtils.Raycast(ray, out hit, ranges[i], ConstantsAndInfoManager.Instance.LaserRangefinderLayerMask)
                    && hit.collider.CompareTag("Penetrable")
                )
                {
                    Detonate();
                    return;
                }
            }

            RaycastHit hit_sphere;
            if (
                Physics.SphereCast(pos, 3f, this.transform.forward, out hit_sphere, 0.1f, ConstantsAndInfoManager.Instance.LaserRangefinderLayerMask) 
                && hit_sphere.collider.CompareTag("Penetrable")
            )
            {
                Detonate();
            }
        }
    }

    [HarmonyPatch(typeof(WeaponHud), "Update")]
    public static class MPATProximityHud
    {
        private static void Postfix(WeaponHud __instance)
        {
            MPATManager mpat_manager = __instance._playerInput?.CurrentPlayerUnit?.GetComponent<MPATManager>();

            if (mpat_manager == null || !mpat_manager.ProximityActive) return;

            __instance._hudText.text = __instance._sb.ToString() + "\nMPAT Proximity";
        }
    }
}