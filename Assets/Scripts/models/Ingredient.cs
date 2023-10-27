namespace models
{
    public struct Ingredient
    {
        public string name { get;  }
        public float timeToCook { get;  }

        public Ingredient(string name, float timeToCook)
        {
            this.name = name;
            this.timeToCook = timeToCook;
        }
    }
}