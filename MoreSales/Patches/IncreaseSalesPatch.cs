using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoreSales.Patches
{
    [HarmonyPatch()]
    public class IncreaseSalesPatch
    {

        [HarmonyPatch(typeof(Terminal), nameof(Terminal.SetItemSales))]
        [HarmonyPostfix]
        static void SetItemSalesPostfix(Terminal __instance)
        {
            System.Random random = new System.Random(StartOfRound.Instance.randomMapSeed + 90);

            int maxSalesPercentage = MoreSales.moreSalesConfigs.maxSalePercentage;
            int minSalesPercentage = MoreSales.moreSalesConfigs.minSalePercentage;
            float saleValueModifier = MoreSales.moreSalesConfigs.saleValueOdds / 50f;
            minSalesPercentage = (minSalesPercentage > maxSalesPercentage) ? maxSalesPercentage : minSalesPercentage;

            int minAmountOfItemsOnSale = MoreSales.moreSalesConfigs.minNumberOfItemsOnSale;
            int maxAmountOfItemsOnSale = MoreSales.moreSalesConfigs.maxNumberOfItemsOnSale;
            float amountOfSalesModifier = MoreSales.moreSalesConfigs.moreSalesOdds / 50f;
            minAmountOfItemsOnSale = (minAmountOfItemsOnSale > maxAmountOfItemsOnSale) ? maxAmountOfItemsOnSale : minAmountOfItemsOnSale;

            __instance.InitializeItemSalesPercentages();

            List<int> list = [];
            for (int i = 0; i < __instance.buyableItemsList.Length; i++)
            {
                list.Add(i);
                __instance.itemSalesPercentages[i] = 100;
            }
            for (int j = 0; j < __instance.buyableVehicles.Length; j++)
            {
                list.Add(j + __instance.buyableItemsList.Length);
                __instance.itemSalesPercentages[j + __instance.buyableItemsList.Length] = 100;
            }

            maxAmountOfItemsOnSale = (maxAmountOfItemsOnSale > list.Count) ? list.Count : maxAmountOfItemsOnSale;
            float amountOfSalesStrength = Math.Clamp(((float)random.Next(0, 100) / 100f) * amountOfSalesModifier, 0, 1);
            int amountOfItemsOnSale = (int)Math.Round((double)Mathf.Lerp(minAmountOfItemsOnSale, maxAmountOfItemsOnSale, amountOfSalesStrength));

            if (MoreSales.moreSalesConfigs.disableAllSales)
            {
                amountOfItemsOnSale = 0;
            }
            else if (MoreSales.moreSalesConfigs.setAllItemsOnSale)
            {
                amountOfItemsOnSale = list.Count;
            }

            //MoreSales.mls.LogWarning("amountofitemsonsale: " + amountOfItemsOnSale);

            for (int k = 0; k < amountOfItemsOnSale; k++)
            {
                if (list.Count <= 0)
                {
                    break;
                }

                int num3 = list[random.Next(0, (list.Count - 1))];

                float randomSalesStrength = Math.Clamp(((float)random.Next(0, 100) / 100f) * saleValueModifier, 0, 1);
                int randomSalesPercentage = (int)Math.Round((double)Mathf.Lerp(minSalesPercentage, maxSalesPercentage, randomSalesStrength));
                //MoreSales.mls.LogWarning("randomSalesPercentage: " + randomSalesPercentage + " --> " + RoundToNearestTen(randomSalesPercentage));

                if (MoreSales.moreSalesConfigs.roundToNearestTen)
                {
                    randomSalesPercentage = RoundToNearestTen(randomSalesPercentage);
                }
                __instance.itemSalesPercentages[num3] = (100 - randomSalesPercentage);

                list.Remove(num3);
            }
        }

        private static int RoundToNearestTen(int value)
        {
            return (int)Math.Round((double)value / 10.0) * 10;
        }

        //[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.PlayerJump))]
        //[HarmonyPostfix]
        //public static void DebugPatch()
        //{
        //    Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
        //    terminal.SetItemSales();
        //}
    }
}
