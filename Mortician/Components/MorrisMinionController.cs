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
        public GameObject owner { get; private set; }
        public GameObject sacrificeOwner { get; private set; }

        public TeamIndex teamIndex { get; private set; }
        public string soundString;

        public bool sacrificed { get; private set; }

        private EntityStateMachine bodyESM;

        private CharacterBody characterBody;
        private HealthComponent healthComponent;

        public enum MorrisMinionType
        {
            None,
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

            if (characterBody.inventory)
                characterBody.inventory.RemoveItem(RoR2Content.Items.MinionLeash, 1);

            if(characterBody.master)
            {
                MinionOwnership minionOwnership = characterBody.master.GetComponent<MinionOwnership>();

                if (minionOwnership)
                    this.owner = minionOwnership.ownerMaster.GetBodyObject();
            }
        }

        //Launch minion in target direction
        public void Launch(Vector3 direction)
        {
            switch (minionType)
            {
                case MorrisMinionType.Ghoul:
                    var ghoulState = new SkillStates.Ghoul.LaunchedState()
                    {
                        launchVector = direction.normalized
                    };

                    bodyESM.SetInterruptState(ghoulState, InterruptPriority.Pain);
                    break;
                
                case MorrisMinionType.Tombstone:
                    var tombstoneState = new SkillStates.Tombstone.LaunchedState()
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
            if (minionType != MorrisMinionType.Ghoul) return;

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

                if (owner)
                {
                    TombstoneLocator ownerLocator;
                    if (ownerLocator = owner.GetComponent<TombstoneLocator>())
                    {
                        if (ownerLocator.activeTombstone)
                        {
                            ownerLocator.activeTombstone.AddSoulStock();
                        }
                    }
                }
            }
        }
    }
}
