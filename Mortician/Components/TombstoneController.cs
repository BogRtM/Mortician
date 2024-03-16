using Morris.Content;
using Morris.Modules.NPC;
using RoR2;
using RoR2.Navigation;
using RoR2.Orbs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Morris.Components
{
    internal class TombstoneController : NetworkBehaviour
    {
        public static float spawnTime = 10f;
        public static float searchRadius = 80f;
        public static float soulOrbDamage = 3.5f;

        private MorrisMinionController minionController;
        private TombstoneLocator ownerLocator;
        private ChildLocator childLocator;
        private SoulOrbLocator soulOrbLocator;
        private TeamComponent teamComponent;
        private CharacterBody characterBody;

        private NodeGraph nodeGraph;

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

            nodeGraph = SceneInfo.instance.GetNodeGraph(MapNodeGroup.GraphType.Ground);

            if (!nodeGraph)
            {
                Log.Warning("No node graph to spawn ghouls with!");
            }
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

                if(NetworkServer.active && nodeGraph)
                {
                    //SpawnGhoul();
                    SpawnGhoulAtClosestNode();
                }
            }

            if (soulStock > 0)
            {
                fireStopwatch += Time.fixedDeltaTime;
            }

            if(fireStopwatch >= fireTime && soulStock > 0)
            {
                fireStopwatch = 0f;

                if (NetworkServer.active)
                {
                    var target = FindOrbTarget();

                    if (target && target.healthComponent.alive)
                    {
                        FireSoulOrb(target);
                    }
                }
            }
        }

        [Server]
        public void FireSoulOrb(HurtBox target)
        {
            Vector3 orbMuzzle = soulOrbLocator.GetPosition(soulStock - 1);

            TombstoneSoulOrb soulOrb = new TombstoneSoulOrb();
            soulOrb.attacker = base.gameObject;
            soulOrb.origin = orbMuzzle;
            soulOrb.target = target;
            soulOrb.damageValue = characterBody.damage * soulOrbDamage;
            soulOrb.damageType = DamageType.Generic;
            soulOrb.damageColorIndex = DamageColorIndex.Default;
            soulOrb.isCrit = Util.CheckRoll(characterBody.crit, characterBody.master);
            soulOrb.teamIndex = teamComponent.teamIndex;

            DetractSoulStockServer();
            OrbManager.instance.AddOrb(soulOrb);
        }

        [Server]
        public void AddSoulStockServer()
        {
            soulStock = Mathf.Clamp(soulStock + 1, 0, 10);

            soulOrbLocator.ActivateSphere(soulStock - 1);

            //Log.Warning("Soul Stock on server is: " + soulStock);

            RpcAddSoulStock(soulStock);
        }

        [ClientRpc]
        public void RpcAddSoulStock(int i)
        {
            soulOrbLocator.ActivateSphere(i - 1);
        }

        [Server]
        public void DetractSoulStockServer()
        {
            soulOrbLocator.DeactivateSphere(soulStock - 1);

            soulStock = Mathf.Clamp(soulStock - 1, 0, 10);

            //Log.Warning("Soul Stock on server is: " + soulStock);

            RpcDetractSoulStock(soulStock);
        }

        [ClientRpc]
        public void RpcDetractSoulStock(int i)
        {
            soulOrbLocator.DeactivateSphere(i);
        }

        public HurtBox FindOrbTarget()
        {
            search.origin = base.transform.position;
            search.mask = LayerIndex.entityPrecise.mask;
            search.radius = searchRadius;
            search.RefreshCandidates();
            search.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(teamComponent.teamIndex));
            search.OrderCandidatesByDistance();
            search.FilterCandidatesByDistinctHurtBoxEntities();
            
            HurtBox bestTarget = search.GetHurtBoxes().FirstOrDefault<HurtBox>();
            search.ClearCandidates();

            return bestTarget;
        }

        [Server]
        public void SpawnGhoul()
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

        [Server]
        public void SpawnGhoulAtClosestNode()
        {
            var nodeIndex = nodeGraph.FindClosestNodeWithFlagConditions(base.transform.position, HullClassification.Human, NodeFlags.None, NodeFlags.NoCharacterSpawn, false);

            Vector3 vector3;
            nodeGraph.GetNodePosition(nodeIndex, out vector3);

            MasterSummon masterSummon = new MasterSummon();
            masterSummon.masterPrefab = GhoulMinion.ghoulMasterPrefab;
            masterSummon.ignoreTeamMemberLimit = true;
            masterSummon.teamIndexOverride = TeamIndex.Player;
            masterSummon.summonerBodyObject = minionController.owner;
            //masterSummon.inventoryToCopy = base.characterBody.inventory;
            masterSummon.position = vector3;
            masterSummon.rotation = base.transform.rotation;

            CharacterMaster ghoulMaster;
            ghoulMaster = masterSummon.Perform();
            if (ghoulMaster)
            {
                ghoulMaster.inventory.CopyEquipmentFrom(characterBody.inventory);
            }
        }
    }
}
