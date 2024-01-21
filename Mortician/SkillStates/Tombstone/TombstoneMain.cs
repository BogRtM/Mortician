using UnityEngine;
using RoR2;
using EntityStates;
using Morris.Modules.NPC;
using Morris.Components;
using Morris;
using UnityEngine.Networking;

namespace SkillStates.Tombstone
{
    internal class TombstoneMain : GenericCharacterMain
    {
        private MorrisMinionController minionController;

        private float summonTimer;

        public override void OnEnter()
        {
            base.OnEnter();
            minionController = base.GetComponent<MorrisMinionController>();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void Update()
        {
            if (base.characterMotor.isGrounded)
            {
                base.characterMotor.velocity = Vector3.zero;
                base.characterMotor.rootMotion = Vector3.zero;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
