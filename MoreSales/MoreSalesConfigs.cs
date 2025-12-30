using BepInEx.Configuration;
using CSync.Extensions;
using CSync.Lib;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace MoreSales
{
    [DataContract]
    internal class MoreSalesConfigs : SyncedConfig2<MoreSalesConfigs>
    {
        internal int saleOddsBV = 50;
        internal int minNumberOfItemsOnSaleBaseValue = 5;
        internal int maxNumberOfItemsOnSaleBaseValue = 8;
        internal int minSalePercentageBaseValue = 30;
        internal int maxSalePercentageBaseValue = 90;

        [SyncedEntryField]
        internal SyncedEntry<bool> setAllItemsOnSale, disableAllSales, roundToNearestTen;
        [SyncedEntryField]
        internal SyncedEntry<int> saleValueOdds, moreSalesOdds;
        [SyncedEntryField]
        internal SyncedEntry<int> minNumberOfItemsOnSale, maxNumberOfItemsOnSale;
        [SyncedEntryField]
        internal SyncedEntry<int> minSalePercentage, maxSalePercentage;


        public MoreSalesConfigs(ConfigFile cfg) : base(MyPluginInfo.PLUGIN_NAME)
        {

            minNumberOfItemsOnSale = cfg.BindSyncedEntry("DiscountSettings", "minNumberOfItemsOnSale", minNumberOfItemsOnSaleBaseValue, 
                new ConfigDescription("This sets the minimum amount of items that are on sale each shop rotation.", new AcceptableValueRange<int>(0, 999)));
            maxNumberOfItemsOnSale = cfg.BindSyncedEntry("DiscountSettings", "maxNumberOfItemsOnSale", maxNumberOfItemsOnSaleBaseValue,
                new ConfigDescription("This sets the maximum amount of items that are on sale each shop rotation.", new AcceptableValueRange<int>(0, 999)));

            minSalePercentage = cfg.BindSyncedEntry("DiscountSettings", "minSalePercentage", minSalePercentageBaseValue, 
                new ConfigDescription("This sets min discount-percentage of sales for all items that are on sale.", new AcceptableValueRange<int>(0, 100)));
            maxSalePercentage = cfg.BindSyncedEntry("DiscountSettings", "maxSalePercentage", maxSalePercentageBaseValue,
                new ConfigDescription("This sets max discount-percentage of sales for all items that are on sale.", new AcceptableValueRange<int>(0, 100)));

            setAllItemsOnSale = cfg.BindSyncedEntry("DiscountSettings", "setAllItemsOnSale", false, "This sets all shop Items on sale.");
            disableAllSales = cfg.BindSyncedEntry("DiscountSettings", "disableAllSales", false, "This disables all sales (will override setAllItemsOnSale).");
            roundToNearestTen = cfg.BindSyncedEntry("DiscountSettings", "roundToNearestTen", true, "This rounds the percentage numbers to the nearest 10x value (looks nicer).");

            saleValueOdds = cfg.BindSyncedEntry("SaleOdds", "saleValueOdds", saleOddsBV,
                new ConfigDescription("Values above 50 increase the odds of higher value sales, values below 50 decrease.", new AcceptableValueRange<int>(0, 100)));
            moreSalesOdds = cfg.BindSyncedEntry("SaleOdds", "moreSalesOdds", saleOddsBV,
                new ConfigDescription("Values above 50 increase the odds of more items being on sale, values below 50 decrease.", new AcceptableValueRange<int>(0, 100)));

            ConfigManager.Register(this);
        }
    }
}
