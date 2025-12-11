using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BalanceableBreakfast.Model;

public class NutritionDependentThresholdModifier (Dictionary<float, float> fruitPercentageToModifier, Dictionary<float, float> vegetablePercentageToModifier, Dictionary<float, float> proteinPercentageToModifier, Dictionary<float, float> grainPercentageToModifier, Dictionary<float, float> dairyPercentageToModifier)
{
    [JsonPropertyName("fruitPercentageToModifier")]
    public Dictionary<float, float> FruitPercentageToModifier { get; set; } = fruitPercentageToModifier;
    [JsonPropertyName("vegetablePercentageToModifier")]
    public Dictionary<float, float> VegetablePercentageToModifier{ get; set; } = vegetablePercentageToModifier;
    [JsonPropertyName("proteinPercentageToModifier")]
    public Dictionary<float, float> ProteinPercentageToModifier{ get; set; } = proteinPercentageToModifier;
    [JsonPropertyName("grainPercentageToModifier")]
    public Dictionary<float, float> GrainPercentageToModifier{ get; set; } = grainPercentageToModifier;
    [JsonPropertyName("dairyPercentageToModifier")]
    public Dictionary<float, float> DairyPercentageToModifier{ get; set; } = dairyPercentageToModifier;
    
}