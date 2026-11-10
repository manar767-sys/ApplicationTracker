using ApplicationTracker.Data;
using ApplicationTracker.Models;
using ApplicationTracker.Practice;
using ApplicationTracker.Services;

namespace ApplicationTracker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CollectionsPractice practice = new CollectionsPractice();
            //practice.practiceCollections();

            var people = new Any<Person>();
            people.Add(new Person { id = 1, firstName = "John", lastName = "Doe" });
            people.Add(new Person { id = 2, firstName = "Jane", lastName = "Smith" });
            //people.PrintItems();
            //Console.WriteLine($"Count of people: {people.Count()}");

            SeedingData sd = new SeedingData();
            sd.



           

        }

    }
}
