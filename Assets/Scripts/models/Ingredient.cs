namespace models
{
    public struct Ingredient
    {
        public string name { get;  }
        public long timeToFire { get;  }
        public long timeToWater { get;  }

        public Ingredient(string name, long timeToFire, long timeToWater)
        {
            this.name = name;
            this.timeToFire = timeToFire;
            this.timeToWater = timeToWater;
        }
        
    }
}