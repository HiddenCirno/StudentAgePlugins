using BepInEx.Configuration;
using Config;
using DG.Tweening;
using GenUI.MiniGame;
using HarmonyLib;
using MiniGame.Exam;
using Sdk;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using TheEntity;
using UnityEngine;

namespace 学生时代个人插件
{
    [HarmonyPatch(typeof(MiniGame.Exam.ExamMiniGameView), "ShowDice")]
    public class ShowDicePatch
    {
        [HarmonyPrefix]
        public static bool ShowDice(ExamMiniGameView __instance, int _id, Cell_ExamItemUI _cell)
        {
            //恢复上限没找到倒是TM找到这个了……顺手的事
            ExamBrickCfg examBrickCfg = Cfg.ExamBrickCfgMap[_id];
            int score = MySAPlugin.LockDice.Value == true ? 20 : UnityEngine.Random.Range(examBrickCfg.value[0], examBrickCfg.value[1] + 1);
            __instance.group_dice.gameObject.SetActive(true);
            __instance.img_dice.transform.localScale = Vector3.zero;
            __instance.txt_dice.text = "?";
            Sequence s = DOTween.Sequence();
            s.Append(__instance.img_dice.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack));
            s.Append(__instance.img_dice.transform.DOLocalRotate(new Vector3(0f, 0f, -7200f), 2f, RotateMode.WorldAxisAdd).SetEase(Ease.InOutCubic));
            s.InsertCallback(1.5f, delegate
            {
                __instance.txt_dice.text = ((score >= 0) ? string.Format("+{0}", score) : string.Format("{0}", score));
            });
            s.AppendInterval(0.5f);
            s.Append(__instance.img_dice.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack));
            s.AppendCallback(delegate
            {
                __instance.group_dice.gameObject.SetActive(false);

                AccessTools.Method(typeof(ExamMiniGameView), "AddScore").Invoke(__instance, new object[] { _id, score, _cell.transform.position });
                //__instance.AddScore(_id, score, _cell.transform.position);
                AccessTools.Method(typeof(ExamMiniGameView), "OpenNeighboor").Invoke(__instance, new object[] { _cell, 1, CellState.Reachable });
                //__instance.OpenNeighboor(_cell, 1, CellState.Reachable);
                AccessTools.Method(typeof(ExamMiniGameView), "CheckResult").Invoke(__instance, new object[] { });
                //__instance.CheckResult();
            });
            return false;
        }
    }
    [HarmonyPatch(typeof(MiniGame.Exam.Exam2MiniGameView), "ShowDice")]
    public class ShowDice2Patch
    {
        [HarmonyPrefix]
        public static bool ShowDice(Exam2MiniGameView __instance, int _id, Cell_ExamItemUI _cell)
        {
            //恢复上限没找到倒是TM找到这个了……顺手的事
            ExamBrickCfg examBrickCfg = Cfg.ExamBrickCfgMap[_id];
            int score = MySAPlugin.LockDice.Value == true ? 20 : UnityEngine.Random.Range(examBrickCfg.value[0], examBrickCfg.value[1] + 1);
            __instance.group_dice.gameObject.SetActive(true);
            __instance.img_dice.transform.localScale = Vector3.zero;
            __instance.txt_dice.text = "?";
            Sequence s = DOTween.Sequence();
            s.Append(__instance.img_dice.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack));
            s.Append(__instance.img_dice.transform.DOLocalRotate(new Vector3(0f, 0f, -7200f), 2f, RotateMode.WorldAxisAdd).SetEase(Ease.InOutCubic));
            s.InsertCallback(1.5f, delegate
            {
                __instance.txt_dice.text = ((score >= 0) ? string.Format("+{0}", score) : string.Format("{0}", score));
            });
            s.AppendInterval(0.5f);
            s.Append(__instance.img_dice.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack));
            s.AppendCallback(delegate
            {
                __instance.group_dice.gameObject.SetActive(false);

                AccessTools.Method(typeof(Exam2MiniGameView), "AddScore").Invoke(__instance, new object[] { _id, score, _cell.transform.position });
                //__instance.AddScore(_id, score, _cell.transform.position);
                AccessTools.Method(typeof(Exam2MiniGameView), "OpenNeighboor").Invoke(__instance, new object[] { _cell, 1, CellState.Reachable });
                //__instance.OpenNeighboor(_cell, 1, CellState.Reachable);
                AccessTools.Method(typeof(Exam2MiniGameView), "CheckResult").Invoke(__instance, new object[] { });
                //__instance.CheckResult();
            });
            return false;
        }
    }
    [HarmonyPatch(typeof(MiniGame.Exam.ExamMiniGameView), "OnClickCell")]
    public class OnClickCellPatch
    {
        [HarmonyPrefix]
        public static bool OnClickCell(ExamMiniGameView __instance, Cell_ExamItemUI cell)
        {
            ExamData examData = cell.data as ExamData;
            var restStepCnt = AccessTools.Field(typeof(ExamMiniGameView), "restStepCnt");
            var restStepCntValue = (int)restStepCnt.GetValue(__instance);
            var lastClickType = AccessTools.Field(typeof(ExamMiniGameView), "lastClickType");
            var lastClickTypeValue = (int)lastClickType.GetValue(__instance);
            var showType = (int)AccessTools.Field(typeof(ExamMiniGameView), "showType").GetValue(__instance);
            var eqRank = (int)AccessTools.Field(typeof(ExamMiniGameView), "eqRank").GetValue(__instance);
            var healthRank = (int)AccessTools.Field(typeof(ExamMiniGameView), "healthRank").GetValue(__instance);

            var addScore = AccessTools.Field(typeof(ExamMiniGameView), "addScore");

            var cellDict = (Dictionary<int, Cell_ExamItemUI>)AccessTools.Field(typeof(ExamMiniGameView), "cellDict").GetValue(__instance);

            var SetCellState = AccessTools.Method(typeof(ExamMiniGameView), "SetCellState");
            var SetCellInteractable = AccessTools.Method(typeof(ExamMiniGameView), "SetCellInteractable");
            var AddScore = AccessTools.Method(typeof(ExamMiniGameView), "AddScore");
            var OpenNeighboor = AccessTools.Method(typeof(ExamMiniGameView), "OpenNeighboor");
            var ShowQuestion = AccessTools.Method(typeof(ExamMiniGameView), "ShowQuestion");
            var ShowDice = AccessTools.Method(typeof(ExamMiniGameView), "ShowDice");
            Console.WriteLine($"当前格子类型: {examData.id}");
            if (examData.id == 6 && MySAPlugin.NoneBlockNoStep.Value || examData.id == 7 && MySAPlugin.AddBlockNoStep.Value)
            {
                examData.step = 0;
            }
            if (MySAPlugin.InfinityStep.Value)
            {
                examData.step = 0;
            }
            if (examData.step > 0 && restStepCntValue <= 0 && lastClickTypeValue != 9)
            {
                return false;
            }

            SetCellState.Invoke(__instance, new object[] { cell, CellState.Finish, false, false }); ;
            bool flag = false;
            if (lastClickTypeValue == 0)
            {
                flag = true;
                restStepCnt.SetValue(__instance, restStepCntValue - examData.step);
                //restStepCntValue -= examData.step;
                if (!MySAPlugin.BlackSheepWall.Value)
                {
                    foreach (KeyValuePair<int, Cell_ExamItemUI> item in cellDict)
                    {

                        if ((item.Value.data as ExamData).state == CellState.Unknown)
                        {
                            SetCellState.Invoke(__instance, new object[] { item.Value, CellState.Unknown, false, true });
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<int, Cell_ExamItemUI> item in cellDict)
                    {
                        var cachedata = item.Value.data as ExamData;
                        if (cachedata.state == CellState.Unknown)
                        {
                            if ((cachedata.id != 2 && cachedata.id != 3 && cachedata.id != 4))
                            {
                                SetCellState.Invoke(__instance, new object[] { item.Value, CellState.Known, false, true });
                            }
                            else
                            {

                                SetCellState.Invoke(__instance, new object[] { item.Value, CellState.Unknown, false, true });
                            }
                        }
                    }
                }
            }
            else
            {
                restStepCnt.SetValue(__instance, restStepCntValue - examData.step);
                //restStepCntValue -= examData.step;
                foreach (KeyValuePair<int, Cell_ExamItemUI> item2 in cellDict)
                {
                    ExamData examData2 = item2.Value.data as ExamData;
                    if (examData2.state == CellState.Reachable)
                    {
                        SetCellState.Invoke(__instance, new object[] { item2.Value, CellState.Known, false, false });
                    }

                    if (examData2.state == CellState.Known || examData2.id == showType)
                    {
                        SetCellInteractable.Invoke(__instance, new object[] { item2.Value, false });
                    }
                }
            }

            lastClickType.SetValue(__instance, examData.id);
            if (flag && showType != examData.id)
            {
                AudioMgrEx.PlayUISound(43031);
                AddScore.Invoke(__instance, new object[] { 4, Cfg.ExamBrickCfgMap[4].value[0], cell.transform.position });
                OpenNeighboor.Invoke(__instance, new object[] { cell, 1, CellState.Reachable });
            }
            else
            {
                if (examData.id == 9)
                {
                    AudioMgrEx.PlayUISound(42010);
                    OpenNeighboor.Invoke(__instance, new object[] { cell, 1, CellState.Reachable });
                    foreach (KeyValuePair<int, Cell_ExamItemUI> item3 in cellDict)
                    {
                        ExamData examData3 = item3.Value.data as ExamData;
                        if (restStepCntValue <= 0 && examData3.step > 0)
                        {
                            continue;
                        }

                        if (flag)
                        {
                            if (examData3.id == showType)
                            {
                                SetCellInteractable.Invoke(__instance, new object[] { item3.Value, examData3.id != 12 });
                            }
                        }
                        else if (examData3.state == CellState.Known || examData3.id == showType)
                        {
                            SetCellInteractable.Invoke(__instance, new object[] { item3.Value, examData3.id != 12 });
                        }
                    }
                }
                else if (examData.id == 8)
                {
                    AudioMgrEx.PlayUISound(43032);
                    OpenNeighboor.Invoke(__instance, new object[] { cell, 2, CellState.Reachable });
                }
                else if (examData.id == 7)
                {
                    AudioMgrEx.PlayUISound(43036);
                    addScore.SetValue(__instance, (int)addScore.GetValue(__instance) + MySAPlugin.AddBlockMutiper.Value);
                    OpenNeighboor.Invoke(__instance, new object[] { cell, 1, CellState.Reachable });
                }
                else if (examData.type == 1)
                {
                    AudioMgrEx.PlayUISound(43031);
                    AddScore.Invoke(__instance, new object[] { examData.id, examData.score, cell.transform.position });
                    OpenNeighboor.Invoke(__instance, new object[] { cell, 1, CellState.Reachable });
                }
                else if (examData.id == 5)
                {
                    AudioMgrEx.PlayUISound(43034);
                    ShowQuestion.Invoke(__instance, new object[] { examData.id, examData.score, cell.transform.position });
                    OpenNeighboor.Invoke(__instance, new object[] { cell, 1, CellState.Reachable });
                }
                else if (examData.id == 13)
                {
                    AudioMgrEx.PlayUISound(41011);
                    ShowDice.Invoke(__instance, new object[] { examData.id, cell });
                }
                else if (examData.id == 10)
                {
                    AudioMgrEx.PlayUISound(43031);
                    AddScore.Invoke(__instance, new object[] { examData.id, /*examData.score*/MySAPlugin.SkillBlockMutiper.Value * eqRank, cell.transform.position });
                    OpenNeighboor.Invoke(__instance, new object[] { cell, 1, CellState.Reachable });
                }
                else if (examData.id == 11)
                {
                    AudioMgrEx.PlayUISound(43031);
                    AddScore.Invoke(__instance, new object[] { examData.id, /*examData.score*/MySAPlugin.SkillBlockMutiper.Value * healthRank, cell.transform.position });
                    OpenNeighboor.Invoke(__instance, new object[] { cell, 1, CellState.Reachable });
                }
                else
                {
                    AudioMgrEx.PlayUISound(43035);
                    OpenNeighboor.Invoke(__instance, new object[] { cell, 1, CellState.Reachable });
                }

                AccessTools.Method(typeof(ExamMiniGameView), "CheckSolveHardQuestion").Invoke(__instance, new object[] { cell });
            }

            __instance.txt_step.text = HtmlTxtUtil.ToNegative(restStepCntValue.ToString(), (restStepCntValue != 0) ? 1 : 0);
            if (examData.id != 13 || examData.id != 5)
            {
                AccessTools.Method(typeof(ExamMiniGameView), "CheckResult").Invoke(__instance, new object[] { });
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(MiniGame.Exam.Exam2MiniGameView), "OnClickCell")]
    public class OnClickCell2Patch
    {
        [HarmonyPrefix]
        public static bool OnClickCell(Exam2MiniGameView __instance, Cell_ExamItemUI cell)
        {
            ExamData examData = cell.data as ExamData;
            var restStepCnt = AccessTools.Field(typeof(Exam2MiniGameView), "restStepCnt");
            var restStepCntValue = (int)restStepCnt.GetValue(__instance);
            var lastClickType = AccessTools.Field(typeof(Exam2MiniGameView), "lastClickType");
            var lastClickTypeValue = (int)lastClickType.GetValue(__instance);
            var showType = (int)AccessTools.Field(typeof(Exam2MiniGameView), "showType").GetValue(__instance);
            var eqRank = (int)AccessTools.Field(typeof(Exam2MiniGameView), "eqRank").GetValue(__instance);
            var healthRank = (int)AccessTools.Field(typeof(Exam2MiniGameView), "healthRank").GetValue(__instance);

            var addScore = AccessTools.Field(typeof(Exam2MiniGameView), "addScore");

            var cellDict = (Dictionary<int, Cell_ExamItemUI>)AccessTools.Field(typeof(Exam2MiniGameView), "cellDict").GetValue(__instance);

            var SetCellState = AccessTools.Method(typeof(Exam2MiniGameView), "SetCellState");
            var SetCellInteractable = AccessTools.Method(typeof(Exam2MiniGameView), "SetCellInteractable");
            var AddScore = AccessTools.Method(typeof(Exam2MiniGameView), "AddScore");
            var OpenNeighboor = AccessTools.Method(typeof(Exam2MiniGameView), "OpenNeighboor");
            var ShowQuestion = AccessTools.Method(typeof(Exam2MiniGameView), "ShowQuestion");
            var ShowDice = AccessTools.Method(typeof(Exam2MiniGameView), "ShowDice");
            Console.WriteLine($"当前格子类型: {examData.id}");
            if (examData.id == 6 && MySAPlugin.NoneBlockNoStep.Value || examData.id == 7 && MySAPlugin.AddBlockNoStep.Value)
            {
                examData.step = 0;
            }
            if (MySAPlugin.InfinityStep.Value)
            {
                examData.step = 0;
            }
            if (examData.step > 0 && restStepCntValue <= 0 && lastClickTypeValue != 9)
            {
                return false;
            }

            SetCellState.Invoke(__instance, new object[] { cell, CellState.Finish, false, false }); ;
            bool flag = false;
            if (lastClickTypeValue == 0)
            {
                flag = true;
                restStepCnt.SetValue(__instance, restStepCntValue - examData.step);
                //restStepCntValue -= examData.step;
                if (!MySAPlugin.BlackSheepWall.Value)
                {
                    foreach (KeyValuePair<int, Cell_ExamItemUI> item in cellDict)
                    {

                        if ((item.Value.data as ExamData).state == CellState.Unknown)
                        {
                            SetCellState.Invoke(__instance, new object[] { item.Value, CellState.Unknown, false, true });
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<int, Cell_ExamItemUI> item in cellDict)
                    {
                        var cachedata = item.Value.data as ExamData;
                        if (cachedata.state == CellState.Unknown)
                        {
                            if ((cachedata.id != 2 && cachedata.id != 3 && cachedata.id != 4))
                            {
                                SetCellState.Invoke(__instance, new object[] { item.Value, CellState.Known, false, true });
                            }
                            else
                            {

                                SetCellState.Invoke(__instance, new object[] { item.Value, CellState.Unknown, false, true });
                            }
                        }
                    }
                }
            }
            else
            {
                restStepCnt.SetValue(__instance, restStepCntValue - examData.step);
                //restStepCntValue -= examData.step;
                foreach (KeyValuePair<int, Cell_ExamItemUI> item2 in cellDict)
                {
                    ExamData examData2 = item2.Value.data as ExamData;
                    if (examData2.state == CellState.Reachable)
                    {
                        SetCellState.Invoke(__instance, new object[] { item2.Value, CellState.Known, false, false });
                    }

                    if (examData2.state == CellState.Known || examData2.id == showType)
                    {
                        SetCellInteractable.Invoke(__instance, new object[] { item2.Value, false });
                    }
                }
            }

            lastClickType.SetValue(__instance, examData.id);
            if (flag && showType != examData.id)
            {
                AudioMgrEx.PlayUISound(43031);
                AddScore.Invoke(__instance, new object[] { 4, Cfg.ExamBrickCfgMap[4].value[0], cell.transform.position });
                OpenNeighboor.Invoke(__instance, new object[] { cell, 1, CellState.Reachable });
            }
            else
            {
                if (examData.id == 9)
                {
                    AudioMgrEx.PlayUISound(42010);
                    OpenNeighboor.Invoke(__instance, new object[] { cell, 1, CellState.Reachable });
                    foreach (KeyValuePair<int, Cell_ExamItemUI> item3 in cellDict)
                    {
                        ExamData examData3 = item3.Value.data as ExamData;
                        if (restStepCntValue <= 0 && examData3.step > 0)
                        {
                            continue;
                        }

                        if (flag)
                        {
                            if (examData3.id == showType)
                            {
                                SetCellInteractable.Invoke(__instance, new object[] { item3.Value, examData3.id != 12 });
                            }
                        }
                        else if (examData3.state == CellState.Known || examData3.id == showType)
                        {
                            SetCellInteractable.Invoke(__instance, new object[] { item3.Value, examData3.id != 12 });
                        }
                    }
                }
                else if (examData.id == 8)
                {
                    AudioMgrEx.PlayUISound(43032);
                    OpenNeighboor.Invoke(__instance, new object[] { cell, 2, CellState.Reachable });
                }
                else if (examData.id == 7)
                {
                    AudioMgrEx.PlayUISound(43036);
                    addScore.SetValue(__instance, (int)addScore.GetValue(__instance) + MySAPlugin.AddBlockMutiper.Value);
                    OpenNeighboor.Invoke(__instance, new object[] { cell, 1, CellState.Reachable });
                }
                else if (examData.type == 1)
                {
                    AudioMgrEx.PlayUISound(43031);
                    AddScore.Invoke(__instance, new object[] { examData.id, examData.score, cell.transform.position });
                    OpenNeighboor.Invoke(__instance, new object[] { cell, 1, CellState.Reachable });
                }
                else if (examData.id == 5)
                {
                    AudioMgrEx.PlayUISound(43034);
                    ShowQuestion.Invoke(__instance, new object[] { examData.id, examData.score, cell.transform.position });
                    OpenNeighboor.Invoke(__instance, new object[] { cell, 1, CellState.Reachable });
                }
                else if (examData.id == 13)
                {
                    AudioMgrEx.PlayUISound(41011);
                    ShowDice.Invoke(__instance, new object[] { examData.id, cell });
                }
                else if (examData.id == 10)
                {
                    AudioMgrEx.PlayUISound(43031);
                    AddScore.Invoke(__instance, new object[] { examData.id, /*examData.score*/MySAPlugin.SkillBlockMutiper.Value * eqRank, cell.transform.position });
                    OpenNeighboor.Invoke(__instance, new object[] { cell, 1, CellState.Reachable });
                }
                else if (examData.id == 11)
                {
                    AudioMgrEx.PlayUISound(43031);
                    AddScore.Invoke(__instance, new object[] { examData.id, /*examData.score*/MySAPlugin.SkillBlockMutiper.Value * healthRank, cell.transform.position });
                    OpenNeighboor.Invoke(__instance, new object[] { cell, 1, CellState.Reachable });
                }
                else
                {
                    AudioMgrEx.PlayUISound(43035);
                    OpenNeighboor.Invoke(__instance, new object[] { cell, 1, CellState.Reachable });
                }

                AccessTools.Method(typeof(Exam2MiniGameView), "CheckSolveHardQuestion").Invoke(__instance, new object[] { cell });
            }

            __instance.txt_step.text = HtmlTxtUtil.ToNegative(restStepCntValue.ToString(), (restStepCntValue != 0) ? 1 : 0);
            if (examData.id != 13 || examData.id != 5)
            {
                AccessTools.Method(typeof(Exam2MiniGameView), "CheckResult").Invoke(__instance, new object[] { });
            }
            return false;
        }
    }
}
