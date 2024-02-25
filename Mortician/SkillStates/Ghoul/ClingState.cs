using UnityEngine;
using RoR2;
using EntityStates;
using RoR2.Skills;
using Morris.Modules;
using UnityEngine.Networking;
using Morris.Components;
using System;
using Morris;

namespace SkillStates.Ghoul
{
    internal class ClingState : BaseState
    {
        public static float damageCoefficient = GhoulMelee.damageCoefficient;
        public static float biteInterval = 0.7f;
        public static Vector3 downwardForce = Vector3.down * 800f;
        
        public HurtBox initialTarget;
        //public HealthComponent targetHealthComponent;
        public HurtBoxGroup targetGroup;

        private Transform modelTransform;
        private HurtBox clingHurtbox;
        private Collider targetCollider;
        private bool negativeOffset;

        private MorrisMinionController minionController;
        private float stopwatch;

        public override void OnEnter()
        {
            base.OnEnter();

            base.PlayAnimation("FullBody, Override", "ClingLoop");
            base.PlayCrossfade("FullBody, Additive", "ClingBite", "Attack.playbackRate", biteInterval, 0.1f);

            //Set up values to cling model
            base.modelLocator.enabled = false;
            base.characterDirection.enabled = false;
            modelTransform = base.GetModelTransform();
            negativeOffset = UnityEngine.Random.value > 0.5f;

            //Get random hurtbox to cling to
            if(initialTarget)
            {
                targetGroup = initialTarget.hurtBoxGroup;
                int randomIndex = UnityEngine.Random.Range(0, targetGroup.hurtBoxes.Length);
                clingHurtbox = targetGroup.hurtBoxes[randomIndex];
                targetCollider = clingHurtbox.GetComponent<Collider>();
            }

            if(base.isAuthority)
            {
                base.characterMotor.velocity = Vector3.zero;
            }

            minionController = base.GetComponent<MorrisMinionController>();
            minionController.isInClingState = true;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            writer.Write(HurtBoxReference.FromHurtBox(this.initialTarget));
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            initialTarget = reader.ReadHurtBoxReference().ResolveHurtBox();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            UpdateClingPoint();

            stopwatch += Time.fixedDeltaTime;

            if (stopwatch >= biteInterval && initialTarget.healthComponent.alive)
            {
                stopwatch = 0f;
                Bite();
            }

            if (!initialTarget.healthComponent.alive && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public void UpdateClingPoint()
        {
            base.characterMotor.velocity = Vector3.zero;
            base.characterMotor.rootMotion = Vector3.zero;

            //Find bounds of hurtbox collider
            Vector3 pointWithOffset = targetCollider.bounds.center;

            float zOffset = targetCollider.bounds.extents.z;
            Quaternion rotationToSet = targetCollider.transform.rotation;

            if (negativeOffset)
            {
                zOffset *= -1f;
                rotationToSet = Quaternion.Inverse(rotationToSet);
            }

            pointWithOffset.z -= zOffset;

            //Set position and rotation
            base.characterMotor.Motor.SetPosition(pointWithOffset, true);
            modelTransform.position = pointWithOffset;
            modelTransform.rotation = rotationToSet;
        }

        public void Bite()
        {
            base.PlayCrossfade("FullBody, Additive", "ClingBite", "Attack.playbackRate", biteInterval, 0.1f);

            EffectData effectData = new EffectData()
            {
                origin = clingHurtbox.transform.position,
                scale = 1.5f
            };
            EffectManager.SpawnEffect(Assets.OmniImpactVFXGhoul, effectData, true);

            Util.PlaySound("Play_acrid_m2_bite_hit", base.gameObject, "Volume_SFX", 0.2f);

            try
            {
                DamageInfo damageInfo = new DamageInfo
                {
                    position = initialTarget.transform.position,
                    attacker = minionController.owner ? minionController.owner : base.gameObject,
                    inflictor = gameObject,
                    damage = damageCoefficient * base.damageStat,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Generic,
                    crit = RollCrit(),
                    force = downwardForce,
                    procChainMask = default,
                    procCoefficient = 1f
                };

                if (NetworkServer.active)
                {
                    initialTarget.healthComponent.TakeDamage(damageInfo);
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, initialTarget.healthComponent.gameObject);
                    GlobalEventManager.instance.OnHitAll(damageInfo, initialTarget.healthComponent.gameObject);
                }
            }
            catch (Exception e)
            {
                Log.Warning("NRE in Bite()");
            }
        }

        public override void OnExit()
        {
            base.modelLocator.enabled = true;
            base.characterDirection.enabled = true;

            minionController.isInClingState = false;

            gameObject.layer = LayerIndex.defaultLayer.intVal;
            characterMotor.Motor.RebuildCollidableLayers();

            base.PlayAnimation("FullBody, Override", "BufferEmpty");

            //base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            //base.characterMotor.Motor.RebuildCollidableLayers();

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}
