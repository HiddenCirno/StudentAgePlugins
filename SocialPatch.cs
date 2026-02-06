using BepInEx.Configuration;
using Config;
using HarmonyLib;
using Sdk;
using System;
using System.Reflection;
using TheEntity;

namespace 学生时代个人插件
{
    [HarmonyPatch(typeof(ActionData), "GetSocialCost")]
    public class GetSocialCostPatch
    {
        [HarmonyPrefix]
        public static bool GetSocialCost(ActionData __instance, int _npcId, ref float __result)
        {
            if (MySAPlugin.StaySocialCost.Value)
            {
                int npcSocialCnt = __instance.GetNpcSocialCnt(_npcId);
                SocialCostCfg socialCostCfg = null;
                if (!Cfg.SocialCostCfgMap.TryGetValue(1, out socialCostCfg))
                {
                    socialCostCfg = Cfg.SocialCostCfgMap[Cfg.SocialCostCfgMap.Count];
                }
                if (Singleton<RoleMgr>.Ins.GetRole().IsUnlock(9037))
                {
                    __result = socialCostCfg.cost2;
                }
                __result = socialCostCfg.cost;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
