using System.Collections;
using System.Linq.Expressions;
using System.Text;
using ApplicationTracker.Data;
using ApplicationTracker.Menu;
using ApplicationTracker.Models;
using ApplicationTracker.Practice;
using ApplicationTracker.Services;



namespace ApplicationTracker
{
    internal class Program
    {
        static void Notify(object? sender, OrderEventArgs e)
        {
            Console.WriteLine($"Order {e.OrderId} created!");
        }
        static void Main(string[] args)
        {
            CollectionsPractice practice = new CollectionsPractice();
            //practice.practiceCollections();

            //Generics Practice
            var people = new Any<Person>();
            people.Add(new Person { id = 1, firstName = "John", lastName = "Doe" });
            people.Add(new Person { id = 2, firstName = "Jane", lastName = "Smith" });
            //people.PrintItems();
            //Console.WriteLine($"Count of people: {people.Count()}");


            //sprint2 practice 
            DelegatePractice delegatePractice = new DelegatePractice();
            //Console.WriteLine($"Square of 5 is: {delegatePractice.squareFunc(5)}");
            //delegatePractice.printMessage("Application submitted successfully");
            bool result = delegatePractice.isEligible(21);
            //Console.WriteLine(result);

            //a negative value indicates that the first value is less than the second
            //if they are equal, it returns 0
            //Console.WriteLine($"Comparison of 5 and 10: {delegatePractice.compareNumbers(5, 10)}");
            //Console.WriteLine($"Comparison of 5 and 5: {delegatePractice.compareNumbers(5, 5)}");

            //event practice
            var orderService = new OrderService();
            orderService.OrderCreated += Notify; //subscribe to the event of order creation

            EventHandler<OrderEventArgs> OrderEventHandler = 
                (sender, e) => Console.WriteLine($"Order {e.OrderId} created with lambda!");

            //subscribe
            //orderService.OrderCreated += OrderEventHandler;   //order 101 created with lambda
            //orderService.CreateOrder(101);    //order 101 created

            //unsubscribe
            orderService.OrderCreated -= Notify;
            //orderService.OrderCreated += OrderEventHandler;   //order 101 created with lambda

            //_____________________________________________________________________________________
            //Event practice with stock price change **Example TWO**

            var Stock = new Stock("Samsung");
            Stock.Price = 150.00m;

            //subscribe to the PriceChanged event using a lambda expression instead of a named method.**Notify**
            //This allows us to define the event handler inline without creating a separate method.
            Stock.PriceChanged += (stock, oldPrice) =>
            {
                if(stock.Price > oldPrice)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Stock {stock.Name} price increased from {oldPrice} to {stock.Price} - up");
                }
                else if (stock.Price < oldPrice)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Stock {stock.Name} price decreased from {oldPrice} to {stock.Price} - down");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Stock {stock.Name} price remains unchanged at {stock.Price}");
                }
            };

            // after decalring the subscription, we can now update the stock price and see the event being triggered and handled by the subscribed event handler.
            Stock.UpdatePriceBy(0.05m); // Increase price by 5%
            Stock.UpdatePriceBy(-0.02m); // Decrease price by 2%
            Stock.UpdatePriceBy(0.00m); // No change in price


            //_____________________________________project_____________________________________________

            //passing services 
            SeedingData seedingData = new SeedingData();
            CompanyService companyService = new CompanyService();
            RecruiterService recruiterService = new RecruiterService();
            CandidateService candidateService = new CandidateService();
            JobService jobService = new JobService(companyService);

            seedingData.SeedData(companyService, recruiterService, jobService, candidateService);

            MenuHandler menuHandler = new MenuHandler(companyService, recruiterService, candidateService, jobService, new ApplicationService(jobService, candidateService, companyService));
            //menuHandler.DisplayMenu();
           






        }

    }
}
