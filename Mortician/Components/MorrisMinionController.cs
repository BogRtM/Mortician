using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using RoR2.CharacterAI;
using EntityStates;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using SkillStates.Ghoul;
using SkillStates.Tombstone;
using RoR2.UI;
using Morris.Modules;
using Morris.Modules.Survivors;
using Morris.Modules.NPC;

namespace Morris.Components
{
    internal class MorrisMinionController : MonoBehaviour, ILifeBehavior
    {
        public CharacterMaster ownerMaster { get; private set; }
        public GameObject owner { get; private set; }
        public CharacterBody ownerBody { get; private set; }
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

        public bool isInClingState { get; set; }

        public MorrisMinionType minionType;

        private void Start()
        {
            if (minionType == MorrisMinionType.Ghoul)
            {
                base.gameObject.layer = LayerIndex.playerFakeActor.intVal;
            }
            
            bodyESM = EntityStateMachine.FindByCustomName(base.gameObject, "Body");
            teamIndex = base.GetComponent<TeamComponent>().teamIndex;
            characterBody = base.GetComponent<CharacterBody>();
            healthComponent = base.GetComponent<HealthComponent>();

            if (characterBody.inventory && NetworkServer.active)
            {
                characterBody.inventory.RemoveItemPermanent(RoR2Content.Items.MinionLeash, 1);
            }

            if(characterBody.master)
            {
                MinionOwnership minionOwnership = characterBody.master.GetComponent<MinionOwnership>();

                if (minionOwnership && minionOwnership.ownerMaster)
                {
                    this.ownerMaster = minionOwnership.ownerMaster;
                    this.owner = ownerMaster.GetBodyObject();
                    this.ownerBody = ownerMaster.GetBody();

                    ownerMaster.onBodyStart += FollowRespawnedOwner;

                    if (minionType == MorrisMinionType.Ghoul && Config.ghoulLimit.Value > 0 && NetworkServer.active)
                    {
                        var deployable = characterBody.masterObject.GetComponent<Deployable>();
                        deployable.onUndeploy = new UnityEvent();
                        deployable.onUndeploy.AddListener(characterBody.master.TrueKill);
                        minionOwnership.ownerMaster.AddDeployable(deployable, Morris.Modules.Survivors.Morris.ghoulSlot);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (ownerMaster)
            {
                ownerMaster.onBodyStart -= FollowRespawnedOwner;
            }
        }

        private void FollowRespawnedOwner(CharacterBody newOwnerBody)
        {
            this.owner = newOwnerBody.gameObject;
            this.ownerBody = newOwnerBody;

            BaseAI ai = characterBody.master.GetComponent<BaseAI>();

            if (ai && NetworkServer.active)
            {
                ai.leader.gameObject = newOwnerBody.gameObject;
            }
        }

        //Launch minion in target direction
        public void Launch(Vector3 direction)
        {
            switch (minionType)
            {
                case MorrisMinionType.Ghoul:
                    var ghoulState = new SkillStates.Ghoul.GhoulLaunched()
                    {
                        launchVector = direction.normalized
                    };
                    bodyESM.SetInterruptState(ghoulState, InterruptPriority.Pain);
                    break;
                
                case MorrisMinionType.Tombstone:
                    var tombstoneState = new SkillStates.Tombstone.TombstoneLaunched()
                    {
                        launchVector = direction.normalized
                    };
                    bodyESM.SetInterruptState(tombstoneState, InterruptPriority.Pain);
                    break;

                default:
                    break;
            }
        }

        public void Suicide()
        {
            Log.Warning("Undeploying ghoul");

            if (NetworkServer.active)
            {
                this.healthComponent.Suicide();
            }
        }

        public void Sacrifice(GameObject sacrificer)
        {
            sacrificed = true;
            sacrificeOwner = sacrificer;

            if(NetworkServer.active)
            { 
                this.healthComponent.Suicide(); 
            }
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
                GlobalEventManager.instance.TriggerCharacterDeathProc(damageReport);

                if (owner)
                {
                    TombstoneLocator ownerLocator;
                    if (ownerLocator = owner.GetComponent<TombstoneLocator>())
                    {
                        if (ownerLocator.activeTombstone)
                        {
                            ownerLocator.activeTombstone.AddSoulStockServer();
                        }
                    }
                }
            }
        }
    }
}
