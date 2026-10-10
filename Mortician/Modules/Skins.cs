using RoR2;
using System;
using UnityEngine;

namespace Morris.Modules
{
    internal static class Skins
    {
        internal static SkinDef CreateSkinDef(string skinName, Sprite skinIcon, CharacterModel.RendererInfo[] rendererInfos, SkinnedMeshRenderer mainRenderer, GameObject root)
        {
            SkinDefParams skinDefParams = ScriptableObject.CreateInstance<SkinDefParams>();
            skinDefParams.name = skinName + "_PARAMS";
            skinDefParams.rendererInfos = rendererInfos;

            SkinDef skinDef = ScriptableObject.CreateInstance<SkinDef>();
            skinDef.name = skinName;
            skinDef.nameToken = skinName;
            skinDef.icon = skinIcon;
            skinDef.rootObject = root;
            skinDef.baseSkins = Array.Empty<SkinDef>();
            skinDef.skinDefParams = skinDefParams;

            return skinDef;
        }
    }
}
