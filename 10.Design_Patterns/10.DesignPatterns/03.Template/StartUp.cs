using System;

namespace _03.Template
{
    class StartUp
    {
        static void Main(string[] args)
        {
            Sourdough sourdough = new Sourdough();
            sourdough.Make();

            TwelveGrain twelveGrain = new TwelveGrain();
            twelveGrain.Make();

            WholeWheat wholeWheat = new WholeWheat();
            wholeWheat.Make();
        }
    }
}
