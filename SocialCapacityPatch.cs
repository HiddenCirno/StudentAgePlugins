using BepInEx.Configuration;
using GenUI.Common;
using HarmonyLib;
using Sdk;
using System;
using System.Collections.Generic;
using System.Reflection;
using TheEntity;
using UnityEngine;
using View.Common;

namespace 学生时代个人插件
{
    [HarmonyPatch(typeof(RelationData), "GetSocialCapacity")]
    public class GetSocialCapacityPatch
    {
        [HarmonyPostfix]
        public static void GetSocialCapacity(ref int __result)
        {
            //恢复上限没找到倒是TM找到这个了……顺手的事
            if (MySAPlugin.InfinitySocialCapacity.Value==true)
            {
                __result = int.MaxValue;
            }
        }
    }
}
