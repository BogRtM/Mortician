using UnityEngine;
using RoR2;
using EntityStates;
using Morris.Modules;

namespace SkillStates.Tombstone
{
    internal class SpawnState : BaseState
    {
        public static float duration = 1f;
        public override void OnEnter()
        {
            base.OnEnter();

            Transform modelTransform = base.GetModelTransform();
            TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
            temporaryOverlay.duration = duration * 1.5f;
            temporaryOverlay.animateShaderAlpha = true;
            temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.1f);
            temporaryOverlay.destroyComponentOnEnd = true;
            temporaryOverlay.originalMaterial = Assets.TombstoneSpawnMat;
            temporaryOverlay.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void Update()
        {
            base.Update();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
