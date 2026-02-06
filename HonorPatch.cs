using BepInEx.Configuration;
using Config;
using GenUI.Main;
using HarmonyLib;
using MiniGame.Exam;
using Sdk;
using Sdk.PlatformAPI;
using System;
using System.Collections.Generic;
using System.Reflection;
using View.Main;

namespace 学生时代个人插件
{
    [HarmonyPatch(typeof(GlobalMgr), "GetHonor")]
    public class GetHonorPatch
    {
        // 用于修改 GetHonor 方法的行为
        [HarmonyPrefix]
        public static bool GetHonor(GlobalMgr __instance, ref float __result)
        {
            return true;
            // 强制荣誉值返回一个固定值，比如 1000
            __result = 100000f;
            //Console.WriteLine($"GetHonor: {__result}");
            return false; // 返回 false 以阻止原始方法执行
        }
    }
    [HarmonyPatch(typeof(HonorShopView), "OnRender")]
    public class OnRenderPatch
    {
        // 用于修改 GetHonor 方法的行为
        [HarmonyPrefix]
        public static bool OnRender(HonorShopView __instance, UICell _cell)
        {
            if (MySAPlugin.FreeHonorShop.Value)
            {

                Cell_HonorShopItemUI cell_HonorShopItemUI = _cell as Cell_HonorShopItemUI;
                HonorShopCfg honorShopCfg = Cfg.HonorShopCfgMap[(int)cell_HonorShopItemUI.data];
                cell_HonorShopItemUI.txt_desc.text = honorShopCfg.desc;
                cell_HonorShopItemUI.txt_name.text = honorShopCfg.name;
                if (honorShopCfg.dlc > 0 && !Platform.Current.HasDLC(honorShopCfg.dlc - 1))
                {
                    cell_HonorShopItemUI.btn_dlc.gameObject.SetActive(true);
                    cell_HonorShopItemUI.btn_buy.gameObject.SetActive(false);
                    cell_HonorShopItemUI.btn_active.gameObject.SetActive(false);
                    return false;
                }
                cell_HonorShopItemUI.btn_dlc.gameObject.SetActive(false);
                int honorState = Singleton<GlobalMgr>.Ins.GetHonorState(honorShopCfg.id);
                cell_HonorShopItemUI.btn_buy.gameObject.SetActive(honorState == 0);
                if (honorShopCfg.cost < 0)
                {
                    cell_HonorShopItemUI.txt_buy.text = DescCtrl.GetTxt(187);
                    cell_HonorShopItemUI.btn_buy.interactable = false;
                }
                else
                {
                    cell_HonorShopItemUI.txt_buy.text = DescCtrl.GetTxt<int>(626, new int[]
                    {
                    0
                    });
                    cell_HonorShopItemUI.btn_buy.interactable = (Singleton<GlobalMgr>.Ins.GetHonor() >= 0f);
                }
                cell_HonorShopItemUI.btn_active.gameObject.SetActive(honorState > 0);
                if (honorState == 2)
                {
                    cell_HonorShopItemUI.icon_toggle.SetAtlasUrl("shiyanshi/img_toggle_open", true);
                    return false;
                }
                cell_HonorShopItemUI.icon_toggle.SetAtlasUrl("shiyanshi/img_toggle_close", true);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    [HarmonyPatch(typeof(GlobalMgr), "UnlockHonor")]
    public class UnlockHonorPatch
    {
        // 用于修改 GetHonor 方法的行为
        [HarmonyPrefix]
        public static bool UnlockHonor(GlobalMgr __instance, int _id)
        {
            if (MySAPlugin.FreeHonorShop.Value)
            {
                var model = (GlobalModel)AccessTools.Field(typeof(GlobalMgr), "model").GetValue(__instance);
                HonorShopCfg honorShopCfg = Cfg.HonorShopCfgMap[_id];
                if (model.honor >= 0)
                {
                    if (model.honors == null)
                    {
                        model.honors = new Dictionary<int, GlobalHonorData>();
                    }
                    else if (model.honors.ContainsKey(_id))
                    {
                        return false;
                    }

                    __instance.UpdateHonor(0);
                    model.honors.Add(_id, new GlobalHonorData
                    {
                        activated = false,
                        version = 1
                    });
                    __instance.ActivateHonor(_id);
                    EventMgr.Send(3);
                    __instance.CheckAchievement(999, 1);
                }
                return false; // 返回 false 以阻止原始方法执行
            }
            else
            {
                return true;
            }
        }
    }
}
