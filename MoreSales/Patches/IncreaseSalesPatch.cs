using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace MoreSales.Patches
{
    [HarmonyPatch()]
    public class IncreaseSalesPatch
    {

        [HarmonyPatch(typeof(Terminal), nameof(Terminal.SetItemSales))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i].operand is MethodInfo clampMethod)
                {
                    if (clampMethod.Name == "Clamp" && clampMethod.DeclaringType == typeof(UnityEngine.Mathf))
                    {
                        if (i > 2 && codes[i - 1].opcode == OpCodes.Ldc_I4_5 && codes[i - 2].opcode == OpCodes.Ldc_I4_0)
                        {
                            codes.Insert(i + 2, new CodeInstruction(OpCodes.Ldc_I4_S, MoreSales.moreSalesConfigs.actualOfItemsInSale));
                            codes.Insert(i + 3, new CodeInstruction(OpCodes.Stloc_1));
                        }
                    }
                }
            }

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Callvirt && codes[i].operand is MethodInfo randomMethod)
                {
                    if (randomMethod.Name == "Next" && randomMethod.DeclaringType == typeof(System.Random))
                    {
                        if (i > 2 && codes[i - 1].opcode == OpCodes.Ldloc_S && codes[i - 2].opcode == OpCodes.Ldc_I4_0)
                        {

                            codes.RemoveAt(i - 2);
                            codes.Insert(i - 2, new CodeInstruction(OpCodes.Ldc_I4_S, MoreSales.moreSalesConfigs.minSalePercentage.Value));

                            codes.RemoveAt(i - 1);
                            codes.Insert(i - 1, new CodeInstruction(OpCodes.Ldc_I4_S, MoreSales.moreSalesConfigs.maxSalePercentage.Value));

                            break;
                        }
                    }
                }
            }

            //MoreSales.mls.LogDebug("finished IL Code, printing to logs now: ");
            //for (int i = 0; i < codes.Count; i++)
            //{
            //    MoreSales.mls.LogDebug(codes[i].ToString());
            //}

            return codes.AsEnumerable();
        }


        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.ConnectClientToPlayerObject))]
        [HarmonyPostfix]
        static void updateItemSalesOnLobbyJoin()
        {
            Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
            terminal.SetItemSales();

            //Debug_GenerateRandomSales(terminal);
        }

        private static void Debug_LogILCodeAtIndex(List<CodeInstruction> codes, int index)
        {
            if (index >= 2)
            {
                MoreSales.mls.LogDebug("i - 2: " + codes[index - 2].ToString());
                MoreSales.mls.LogDebug("i - 1: " + codes[index - 1].ToString());
            }

            MoreSales.mls.LogDebug("i: " + codes[index].ToString());
            MoreSales.mls.LogDebug("i + 1: " + codes[index + 1].ToString());
            MoreSales.mls.LogDebug("i + 2: " + codes[index + 2].ToString());
            MoreSales.mls.LogDebug("i + 3: " + codes[index + 3].ToString());
            MoreSales.mls.LogDebug("i + 4: " + codes[index + 4].ToString());
        }

        private static void Debug_GenerateRandomSales(Terminal terminal)
        {
            System.Random random = new System.Random(StartOfRound.Instance.randomMapSeed + 90);

            int num = 999999;
            List<int> list = new List<int>();
            for (int i = 0; i < terminal.buyableItemsList.Length; i++)
            {
                list.Add(i);
            }

            for (int j = 0; j < num; j++)
            {
                if (list.Count <= 0)
                {
                    break;
                }
                int num2 = random.Next(0, list.Count);
                int i2 = 100 - random.Next(MoreSales.moreSalesConfigs.minSalePercentage.Value, MoreSales.moreSalesConfigs.maxSalePercentage.Value);
                i2 = RoundToNearestTen(i2);

                MoreSales.mls.LogWarning("num2: " + num2 + " number of list: " + list[num2] + " should be percentage: " + i2);
                MoreSales.mls.LogWarning("itemName: " + terminal.buyableItemsList[num2].itemName + " itemSaleP " + terminal.itemSalesPercentages[num2]);

                list.RemoveAt(num2);
            }
        }

        private static int RoundToNearestTen(int i)
        {
            return (int)Math.Round((double)i / 10.0) * 10;
        }
    }
}
