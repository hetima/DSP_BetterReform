//using System;
//using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BetterReformMod
{

    [BepInPlugin(__GUID__, __NAME__, "1.0.0")]
    public class BetterReform : BaseUnityPlugin
    {
        public const string __NAME__ = "BetterReform";
        public const string __GUID__ = "com.hetima.dsp." + __NAME__;

        public static UIBetterReformPanel _mainPanel;
        public const int maxStampSize = 20;

        new internal static ManualLogSource Logger;

        void Awake()
        {
            Logger = base.Logger;

            Harmony harmony = new Harmony(__GUID__);
            harmony.PatchAll(typeof(Patch));

        }
        public static void Log(string str)
        {
            Logger.LogInfo(str);
        }

        public static void HoldFoundationInHands()
        {
            UIBuildMenu buildMenu = UIRoot.instance.uiGame.buildMenu;
            if (buildMenu.currentCategory == 9 && !buildMenu.childButtons[1].highlighted)
            {
                buildMenu.OnChildButtonClick(1);
            }
        }


        //private static int _lastBrushSize = 0;

        public static int ModBrushSize(BuildTool_Reform tool)
        {
            _mainPanel.RefreshState();
            return tool.brushSize;

            //if (_mainPanel.IsStampReady())
            //{
            //    //if (_lastBrushSize == 0)
            //    //{
            //    //    _lastBrushSize = tool.brushSize;
            //    //}
            //    //tool.brushSize = _mainPanel.reformSize;
            //}
            //else if (_lastBrushSize != 0)
            //{
            //    tool.brushSize = _lastBrushSize;
            //    _lastBrushSize = 0;
            //}
            //else
            //{
            //    //wheel
            //    if (VFInput.control && VFInput.mouseWheel != 0)
            //    {
            //        int num = 0;
            //        if (VFInput.mouseWheel < -0.08f)
            //        {
            //            num = -1;
            //        }
            //        else if (VFInput.mouseWheel > 0.08f)
            //        {
            //            num = 1;
            //        }
            //        if (num != 0)
            //        {
            //            VFInput.inScrollView = true;
            //            tool.brushSize += num;
            //        }
            //    }
            //}
            //return tool.brushSize;
        }

        public static int ModCursorPointCount(BuildTool_Reform tool)
        {
            if (_mainPanel.IsStampReady())
            {
            }

            return tool.cursorPointCount;
        }


        static class Patch
        {
            internal static bool _initialized = false;


            [HarmonyPrefix, HarmonyPatch(typeof(GameMain), "Begin")]
            public static void GameMain_Begin_Prefix()
            {
                if (!_initialized)
                {
                    _initialized = true;
                    _mainPanel = UIBetterReformPanel.CreateInstance();

                    //Color[] reformColors = Configs.builtin.reformColors;
                    //for (int i = 0; i < 16; i++)
                    //{
                    //    Color color = reformColors[i];
                    //    Log(""+ color.ToString());
                    //}
                }

            }

            [HarmonyPostfix, HarmonyPatch(typeof(UIBuildMenu), "OnCategoryButtonClick")]
            public static void UIBuildMenu_OnCategoryButtonClick_Postfix(UIBuildMenu __instance)
            {
                //引数 index ではなく __instance.currentCategory を使う
                if (__instance.currentCategory == 9 && _mainPanel != null)
                {
                    HoldFoundationInHands();
                }
            }

            [HarmonyPostfix, HarmonyPatch(typeof(BuildTool_Reform), MethodType.Constructor)]
            public static void BuildTool_Reform_Constructor_Postfix(BuildTool_Reform __instance)
            {
                int size = maxStampSize * maxStampSize;
                if (__instance.cursorIndices.Length < size)
                {
                    __instance.cursorIndices = new int[size];
                }
                if (__instance.cursorPoints.Length < size)
                {
                    __instance.cursorPoints = new Vector3[size];
                }
            }

            [HarmonyTranspiler, HarmonyPatch(typeof(BuildTool_Reform), "ReformAction"), HarmonyBefore("Appun.DSP.plugin.BigFormingSize")]
            public static IEnumerable<CodeInstruction> BuildTool_Reform_ReformAction_Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> ins = instructions.ToList();
                MethodInfo m_ModBrushSize = typeof(BetterReform).GetMethod(nameof(ModBrushSize));
                FieldInfo f_brushSize = AccessTools.Field(typeof(BuildTool_Reform), nameof(BuildTool_Reform.brushSize));
                MethodInfo m_SetReformColor = typeof(PlatformSystem).GetMethod(nameof(PlatformSystem.SetReformColor));
                FieldInfo f_cursorPointCount = AccessTools.Field(typeof(BuildTool_Reform), nameof(BuildTool_Reform.cursorPointCount));
                MethodInfo m_ModCursorPointCount = typeof(BetterReform).GetMethod(nameof(ModCursorPointCount));

                //this.brushSize を置き換える

                bool brushSizePatched = false;
                bool setReformColorReached = false;
                bool setReformColorPatched = false;
                int brushMaxSizeCheck = 0;
                for (int i = 0; i < ins.Count - 40; i++)
                {
                    //if (this.brushSize > 10) //first check
                    //if (this.brushSize < 10) //after VFInput._cursorPlusKey.onDown check
                    if (brushMaxSizeCheck < 2 && ins[i].opcode == OpCodes.Ldfld && ins[i].operand is FieldInfo o1 && o1 == f_brushSize)
                    {
                        if (ins[i + 1].opcode == OpCodes.Ldc_I4_S && (ins[i + 1].operand is sbyte o && o == (sbyte)10) &&
                            (ins[i + 2].opcode == OpCodes.Ble || ins[i + 2].opcode == OpCodes.Bge))
                        {
                            ins[i + 1].operand = (sbyte)maxStampSize;
                            brushMaxSizeCheck++;
                        }
                    }

                    // float radius = 0.990946f * (float)this.brushSize;
                    if (!brushSizePatched && ins[i].opcode == OpCodes.Ldc_R4 && ins[i].operand is float f && f > 0.990f && f < 0.994f && ins[i + 4].opcode == OpCodes.Mul)
                    {
                        if (ins[i + 2].opcode == OpCodes.Ldfld && ins[i + 2].operand is FieldInfo o2 && o2 == f_brushSize)
                        {
                            ins[i + 2].opcode = OpCodes.Call;
                            ins[i + 2].operand = m_ModBrushSize;
                            brushSizePatched = true;
                        }
                    }

                    // this.factory.platformSystem.SetReformColor(num13, this.brushColor);
                    if (!setReformColorPatched && ins[i].opcode == OpCodes.Callvirt && ins[i].operand is MethodInfo o3 && o3 == m_SetReformColor)
                    {
                        setReformColorReached = true;
                    }
                    // int num14 = this.cursorPointCount;
                    if (setReformColorReached && !setReformColorPatched && ins[i].opcode == OpCodes.Ldfld && ins[i].operand is FieldInfo o4 && o4 == f_cursorPointCount)
                    {
                        //ins[i].opcode = OpCodes.Call;
                        //ins[i].operand = m_ModCursorPointCount;
                        setReformColorPatched = true;
                    }
                }
                
                return ins.AsEnumerable();
            }

        }

    }
}
