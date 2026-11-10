using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationTracker.Practice
{
    public class CollectionsPractice
    {
        public List<String> food = new List<String>();

        // List = data structure that represents a list of objects that can be accessed by index.
        //        Similar to array, but can dynamically increase/decrease in size 

        HashSet<string> emailAddresses = new HashSet<string>();
        //The HashSet<T> class offers developers an efficient way to handle collections of unique elements, ensuring no duplicates are present. For instance, a HashSet<T> is useful in maintaining lists of unique email addresses, where verifying and adding items without repetition is crucial.

        Stack<string> stack = new Stack<string>();
        Queue<string> queue = new Queue<string>();
        //stack is basically a collection of objects that are stored in a last-in, first-out (LIFO) manner. The last item added to the stack is the first one to be removed. A queue is a collection of objects that are stored in a first-in, first-out (FIFO) manner. The first item added to the queue is the first one to be removed.


        public CollectionsPractice()
        {

        }

        public void practiceCollections()
        {
            //list
            food.Add("Pizza");
            food.Add("Burgers");
            food.Add("Pasta");
            food.Add("Salad");
            food.Add("Sushi");

            food.Remove("Salad");
            food.Insert(0, "lamp");
            Console.WriteLine(food.Count);
            Console.WriteLine(food.IndexOf("Pizza"));
            Console.WriteLine(food.LastIndexOf("fries"));
            Console.WriteLine(food.Contains("Pizza"));
            food.Sort();
            food.Reverse();
            String[] foodArray = food.ToArray();

            //hashset
            emailAddresses.Add("user1@example.com");
            emailAddresses.Add("user2@example.com");
            emailAddresses.Add("user1@example.com"); // Duplicate, won't be added

            //dictionary
            //A Dictionary<TKey, TValue> is part of the System.Collections.Generic namespace in C#. It stores data as key-value pairs, where each key is unique and maps to a specific value. This structure is ideal for scenarios where quick lookups are required based on unique identifiers.
            var students = new Dictionary<int, string>
{
            { 101, "Ji-min Jo" },
            { 102, "Catalina Blaga" },
            { 103, "Milan Golob" } };



        stack.Push("A");
        stack.Push("B");
        stack.Push("C");

        Console.WriteLine("Top of stack: " + stack.Peek());   // C (doesn't remove)
        Console.WriteLine("Popped: " + stack.Pop());          // C
        Console.WriteLine("Popped: " + stack.Pop());          // B
        Console.WriteLine("Count: " + stack.Count);           // 1

        // ---------- QUEUE (FIFO) ----------

        queue.Enqueue("Ahmed");
        queue.Enqueue("Sara");
        queue.Enqueue("Omar");

        Console.WriteLine("Front of queue: " + queue.Peek());      // Ahmed (doesn't remove)
        Console.WriteLine("Dequeued: " + queue.Dequeue());         // Ahmed
        Console.WriteLine("Dequeued: " + queue.Dequeue());         // Sara
        Console.WriteLine("Count: " + queue.Count);    //1
            
            
            
            foreach (String item in food)
            {
                Console.WriteLine(item);
                //console.writeline(email);
                //Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
            }


        }
    }
}

       
     



