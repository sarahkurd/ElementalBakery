using System.Collections.Generic;

namespace models
{
    public class IngredientMap
    {
        public static Dictionary<string, Ingredient> dict = new Dictionary<string, Ingredient>()
        {
            { "chicken", new Ingredient("chicken", 3.0f) },
            { "toast", new Ingredient("toast", 1.5f) },
            { "steak", new Ingredient("steak", 5.0f) },
            { "bacon", new Ingredient("bacon", 4.0f) },
            { "croissant", new Ingredient("croissant", 1.5f) },
            { "tomato", new Ingredient("tomato",1f) },
            { "pepper", new Ingredient("pepper", 1f) },
            { "soup", new Ingredient("soup", 2.0f) },
        };
    }
}