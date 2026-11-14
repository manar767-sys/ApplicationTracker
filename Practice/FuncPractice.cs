using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTracker.Practice
{

    public class FuncPractice
    {

        public static int Square(int x)
        {
            return x * x;
        }

        public Func<int, int> squareFunc = Square;



    }
}
