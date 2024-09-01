using UnityEngine;
using RoR2;
using EntityStates.SurvivorPod;
using EntityStates;

namespace SkillStates.CoffinPod
{
    internal class Descent : SurvivorPodBaseState
    {
        private const float duration = 4f;

        private ShakeEmitter shakeEmitter;

        public override void OnEnter()
        {
            base.OnEnter();

            Transform modelTransform = base.GetModelTransform();
            if (modelTransform)
            {
                ChildLocator component = modelTransform.GetComponent<ChildLocator>();
                if (component)
                {
                    Transform transform = component.FindChild("Pivot");
                    if (transform)
                    {
                        this.shakeEmitter = transform.gameObject.AddComponent<ShakeEmitter>();
                        this.shakeEmitter.wave = new Wave
                        {
                            amplitude = 1f,
                            frequency = 180f,
                            cycleOffset = 0f
                        };
                        this.shakeEmitter.duration = 10000f;
                        this.shakeEmitter.radius = 400f;
                        this.shakeEmitter.amplitudeTimeDecay = false;
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.isAuthority && base.fixedAge > duration)
            {
                this.outer.SetNextState(new Landed());
            }
        }

        public override void OnExit()
        {
            EntityState.Destroy(shakeEmitter);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
