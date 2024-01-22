using RoR2;
using R2API;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;

namespace Morris.Modules
{
    public static class Buffs
    {
        internal static BuffDef exhaustionDebuff;
        public static float exhaustStatReduction = 0.7f;
        public static float exhaustCooldownScale = 1.2f;

        internal static void RegisterBuffs()
        {
            var weakBuff = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Treebot/bdWeak.asset").WaitForCompletion();

            exhaustionDebuff = AddNewBuff(
                "Exhaustion",
                weakBuff.iconSprite,
                new Color(79, 255, 244),
                false,
                true
                );
        }

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            Modules.Content.AddBuffDef(buffDef);

            return buffDef;
        }
    }
}