using System;

namespace _03.Template
{
    public class Sourdough: Bread
    {
        public override void MixIngredients()
        {
            Console.WriteLine("Gathering Ingredients for Sourdough bread!");
        }

        public override void Bake()
        {
            Console.WriteLine("Baking the Sourdough bread. (20 minutes)!");
        }
    }
}