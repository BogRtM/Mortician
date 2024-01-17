using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.AddressableAssets;
using Morris;
using System;
using Morris.Components;

namespace SkillStates.Ghoul
{
    internal class GhoulMelee : BaseState
    {
        public static float baseDuration = 1f;
        public static float damageCoefficient = 1.5f;

        public static GameObject hitPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/OmniImpactVFXSlashSyringe.prefab").WaitForCompletion();

        private MorrisMinionController minionController;

        private OverlapAttack attack;

        private float duration;
        private float fireTime;
        private bool hasFired;

        public override void OnEnter()
        {
            base.OnEnter();

            duration = baseDuration / this.attackSpeedStat;
            fireTime = duration * 0.2f;

            StartAimMode(2f, false);

            minionController = base.GetComponent<MorrisMinionController>();

            Transform modelTransform = base.GetModelTransform();
            HitBoxGroup hitBoxGroup = new HitBoxGroup();

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "GhoulMelee");
            }

            attack = new OverlapAttack();
            attack.attacker = minionController.owner ? minionController.owner : base.gameObject;
            attack.inflictor = base.gameObject;
            attack.damageType = DamageType.Generic;
            attack.procCoefficient = 1f;
            attack.teamIndex = base.GetTeam();
            attack.isCrit = base.RollCrit();
            attack.forceVector = Vector3.zero;
            attack.pushAwayForce = 1f;
            attack.damage = damageCoefficient * base.damageStat;
            attack.hitBoxGroup = hitBoxGroup;
            attack.hitEffectPrefab = hitPrefab;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= fireTime && !hasFired && base.isAuthority)
            {
                hasFired = true;
                attack.Fire();
            }

            if(base.fixedAge >= duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
