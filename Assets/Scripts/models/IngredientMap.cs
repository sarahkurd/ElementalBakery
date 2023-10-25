using System.Collections.Generic;

namespace models
{
    public class IngredientMap
    {
        public static Dictionary<string, Ingredient> dict = new Dictionary<string, Ingredient>()
        {
            { "chicken", new Ingredient("chicken", 15000) },
            { "toast", new Ingredient("toast", 5000) },
            { "steak", new Ingredient("steak", 20000) },
            { "bacon", new Ingredient("bacon", 10000) },
            { "croissant", new Ingredient("croissant", 15000) },
            { "tomato", new Ingredient("tomato",5000) },
            { "pepper", new Ingredient("pepper", 5000) },
            { "soup", new Ingredient("soup", 10000) },
        };
    }
}