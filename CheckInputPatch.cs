using BepInEx.Configuration;
using Config;
using DG.Tweening;
using GenUI.MiniGame;
using HarmonyLib;
using MiniGame.Exam;
using Sdk;
using Sdk.PlatformAPI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using TheEntity;
using UnityEngine;

namespace 学生时代个人插件
{
    [HarmonyPatch(typeof(Utils), "CheckInput", new Type[] { typeof(string), typeof(bool) })]
    public class CheckInputPatch
    {
        [HarmonyPrefix]
        public static bool CheckInput(string _txt, bool _toast, ref string __result)
        {
            //莫名其妙. 单机搞啥屏蔽词嘞?
            //屏蔽就算了世界俩字招你惹你了....杀!
            __result = _txt;
            return false;
        }
    }
    [HarmonyPatch(typeof(BasePlatform), "FilterDirtyWords")]
    public class FilterDirtyWordsPatch
    {
        [HarmonyPrefix]
        public static bool FilterDirtyWords(string _words, ref string __result)
        {
            //莫名其妙. 单机搞啥屏蔽词嘞?
            //屏蔽就算了世界俩字招你惹你了....杀!
            __result = _words;
            return false;
        }
    }
}
