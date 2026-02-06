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
    [HarmonyPatch(typeof(StudyData), "GetNextStrategyBtnCost")]
    public class GetNextStrategyBtnCostPatch
    {
        [HarmonyPrefix]
        public static bool GetNextStrategyBtnCost(StudyData __instance, int _id, int _cnt, ref float __result)
        {
            if (MySAPlugin.StayStudyCost.Value)
            {
                StudyStrategyCostCfg studyStrategyCostCfg = Cfg.StudyStrategyCostCfgMap[Mathf.Min(1, Cfg.StudyStrategyCostCfgMap.Count)];
                __result = _id switch
                {
                    9930 => studyStrategyCostCfg.costZuanyan,
                    9931 => studyStrategyCostCfg.costYuedu,
                    9932 => studyStrategyCostCfg.costZheli,
                    _ => 0f,
                };
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    [HarmonyPatch(typeof(StudyData), "GetStudyCost")]
    public class GetStudyCostPatch
    {
        [HarmonyPrefix]
        public static bool GetStudyCost(StudyData __instance, bool _isStudy, int _cnt, ref (float energy, float mood) __result)
        {
            if (MySAPlugin.StayStudyCost.Value)
            {
                float num = 0f;
                float num2 = 0f;
                float value = Singleton<RoleMgr>.Ins.GetRole().IncCtrl.GetValue(RoleIncType.OtherAttrInc, 2125);
                for (int i = 0; i < _cnt; i++)
                {
                    num += Cfg.PersonConstCfgMap[1003].value;
                    if (!_isStudy || __instance.GetEnableFreeStudyCnt() == 0)
                    {
                        num2 += Cfg.StudyMethodIncresseCfgMap[Mathf.Min(1, Cfg.StudyMethodIncresseCfgMap.Count)].cost[Singleton<RoleMgr>.Ins.GetRole().Grade - 1] + value;
                    }
                }
                __result = (energy: num, mood: Mathf.Max(0f, num2));
                return false;
            }
            else
            {
                return true;
            }

        }
    }
}
