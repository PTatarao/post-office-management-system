namespace PostOffice.Domain.Entities;

public enum WeightCategory
{
    LessThan1Kg = 1,
    Between1And5Kg = 2,
    MoreThan5Kg = 3
}

public static class WeightCategoryExtensions
{
    public static WeightCategory From(decimal weightKg) =>
        weightKg < 1m ? WeightCategory.LessThan1Kg :
        weightKg <= 5m ? WeightCategory.Between1And5Kg :
        WeightCategory.MoreThan5Kg;
}
