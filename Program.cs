using System.Text;
using ApplicationTracker.Data;
using ApplicationTracker.Models;
using ApplicationTracker.Practice;
using static System.Net.WebRequestMethods;

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

            //sprint one

            //(remove the first company, add a fourth, and the new Id must be 4) and check 3(GetById(99) throws “Item not found”).
            DataStore<Company> companyStore = new DataStore<Company>();
            //companyStore.Remove(companyStore.GetById(1));
            companyStore.Add(new Company { Name = "Company D" });
            //companyStore.GetById(99); // This will throw an exception since there is no company with Id 99
            companyStore.GetAll().ForEach(c => Console.WriteLine($"Company Id: {c.Id}, Name: {c.Name}"));





        }

    }
}
