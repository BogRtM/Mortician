using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using EntityStates;
using UnityEngine;
using Skillstates.Ghoul;
using RoR2.UI;
using UnityEngine.Networking;

namespace Morris.Components
{
    internal class MorrisMinionController : MonoBehaviour, ILifeBehavior
    {
        public GameObject owner { get; set; }

        private EntityStateMachine bodyESM;

        public TeamIndex teamIndex;

        public string hitboxGroupName;
        public string soundString;

        public bool sacrificed { get; private set; }

        private CharacterBody characterBody;
        private HealthComponent healthComponent;

        private void Start()
        {
            bodyESM = EntityStateMachine.FindByCustomName(base.gameObject, "Body");
            teamIndex = base.GetComponent<TeamComponent>().teamIndex;
            characterBody = base.GetComponent<CharacterBody>();
            healthComponent = base.GetComponent<HealthComponent>();
        }

        public void Launch(Vector3 direction)
        {
            var nextState = new LaunchedState()
            {
                launchVector = direction.normalized,
                hitboxGroupName = this.hitboxGroupName
            };

            bodyESM.SetInterruptState(nextState, InterruptPriority.Pain);
        }

        public void Sacrifice()
        {
            sacrificed = true;

            this.healthComponent.Suicide();
        }

        public void OnDeathStart()
        {
            if (characterBody.bodyIndex != MorrisPlugin.GhoulBodyIndex) return;

            if (NetworkServer.active)
            {
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.attacker = owner;
                damageInfo.damage = 0;
                damageInfo.position = base.transform.position;

                DamageReport damageReport = new DamageReport(damageInfo, healthComponent, damageInfo.damage, healthComponent.combinedHealth);
                GlobalEventManager.instance.OnCharacterDeath(damageReport);
            }
        }
    }
}
