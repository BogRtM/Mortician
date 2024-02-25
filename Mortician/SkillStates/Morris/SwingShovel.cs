using UnityEngine;
using RoR2;
using R2API;
using EntityStates;
using RoR2.Skills;
using Morris;
using Morris.Modules;
using System;
using Morris.Components;
namespace SkillStates.Morris
{
    internal class SwingShovel : BaseState, SteppedSkillDef.IStepSetter
    {
        public static float baseDuration = 1.833f;
        public static float damageCoefficient = 8f;
        public static float smallHopVelocity = 6f;
        public static float hitPauseDuration = 0.12f;

        private Animator animator;

        private OverlapAttack attack;
        private HitBoxGroup hitBoxGroup;

        private float duration;
        private float step;
        private float fireTime;
        private float fireEndTime;
        private float earlyExitTime;

        private bool hasFired;
        private bool hasHopped;

        public string swingSoundString = "Morris_SwingShovel";
        public string hitGhoulSoundString = "Morris_HitGhoulWithShovel";
        public string hitTombstoneSoundString = "Morris_HitTombstoneWithShovel";

        private HitStopCachedState hitStopCachedState;
        private float stopWatch;
        private float hitPauseTimer;
        private bool inHitPause;

        public override void OnEnter()
        {
            base.OnEnter();

            animator = base.GetModelAnimator();

            duration = baseDuration / base.attackSpeedStat;
            fireTime = duration * 0.18f;
            fireEndTime = duration * 0.22f;
            earlyExitTime = duration * 0.54f;

            base.StartAimMode(2f, false);

            string swingIndex = "Swing" + step;

            if(!animator.GetBool("isMoving") && base.characterMotor.isGrounded)
                base.PlayCrossfade("FullBody, Override", swingIndex, "Swing.playbackRate", duration, 0.1f);

            base.PlayCrossfade("Gesture, Override", swingIndex, "Swing.playbackRate", duration, 0.1f);

            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Swing");
            }

            attack = new OverlapAttack();
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.damageType = DamageType.Generic;
            attack.procCoefficient = 1f;
            attack.teamIndex = base.GetTeam();
            attack.isCrit = base.RollCrit();
            attack.forceVector = Vector3.zero;
            attack.pushAwayForce = 1f;
            attack.damage = damageCoefficient * base.damageStat;
            attack.hitBoxGroup = hitBoxGroup;
            attack.hitEffectPrefab = Assets.MorrisShovelHit;
            attack.AddModdedDamageType(MorrisPlugin.LaunchGhoul);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            hitPauseTimer -= Time.fixedDeltaTime;

            /*
            if (base.fixedAge >= fireTime && !hasFired)
            {
                FireAttack();
                HitGhoul(this.hitBoxGroup);
            }
            */

            if (base.fixedAge >= fireTime && base.fixedAge <= fireEndTime)
            {
                if (!hasFired)
                {
                    HitGhoul(this.hitBoxGroup);
                }

                FireAttack();
            }

            if (hitPauseTimer <= 0f && inHitPause)
            {
                base.ConsumeHitStopCachedState(hitStopCachedState, base.characterMotor, animator);
                inHitPause = false;
            }

            if (!inHitPause)
            {
                stopWatch += Time.fixedDeltaTime;
            }
            else
            {
                base.characterMotor.velocity = Vector3.zero;
                animator.SetFloat("Swing.playbackRate", 0f);
            }

            if (stopWatch >= this.duration && hasFired && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public void FireAttack()
        {
            if (!hasFired)
            {
                PlaySwingEffect();
                hasFired = true;
            }

            if (base.isAuthority)
            {
                if (attack.Fire())
                {
                    if (!base.characterMotor.isGrounded && !hasHopped)
                    {
                        hasHopped = true;
                        base.SmallHop(base.characterMotor, smallHopVelocity);
                    }

                    if (!inHitPause)
                    {
                        hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, animator, "Swing.playbackRate");
                        hitPauseTimer = hitPauseDuration / attackSpeedStat;
                        inHitPause = true;
                    }
                }
            }
        }

        public void HitGhoul(HitBoxGroup hitBoxGroup)
        {
            Ray aimRay = base.GetAimRay();
            Vector3 launchVector = aimRay.direction;

            foreach(HitBox hitBox in hitBoxGroup.hitBoxes)
            {
                Transform transform = hitBox.transform;
                Vector3 position = transform.position;
                Vector3 halfExtent = transform.lossyScale * 0.5f;
                Quaternion rotation = transform.rotation;

                Collider[] hitObjects = Physics.OverlapBox(position, halfExtent, rotation, LayerIndex.defaultLayer.mask);

                foreach(Collider collider in hitObjects)
                {
                    MorrisMinionController minionController = collider.GetComponent<MorrisMinionController>();
                    if (minionController && minionController.teamIndex == base.teamComponent.teamIndex)
                    {
                        Log.Warning("Hit ghoul collider " + collider.name + " on layer: " + collider.gameObject.layer);

                        switch (minionController.minionType)
                        {
                            case MorrisMinionController.MorrisMinionType.Ghoul:
                                Util.PlaySound(hitGhoulSoundString, minionController.gameObject);
                                break;

                            case MorrisMinionController.MorrisMinionType.Tombstone:
                                //Util.PlaySound(hitTombstoneSoundString, minionController.gameObject);
                                break;

                            default:
                                break;
                        }

                        minionController.Launch(launchVector);
                    }
                }
            }
        }

        public void PlaySwingEffect()
        {
            string muzzleName = step == 0 ? "SwingLeft" : "SwingRight";
            string soundString = step == 0 ? "1" : "2";

            soundString = swingSoundString + soundString;

            Util.PlaySound(soundString, base.gameObject);

            EffectManager.SimpleMuzzleFlash(Assets.ShovelSwingVFX, base.gameObject, muzzleName, false);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return (base.fixedAge >= earlyExitTime) ? InterruptPriority.Any : InterruptPriority.Skill;
        }

        public void SetStep(int i)
        {
            step = i;
        }
    }
}
