using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTracker.Practice
{

    public class DelegatePractice
    {

        public static int Square(int x)
        {
            return x * x;
        }

        // Func<T, TResult> is a delegate type in C# that represents a method that takes a parameter of type T and returns a value of type TResult. It is commonly used for defining methods that perform a calculation or transformation and return a result. The Func<T, TResult> delegate is part of the System namespace and is often used in scenarios where you want to pass a method as an argument to another method or event handler.
        public Func<int, int> squareFunc = Square;

        // Action<T> is a delegate type in C# that represents a method that takes a parameter of type T and does not return a value (void). It is commonly used for defining methods that perform an action or operation without producing a result. The Action<T> delegate is part of the System namespace and is often used in scenarios where you want to pass a method as an argument to another method or event handler, but you don't need to return any value.
        // In this case, the Action<string> delegate is used to define a method that takes a string parameter and performs an action with it, such as printing a message to the console.
        //lambda expression is used to define the method body inline, and it simply writes the provided message to the console using Console.WriteLine.
        public Action<string> printMessage = message =>
        {
            Console.WriteLine(message);
        };

    }
}
