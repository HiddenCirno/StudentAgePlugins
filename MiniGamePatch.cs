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
    [HarmonyPatch(typeof(GlobalMgr), "HasMinigamePlayed")]
    public class HasMinigamePlayedPatch
    {
        [HarmonyPostfix]
        public static void HasMinigamePlayed(ref bool __result)
        {
            //恢复上限没找到倒是TM找到这个了……顺手的事
            if (MySAPlugin.SkipAllMiniGame.Value==true)
            {
                __result = true;
            }
        }
    }
}
