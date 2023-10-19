using System.Collections.Generic;

namespace models
{
    public class IngredientMap
    {
        public static Dictionary<string, Ingredient> dict = new Dictionary<string, Ingredient>()
        {
            { "chicken", new Ingredient("chicken", 15000, 0) },
            { "toast", new Ingredient("toast", 5000, 0) },
            { "steak", new Ingredient("steak", 20000, 0) },
            { "bacon", new Ingredient("bacon", 10000, 0) },
            { "croissant", new Ingredient("croissant", 15000, 0) },
            { "tomato", new Ingredient("tomato", 0, 5000) },
            { "pepper", new Ingredient("pepper", 0, 5000) },
            { "strawberry", new Ingredient("strawberry", 0, 5000) },
            { "soup", new Ingredient("soup", 10000, 5000) },
        };
    }
}