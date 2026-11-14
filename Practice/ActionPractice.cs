using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTracker.Practice
{
    internal class ActionPractice
    {
        // Action<T> is a delegate type in C# that represents a method that takes a single parameter of type T and returns void. It is commonly used for defining methods that perform an action without returning a value. The Action<T> delegate is part of the System namespace and is often used in scenarios where you want to pass a method as an argument to another method or event handler.
        // In this example, the Action<string> delegate is used to define a method that takes a string parameter and prints it to the console.
        // The lambda expression message => Console.WriteLine(message) defines the behavior of the delegate, which is to print the provided message to the console.
        public Action<string> printMessage = message =>
        {
            Console.WriteLine(message);
        };
    }
}
