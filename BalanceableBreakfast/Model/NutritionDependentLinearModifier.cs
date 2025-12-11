using System.Text.Json.Serialization;

namespace BalanceableBreakfast.Model;

public class NutritionDependentLinearModifier (float modifierAtMaxFruit, float modifierAtMaxVegetable, float modifierAtMaxProtein, float modifierAtMaxGrain, float modifierAtMaxDairy)
{
    [JsonPropertyName("modifierAtMaxFruit")]
    public float ModifierAtMaxFruit { get; set; } = modifierAtMaxFruit;
    [JsonPropertyName("modifierAtMaxVegetable")]
    public float ModifierAtMaxVegetable{ get; set; } = modifierAtMaxVegetable;
    [JsonPropertyName("modifierAtMaxProtein")]
    public float ModifierAtMaxProtein{ get; set; } = modifierAtMaxProtein;
    [JsonPropertyName("modifierAtMaxGrain")]
    public float ModifierAtMaxGrain{ get; set; } = modifierAtMaxGrain;
    [JsonPropertyName("modifierAtMaxDairy")]
    public float ModifierAtMaxDairy{ get; set; } = modifierAtMaxDairy;
}