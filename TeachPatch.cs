using BepInEx.Configuration;
using Config;
using HarmonyLib;
using MiniGame.Exam;
using Sdk;
using System;
using System.Reflection;
using System.Xml.Linq;
using TheEntity;

namespace 学生时代个人插件
{
    [HarmonyPatch(typeof(Role), "Teachable")]
    public class TeachablePatch
    {
        [HarmonyPrefix]
        public static bool Teachable(Role __instance, ref bool __result)
        {
            if (MySAPlugin.StayTeachCost.Value)
            {
                if (Cfg.TeachNpcCfgMap.TryGetValue(__instance.TeachCnt + 1, out var teachNpcCfg) &&Cfg.TeachNpcCfgMap.TryGetValue(1, out var cost1))
                {
                    float rate = 1f + Singleton<RoleMgr>.Ins.GetRole().IncCtrl.GetValue(RoleIncType.OtherAttrInc, 2136);
                    __result = __instance.Favor >= (float)teachNpcCfg.need && Singleton<RoleMgr>.Ins.HasEnoughCost(cost1.cost, rate);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
    }
    [HarmonyPatch(typeof(Role), "Teach")]
    public class TeachPatch
    {
        [HarmonyPrefix]
        public static bool Teach(Role __instance, int _studentId = 0, int _bgId = 0)
        {
            if (MySAPlugin.StayTeachCost.Value)
            {
                if (!Cfg.TeachNpcCfgMap.TryGetValue(1, out var cfg))
                {
                    return false;
                }

                float rate = 1f + Singleton<RoleMgr>.Ins.GetRole().IncCtrl.GetValue(RoleIncType.OtherAttrInc, 2136);
                if (Singleton<RoleMgr>.Ins.Cost(cfg.cost, null, _toast: true, rate))
                {
                    //哥们你他妈怎么没反射到卧槽
                    var TeachCntProperty = AccessTools.Property(typeof(Role), "TeachCnt");
                    var TeachCntValue = __instance.TeachCnt;
                    TeachCntProperty.SetValue(__instance, TeachCntValue + 1);
                    //TeachCnt.SetValue(__instance, TeachCntValue + 1);
                    //__instance.TeachCnt++;
                    (int attrId, float value) attr = __instance.GetMaxAttr(_allowSame: true);
                    //Singleton<RoleMgr>.Ins.GetRole(_studentId).TotalTeachCnt++;
                    var student = Singleton<RoleMgr>.Ins.GetRole(_studentId);

                    var TotalTeachCntProperty = AccessTools.Property(typeof(Role), "TotalTeachCnt");
                    var TotalTeachCntValue = student.TotalTeachCnt;
                    TotalTeachCntProperty.SetValue(student, TotalTeachCntValue + 1);
                    string txt = DescCtrl.GetTxt<string>(891, __instance.Name + Cfg.FuncCfgMap[310].name);
                    Action closeCallback = delegate
                    {
                        Singleton<RoleMgr>.Ins.GetRole(_studentId).UpdateAttr(attr.attrId, cfg.add * rate);
                    };
                    int bgId = _bgId;
                    HintHelper.ShowLoadingResult(txt, null, closeCallback, null, null, null, 0, __instance.id, bgId, 1, 10);
                    EventMgr.Send(1603);
                    Singleton<RoleMgr>.Ins.UpdateConditionData(7, 310, 1f);
                }
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
