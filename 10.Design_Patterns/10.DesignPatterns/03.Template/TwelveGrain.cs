using System;

namespace _03.Template
{
    public class TwelveGrain: Bread
    {
        public override void MixIngredients()
        {
            Console.WriteLine("Gathering Ingredients for 12-Grain bread!");
        }

        public override void Bake()
        {
            Console.WriteLine("Baking the 12-Grain bread. (25 minutes)!");
        }
    }
}