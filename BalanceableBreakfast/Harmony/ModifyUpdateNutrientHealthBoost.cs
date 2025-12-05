using System.Collections.Generic;
using HarmonyLib;
using Vintagestory.GameContent;

namespace BalanceableBreakfast.Harmony;

[HarmonyPatch(typeof(EntityBehaviorHunger), "UpdateNutrientHealthBoost")]
public class ModifyUpdateNutrientHealthBoost
{
    public static bool Prefix(EntityBehaviorHunger __instance)
    {
        // https://github.com/anegostudios/vsessentialsmod/blob/e9dbc197df1a329b5b8789e2aa086b525ff4d3c8/Entity/Behavior/BehaviorHunger.cs#L352
        var fruitRel = __instance.FruitLevel / __instance.MaxSaturation;
        var grainRel = __instance.GrainLevel / __instance.MaxSaturation;
        var vegetableRel = __instance.VegetableLevel / __instance.MaxSaturation;
        var proteinRel = __instance.ProteinLevel / __instance.MaxSaturation;
        var dairyRel = __instance.DairyLevel / __instance.MaxSaturation;

        var bh = __instance.entity.GetBehavior<EntityBehaviorHealth>();
        
        if (BalanceableBreakfastCore.config == null)
        {
            return true;
        }

        var healthGain = CalculateModifier(
            fruitRel, BalanceableBreakfastCore.config.fruitModifier,
            grainRel, BalanceableBreakfastCore.config.grainModifier,
            vegetableRel, BalanceableBreakfastCore.config.vegetableModifier,
            proteinRel, BalanceableBreakfastCore.config.proteinModifier,
            dairyRel, BalanceableBreakfastCore.config.dairyModifier);
        bh.SetMaxHealthModifiers("nutrientHealthMod", healthGain);
        bh.SetMaxHealthModifiers(BalanceableBreakfastCore.ModId + "StartingHealthMod", BalanceableBreakfastCore.config.startingHealthModifier);
        
        var walkspeedGain = CalculateModifier(
            fruitRel, BalanceableBreakfastCore.config.fruitWalkspeedModifier, 
            grainRel, BalanceableBreakfastCore.config.grainWalkspeedModifier, 
            vegetableRel, BalanceableBreakfastCore.config.vegetableWalkspeedModifier,
            proteinRel, BalanceableBreakfastCore.config.proteinWalkspeedModifier, 
            dairyRel, BalanceableBreakfastCore.config.dairyWalkspeedModifier);
        __instance.entity.Stats["walkspeed"].Set(BalanceableBreakfastCore.ModId+"WalkspeedMod", walkspeedGain);
        __instance.entity.Stats["walkspeed"].Set(BalanceableBreakfastCore.ModId+"StartingWalkspeedMod", BalanceableBreakfastCore.config.startingWalkspeedModifier);
        
        var healingEffectivenessGain = CalculateModifier(
            fruitRel, BalanceableBreakfastCore.config.fruitHealingEffectivenessModifier, 
            grainRel, BalanceableBreakfastCore.config.grainHealingEffectivenessModifier, 
            vegetableRel, BalanceableBreakfastCore.config.vegetableHealingEffectivenessModifier,
            proteinRel, BalanceableBreakfastCore.config.proteinHealingEffectivenessModifier, 
            dairyRel, BalanceableBreakfastCore.config.dairyHealingEffectivenessModifier);
        __instance.entity.Stats["healingeffectivness"].Set(BalanceableBreakfastCore.ModId+"HealingeffectivenessMod", healingEffectivenessGain);
        __instance.entity.Stats["healingeffectivness"].Set(BalanceableBreakfastCore.ModId+"StartingHealingeffectivenessMod", BalanceableBreakfastCore.config.startingHealingEffectivenessModifier);
        
        var hungerRateReduction = CalculateModifier(
            fruitRel, BalanceableBreakfastCore.config.fruitHungerRateModifier, 
            grainRel, BalanceableBreakfastCore.config.grainHungerRateModifier, 
            vegetableRel, BalanceableBreakfastCore.config.vegetableHungerRateModifier,
            proteinRel, BalanceableBreakfastCore.config.proteinHungerRateModifier, 
            dairyRel, BalanceableBreakfastCore.config.dairyHungerRateModifier);
        __instance.entity.Stats["hungerrate"].Set(BalanceableBreakfastCore.ModId+"HungerRateMod", hungerRateReduction);
        __instance.entity.Stats["hungerrate"].Set(BalanceableBreakfastCore.ModId+"StartingHungerRateMod", BalanceableBreakfastCore.config.startingHungerRateModifier);
        
        var miningSpeedGain = CalculateModifier(
            fruitRel, BalanceableBreakfastCore.config.fruitMiningSpeedModifier, 
            grainRel, BalanceableBreakfastCore.config.grainMiningSpeedModifier, 
            vegetableRel, BalanceableBreakfastCore.config.vegetableMiningSpeedModifier,
            proteinRel, BalanceableBreakfastCore.config.proteinMiningSpeedModifier, 
            dairyRel, BalanceableBreakfastCore.config.dairyMiningSpeedModifier);
        __instance.entity.Stats["miningSpeedMul"].Set(BalanceableBreakfastCore.ModId+"MiningSpeedMod", miningSpeedGain);
        __instance.entity.Stats["miningSpeedMul"].Set(BalanceableBreakfastCore.ModId+"StartingMiningSpeedMod", BalanceableBreakfastCore.config.startingMiningSpeedModifier);
        
        return false;
    }

    private static float CalculateModifier(
        float fruitRel, 
        float fruitModifier, 
        float grainRel, 
        float grainModifier,
        float vegetableRel, 
        float vegetableModifier, 
        float proteinRel, 
        float proteinModifier, 
        float dairyRel,
        float dairyModifier)
    {
        var fruitGain = fruitModifier * fruitRel;
        var grainGain = grainModifier * grainRel;
        var vegetableGain = vegetableModifier * vegetableRel;
        var proteinGain = proteinModifier * proteinRel;
        var dairyGain = dairyModifier * dairyRel;

        return fruitGain + grainGain + vegetableGain + proteinGain + dairyGain;
    }
    
}