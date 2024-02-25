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
            base.Update();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
