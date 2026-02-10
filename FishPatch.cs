using BepInEx.Configuration;
using Config;
using DG.Tweening;
using GenUI.MiniGame;
using HarmonyLib;
using MiniGame.Fishing;
using MiniGame.Fishing;
using Sdk;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using TheEntity;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace 学生时代个人插件
{
    [HarmonyPatch(typeof(FishingMiniGameView), "FixedUpdate")]
    public class FishingFixedUpdatePatch
    {
        [HarmonyPrefix]
        public static bool FixedUpdate(FishingMiniGameView __instance, float _deltaTime)
        {
            if (MySAPlugin.AutoFishing.Value)
            {
                var fillamountAddSpeed = (float)AccessTools.Field(typeof(FishingMiniGameView), "fillamountAddSpeed").GetValue(__instance);
                var icon_progress = (UISprite)AccessTools.Field(typeof(FishingMiniGameView), "icon_progress").GetValue(__instance);
                icon_progress.image.fillAmount += fillamountAddSpeed * 2;
            }
            return true;
        }
    }
}
