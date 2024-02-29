using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using EntityStates.Treebot;
namespace SkillStates.Ghoul
{
    internal class GhoulSpawn : BaseState
    {
        public static GameObject spawnEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/SpawnLemurian.prefab").WaitForCompletion();

        public static float baseDuration = 0.5f;

        public override void OnEnter()
        {
            base.OnEnter();

            if(NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }

            EffectData effectData = new EffectData()
            {
                origin = base.characterBody.footPosition
            };
            EffectManager.SpawnEffect(spawnEffect, effectData, true);

            Util.PlaySound("Play_treeBot_sprint_end", base.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= baseDuration && base.isAuthority) 
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
