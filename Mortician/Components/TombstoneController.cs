using Morris.Content;
using Morris.Modules.NPC;
using RoR2;
using RoR2.Orbs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Morris.Components
{
    internal class TombstoneController : MonoBehaviour
    {
        public static float spawnTime = 10f;
        public static float soulOrbDamage = 3.5f;

        private MorrisMinionController minionController;
        private TombstoneLocator ownerLocator;
        private ChildLocator childLocator;
        private SoulOrbLocator soulOrbLocator;
        private TeamComponent teamComponent;
        private CharacterBody characterBody;

        private readonly SphereSearch search = new SphereSearch();

        private float fireTime = 1f;
        private float spawnStopwatch = spawnTime;
        private float fireStopwatch;
        private int soulStock;

        public void Start()
        {
            minionController = base.GetComponent<MorrisMinionController>();
            teamComponent = base.GetComponent<TeamComponent>();
            characterBody = base.GetComponent<CharacterBody>();

            ModelLocator modelLocator = base.GetComponent<ModelLocator>();
            childLocator = modelLocator.modelTransform.GetComponent<ChildLocator>();
            soulOrbLocator = modelLocator.modelTransform.GetComponent<SoulOrbLocator>();

            ownerLocator = minionController.owner.GetComponent<TombstoneLocator>();
            ownerLocator.SetActiveTombstone(this);
        }

        public void OnDestroy()
        {
            if(ownerLocator.activeTombstone == this)
            {
                ownerLocator.SetActiveTombstone(null);
            }
        }

        public void FixedUpdate()
        {
            spawnStopwatch += Time.fixedDeltaTime;

            if (spawnStopwatch >= spawnTime)
            {
                spawnStopwatch = 0f;
                SpawnGhoul();
            }

            if (soulStock > 0)
            {
                fireStopwatch += Time.fixedDeltaTime;
            }

            if(fireStopwatch >= fireTime && soulStock > 0)
            {
                fireStopwatch = 0f;
                var target = FindOrbTarget();
                if (target && target.healthComponent.alive)
                {
                    FireSoulOrb(target);
                }
            }
        }

        public void FireSoulOrb(HurtBox target)
        {
            Vector3 orbMuzzle = soulOrbLocator.GetPosition(soulStock - 1);
            soulOrbLocator.DeactivateOrb(soulStock - 1);
            soulStock--;

            if (NetworkServer.active)
            {
                TombstoneSoulOrb soulOrb = new TombstoneSoulOrb();
                soulOrb.attacker = base.gameObject;
                soulOrb.origin = orbMuzzle;
                soulOrb.target = target;
                soulOrb.damageValue = characterBody.damage * soulOrbDamage;
                soulOrb.damageType = DamageType.Generic;
                soulOrb.damageColorIndex = DamageColorIndex.Default;
                soulOrb.isCrit = Util.CheckRoll(characterBody.crit, characterBody.master);
                soulOrb.teamIndex = teamComponent.teamIndex;
                OrbManager.instance.AddOrb(soulOrb);
            }
        }

        public void AddSoulStock()
        {
            soulStock = Mathf.Clamp(soulStock + 1, 0, 10);

            soulOrbLocator.ActivateOrb(soulStock - 1);
        }

        public HurtBox FindOrbTarget()
        {
            search.origin = base.transform.position;
            search.mask = LayerIndex.entityPrecise.mask;
            search.radius = 40f;
            search.RefreshCandidates();
            search.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(teamComponent.teamIndex));
            search.OrderCandidatesByDistance();
            search.FilterCandidatesByDistinctHurtBoxEntities();
            
            HurtBox bestTarget = search.GetHurtBoxes().FirstOrDefault<HurtBox>();
            search.ClearCandidates();

            return bestTarget;
        }

        public void SpawnGhoul()
        {
            if (NetworkServer.active)
            {
                DirectorPlacementRule directorPlacementRule = new DirectorPlacementRule()
                {
                    placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                    minDistance = 3f,
                    maxDistance = 40f,
                    spawnOnTarget = base.transform
                };

                DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(GhoulMinion.ghoulSpawnCard, directorPlacementRule, RoR2Application.rng);
                directorSpawnRequest.summonerBodyObject = minionController.owner;
                directorSpawnRequest.ignoreTeamMemberLimit = true;
                directorSpawnRequest.teamIndexOverride = teamComponent.teamIndex;
                DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
            }
        }
    }
}
