using Dissonance;
using GameNetcodeStuff;
using HarmonyLib;
using Steamworks.Ugc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MoreSales.Patches
{
    [HarmonyPatch()]
    public class IncreaseSalesPatch
    {
        public static int customRandomNumber = 15;

        [HarmonyPatch(typeof(Terminal), nameof(Terminal.SetItemSales))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i].operand is MethodInfo method)
                {
                    if (method.Name == "Clamp" && method.DeclaringType == typeof(UnityEngine.Mathf))
                    {
                        if (i > 2 && codes[i - 1].opcode == OpCodes.Ldc_I4_5 && codes[i - 2].opcode == OpCodes.Ldc_I4_0)
                        {
                            MoreSales.mls.LogWarning("i - 1: " + codes[i - 1].ToString());
                            MoreSales.mls.LogWarning("i: " + codes[i].ToString());
                            MoreSales.mls.LogWarning("i + 1: " + codes[i + 1].ToString());
                            MoreSales.mls.LogWarning("i + 2: " + codes[i + 2].ToString());
                            MoreSales.mls.LogWarning("i + 3: " + codes[i + 3].ToString());
                            MoreSales.mls.LogWarning("i + 4: " + codes[i + 4].ToString());

                            codes.Insert(i + 2, new CodeInstruction(OpCodes.Ldc_I4_S, customRandomNumber));
                            codes.Insert(i + 3, new CodeInstruction(OpCodes.Stloc_1));

                            MoreSales.mls.LogWarning("i - 1: " + codes[i - 1].ToString());
                            MoreSales.mls.LogWarning("i: " + codes[i].ToString());
                            MoreSales.mls.LogWarning("i + 1: " + codes[i + 1].ToString());
                            MoreSales.mls.LogWarning("i + 2: " + codes[i + 2].ToString());
                            MoreSales.mls.LogWarning("i + 3: " + codes[i + 3].ToString());
                            MoreSales.mls.LogWarning("i + 4: " + codes[i + 4].ToString());
                            break;
                        }
                    }
                }
            }
            return codes.AsEnumerable();
        }

        [HarmonyPatch(typeof(Terminal), nameof(Terminal.SetItemSales))]
        [HarmonyPostfix]
        static void InfoPatch(Terminal __instance)
        {
            MoreSales.mls.LogWarning("set sales");

            for (int i = 0; i < __instance.buyableItemsList.Length; i++)
            {
                MoreSales.mls.LogWarning("item " + i + ": " + __instance.buyableItemsList[i] + " sale: " + __instance.itemSalesPercentages[i]);
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPostfix]
        static void SetSales(PlayerControllerB __instance)
        {
            if (__instance.isJumping)
            {
                UnityEngine.Object.FindObjectOfType<Terminal>().SetItemSales();
            }
        }
    }
}
