using BepInEx.Configuration;
using Config;
using HarmonyLib;
using Sdk;
using System;
using System.Reflection;
using TheEntity;
using UnityEngine;

namespace 学生时代个人插件
{
    [HarmonyPatch(typeof(BagMgr), "GetReadBookCost")]
    public class GetReadBookCostPatch
    {
        [HarmonyPostfix]
        public static void GetReadBookCost(BagMgr __instance, ref (float cost, float basic, float rate) __result)
        {
            //这玩意儿为啥会定义在背包管理器....
            if (MySAPlugin.StayReadBookCost.Value)
            {
                //Type bagModelType = Type.GetType("BagModel, Assembly-CSharp");
                ReadBookCostCfg readBookCostCfg = Cfg.ReadBookCostCfgMap[Mathf.Min(1, Cfg.ReadBookCostCfgMap.Count)];
                __result.cost = readBookCostCfg.cost;
            }
        }
    }
}
