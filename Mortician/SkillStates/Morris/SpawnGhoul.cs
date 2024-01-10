using UnityEngine;
using RoR2;
using EntityStates;
using System;
namespace SkillStates.Morris
{
    internal class SpawnGhoul : BaseState
    {
        public static GameObject GhoulMasterObject;

        public static float baseDuration = 1f;
        public static float placementCapsuleRadius;
        public static float placementCapsuleHeight;

        private float duration;
        private float earlyExitTime;
        public override void OnEnter()
        {
            base.OnEnter();

            duration = baseDuration / attackSpeedStat;
            earlyExitTime = duration * 0.5f;

            PlayCrossfade("Gesture, Override", "LanternRaise", "Swing.playbackRate", duration, 0.05f);

            StartAimMode(2f, false);

            AttemptSpawnGhoul();
        }

        private void AttemptSpawnGhoul()
        {
            MasterSummon masterSummon = new MasterSummon();
        }

        private Vector3 GetBestSpawnPosition()
        {
            Ray aimRay = base.GetAimRay();
            Vector3 direction = aimRay.direction;
            direction.y = 0f;
            direction.Normalize();
            aimRay.direction = direction;

            Ray ray = new Ray(aimRay.GetPoint(2f) + Vector3.up, Vector3.down);

            return base.characterBody.footPosition;
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
