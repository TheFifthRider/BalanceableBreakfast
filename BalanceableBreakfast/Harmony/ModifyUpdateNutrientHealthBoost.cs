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

        var healthGain = CalculateHealthGain(fruitRel, grainRel, vegetableRel, proteinRel, dairyRel);
        bh.SetMaxHealthModifiers("nutrientHealthMod", healthGain);
        
        var walkspeedGain = CalculateWalkspeedGain(fruitRel, grainRel, vegetableRel, proteinRel, dairyRel);
        __instance.entity.Stats["walkspeed"].Set(BalanceableBreakfastCore.ModId+"WalkspeedMod", walkspeedGain);
        
        return false;
    }

    private static float CalculateHealthGain(float fruitRel, float grainRel, float vegetableRel, float proteinRel, float dairyRel)
    {
        var fruitHealthGain = BalanceableBreakfastCore.config.fruitModifier * fruitRel;
        var grainHealthGain = BalanceableBreakfastCore.config.grainModifier * grainRel;
        var vegetableHealthGain = BalanceableBreakfastCore.config.vegetableModifier * vegetableRel;
        var proteinHealthGain = BalanceableBreakfastCore.config.proteinModifier * proteinRel;
        var dairyHealthGain = BalanceableBreakfastCore.config.dairyModifier * dairyRel;

        return fruitHealthGain + grainHealthGain + vegetableHealthGain + proteinHealthGain + dairyHealthGain;
    }
    
    private static float CalculateWalkspeedGain(float fruitRel, float grainRel, float vegetableRel, float proteinRel, float dairyRel)
    {
        var fruitWalkspeedGain = BalanceableBreakfastCore.config.fruitWalkspeedModifier * fruitRel;
        var grainWalkspeedGain = BalanceableBreakfastCore.config.grainWalkspeedModifier * grainRel;
        var vegetableWalkspeedGain = BalanceableBreakfastCore.config.vegetableWalkspeedModifier * vegetableRel;
        var proteinWalkspeedGain = BalanceableBreakfastCore.config.proteinWalkspeedModifier * proteinRel;
        var dairyWalkspeedGain = BalanceableBreakfastCore.config.dairyWalkspeedModifier * dairyRel;

        return fruitWalkspeedGain + grainWalkspeedGain + vegetableWalkspeedGain + proteinWalkspeedGain + dairyWalkspeedGain;
    }
}