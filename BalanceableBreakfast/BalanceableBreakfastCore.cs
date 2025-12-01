using System;
using BalanceableBreakfast.Config;
using ConfigLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace BalanceableBreakfast;

public class BalanceableBreakfastCore : ModSystem
{
    public static string configName = "BalanceableBreakfast.json";
    public static BalanceableBreakfastConfig config;
    
    public static ILogger Logger { get; private set; }
    public static string ModId { get; private set; }
    public static ICoreAPI Api { get; private set; }
    public static HarmonyLib.Harmony HarmonyInstance { get; private set; }

    public override void StartPre(ICoreAPI api)
    {
        base.StartPre(api);
        Api = api;
        Logger = Mod.Logger;
        ModId = Mod.Info.ModID;
        HarmonyInstance = new HarmonyLib.Harmony(ModId);
        HarmonyInstance.PatchAll();
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        base.StartServerSide(api);
        LoadConfig(api);
        if (api.ModLoader.IsModEnabled("configlib"))
        {
            SubscribeToConfigChange(api);
        }
    }

    public override void Dispose()
    {
        HarmonyInstance?.UnpatchAll(ModId);
        HarmonyInstance = null;
        Logger = null;
        ModId = null;
        Api = null;
        base.Dispose();
    }
    
    private static void SubscribeToConfigChange(ICoreAPI api)
    {
        ConfigLibModSystem system = api.ModLoader.GetModSystem<ConfigLibModSystem>();

        system.SettingChanged += (domain, _, setting) =>
        {
            if (domain != ModId) return;

            setting.AssignSettingValue(config);
        };

        system.ConfigsLoaded += () =>
        {
            system.GetConfig(ModId)?.AssignSettingsValues(config);
        };
    }
    
    private void LoadConfig(ICoreAPI clientApi)
    {
        try
        {
            config = clientApi.LoadModConfig<BalanceableBreakfastConfig>(configName);
        }
        catch (Exception)
        {
            clientApi.Logger.Error("BalanceableBreakfast: Failed to load mod config!");
            return;
        }

        if (config == null)
        {
            clientApi.Logger.Notification("BalanceableBreakfast: non-existant modconfig at 'ModConfig/" + configName +
                                          "', creating default...");
            config = new BalanceableBreakfastConfig();
            clientApi.StoreModConfig(config, configName);
        }
    }
}