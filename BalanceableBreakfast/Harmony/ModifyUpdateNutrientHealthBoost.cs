using System.Collections.Generic;
using System.Linq;
using BalanceableBreakfast.Config;
using BalanceableBreakfast.Model;
using HarmonyLib;
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
            BalanceableBreakfastCore.config.startingHealthModifier,
            "nutrientHealthMod",
            "HealthModifier",
            BalanceableBreakfastCore.config.fruitModifier, 
            BalanceableBreakfastCore.config.vegetableModifier,
            BalanceableBreakfastCore.config.proteinModifier,
            BalanceableBreakfastCore.config.grainModifier,
            BalanceableBreakfastCore.config.dairyModifier
            );
        var healthGain = CalculateModifier(nutritionLevels, healthModifier);
        bh.SetMaxHealthModifiers(healthModifier.ModifierName, healthGain);
        bh.SetMaxHealthModifiers(BalanceableBreakfastCore.ModId + "Starting" + healthModifier.ModifierName, healthModifier.StartingModifier);
        
        var (linearModifiers, thresholdModifiers) = ConfigAdapter.LoadModifiers(BalanceableBreakfastCore.config);
        foreach (var modifier in linearModifiers)
        {
            try
            {
                var statGain = CalculateModifier(nutritionLevels, modifier);
                __instance.entity.Stats[modifier.EntityStatName].Set(
                    BalanceableBreakfastCore.ModId + "Starting" + modifier.ModifierName, modifier.StartingModifier);
                __instance.entity.Stats[modifier.EntityStatName]
                    .Set(BalanceableBreakfastCore.ModId + modifier.ModifierName, statGain);
            }
            catch (KeyNotFoundException e)
            {
                BalanceableBreakfastCore.Logger.Error(e);
                ConfigAdapter.Blocklist.Add(modifier.EntityStatName);
            }
        }

        foreach (var modifier in thresholdModifiers)
        {
            try
            {
                var statGain = CalculateThresholdModifier(nutritionLevels, modifier);
                __instance.entity.Stats[modifier.EntityStatName].Set(
                    BalanceableBreakfastCore.ModId + "Starting" + modifier.ModifierName, modifier.StartingModifier);
                __instance.entity.Stats[modifier.EntityStatName]
                    .Set(BalanceableBreakfastCore.ModId + modifier.ModifierName, statGain);
            }
            catch (KeyNotFoundException e)
            {
                BalanceableBreakfastCore.Logger.Error(e);
                ConfigAdapter.Blocklist.Add(modifier.EntityStatName);
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