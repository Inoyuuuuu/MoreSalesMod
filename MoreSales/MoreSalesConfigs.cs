using BepInEx.Configuration;
using System;

namespace MoreSales
{
    internal class MoreSalesConfigs
    {
        internal int numberOfItemsInSaleBaseValue = 10;
        internal int minSalePercentageBaseValue = 20;
        internal int maxSalePercentageBaseValue = 90;


        internal ConfigEntry<bool> isAllItemsOnSale;
        internal ConfigEntry<int> numberOfItemsInSale;
        internal ConfigEntry<int> minSalePercentage;
        internal ConfigEntry<int> maxSalePercentage;

        internal ConfigEntry<string> header;

        internal int actualOfItemsInSale = 10;

        public MoreSalesConfigs(ConfigFile cfg)
        {

            header = cfg.Bind("DiscountPercentageRange", "IMPORTANT", "(＿ ＿*) Z z z", "SOME ITEMS CAN'T BE ON SALE, THOSE WILL BE IGNORED AND WON'T GO ON SALE!" + Environment.NewLine + 
                "All percentage values will be rounded (by the game) to the nearest ten.");
            isAllItemsOnSale = cfg.Bind("Discounts", "setAllItemsOnSale", false, "[This does not work atm!] This sets all shop Items on sale.");
            numberOfItemsInSale = cfg.Bind("Discounts", "numberOfItemsInSale", numberOfItemsInSaleBaseValue, "This sets the number of items in sale.");
            minSalePercentage = cfg.Bind("DiscountPercentageRange", "minSalePercentage", minSalePercentageBaseValue, "This sets min discount-percentage of sales for all items that are on sale." + Environment.NewLine +
                "If min > max, minSalePercentage will be set to maxSalePercentage");
            maxSalePercentage = cfg.Bind("DiscountPercentageRange", "maxSalePercentage", maxSalePercentageBaseValue, "This sets max discount-percentage of sales for all items that are on sale.");

            fixConfigs();
        }

        private void fixConfigs()
        {
            //min and max percentages
            if (maxSalePercentage.Value < 0)
            {
                maxSalePercentage.Value = 0;
            }
            else if (maxSalePercentage.Value > 90)
            {
                maxSalePercentage.Value = 90;
            }

            if (minSalePercentage.Value < 0)
            {
                minSalePercentage.Value = 0;
            }
            else if (minSalePercentage.Value > 90)
            {
                minSalePercentage.Value = 90;
            }

            if (minSalePercentage.Value > maxSalePercentage.Value)
            {
                minSalePercentage.Value = maxSalePercentage.Value;
            }

            //number of items
            if (numberOfItemsInSale.Value < 0)
            {
                numberOfItemsInSale.Value = 0;
            }

            if (isAllItemsOnSale.Value)
            {
                actualOfItemsInSale = 10000;
            } 
            else
            {
                actualOfItemsInSale = numberOfItemsInSale.Value;
            }
        }
    }
}
