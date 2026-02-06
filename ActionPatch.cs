using BepInEx.Configuration;
using Config;
using HarmonyLib;
using Sdk;
using System;
using System.Collections.Generic;
using System.Reflection;
using TheEntity;

namespace 学生时代个人插件
{
    [HarmonyPatch(typeof(ActionData), "GetActionCost")]
    public class GetActionCostPatch
    {
        [HarmonyPrefix]
        public static bool GetActionCost(ActionData __instance, int _id, ref Dictionary<int, float> __result)
        {
            //方法不对, cost计算不对, 见鬼了
            ActionCfg actionCfg = Cfg.ActionCfgMap[_id];
            List<List<float>> cost = actionCfg.cost;
            Role role = Singleton<RoleMgr>.Ins.GetRole();
            Dictionary<int, float> dictionary = new Dictionary<int, float>();
            float value = role.IncCtrl.GetValue(RoleIncType.ActionCostMoneyInc, _id);
            float value2 = role.IncCtrl.GetValue(RoleIncType.ActionCostEnergyInc, _id);
            float value3 = role.IncCtrl.GetValue(RoleIncType.ActionCostEnergyMul, _id);
            Console.WriteLine($"ActionCostMoneyInc: {value}");
            Console.WriteLine($"ActionCostEnergyInc: {value2}");
            Console.WriteLine($"ActionCostEnergyMul: {value3}");
            float num = 0f;
            switch (_id)
            {
                case 3191:
                case 3192:
                    cost.Clear();
                    if (Cfg.HomeworkCfgMap.ContainsKey(role.Grade))
                    {
                        HomeworkCfg homeworkCfg = Cfg.HomeworkCfgMap[role.Grade];
                        cost.Add(new List<float>
                {
                    7f,
                    homeworkCfg.effect[0]
                });
                        if (homeworkCfg.effect.Count > 1 && homeworkCfg.effect[1] > 0f)
                        {
                            cost.Add(new List<float>
                    {
                        0f,
                        homeworkCfg.effect[1]
                    });
                        }

                        num = role.IncCtrl.GetValue(RoleIncType.OtherAttrMul, 2121);
                    }

                    break;
                case 9930:
                case 9931:
                case 9932:
                    {
                        cost.Clear();
                        (int, int) actionCnt = __instance.GetActionCnt(_id);
                        float nextStrategyBtnCost = Singleton<RoleMgr>.Ins.GetStudyData().GetNextStrategyBtnCost(_id, actionCnt.Item2 + 1);
                        cost.Add(new List<float> { 303f, nextStrategyBtnCost });
                        break;
                    }
                default:
                    if (actionCfg.type == 4)
                    {
                        num = (float)__instance.doJobCntThisRound * 0.6f;
                    }

                    break;
            }

            foreach (List<float> item in cost)
            {
                float num2 = item[1];
                switch ((int)item[0])
                {
                    case 30:
                        num2 += value;
                        break;
                    case 7:
                        num2 = (num2 + value2) * (1f + value3);
                        break;
                    case 0:
                        num2 *= 1f + num;
                        break;
                }

                dictionary.Add((int)item[0], num2);
            }
            //不对啊, 我TM遍历字典啊
            //洗完澡再说
            foreach(var kvp in dictionary)
            {
                Console.WriteLine($"key: {kvp.Key}: Value: {kvp.Value}");
                //搞定
                //30是钱7是精力
            }
            if (dictionary.TryGetValue(7, out var energy))
            {
                switch (energy)
                {
                    case 20:
                        dictionary[7] = 10 + 1*MySAPlugin.ActionEnergyCostMutiper.Value;
                        break;
                    case 30:
                        dictionary[7] = 10 + 2 * MySAPlugin.ActionEnergyCostMutiper.Value;
                        break;
                    case 40:
                        dictionary[7] = 10 + 3 * MySAPlugin.ActionEnergyCostMutiper.Value;
                        break;
                }
            }
            if (MySAPlugin.RevertMoneyCost.Value)
            {
                if (dictionary.TryGetValue(30, out var money))
                {
                    var newmoney = Math.Abs(money);
                    dictionary[30] = -newmoney;
                }
            }
            __result = dictionary;

            return false;
        }
    }
}
