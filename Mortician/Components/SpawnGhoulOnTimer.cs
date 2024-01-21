using Morris.Modules.NPC;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Morris.Components
{
    internal class SpawnGhoulOnTimer : MonoBehaviour
    {
        public static float spawnTime = 10f;

        private MorrisMinionController minionController;
        private TeamComponent teamComponent;

        private float stopwatch = 9f;

        public void Start()
        {
            minionController = base.GetComponent<MorrisMinionController>();
            teamComponent = base.GetComponent<TeamComponent>();
        }

        public void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;

            if(stopwatch >= spawnTime)
            {
                stopwatch = 0f;
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
                    maxDistance = 25f,
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
