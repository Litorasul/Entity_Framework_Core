using System;

namespace _03.Template
{
    public class WholeWheat: Bread
    {
        public override void MixIngredients()
        {
            Console.WriteLine("Gathering Ingredients for Whole Wheat bread!");
        }

        public override void Bake()
        {
            Console.WriteLine("Baking the Whole Wheat bread. (15 minutes)!");
        }
    }
}