using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChickenEcommerceSystem
{
    public delegate void priceCutEvent(int prevPrice, int newPrice); //define priceCutEvent delegate
    public delegate void priceRaiseEvent(); //define priceCutEvent delegate
    public class ChickenFarm
	{
        private const int MAX_NUM_DAILTY_PRICE_CUTS = 5;
        private static Random rng = new Random(); //used to generate random numbers
        private static int chickenPrice = 11; //init to highest possible price
        private static int numberOfPriceCuts = 0; //counter of price cuts, thread will terminate after 10 price cuts
        private static bool producing = true;
        private static Decoder orderDecoder = new Decoder();
        private static OrderProcessing orderProcessor = new OrderProcessing();
        private static List<Thread> orderProcessingThreads = new List<Thread>();
        public static event priceCutEvent priceCut; //link event to delegate
        public static event priceRaiseEvent priceRaise; //link event to delegate

        public void PricingModel() //determines price of chickens and fires event if price is cut
        {
            //formula to determine price of chickens
            int newPrice = determineNewPrice();
            //determine if there has been a price cut
            if (newPrice < getCurrentPrice())
            {
                numberOfPriceCuts++; //there has been a price cut
                if (priceCut != null) //there is at least one subscriber to priceCut event
                {
                    priceCut(chickenPrice, newPrice); //emit event to subscribers
                }
            }
            else
            {
                priceRaise(); //emit event to subscribers
            }
            setPrice(newPrice); //always change the price even if not cut
        }
        public void ChickenFarmer() //use PricingModel to determine chicken prices
        {
            while (numberOfPriceCuts < MAX_NUM_DAILTY_PRICE_CUTS) //after 10 price cut iterations, the thread terminates
            {
                PricingModel(); //determine new price of chickens
                Thread.Sleep(1000); //wait to determine new price
                //Consume order from the buffer
                string orderPlaced = ChickenEcommerceApplication.mb.getOneCell();
                //Decode encodedOrder from the buffer
                OrderClass newOrder = orderDecoder.decode(orderPlaced);
                //Create new threads to process orders
                processOrder(newOrder);

            }
            for(int i = 0; i < orderProcessingThreads.Count; i++) //iterate through all the orderProcessing threads
            {
                while (orderProcessingThreads.ElementAt(i).IsAlive); //wait for all orders to be processed
            }

            Console.WriteLine("There have been {0} price cuts.", numberOfPriceCuts);
            Console.WriteLine("{0} is done selling chickens for the day.", Thread.CurrentThread.Name);
            producing = false;
        }
        public static int getCurrentPrice()
        {
            return chickenPrice;
        }
        public static void setPrice(int newPrice)
        {
            chickenPrice = newPrice;
        }
        public static bool isProducing()
        {
            return producing;
        }
        private static int determineNewPrice()
        {
            int newPrice = 10;
            int newRandom = rng.Next(1, 10001);
            //Higher prices have higher probability of being picked
            if (8000 < newRandom && newRandom <= 10000){ newPrice = 10; } //20%
            if (6000 < newRandom && newRandom <= 8000) { newPrice = 9; } //20%
            if (4500 < newRandom && newRandom <= 6000) { newPrice = 8; } //15%
            if (3000 < newRandom && newRandom <= 4500) { newPrice = 7; } //15%
            if (2000 < newRandom && newRandom <= 3000) { newPrice = 6; } //10%
            if (1500 < newRandom && newRandom <= 2000) { newPrice = 5; } //5%
            if (1000 < newRandom && newRandom <= 1500) { newPrice = 4; } //5%
            if (500 < newRandom && newRandom <= 1000) { newPrice = 3; } //5%
            if (250 < newRandom && newRandom <= 500) { newPrice = 2; } //2.5%
            else if (newRandom < 250) { newPrice = 1; } //2.5%

            return newPrice;
        }
        private static void processOrder(OrderClass newOrder)
        {
            Console.WriteLine("Farmer has recieved new order from Store {0}", newOrder.getSenderId());

            Thread processOrder = new Thread( new ParameterizedThreadStart(OrderProcessing.processOrder));
            processOrder.Name = "Order Processer";
            processOrder.Start(newOrder);
            orderProcessingThreads.Add(processOrder);
        }
    }
}
