using UnityEngine;
using RoR2;
using EntityStates;
using Morris.Modules.NPC;
using Morris.Components;
using Morris;
using UnityEngine.Networking;

namespace SkillStates.Tombstone
{
    internal class TombstoneMainState : GenericCharacterMain
    {
        public static float spawnTime = 8f;

        private MorrisMinionController minionController;

        private float summonTimer;

        public override void OnEnter()
        {
            base.OnEnter();
            minionController = base.GetComponent<MorrisMinionController>();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            summonTimer += Time.fixedDeltaTime;
            if(summonTimer >= spawnTime)
            {
                summonTimer = 0;

                SpawnGhoul();
            }
        }

        public void SpawnGhoul()
        {
            if (NetworkServer.active)
            {
                DirectorPlacementRule directorPlacementRule = new DirectorPlacementRule()
                {
                    placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                    minDistance = 3f,
                    maxDistance = 10f,
                    spawnOnTarget = base.transform
                };

                DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(LesserGhoul.ghoulSpawnCard, directorPlacementRule, RoR2Application.rng);
                directorSpawnRequest.summonerBodyObject = minionController.owner;
                directorSpawnRequest.ignoreTeamMemberLimit = true;
                directorSpawnRequest.teamIndexOverride = base.teamComponent.teamIndex;
                //directorSpawnRequest.onSpawnedServer += ValidateSpawnCard;
                DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
            }
        }

        public void ValidateSpawnCard(SpawnCard.SpawnResult result)
        {
            Log.Warning(result.spawnedInstance.ToString());
            Log.Warning("Spawn card success : " + result.success);
        }

        public override void Update()
        {
            if (base.characterMotor.isGrounded)
            {
                base.characterMotor.velocity = Vector3.zero;
                base.characterMotor.rootMotion = Vector3.zero;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
