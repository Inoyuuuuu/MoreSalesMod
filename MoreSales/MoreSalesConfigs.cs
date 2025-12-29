using BepInEx.Configuration;
using CSync.Extensions;
using CSync.Lib;
using System;
using System.Runtime.Serialization;

namespace MoreSales
{
    [DataContract]
    internal class MoreSalesConfigs : SyncedConfig2<MoreSalesConfigs>
    {
        internal int numberOfItemsInSaleBaseValue = 5;
        internal int minSalePercentageBaseValue = 10;
        internal int maxSalePercentageBaseValue = 80;

        internal ConfigEntry<string> header;

        [SyncedEntryField]
        internal SyncedEntry<bool> setAllItemsOnSale, disableAllSales, roundToNearestTen;
        [SyncedEntryField]
        internal SyncedEntry<int> numberOfItemsOnSale;
        [SyncedEntryField]
        internal SyncedEntry<int> minSalePercentage, maxSalePercentage;

        public MoreSalesConfigs(ConfigFile cfg) : base(MyPluginInfo.PLUGIN_NAME)
        {

            header = cfg.Bind("DiscountPercentageRange", "IMPORTANT", "(＿ ＿*) Z z z", "nothing here");
            setAllItemsOnSale = cfg.BindSyncedEntry("Discounts", "setAllItemsOnSale", false, "This sets all shop Items on sale.");
            disableAllSales = cfg.BindSyncedEntry("Discounts", "disableAllSales", false, "This disables all sales (will override setAllItemsOnSale).");
            roundToNearestTen = cfg.BindSyncedEntry("Discounts", "roundToNearestTen", true, "This rounds the percentage numbers to the nearest 10x value (looks nicer).");
            numberOfItemsOnSale = cfg.BindSyncedEntry("Discounts", "numberOfItemsOnSale", numberOfItemsInSaleBaseValue, 
                new ConfigDescription("This sets the amount of items that are on sale.", new AcceptableValueRange<int>(0, 10000)));
            minSalePercentage = cfg.BindSyncedEntry("DiscountPercentageRange", "minSalePercentage", minSalePercentageBaseValue, 
                new ConfigDescription("This sets min discount-percentage of sales for all items that are on sale.", new AcceptableValueRange<int>(0, 100)));
            maxSalePercentage = cfg.BindSyncedEntry("DiscountPercentageRange", "maxSalePercentage", maxSalePercentageBaseValue,
                new ConfigDescription("This sets max discount-percentage of sales for all items that are on sale.", new AcceptableValueRange<int>(0, 100)));

            ConfigManager.Register(this);
        }
    }
}
