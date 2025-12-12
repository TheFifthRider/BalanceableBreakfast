using System.Collections.Generic;
using BalanceableBreakfast.Model;
using Newtonsoft.Json.Linq;

namespace BalanceableBreakfast.Config;

public class ConfigAdapter
{
    public static List<string> Blocklist = [];
    
    public static (List<NutritionDependentLinearModifier>, List<NutritionDependentThresholdModifier>) LoadModifiers(BalanceableBreakfastConfig config)
    {
        var linearModifiersInConfig = new List<NutritionDependentLinearModifier>
        {
            new(
                config.startingWalkspeedModifier,
                "walkspeed",
                "WalkSpeedModifier",
                config.fruitWalkspeedModifier,
                config.vegetableWalkspeedModifier,
                config.proteinWalkspeedModifier,
                config.grainWalkspeedModifier,
                config.dairyWalkspeedModifier
            ),
            new(
                config.startingHealingEffectivenessModifier,
                "healingeffectivness",
                "HealingEffectivenessModifier", 
                config.fruitHealingEffectivenessModifier,
                config.vegetableHealingEffectivenessModifier,
                config.proteinHealingEffectivenessModifier,
                config.grainHealingEffectivenessModifier,
                config.dairyHealingEffectivenessModifier
            ),
            new(
                config.startingHungerRateModifier,
                "hungerrate",
                "HungerRateModifier",
                config.fruitHungerRateModifier,
                config.vegetableHungerRateModifier,
                config.proteinHungerRateModifier,
                config.grainHungerRateModifier,
                config.dairyHungerRateModifier
            ),
            new(
                config.startingMiningSpeedModifier,
                "miningSpeedMul",
                "MiningSpeedModifier",
                config.fruitMiningSpeedModifier,
                config.vegetableMiningSpeedModifier,
                config.proteinMiningSpeedModifier,
                config.grainMiningSpeedModifier,
                config.dairyMiningSpeedModifier
            ),
        };
        
        var thresholdModifiersInConfig = new List<NutritionDependentThresholdModifier>();
        foreach (KeyValuePair<string, Dictionary<string, object>> pair in BalanceableBreakfastCore.config.advanced)
        {
            var name = pair.Key;
            if (Blocklist.Contains(name))
            {
                continue;
            }
            if (pair.Value["modifierStyle"] is not string modifierStyle)
            {
                continue;
            }
            
            var obj = JObject.FromObject(pair.Value);
            switch (modifierStyle.ToLower())
            {
                case ConfigConstants.LINEAR:
                {
                    var linearModifier = obj.ToObject<LinearModifierConfig>();
                    linearModifiersInConfig.Add(new NutritionDependentLinearModifier(
                        linearModifier.startingModifier,
                        linearModifier.entityStatName,
                        name,
                        linearModifier.maxFruitModifier,
                        linearModifier.maxVegetableModifier,
                        linearModifier.maxProteinModifier,
                        linearModifier.maxGrainModifier,
                        linearModifier.maxDairyModifier
                    ));
                    break;
                }
                case ConfigConstants.THRESHOLD:
                {
                    var thresholdModifier = obj.ToObject<ThresholdModifierConfig>();
                    thresholdModifiersInConfig.Add(new NutritionDependentThresholdModifier(
                        thresholdModifier.startingModifier,
                        thresholdModifier.entityStatName,
                        name,
                        thresholdModifier.fruitPercentageToModifier,
                        thresholdModifier.vegetablePercentageToModifier,
                        thresholdModifier.proteinPercentageToModifier,
                        thresholdModifier.grainPercentageToModifier,
                        thresholdModifier.dairyPercentageToModifier
                    ));
                    break;
                }
            }
        }
        return (linearModifiersInConfig, thresholdModifiersInConfig);
    }
    
}