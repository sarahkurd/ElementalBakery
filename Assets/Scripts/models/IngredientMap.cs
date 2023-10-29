using System.Collections.Generic;

namespace models
{
    public class IngredientMap
    {
        public static Dictionary<string, Ingredient> dict = new Dictionary<string, Ingredient>()
        {
            { "chicken", new Ingredient("chicken", 3.0f, CookType.FIRE) },
            { "toast", new Ingredient("toast", 1.5f, CookType.FIRE) },
            { "steak", new Ingredient("steak", 5.0f, CookType.FIRE) },
            { "bacon", new Ingredient("bacon", 4.0f, CookType.FIRE) },
            { "croissant", new Ingredient("croissant", 1.5f, CookType.FIRE) },
            { "tomato", new Ingredient("tomato",1f, CookType.WATER) },
            { "lettuce", new Ingredient("lettuce", 1f, CookType.WATER) },
            { "blueberry", new Ingredient("blueberry", 3f, CookType.WATER) },
            { "soup", new Ingredient("soup", 5.0f, CookType.AIR) },
        };
    }
}