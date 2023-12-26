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
        //internal static BuffDef MorrisBuff;

        internal static void RegisterBuffs()
        {
            
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