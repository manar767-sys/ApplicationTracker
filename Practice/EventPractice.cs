using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTracker.Practice
{
    public class OrderEventArgs : EventArgs
    {
        public int OrderId { get; }

        public OrderEventArgs(int orderId)
        {
            OrderId = orderId;
        }
    }

    public class OrderService
    {
        public event EventHandler<OrderEventArgs>? OrderCreated;

        public void CreateOrder(int orderId)
        {
            Console.WriteLine("Creating order...");

            OrderCreated?.Invoke(
                this,
                new OrderEventArgs(orderId));
        }
    }

    //we are delegating the responsiblity of handling the price change event to the PriceChangedEventHandler delegate.
    //all we know is that when the price changes, we will call the delegate and pass in the stock and the old price.
    //The delegate will then call all the methods that are subscribed to it.
    public delegate void PriceChangedEventHandler(Stock stock, decimal oldPrice);
    public class Stock
    {
        public string Name { get; }
        public decimal Price { get; set; }
        public event PriceChangedEventHandler? PriceChanged;
        public Stock(string stockName)
        {
            Name = stockName;
        }

        public void UpdatePriceBy(decimal percentChange)
        {
            decimal oldPrice = this.Price;
            this.Price += Math.Round(this.Price * percentChange, 2);
            PriceChanged?.Invoke(this, oldPrice); // Raise the PriceChanged event **FIRING** the event to notify subscribers about the price change
        }
    }
}
