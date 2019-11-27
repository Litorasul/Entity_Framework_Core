using System;

namespace _02.Composite
{
    class StartUp
    {
        static void Main(string[] args)
        {
            var phone = new SingleGift("Phone", 256);
            phone.CalculateTotalPrice();
            Console.WriteLine();

            var rootBox = new CompositeGift("Root Box", 0); 
            var truckToy = new SingleGift("Truck Toy", 289);
            var plainToy = new SingleGift("Plain Toy", 587);

            rootBox.Add(truckToy);
            rootBox.Add(plainToy);

            var childBox = new CompositeGift("Child Box", 0);
            var soldierToy = new SingleGift("Soldier Toy", 200);

            childBox.Add(soldierToy);
            rootBox.Add(childBox);

            Console.WriteLine($"Total price of this Composite Present is: {rootBox.CalculateTotalPrice()}");
        }
    }
}
