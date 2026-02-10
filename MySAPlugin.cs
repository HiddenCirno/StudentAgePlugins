using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Newtonsoft.Json;
using Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace 学生时代个人插件
{
    [BepInPlugin("studentage.hiddenhiragi.customplugins", "学生时代个人插件", "1.0.0")]
    public class MySAPlugin : BaseUnityPlugin
    {
        public static string modPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public void Awake()
        {
            var harmony = new Harmony("studentage.hiddenhiragi.customplugins");
            harmony.PatchAll(); // 应用所有的 Patch
            InitConfigEntry();
            //SavePatch.SetCustomSavePath();
        }
        public void Update()
        {
            if (_debugKey.IsPressed() && !_debugKeyLastFrame)
            {
                var instance = Singleton<RoleMgr>.Ins;
                Console.WriteLine($"标签值为: {instance.GetRole()?.GetUnlockValue(9022, true)}");
                ExportIdMap();
                _debugKeyLastFrame = _debugKey.IsPressed();
            }
            if (!_debugKey.IsPressed())
            {
                _debugKeyLastFrame = false;
            }

        }
        public void ExportIdMap()
        {
            var dict = new Dictionary<int, string>();
            for(var i = 0; i< 300000; i++)
            {
                var text = DescCtrl.GetFromTag(i);
                if (text != null)
                {
                    dict.TryAdd(i, text);
                }
            }
            Console.WriteLine($"{dict.Count}");
            string json = JsonConvert.SerializeObject(dict, Formatting.Indented);
            string pluginRootDir = Application.dataPath; // 在 Unity 中获取插件的根目录
            string filePath = Path.Combine(modPath, "dict_data.json"); // 文件保存路径
            File.WriteAllText(filePath, json);

        }
        public void InitConfigEntry()
        {
            /*
            SearchTimeMutiper = Config.Bind(
            "搜索设置",
            "搜索时间倍率",
            0.75f,
            new ConfigDescription("调整搜索时间的倍率, 仅在修改搜索时长启用时生效",
                new AcceptableValueRange<float>(0.0f, 1.0f)) // 音量范围从 0 到 1
            );
            */
            InfinitySaveEnergy = Config.Bind(
            "精力条",
            "次回继承无上限",
            true,
            "启用后继承到下回合的精力不再有最大限制"
            );
            InfinityRecoverEnergyLimit = Config.Bind(
            "精力条",
            "精力恢复无上限",
            true,
            "启用后精力恢复不再有最大限制"
            );
            LockDice = Config.Bind(
            "考试小游戏",
            "锁定骰子",
            true,
            "骰子格子永远骰出20"
            );
            BlackSheepWall = Config.Bind(
            "考试小游戏",
            "格子全亮",
            true,
            "点击一次后点亮所有非数值格子"
            );
            AddBlockNoStep = Config.Bind(
            "考试小游戏",
            "累加格子不消耗步数",
            true,
            "累加(绿底白色加号)格子不再消耗步数"
            );
            NoneBlockNoStep = Config.Bind(
            "考试小游戏",
            "杂念格子不消耗步数",
            true,
            "杂念(红底禁止符号)格子不再消耗步数"
            );
            InfinityStep = Config.Bind(
            "考试小游戏",
            "无限步数",
            false,
            "考试小游戏不消耗步数"
            );
            SkillBlockMutiper = Config.Bind(
            "考试小游戏",
            "属性格子倍率",
            3,
            new ConfigDescription("考试小游戏里根据个人属性等级计算分数的格子的乘算倍率, 技术问题, 不会改变显示数值, 实际有效",
                new AcceptableValueRange<int>(1, 20)) // 音量范围从 0 到 1
            );
            AddBlockMutiper = Config.Bind(
            "考试小游戏",
            "累加格子倍率",
            2,
            new ConfigDescription("累加(绿底白色加号)格子的增长幅度",
                new AcceptableValueRange<int>(1, 20)) // 音量范围从 0 到 1
            );
            StayStudyCost = Config.Bind(
            "学习",
            "学习消耗不累加",
            true,
            "在家学习科目的消耗不再累加"
            );
            StayReadBookCost = Config.Bind(
            "学习",
            "阅读消耗不累加",
            true,
            "主动读书的消耗不再累加"
            );
            StaySocialCost = Config.Bind(
            "社交",
            "社交消耗不累加",
            true,
            "社交的热情消耗不再累加"
            );
            StaySocialCost = Config.Bind(
            "社交",
            "社交消耗不累加",
            true,
            "社交的热情消耗不再累加"
            );
            StayTeachCost = Config.Bind(
            "社交",
            "请教消耗不累加",
            true,
            "请教的消耗不再累加"
            );
            ActionEnergyCostMutiper = Config.Bind(
            "行动",
            "精力行动消耗梯度增幅",
            5,
            new ConfigDescription("仅影响10-20-30-40四个档位, 可以自定义梯度增幅的数值",
                new AcceptableValueRange<int>(0, 10)) // 音量范围从 0 到 1
            );
            FreeHonorShop = Config.Bind(
            "荣誉点数",
            "荣誉商店全免费",
            true,
            "荣誉商店(鹅城实验室)的解锁选项不再消耗荣誉值"
            );
            /*
            //弃用
            ChangeSavePath = Config.Bind(
            "杂项",
            "修改存档路径",
            true,
            "将存档路径修改到BepInEx内\n开关此项需要重启游戏生效\n默认存档路径位于C:/Users/UserName/AppData/LocalLow/PakyiGame/StudentAge/Saves"
            );
            */
            EasyMusicMiniGame = Config.Bind(
            "音游小游戏",
            "放宽音游判定",
            0.5f,
            new ConfigDescription("放宽音游的所有判定",
                new AcceptableValueRange<float>(0f, 2f)) // 音量范围从 0 到 1
            );
            QteMiniGameSpeed = Config.Bind(
            "砍价小游戏",
            "指针移动速度",
            0.5f,
            new ConfigDescription("砍价小游戏的判定指针移动速度",
                new AcceptableValueRange<float>(0.01f, 1f)) // 音量范围从 0 到 1
            );
            QteMiniGameCostMutiper = Config.Bind(
            "砍价小游戏",
            "折扣力度额外增益",
            1,
            new ConfigDescription("激活方法论「讨价还价」后提供的额外打折力度",
                new AcceptableValueRange<int>(0, 2)) // 音量范围从 0 到 1
            );
            RevertMoneyCost = Config.Bind(
            "作弊功能",
            "反转零花钱消耗",
            false,
            "启用后所有花钱项目反转"
            );
            SkipAllMiniGame = Config.Bind(
            "作弊功能",
            "所有小游戏可跳过",
            false,
            "为所有小游戏点亮跳过按钮, 不管你是否通关过"
            );
            InfinitySocialCapacity = Config.Bind(
            "作弊功能",
            "社交容量无上限",
            false,
            "启用后社交容量无限"
            );
            FightMiniGameInfinityHealth = Config.Bind(
            "打架小游戏",
            "锁定自身血量",
            true,
            "启用后打架小游戏自己挨打不掉血"
            );
            AutoFishing = Config.Bind(
            "钓鱼小游戏",
            "自动钓鱼",
            true,
            "启用后自动增加钓鱼进度"
            );
        }

        public KeyboardShortcut _debugKey = new KeyboardShortcut(KeyCode.F2);
        public bool _debugKeyLastFrame = false;
        //public static ConfigEntry<float> SearchTimeMutiper { get; set; }
        public static ConfigEntry<bool> LockDice { get; set; }
        public static ConfigEntry<int> SkillBlockMutiper { get; set; }
        public static ConfigEntry<bool> InfinitySaveEnergy { get; set; }
        public static ConfigEntry<bool> BlackSheepWall { get; set; }
        public static ConfigEntry<bool> AddBlockNoStep { get; set; }
        public static ConfigEntry<int> AddBlockMutiper { get; set; }
        public static ConfigEntry<bool> NoneBlockNoStep { get; set; }
        public static ConfigEntry<bool> InfinityStep { get; set; }
        public static ConfigEntry<bool> StaySocialCost { get; set; }
        public static ConfigEntry<bool> StayStudyCost { get; set; }
        public static ConfigEntry<bool> StayTeachCost { get; set; }
        public static ConfigEntry<bool> StayReadBookCost { get; set; }
        public static ConfigEntry<int> ActionEnergyCostMutiper { get; set; }
        public static ConfigEntry<int> QteMiniGameCostMutiper { get; set; }
        public static ConfigEntry<bool> FreeHonorShop { get; set; }
        public static ConfigEntry<float> EasyMusicMiniGame { get; set; }
        public static ConfigEntry<float> QteMiniGameSpeed { get; set; }
        //public static ConfigEntry<bool> ChangeSavePath { get; set; }
        public static ConfigEntry<bool> InfinityRecoverEnergyLimit { get; set; }
        public static ConfigEntry<bool> RevertMoneyCost { get; set; }
        public static ConfigEntry<bool> SkipAllMiniGame { get; set; }
        public static ConfigEntry<bool> InfinitySocialCapacity { get; set; }
        public static ConfigEntry<bool> FightMiniGameInfinityHealth { get; set; }
        public static ConfigEntry<bool> AutoFishing { get; set; }
    }
}
