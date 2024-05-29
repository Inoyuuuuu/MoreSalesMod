using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace MoreSales
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class MoreSales : BaseUnityPlugin
    {
        public static MoreSales Instance { get; private set; } = null!;
        internal new static ManualLogSource mls { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }
        internal static MoreSalesConfigs moreSalesConfigs;

        private void Awake()
        {
            mls = base.Logger;
            Instance = this;
            moreSalesConfigs = new MoreSalesConfigs(base.Config);
            Patch();

            mls.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }

        internal static void Patch()
        {
            Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

            mls.LogDebug("Patching...");

            Harmony.PatchAll();

            mls.LogDebug("Finished patching!");
        }

        internal static void Unpatch()
        {
            mls.LogDebug("Unpatching...");

            Harmony?.UnpatchSelf();

            mls.LogDebug("Finished unpatching!");
        }
    }
}
