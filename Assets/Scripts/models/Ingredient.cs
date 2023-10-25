namespace models
{
    public struct Ingredient
    {
        public string name { get;  }
        public long timeToCook { get;  }

        public Ingredient(string name, long timeToCook)
        {
            this.name = name;
            this.timeToCook = timeToCook;
        }
    }
}