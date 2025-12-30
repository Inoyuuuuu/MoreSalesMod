using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            
            RemoveNotFullyRemovedItems(__instance);
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

            MoreSales.mls.LogWarning("amountofitemsonsale: " + amountOfItemsOnSale);

            for (int k = 0; k < amountOfItemsOnSale; k++)
            {

                if (list.Count <= 0)
                {
                    break;
                }

                int num3 = list[random.Next(0, (list.Count - 1))];

                float randomSalesStrength = Math.Clamp(((float)random.Next(0, 100) / 100f) * saleValueModifier, 0, 1);
                int randomSalesPercentage = (int)Math.Round((double)Mathf.Lerp(minSalesPercentage, maxSalesPercentage, randomSalesStrength));
                MoreSales.mls.LogWarning("randomSalesPercentage: " + randomSalesPercentage + " --> " + RoundToNearestTen(randomSalesPercentage));

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

        private static bool WasItemRemoved(Item item)
        {
            Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
            return terminal.ParseWord(item.itemName.ToLower()) == null;
        }

        private static void RemoveNotFullyRemovedItems(Terminal terminal)
        {
            List<Item> newItems = [];

            for (int i = 0; i < terminal.buyableItemsList.Length; i++)
            {
                if (terminal.ParseWord(terminal.buyableItemsList[i].itemName.ToLower()) != null)
                {
                    newItems.Add(terminal.buyableItemsList[i]);
                } else
                {
                    MoreSales.mls.LogWarning("tn c " + terminal.terminalNodes.terminalNodes.Count);
                    MoreSales.mls.LogWarning("stn c " + terminal.terminalNodes.specialNodes.Count);

                    for (int k = 0; k < terminal.terminalNodes.specialNodes.Count; k++)
                    {
                        MoreSales.mls.LogWarning("node: " + terminal.terminalNodes.specialNodes[k].name.ToLower() 
                            + " --------- item: " + terminal.buyableItemsList[i].itemName.ToLower());

                        if (terminal.terminalNodes.specialNodes[k].name.ToLower().Equals(terminal.buyableItemsList[i]))
                        {
                            terminal.terminalNodes.specialNodes.RemoveAt(k);
                        }
                    }
                }
            }

            terminal.buyableItemsList = newItems.ToArray();
        }

        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.Start))]
        [HarmonyPostfix]
        public static void RedoSalesPatch(StartOfRound __instance)
        {
            __instance.StartCoroutine(DelayedSetItemSales(4));
        }

        public static IEnumerator DelayedSetItemSales(int seconds)
        {
            yield return new WaitForSeconds(seconds);

            Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
            terminal.SetItemSales();

            MoreSales.mls.LogWarning("itemlist: " + terminal.buyableItemsList.Length + " itemSalesPercentages: " + terminal.itemSalesPercentages.Length);
            for (int i = 0; i < terminal.buyableItemsList.Length; i++)
            {
                MoreSales.mls.LogWarning("item: " + terminal.buyableItemsList[i].itemName + " price: " + terminal.buyableItemsList[i].creditsWorth + " sale: " + (100 - terminal.itemSalesPercentages[i]));
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.PlayerJump))]
        [HarmonyPostfix]
        public static void DebugPatch()
        {
            Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
            terminal.SetItemSales();

            MoreSales.mls.LogWarning("itemlist: " + terminal.buyableItemsList.Length + " itemSalesPercentages: " + terminal.itemSalesPercentages.Length);

            for (int i = 0; i < terminal.buyableItemsList.Length; i++)
            {
                //if (terminal.ParseWord(terminal.buyableItemsList[i].itemName.ToLower()) == null)
                //{
                //    MoreSales.mls.LogWarning("found unbuyable item: " + terminal.buyableItemsList[i].itemName);
                //}

                MoreSales.mls.LogWarning("item: " + terminal.buyableItemsList[i].itemName + " price: " + terminal.buyableItemsList[i].creditsWorth + " sale: " + (100 - terminal.itemSalesPercentages[i]));
            }
        }
    }
}
