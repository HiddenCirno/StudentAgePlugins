using BepInEx.Configuration;
using Config;
using HarmonyLib;
using MiniGame.Exam;
using Sdk;
using System;
using System.Diagnostics;
using System.Reflection;
using TheEntity;
using UnityEngine;

namespace 学生时代个人插件
{
    [HarmonyPatch(typeof(ShopMgr), "Buy", new Type[] { typeof(int) })]
    public class BuyPatch
    {
        [HarmonyPrefix]
        public static bool Buy(ShopMgr __instance, int _shopId, ref bool __result)
        {
            float attr = Singleton<RoleMgr>.Ins.GetRole().GetAttr(30);
            ShopCfg shopCfg = Cfg.ShopCfgMap[_shopId];
            float price = shopCfg.price;
            if (attr < price && !MySAPlugin.RevertMoneyCost.Value)
            {
                __result = false;
                return false;
            }

            if (Singleton<BagMgr>.Ins.HasBook(_shopId))
            {
                ToastHelper.Toast(DescCtrl.GetTxt(544));
                __result = false;
                return false;
            }

            if ((shopCfg.type == 2 || shopCfg.type == 4) && Singleton<BagMgr>.Ins.HasEnoughItem(_shopId))
            {
                ToastHelper.Toast(DescCtrl.GetTxt(544));
                __result = false;
                return false;
            }

            //__instance.RecordBuyItem(_shopId);
            AccessTools.Method(typeof(ShopMgr), "RecordBuyItem").Invoke(__instance, new object[] { _shopId });
            Singleton<RoleMgr>.Ins.GetRole().UpdateAttr(30, MySAPlugin.RevertMoneyCost.Value ? 0f + price : 0f - price, 1f, DescCtrl.GetTxt(10029));
            var model = (ShopModel)AccessTools.Field(typeof(ShopMgr), "model").GetValue(__instance);
            model.totalCost += price;
            if (shopCfg.type == 3)
            {
                Singleton<BagMgr>.Ins.AddBook(shopCfg.id, DescCtrl.GetTxt(10029));
            }
            else if (shopCfg.type != 7)
            {
                Singleton<BagMgr>.Ins.AddItem(shopCfg.id, 1L, DescCtrl.GetTxt(10029));
            }

            AudioMgrEx.PlayUISound(42009);
            EventMgr.Send(1002, shopCfg.id);
            __result = true;
            return false;
        }
    }
    [HarmonyPatch(typeof(ShopMgr), "Buy", new Type[] { typeof(ShopData), typeof(float) })]
    public class Buy2Patch
    {
        [HarmonyPrefix]
        public static bool Buy(ShopMgr __instance, ShopData _shopData, float _discount, ref bool __result)
        {
            float attr = Singleton<RoleMgr>.Ins.GetRole().GetAttr(30);
            float num = _shopData.FinalPrize * _discount;
            if ((attr < num && !MySAPlugin.RevertMoneyCost.Value) || _shopData.cnt <= 0)
            {
                __result = false;
                return false;
            }

            if (Singleton<BagMgr>.Ins.HasBook(_shopData.id))
            {
                ToastHelper.Toast(DescCtrl.GetTxt(544));
                __result = false;
                return false;
            }

            if ((_shopData.type == 2 || _shopData.type == 4) && Singleton<BagMgr>.Ins.HasEnoughItem(_shopData.id))
            {
                ToastHelper.Toast(DescCtrl.GetTxt(544));
                __result = false;
                return false;
            }

            //RecordBuyItem(_shopData.id);
            AccessTools.Method(typeof(ShopMgr), "RecordBuyItem").Invoke(__instance, new object[] { _shopData.id });
            ShopCfg shopCfg = Cfg.ShopCfgMap[_shopData.id];
            Singleton<RoleMgr>.Ins.GetRole().UpdateAttr(30, MySAPlugin.RevertMoneyCost.Value ? 0f + num : 0f - num, 1f, DescCtrl.GetTxt(10029));
            var model = (ShopModel)AccessTools.Field(typeof(ShopMgr), "model").GetValue(__instance);
            model.totalCost += num;
            _shopData.discount = 1f;
            if (shopCfg.type == 3)
            {
                _shopData.cnt--;
                Singleton<BagMgr>.Ins.AddBook(shopCfg.id, DescCtrl.GetTxt(10029));
            }
            else if (shopCfg.type != 7)
            {
                _shopData.cnt--;
                Singleton<BagMgr>.Ins.AddItem(shopCfg.id, 1L, DescCtrl.GetTxt(10029));
            }

            if (_shopData.cnt <= 0)
            {
                __instance.AddShopItem(shopCfg.next, _canRemove: false);
            }

            AudioMgrEx.PlayUISound(42009);
            EventMgr.Send(1002, shopCfg.id);
            __result = true;
            return false;
        }
    }
}
