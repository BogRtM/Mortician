using EmotesAPI;
using System;
using System.Collections.Generic;
using System.Text;
using Morris;
using RoR2;
using System.Linq;

namespace Morris.Modules
{
    public class Compat
    {
        public static void DoCustomEmoteCompat()
        {
        }

        private void Subscriptions()
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.weliveinasociety.CustomEmotesAPI"))
            {
                CustomEmotesAPI.animChanged += CustomEmotesAPI_animChanged;
            }
        }

        private void CustomEmotesAPI_animChanged(string newAnimation, BoneMapper mapper)
        {
            if (mapper.mapperBody.bodyIndex == MorrisPlugin.MorrisBodyIndex)
            {
                var minions = CharacterMaster.readOnlyInstancesList.Where(el => el.minionOwnership.ownerMaster == mapper.mapperBody.master);

                foreach (var minion in minions)
                {
                    if (minion.GetBody().bodyIndex == MorrisPlugin.GhoulBodyIndex)
                    {
                        Chat.AddMessage("Getting bone mapper for ghoul");

                        ModelLocator modelLocator = minion.GetBodyObject().GetComponent<ModelLocator>();
                        if (modelLocator)
                        {
                            BoneMapper ghoulMapper = modelLocator.modelTransform.GetComponentInChildren<BoneMapper>();
                            CustomEmotesAPI.PlayAnimation(newAnimation, ghoulMapper);
                        }
                    }
                }
            }
        }
    }
}
