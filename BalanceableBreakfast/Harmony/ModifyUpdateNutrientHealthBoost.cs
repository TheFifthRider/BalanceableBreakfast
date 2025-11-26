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

        var fruitHealthGain = BalanceableBreakfastCore.config.fruitModifier * fruitRel;
        var grainHealthGain = BalanceableBreakfastCore.config.grainModifier * grainRel;
        var vegetableHealthGain = BalanceableBreakfastCore.config.vegetableModifier * vegetableRel;
        var proteinHealthGain = BalanceableBreakfastCore.config.proteinModifier * proteinRel;
        var dairyHealthGain = BalanceableBreakfastCore.config.dairyModifier * dairyRel;

        var healthGain = fruitHealthGain + grainHealthGain + vegetableHealthGain + proteinHealthGain +
                         dairyHealthGain;

        bh.SetMaxHealthModifiers("nutrientHealthMod", healthGain);
        return false;
    }
}