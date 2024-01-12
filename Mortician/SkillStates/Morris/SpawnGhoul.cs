using UnityEngine;
using RoR2;
using EntityStates;
using EntityStates.GolemMonster;
using System;
using Morris.Components;
using UnityEngine.Networking;
namespace Skillstates.Morris
{
    internal class SpawnGhoul : BaseState
    {
        public static GameObject GhoulMasterPrefab;

        public static float baseDuration = 1f;

        private float duration;
        private float earlyExitTime;
        public override void OnEnter()
        {
            base.OnEnter();

            duration = baseDuration / attackSpeedStat;
            earlyExitTime = duration * 0.3f;

            PlayCrossfade("Gesture, Override", "LanternRaise", "Swing.playbackRate", duration, 0.05f);

            StartAimMode(2f, false);

            if (NetworkServer.active)
            {
                AttemptSpawnGhoul();
            }
        }

        private void AttemptSpawnGhoul()
        {
            MasterSummon masterSummon = new MasterSummon();
            masterSummon.masterPrefab = GhoulMasterPrefab;
            masterSummon.ignoreTeamMemberLimit = true;
            masterSummon.teamIndexOverride = TeamIndex.Player;
            masterSummon.summonerBodyObject = base.gameObject;
            masterSummon.inventoryToCopy = base.characterBody.inventory;
            masterSummon.position = GetBestSpawnPosition();
            masterSummon.rotation = Util.QuaternionSafeLookRotation(base.characterDirection.forward);
            
            CharacterMaster characterMaster = masterSummon.Perform();
            if(characterMaster)
            {
                MorrisMinionController minionController = characterMaster.bodyInstanceObject.GetComponent<MorrisMinionController>();
                minionController.owner = base.gameObject;
            }
        }

        private Vector3 GetBestSpawnPosition()
        {
            Ray aimRay = base.GetAimRay();
            Vector3 direction = aimRay.direction;
            direction.y = 0f;
            direction.Normalize();
            aimRay.origin += Vector3.up * 2f;
            aimRay.direction = direction;

            RaycastHit raycastHit;
            bool hitConfirm = Physics.Raycast(aimRay, out raycastHit, 3f, LayerIndex.world.mask);

            Ray spawnRay;
            if (hitConfirm)
            {
                spawnRay = new Ray(raycastHit.point + Vector3.up * 2f, Vector3.down);
            }
            else
            {
                spawnRay = new Ray(aimRay.GetPoint(3f) + Vector3.up * 2f, Vector3.down);
            }

            Physics.Raycast(spawnRay, out raycastHit, LayerIndex.world.mask);

            return raycastHit.point;
            /*
            bool placementOK = ValidateRaycastHit(ray, raycastHit);

            if(placementOK)
            {
                return raycastHit.point;
            }
            else
            {
                return base.characterBody.footPosition;
            }
            */
        }

        private bool ValidateRaycastHit(Ray ray, RaycastHit hit)
        {
            if(hit.normal.y > 0.5f && Vector3.Distance(hit.point, ray.origin) <= 10f)
            {
                return true;
            }

            return false;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return fixedAge >= earlyExitTime ? InterruptPriority.Any : InterruptPriority.PrioritySkill;
        }
    }
}
