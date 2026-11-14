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

        // Func<T, TResult> is a delegate type in C# that represents a method that takes a parameter of type T and returns a value of type TResult. It is commonly used for defining methods that perform a calculation or transformation and return a result. The Func<T, TResult> delegate is part of the System namespace and is often used in scenarios where you want to pass a method as an argument to another method or event handler.
        public Func<int, int> squareFunc = Square;



    }
}
