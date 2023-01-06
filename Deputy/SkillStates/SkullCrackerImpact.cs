using UnityEngine;
using RoR2;
using EntityStates;
using EntityStates.Loader;
using UnityEngine.Networking;

namespace Skillstates.Deputy
{
    internal class SkullCrackerImpact : BaseState
    {
        public static float baseDuration = 0.5f;
        public static float baseHitStopDuration = 0.2f;
        public static float pushAwayForce = 25f;

        internal HealthComponent victim;
        internal Vector3 knockbackDirection;

        private Vector3 impactPoint;
        private float duration;
        private float windUpTime;
        private float hitStopDuration;

        private Animator animator;
        private BaseState.HitStopCachedState hitStop;
        private bool hasPaused;
        private bool inHitPause;
        private float hitPauseTimer;
        private float stopwatch;
        public override void OnEnter()
        {
            base.OnEnter();

            animator = base.GetModelAnimator();

            impactPoint = victim.transform.position;

            base.PlayAnimation("FullBody, Override", "Kick1", "Flip.playbackRate", baseDuration);

            EffectManager.SimpleImpactEffect(SwingZapFist.overchargeImpactEffectPrefab, impactPoint, base.characterDirection.forward, false);

            base.characterMotor.velocity = knockbackDirection * pushAwayForce;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= baseDuration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }

            //hitPauseTimer -= Time.fixedDeltaTime;

            /*
            if (base.isAuthority)
            {

                if(base.fixedAge >= windUpTime && !hasPaused)
                {
                    if (!inHitPause)
                    {
                        hasPaused = true;
                        hitStop = CreateHitStopCachedState(characterMotor, animator, "Flip.playbackRate");
                        hitPauseTimer = hitStopDuration;
                        inHitPause = true;
                    }
                }

                if (!inHitPause)
                    stopwatch += Time.fixedDeltaTime;
                else
                {
                    characterMotor.velocity = Vector3.zero;
                    animator.SetFloat("Flip.playbackRate", 0f);
                }

                if(inHitPause && hitPauseTimer <= 0f)
                {
                    base.ConsumeHitStopCachedState(hitStop, characterMotor, animator);
                    inHitPause = false;
                }

                if (stopwatch >= duration + hitStopDuration)
                {
                    this.outer.SetNextStateToMain();
                }
            }*/
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
