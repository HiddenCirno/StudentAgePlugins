using BepInEx.Configuration;
using Config;
using DG.Tweening;
using GenUI.MiniGame;
using HarmonyLib;
using MiniGame.Exam;
using MiniGame.Qte;
using Sdk;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using TheEntity;
using UnityEngine;
using UnityEngine.UI;

namespace 学生时代个人插件
{
    [HarmonyPatch(typeof(QteMiniGameView), "FixedUpdate")]
    public class FixedUpdatePatch
    {
        [HarmonyPrefix]
        public static bool FixedUpdate(QteMiniGameView __instance, float _deltaTime)
        {

            var isClick = (bool)AccessTools.Field(typeof(QteMiniGameView), "isClick").GetValue(__instance);
            var moveSpeed = (float)AccessTools.Field(typeof(QteMiniGameView), "moveSpeed").GetValue(__instance);
            var direction = AccessTools.Field(typeof(QteMiniGameView), "direction");
            var directionValue = (int)direction.GetValue(__instance);
            if (!isClick)
            {
                Vector2 anchoredPosition = __instance.slider.anchoredPosition;
                anchoredPosition.x += (float)directionValue * moveSpeed * MySAPlugin.QteMiniGameSpeed.Value;
                __instance.slider.anchoredPosition = anchoredPosition;
                if (anchoredPosition.x >= 740f)
                {
                    direction.SetValue(__instance, -1);
                }
                else if (anchoredPosition.x <= 0f)
                {
                    direction.SetValue(__instance, 1);
                }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(Qte2MiniGameView), "FixedUpdate")]
    public class FixedUpdate2Patch
    {
        [HarmonyPrefix]
        public static bool FixedUpdate(Qte2MiniGameView __instance, float _deltaTime)
        {

            var isClicking = (bool)AccessTools.Field(typeof(Qte2MiniGameView), "isClicking").GetValue(__instance);
            var state = (State)AccessTools.Field(typeof(Qte2MiniGameView), "state").GetValue(__instance);
            var moveSpeed = (float)AccessTools.Field(typeof(Qte2MiniGameView), "moveSpeed").GetValue(__instance);
            var direction = AccessTools.Field(typeof(Qte2MiniGameView), "direction");
            var directionValue = (int)direction.GetValue(__instance);
            var successCnt = AccessTools.Field(typeof(Qte2MiniGameView), "successCnt");
            var SetState = AccessTools.Method(typeof(Qte2MiniGameView), "SetState");
            var successCntValue = (int)direction.GetValue(__instance);
            if (state != State.Running || isClicking)
            {
                return false;
            }

            Vector2 anchoredPosition = __instance.slider.anchoredPosition;
            anchoredPosition.x += (float)directionValue * moveSpeed * MySAPlugin.QteMiniGameSpeed.Value;
            __instance.slider.anchoredPosition = anchoredPosition;
            if (anchoredPosition.x >= __instance.bg.rect.width)
            {
                direction.SetValue(__instance, -1);
            }
            else if (anchoredPosition.x <= 0f)
            {
                direction.SetValue(__instance, 1);
            }

            if (DebugMgr.Ins.enableDebug)
            {
                if (Control.GetKeyDown((UnityEngine.InputSystem.Key)94))
                {
                    successCnt.SetValue(__instance, 3);
                    SetState.Invoke(__instance, new object[] { State.End });
                }
                else if (Control.GetKeyDown((UnityEngine.InputSystem.Key)95))
                {
                    successCnt.SetValue(__instance, 0);
                    SetState.Invoke(__instance, new object[] { State.End });
                }
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(Role), "GetUnlockValue")]
    public class GetUnlockValuePatch
    {
        [HarmonyPostfix]
        public static void GetUnlockValues(Role __instance, int _id, bool _max, ref float __result)
        {
            if(_id == 9022)
            {
                __result += (float)MySAPlugin.QteMiniGameCostMutiper.Value;
            }
        }
    }
}
