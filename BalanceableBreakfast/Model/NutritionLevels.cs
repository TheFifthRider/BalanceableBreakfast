namespace BalanceableBreakfast.Model;

public class NutritionLevels (float percentageMaxFruit, float percentageMaxVegetable, float percentageMaxProtein, float percentageMaxGrain, float percentageMaxDairy)
{
    public float PercentageMaxFruit { get; set; } = percentageMaxFruit;
    public float PercentageMaxVegetable{ get; set; } = percentageMaxVegetable;
    public float PercentageMaxProtein{ get; set; } = percentageMaxProtein;
    public float PercentageMaxGrain{ get; set; } = percentageMaxGrain;
    public float PercentageMaxDairy{ get; set; } = percentageMaxDairy;
}