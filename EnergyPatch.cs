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
    [HarmonyPatch(typeof(Role), "GetMaxSaveEnergy")]
    public class GetMaxSaveEnergyPatch
    {
        [HarmonyPrefix]
        public static bool GetMaxSaveEnergy(Role __instance, ref float __result)
        {
            //恢复上限没找到倒是TM找到这个了……顺手的事
            if (MySAPlugin.InfinitySaveEnergy.Value==true)
            {
                __result = float.MaxValue;
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Role), "GetAttr")]
    public class EnergyGetAttrPatch
    {
        [HarmonyPostfix]
        public static void GetAttr(Role __instance, int key, ref float __result)
        {
            //恢复上限没找到倒是TM找到这个了……顺手的事
            if (key == 9 && MySAPlugin.InfinityRecoverEnergyLimit.Value == true)
            {
                __result = float.MaxValue;
            }
        }
    }
    [HarmonyPatch(typeof(EnergyItem), "OnCellRender")]
    public class OnCellRenderPatch
    {
        [HarmonyPrefix]
        public static bool OnCellRender(EnergyItem __instance, UICell _cell)
        {
            Cell_EnergyItemUI cell_EnergyItemUI = _cell as Cell_EnergyItemUI;
            TheEntity.Role role = Singleton<RoleMgr>.Ins.GetRole();
            int num = (int)role.GetAttr(7);
            cell_EnergyItemUI.txt_energy.text = num.ToString();
            float attr = role.GetAttr(8); 

            // 获取最大可保存能量
            float maxSaveEnergy = role.GetMaxSaveEnergy();

            // 修复 float.MaxValue 导致的渲染问题
            if (maxSaveEnergy == float.MaxValue)
            {
                maxSaveEnergy = 30f;  // 设置一个较大的但不会导致渲染问题的值
            }
            float num2 = (((float)num > attr) ? Mathf.Max(num, attr + maxSaveEnergy) : attr);
            float value = (num2 * 0.1f - 1f) * 100f;
            value = Mathf.Clamp(value, 150f, 500f);
            cell_EnergyItemUI.btn_energy.transform.SetSizeX(value);
            cell_EnergyItemUI.img_energy_mask.enabled = false;
            MaterialHelper.SetMatProp(cell_EnergyItemUI.img_energy.material, 1007, cell_EnergyItemUI.img_energy.rectTransform.rect.width, cell_EnergyItemUI.img_energy.rectTransform.rect.height);
            cell_EnergyItemUI.img_energy_mask.enabled = true;
            cell_EnergyItemUI.img_energy.fillAmount = (float)num / num2;
            float num3 = Mathf.Min(num2 * 0.1f, cell_EnergyItemUI.Cell_EnergyScale.transform.rect.width);
            int num4 = Mathf.CeilToInt(num3) - 1;
            if (num4 == cell_EnergyItemUI.itemgroup_energy_scale.GetCells().Count)
            {
                return false;
            }

            float num5 = cell_EnergyItemUI.itemgroup_energy_scale.transform.rect.width / num3;
            if (num4 > 1)
            {
                List<float> list = new List<float>();
                for (int i = 1; i <= num4; i++)
                {
                    list.Add((float)i * num5 - cell_EnergyItemUI.Cell_EnergyScale.transform.rect.width);
                }

                cell_EnergyItemUI.itemgroup_energy_scale.SetDatas(list);
            }
            else
            {
                cell_EnergyItemUI.itemgroup_energy_scale.Clear();
            }
            return false;
        }
    }
}
