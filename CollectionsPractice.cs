using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplicationTracker
{
    public class CollectionsPractice
    {
        public List<String> food = new List<String>();

        // List = data structure that represents a list of objects that can be accessed by index.
        //        Similar to array, but can dynamically increase/decrease in size 
        //        using System.Collections.Generic;
        public CollectionsPractice()
        {

        }

        public void practiceList()
        {
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






            foreach (String item in food)
            {
                Console.WriteLine(item);
            }
        }


    }
}