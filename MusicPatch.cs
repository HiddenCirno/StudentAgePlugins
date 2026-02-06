using BepInEx.Configuration;
using Config;
using DG.Tweening;
using GenUI.Minigame;
using HarmonyLib;
using MiniGame.Exam;
using MiniGame.Music;
using Sdk;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using TheEntity;
using UnityEngine;

namespace 学生时代个人插件
{
    [HarmonyPatch(typeof(MusicMiniGameView), "Check")]
    public class CheckPatch
    {
        [HarmonyPrefix]
        public static bool Check(MusicMiniGameView __instance, float _delta, Cell_MusicItemUI _cell, int _group)
        {

            var combo = AccessTools.Field(typeof(MusicMiniGameView), "combo");
            var comboValue = (int)combo.GetValue(__instance);
            var maxCombo = AccessTools.Field(typeof(MusicMiniGameView), "maxCombo");
            var maxComboValue = (int)maxCombo.GetValue(__instance);
            var score = AccessTools.Field(typeof(MusicMiniGameView), "score");
            var scoreValue = (uint)score.GetValue(__instance);
            var maxScore = (int)AccessTools.Field(typeof(MusicMiniGameView), "maxScore").GetValue(__instance);
            var vfxNames = (string[])AccessTools.Field(typeof(MusicMiniGameView), "vfxNames").GetValue(__instance);
            var keyXforms = (Transform[])AccessTools.Field(typeof(MusicMiniGameView), "keyXforms").GetValue(__instance);
            var clapsIds = (int[])AccessTools.Field(typeof(MusicMiniGameView), "clapsIds").GetValue(__instance);
            var scores = (uint[])AccessTools.Field(typeof(MusicMiniGameView), "scores").GetValue(__instance);
            var line = 150f;
            float num = _delta / (line *= (float)(1+ MySAPlugin.EasyMusicMiniGame.Value));
            ScoreType scoreType = ((num <= 0.25f) ? ScoreType.Perfect : ((num <= 0.5f) ? ScoreType.Great : ((num <= 0.75f) ? ScoreType.Good : ((num <= 1f) ? ScoreType.Normal : ScoreType.Miss))));
            if (scoreType != 0)
            {
                //__instance.combo++;
                combo.SetValue(__instance, comboValue + 1);
                //__instance.maxCombo = Mathf.Max(combo, maxCombo);
                maxCombo.SetValue(__instance, Math.Max(comboValue, maxComboValue));
                __instance.txtex_combo.text = $"x{combo}";
                __instance.txtex_combo.DOKill();
                __instance.txtex_combo.rectTransform.localScale = Vector3.one;
                __instance.txtex_combo.rectTransform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.1f, 1);
                __instance.group_combo.gameObject.SetActive(value: true);
                VFXMgr.Get(vfxNames[(int)(scoreType - 1)], delegate (ParticleSystem _item)
                {
                    ((Component)(object)_item).transform.SetParent(__instance.group_effect);
                    ((Component)(object)_item).transform.localScale = Vector3.one;
                    RectTransform orAddComponent = ((Component)(object)_item).transform.GetOrAddComponent<RectTransform>();
                    Vector2 vector = UIMgr.WorldPointToLocalPoint(keyXforms[_group].position, __instance.group_effect);
                    orAddComponent.anchoredPosition3D = new Vector3(vector.x, vector.y, -100f);
                }, _autoRecycle: false, UIMgr.GetLayerSortingOrder(__instance.layerType));
                AudioMgrEx.PlayUISound(clapsIds[UnityEngine.Random.Range(0, clapsIds.Length)]);
            }
            else
            {
                //__instance.maxCombo = Mathf.Max(__instance.combo, __instance.maxCombo);
                maxCombo.SetValue(__instance, Math.Max(comboValue, maxComboValue));
                //__instance.combo = 0;
                combo.SetValue(__instance, 0);
                __instance.group_combo.gameObject.SetActive(value: false);
            }

            uint num2 = scores[(int)scoreType];
            //__instance.score += num2;
            score.SetValue(__instance, scoreValue + num2);
            __instance.txt_score.text = scoreValue.ToString();
            __instance.img_progress.fillAmount = ((float)scoreValue + 0f) / (float)maxScore;
            //__instance.Fly(scoreType, _group);
            AccessTools.Method(typeof(MusicMiniGameView), "Fly").Invoke(__instance, new object[] { scoreType, _group });

            return false;
        }
    }
}
