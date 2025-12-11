using System.Collections.Generic;
using System.Linq;
using BalanceableBreakfast.Config;
using BalanceableBreakfast.Model;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace BalanceableBreakfast.Harmony;

[HarmonyPatch(typeof(EntityBehaviorHunger), "UpdateNutrientHealthBoost")]
public class ModifyUpdateNutrientHealthBoost
{
    // https://github.com/anegostudios/vsessentialsmod/blob/e9dbc197df1a329b5b8789e2aa086b525ff4d3c8/Entity/Behavior/BehaviorHunger.cs#L352
    public static bool Prefix(EntityBehaviorHunger __instance)
    {
        var nutritionLevels = new NutritionLevels(
            __instance.FruitLevel / __instance.MaxSaturation,
            __instance.VegetableLevel / __instance.MaxSaturation,
            __instance.ProteinLevel / __instance.MaxSaturation,
            __instance.GrainLevel / __instance.MaxSaturation,
            __instance.DairyLevel / __instance.MaxSaturation
        );

        var bh = __instance.entity.GetBehavior<EntityBehaviorHealth>();
        
        if (BalanceableBreakfastCore.config == null)
        {
            return true;
        }

        var healthModifier = new NutritionDependentLinearModifier(
            BalanceableBreakfastCore.config.fruitModifier, 
            BalanceableBreakfastCore.config.vegetableModifier,
            BalanceableBreakfastCore.config.proteinModifier,
            BalanceableBreakfastCore.config.grainModifier,
            BalanceableBreakfastCore.config.dairyModifier
            );
        var healthGain = CalculateModifier(nutritionLevels, healthModifier);
        bh.SetMaxHealthModifiers("nutrientHealthMod", healthGain);
        bh.SetMaxHealthModifiers(BalanceableBreakfastCore.ModId + "StartingHealthMod", BalanceableBreakfastCore.config.startingHealthModifier);
        
        var walkspeedModifier = new NutritionDependentLinearModifier(
            BalanceableBreakfastCore.config.fruitWalkspeedModifier, 
            BalanceableBreakfastCore.config.vegetableWalkspeedModifier,
            BalanceableBreakfastCore.config.proteinWalkspeedModifier,
            BalanceableBreakfastCore.config.grainWalkspeedModifier,
            BalanceableBreakfastCore.config.dairyWalkspeedModifier
        );
        var walkspeedGain = CalculateModifier(nutritionLevels, walkspeedModifier);
        __instance.entity.Stats["walkspeed"].Set(BalanceableBreakfastCore.ModId+"WalkspeedMod", walkspeedGain);
        __instance.entity.Stats["walkspeed"].Set(BalanceableBreakfastCore.ModId+"StartingWalkspeedMod", BalanceableBreakfastCore.config.startingWalkspeedModifier);
        
        var healingEffectivenessModifier = new NutritionDependentLinearModifier(
            BalanceableBreakfastCore.config.fruitHealingEffectivenessModifier, 
            BalanceableBreakfastCore.config.vegetableHealingEffectivenessModifier,
            BalanceableBreakfastCore.config.proteinHealingEffectivenessModifier,
            BalanceableBreakfastCore.config.grainHealingEffectivenessModifier,
            BalanceableBreakfastCore.config.dairyHealingEffectivenessModifier
        );
        var healingEffectivenessGain = CalculateModifier(nutritionLevels, healingEffectivenessModifier);
        __instance.entity.Stats["healingeffectivness"].Set(BalanceableBreakfastCore.ModId+"HealingeffectivenessMod", healingEffectivenessGain);
        __instance.entity.Stats["healingeffectivness"].Set(BalanceableBreakfastCore.ModId+"StartingHealingeffectivenessMod", BalanceableBreakfastCore.config.startingHealingEffectivenessModifier);
        
        
        var hungerRateModifier = new NutritionDependentLinearModifier(
            BalanceableBreakfastCore.config.fruitHungerRateModifier, 
            BalanceableBreakfastCore.config.vegetableHungerRateModifier,
            BalanceableBreakfastCore.config.proteinHungerRateModifier,
            BalanceableBreakfastCore.config.grainHungerRateModifier,
            BalanceableBreakfastCore.config.dairyHungerRateModifier
        );
        var hungerRateReduction = CalculateModifier(nutritionLevels, hungerRateModifier);
        __instance.entity.Stats["hungerrate"].Set(BalanceableBreakfastCore.ModId+"HungerRateMod", hungerRateReduction);
        __instance.entity.Stats["hungerrate"].Set(BalanceableBreakfastCore.ModId+"StartingHungerRateMod", BalanceableBreakfastCore.config.startingHungerRateModifier);
        
        var miningSpeedModifier = new NutritionDependentLinearModifier(
            BalanceableBreakfastCore.config.fruitMiningSpeedModifier, 
            BalanceableBreakfastCore.config.vegetableMiningSpeedModifier,
            BalanceableBreakfastCore.config.proteinMiningSpeedModifier,
            BalanceableBreakfastCore.config.grainMiningSpeedModifier,
            BalanceableBreakfastCore.config.dairyMiningSpeedModifier
        );
        var miningSpeedGain = CalculateModifier(nutritionLevels, miningSpeedModifier);
        __instance.entity.Stats["miningSpeedMul"].Set(BalanceableBreakfastCore.ModId+"MiningSpeedMod", miningSpeedGain);
        __instance.entity.Stats["miningSpeedMul"].Set(BalanceableBreakfastCore.ModId+"StartingMiningSpeedMod", BalanceableBreakfastCore.config.startingMiningSpeedModifier);
        
        if (BalanceableBreakfastCore.config.advanced == null)
        {
            return false;
        }
        
        foreach (KeyValuePair<string, Dictionary<string, object>> pair in BalanceableBreakfastCore.config.advanced)
        {
            var name = pair.Key;
            var modifierStyle = pair.Value["modifierStyle"] as string;
            if (modifierStyle == null)
            {
                continue;
            }
            
            var obj = JObject.FromObject(pair.Value);
            switch (modifierStyle.ToLower())
            {
                case ConfigConstants.LINEAR:
                {
                    var linearModifier = obj.ToObject<LinearModifierConfig>();
                    try
                    {
                        var linearModifierModifier = new NutritionDependentLinearModifier(
                            linearModifier.maxFruitModifier,
                            linearModifier.maxVegetableModifier,
                            linearModifier.maxProteinModifier,
                            linearModifier.maxGrainModifier,
                            linearModifier.maxDairyModifier
                        );
                        var linearModifierGain = CalculateModifier(nutritionLevels, linearModifierModifier);
                        __instance.entity.Stats[linearModifier.entityStatName].Set(BalanceableBreakfastCore.ModId+"Starting"+name, linearModifier.startingModifier);
                        __instance.entity.Stats[linearModifier.entityStatName].Set(BalanceableBreakfastCore.ModId+name, linearModifierGain);
                    }
                    catch (KeyNotFoundException _)
                    {
                        // Not huge on blowing up their logs all the time
                    }
                    break;
                }
                case ConfigConstants.THRESHOLD:
                {
                    try 
                    {
                        var thresholdModifier = obj.ToObject<ThresholdModifierConfig>();
                        var nutritionDependentThresholdModifier = new NutritionDependentThresholdModifier(
                            thresholdModifier.fruitPercentageToModifier,
                            thresholdModifier.vegetablePercentageToModifier,
                            thresholdModifier.proteinPercentageToModifier,
                            thresholdModifier.grainPercentageToModifier,
                            thresholdModifier.dairyPercentageToModifier
                        );
                        var thresholdModifierGain = CalculateThresholdModifier(nutritionLevels, nutritionDependentThresholdModifier);
                        __instance.entity.Stats[thresholdModifier.entityStatName].Set(BalanceableBreakfastCore.ModId+"Starting"+name, thresholdModifier.startingModifier);
                        __instance.entity.Stats[thresholdModifier.entityStatName].Set(BalanceableBreakfastCore.ModId+name, thresholdModifierGain);
                    }
                    catch (KeyNotFoundException _)
                    {
                        // Not huge on blowing up their logs all the time
                    }

                    break;
                }
            }
        }
        
        return false;
    }

    private static float CalculateModifier(
        NutritionLevels nutritionLevels, NutritionDependentLinearModifier nutritionDependentLinearModifier)
    {
        var fruitGain = nutritionDependentLinearModifier.ModifierAtMaxFruit * nutritionLevels.PercentageMaxFruit;
        var vegetableGain = nutritionDependentLinearModifier.ModifierAtMaxVegetable * nutritionLevels.PercentageMaxVegetable;
        var grainGain = nutritionDependentLinearModifier.ModifierAtMaxGrain * nutritionLevels.PercentageMaxGrain;
        var proteinGain = nutritionDependentLinearModifier.ModifierAtMaxProtein * nutritionLevels.PercentageMaxProtein;
        var dairyGain = nutritionDependentLinearModifier.ModifierAtMaxDairy * nutritionLevels.PercentageMaxDairy;

        return fruitGain + vegetableGain + grainGain + proteinGain + dairyGain;
    }
    
    private static float CalculateThresholdModifier(
        NutritionLevels nutrition, NutritionDependentThresholdModifier modifier)
    {
        var fruitThreshold = modifier.FruitPercentageToModifier.Keys
            .Where(key => nutrition.PercentageMaxFruit > key )
            .OrderByDescending(key => key)
            .FirstOrDefault(0.0f);
        var fruitGain = modifier.VegetablePercentageToModifier.Get(fruitThreshold);
        
        var vegetableThreshold = modifier.VegetablePercentageToModifier.Keys
            .Where(key => nutrition.PercentageMaxVegetable > key)
            .OrderByDescending(key => key)
            .FirstOrDefault(0.0f);
        var vegetableGain = modifier.VegetablePercentageToModifier.Get(vegetableThreshold);
        
        var proteinThreshold = modifier.ProteinPercentageToModifier.Keys
            .Where(key => nutrition.PercentageMaxProtein > key)
            .OrderByDescending(key => key)
            .FirstOrDefault(0.0f);
        var proteinGain = modifier.ProteinPercentageToModifier.Get(proteinThreshold);
        
        var grainThreshold = modifier.GrainPercentageToModifier.Keys
            .Where(key => nutrition.PercentageMaxGrain > key)
            .OrderByDescending(key => key)
            .FirstOrDefault(0.0f);
        var grainGain = modifier.GrainPercentageToModifier.Get(grainThreshold);
        
        var dairyThreshold = modifier.DairyPercentageToModifier.Keys
            .Where(key => nutrition.PercentageMaxDairy > key)
            .OrderByDescending(key => key)
            .FirstOrDefault(0.0f);
        var dairyGain = modifier.DairyPercentageToModifier.Get(dairyThreshold);
        
        return fruitGain + vegetableGain + grainGain + proteinGain + dairyGain;
    }
    
}