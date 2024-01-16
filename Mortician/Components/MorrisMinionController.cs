using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using EntityStates;
using UnityEngine;
using SkillStates.Ghoul;
using SkillStates.Tombstone;
using RoR2.UI;
using UnityEngine.Networking;

namespace Morris.Components
{
    internal class MorrisMinionController : MonoBehaviour, ILifeBehavior
    {
        public GameObject owner { get; set; }
        public GameObject sacrificeOwner { get; private set; }

        private EntityStateMachine bodyESM;

        public TeamIndex teamIndex { get; private set; }
        public string soundString;

        public bool sacrificed { get; private set; }

        private CharacterBody characterBody;
        private HealthComponent healthComponent;

        public enum MorrisMinionType
        {
            Ghoul,
            Tombstone
        }

        public MorrisMinionType minionType;

        private void Start()
        {
            bodyESM = EntityStateMachine.FindByCustomName(base.gameObject, "Body");
            teamIndex = base.GetComponent<TeamComponent>().teamIndex;
            characterBody = base.GetComponent<CharacterBody>();
            healthComponent = base.GetComponent<HealthComponent>();
        }

        public void Launch(Vector3 direction)
        {
            switch (minionType)
            {
                case MorrisMinionType.Ghoul:
                    var ghoulState = new GhoulLaunchedState()
                    {
                        launchVector = direction.normalized
                    };

                    bodyESM.SetInterruptState(ghoulState, InterruptPriority.Pain);
                    break;

                case MorrisMinionType.Tombstone:
                    var tombstoneState = new TombstoneLaunchedState()
                    {
                        launchVector = direction.normalized
                    };

                    bodyESM.SetInterruptState(tombstoneState, InterruptPriority.Pain);
                    break;

                default:
                    break;
            }
        }

        public void Sacrifice(GameObject sacrificer)
        {
            sacrificed = true;
            sacrificeOwner = sacrificer;

            this.healthComponent.Suicide();
        }

        public void OnDeathStart()
        {
            if (characterBody.bodyIndex != MorrisPlugin.GhoulBodyIndex || !owner) return;

            if (NetworkServer.active)
            {
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.attacker = owner;
                damageInfo.crit = false;
                damageInfo.damage = 0f;
                damageInfo.position = base.transform.position;
                damageInfo.procCoefficient = 1f;
                damageInfo.damageType = DamageType.Generic;
                damageInfo.damageColorIndex = DamageColorIndex.Default;

                DamageReport damageReport = new DamageReport(damageInfo, healthComponent, damageInfo.damage, healthComponent.combinedHealth);
                GlobalEventManager.instance.OnCharacterDeath(damageReport);
            }
        }
    }
}
