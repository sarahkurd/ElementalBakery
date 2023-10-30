using System.Collections.Generic;

namespace models
{
    public class IngredientMap
    {
        public static Dictionary<string, Ingredient> dict = new Dictionary<string, Ingredient>()
        {
            { "chicken", new Ingredient("chicken", 10.0f, CookType.FIRE) },
            { "toast", new Ingredient("toast", 2.0f, CookType.FIRE) },
            { "steak", new Ingredient("steak", 6.0f, CookType.FIRE) },
            { "bacon", new Ingredient("bacon", 5.0f, CookType.FIRE) },
            { "croissant", new Ingredient("croissant", 5.0f, CookType.FIRE) },
            { "tomato", new Ingredient("tomato",3.0f, CookType.WATER) },
            { "lettuce", new Ingredient("lettuce", 3.0f, CookType.WATER) },
            { "blueberry", new Ingredient("blueberry", 3.0f, CookType.WATER) },
            { "soup", new Ingredient("soup", 5.0f, CookType.AIR) },
        };
    }
}