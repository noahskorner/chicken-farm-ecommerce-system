using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

/* Operation Scenario
 * 1. ChickenFarm uses PricingModel to determine price of chicken. If new chickenPrice is lower than the previous price, fire a priceCut event
 * 2. Retailer evaluates chickenPrice, generates Order object, and sends order to encoder 
 *      2a.) Encoder encodes object into string and sends back to Retailer
 * 3. Retailer (produces) sends encoded string into one of the FREE cells in MultiCellBuffer
 * 4. ChickenFarm (consumes) encoded string from MultiCellBuffer 
 *      4a. ChickenFarm sends string to Decoder
 *      4b. Decoder sends back decoded Order object
 * 5. ChickenFarm creates new thead to process the order
 * 6. OrderProcessingThread processes the order 
 *      6a. Validates Credit Card 
 *      6b. Calculates amount
 * 7. OrderProcessingThread sends confirmation to retailer and prints the order
 */
namespace ChickenEcommerceSystem
{
    /**Main Entry / GUI Functionality**/
    class ChickenEcommerceApplication
    {
        public static  MultiCellBuffer mb = new MultiCellBuffer(); //init multiCellBuffer to share between farmers and retailers
        private const int MAX_NUM_RETAILERS = 5;
        static void Main(string[] args)
        {
            ChickenFarm chickenFarm = new ChickenFarm(); //init chicken farm
            //Initialize Farmer (Producer) thread
            Thread farmer = new Thread(new ThreadStart(chickenFarm.ChickenFarmer));
            farmer.Name = "Farmer"; 
            farmer.Start(); //Start farmer thread

            //Initialize Retailer (Consumer) threads
            Retailer chickenStore = new Retailer(); //init ChickenStore
            ChickenFarm.priceCut += new priceCutEvent(chickenStore.chickenPriceCut); //Subscribe chickenStore to Price Cut event
            ChickenFarm.priceRaise += new priceRaiseEvent(chickenStore.chickenPriceRaise); //Subscribe to Price Raise event
            OrderProcessing.orderConfirmation += new orderConfirmationEvent(chickenStore.orderConfirmed);

            Thread[] retailerThreads = new Thread[MAX_NUM_RETAILERS]; //init retailerThreads array
            for (int i = 0; i < MAX_NUM_RETAILERS; i++)
            {
                retailerThreads[i] = new Thread(new ThreadStart(chickenStore.ChickenRetailer));
                retailerThreads[i].Name = (i + 1).ToString();
                retailerThreads[i].Start();
            }

            //Wait for 10 price cuts
            while (farmer.IsAlive) ;
            //Tell retailers that farm is not producing anymore
            chickenStore.setFarmIsProducing(false);

            //wait for retailers to close
   
            for(int i = 0; i < MAX_NUM_RETAILERS; i++)
            {
                while (retailerThreads[i].IsAlive) 
                    mb.getOneCell(); //empty the buffer
            }
            
            //Wait for user input to end program
            Console.WriteLine("Enter any key to end the program...");
            Console.ReadLine();
        }
    }
}

