using BepInEx.Configuration;
using Config;
using DG.Tweening;
using GenUI.MiniGame;
using HarmonyLib;
using MiniGame.Exam;
using MiniGame.Fight;
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
    [HarmonyPatch(typeof(FightPlayer), "BePunch")]
    public class BePunchPatch
    {
        [HarmonyPrefix]
        public static bool BePunch(FightPlayer __instance, float _dmgPunch)
        {
            if(__instance.playerType == PlayerType.Me && MySAPlugin.FightMiniGameInfinityHealth.Value)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    
}
