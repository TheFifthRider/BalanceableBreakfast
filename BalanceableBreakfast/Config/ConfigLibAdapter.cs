
using ConfigLib;
using ImGuiNET;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace BalanceableBreakfast.Config;

public class ConfigLibAdapter
{
    public void OptionallyRegisterWithConfigLib(ICoreAPI api)
    {
        if (!api.ModLoader.IsModEnabled("configlib"))
        {
            return;
        }
        
        api.ModLoader.GetModSystem<ConfigLibModSystem>().RegisterCustomConfig("balanceablebreakfast", (id, buttons) =>
        {
            if (buttons.Save)
            {
                api.StoreModConfig(BalanceableBreakfastCore.config, BalanceableBreakfastCore.configName);
            }
            if (buttons.Restore)
            {
                BalanceableBreakfastCore.config =
                    api.LoadModConfig<BalanceableBreakfastConfig>(BalanceableBreakfastCore.configName);
            }
            if (buttons.Defaults)
            {
                BalanceableBreakfastCore.config = new();
            }
            UpdateConfig(BalanceableBreakfastCore.config, id);
        });
    }

    private void UpdateConfig(BalanceableBreakfastConfig config, string id)
    {
        ImGui.TextWrapped(Lang.Get(BalanceableBreakfastCore.ModId + ":balanceableBreakfastSettings"));
        config.fruitModifier = OnSlider(id, config.fruitModifier, 0.0f, 10.0f, nameof(config.fruitModifier));
        config.fruitWalkspeedModifier = OnSlider(id, config.fruitWalkspeedModifier, 0.0f, 0.1f, nameof(config.fruitWalkspeedModifier));
        config.proteinModifier = OnSlider(id, config.proteinModifier, 0.0f, 10.0f, nameof(config.proteinModifier));
        config.proteinWalkspeedModifier = OnSlider(id, config.proteinWalkspeedModifier, 0.0f, 0.1f, nameof(config.proteinWalkspeedModifier));
        config.grainModifier = OnSlider(id, config.grainModifier, 0.0f, 10.0f, nameof(config.grainModifier));
        config.grainWalkspeedModifier = OnSlider(id, config.grainWalkspeedModifier, 0.0f, 0.1f, nameof(config.grainWalkspeedModifier));
        config.vegetableModifier = OnSlider(id, config.vegetableModifier, 0.0f, 10.0f, nameof(config.vegetableModifier));
        config.vegetableWalkspeedModifier = OnSlider(id, config.vegetableWalkspeedModifier, 0.0f, 0.1f, nameof(config.vegetableWalkspeedModifier));
        config.dairyModifier = OnSlider(id, config.dairyModifier, 0.0f, 10.0f, nameof(config.dairyModifier));
        config.dairyWalkspeedModifier = OnSlider(id, config.dairyWalkspeedModifier, 0.0f, 0.1f, nameof(config.dairyWalkspeedModifier));
    }

    private float OnSlider(string id, float value, float min, float max, string name)
    {
        float newValue = value;
        ImGui.SliderFloat(Lang.Get(BalanceableBreakfastCore.ModId + ":" + name) + $"##{name}-{id}", ref newValue, min, max);
        return newValue;
    }
    
}