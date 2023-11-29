namespace models
{
    public struct Ingredient
    {
        public string name { get;  }
        public float timeToCook { get;  }

        public CookType cookType { get;  }

        public Ingredient(string name, float timeToCook, CookType cookType)
        {
            this.name = name;
            this.timeToCook = timeToCook;
            this.cookType = cookType;
        }
    }

    public enum CookType
    {
        FIRE, WATER, AIR
    }
}