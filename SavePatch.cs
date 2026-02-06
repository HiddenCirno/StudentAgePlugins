using BepInEx.Configuration;
using Config;
using HarmonyLib;
using Sdk;
using Sdk.PlatformAPI;
using System;
using System.Collections.Generic;
using System.Reflection;
using TheEntity;

namespace 学生时代个人插件
{
    public class SavePatch
    {
        public static void SetCustomSavePath()
        {
            var modPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "StudentAgeSave");
            //if (MySAPlugin.ChangeSavePath.Value)
            //{
                //PathDefine.SAVE_PATH = System.IO.Path.Combine(modPath, "Saves", Platform.Current.GetUserId());
                //PathDefine.TEST_SAVE_PATH = System.IO.Path.Combine(modPath, "Saves_Test", Platform.Current.GetUserId());
            //}
        }
    }
}
