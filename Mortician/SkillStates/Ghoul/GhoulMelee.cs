using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.AddressableAssets;
using Morris.Modules;
using System;
using Morris.Components;
using UnityEngine.Networking;

namespace SkillStates.Ghoul
{
    internal class GhoulMelee : BaseState
    {
        public static float baseDuration = 1f;
        public static float damageCoefficient = 1.5f;

        private MorrisMinionController minionController;

        private OverlapAttack attack;

        private string animString = "Melee";
        private string muzzleName = "MuzzleMelee";

        private int meleeIndex;
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

            if(meleeIndex == 0)
            {
                meleeIndex = UnityEngine.Random.RandomRangeInt(1, 5);
            }
            animString += meleeIndex;
            muzzleName += meleeIndex;
            base.PlayCrossfade("Gesture, Override", animString, "Attack.playbackRate", duration, 0.1f);

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
            attack.isCrit = minionController.owner ? Util.CheckRoll(minionController.ownerBody.crit, minionController.ownerBody.master) : base.RollCrit();
            attack.forceVector = Vector3.zero;
            attack.pushAwayForce = 1f;
            attack.damage = damageCoefficient * base.damageStat;
            attack.hitBoxGroup = hitBoxGroup;
            attack.hitEffectPrefab = Assets.OmniImpactVFXGhoul;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write((byte)meleeIndex);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            meleeIndex = (int)reader.ReadByte();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= fireTime && !hasFired)
            {
                hasFired = true;

                EffectManager.SimpleMuzzleFlash(Assets.GhoulMeleeEffects[meleeIndex], base.gameObject, muzzleName, true);

                if(base.isAuthority)
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
