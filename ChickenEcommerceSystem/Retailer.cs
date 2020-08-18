using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChickenEcommerceSystem
{
    /**Client-Side Functionality**/
    class Retailer //each retailer is a thread
    {
        //contains callback method for when price cut event occurs
        //calculate number of chickens to order based on need and price cut
        //terminates after ChickenFarm thread terminates
        //each order is an OrderClass object
        //each order is sent to Encoder -> sends back encodedObject
        //encodedObject string is send to MultiCellBuffer
        //before sending, time-stamp must be saved
        //upon recieving confirmation of order, calculate and print time of order
        /*
        The retailer will
        calculate the number of chickens to order, for example, based on the need and the difference between the
        previous price and the current price. The thread will terminate after the ChickenFarm thread has
        terminated. Each order is an OrderClass object. The object is sent to the Encoder for encoding. The
        encoded string is sent back to the retailer. Then, the retailer will send the order in String format to the
        MultiCellBuffer. Before sending the order to the MultiCellBuffer, a time stamp must be saved. When the
        confirmation of order completion is received, the time of the order will be calculated and saved (or
        printed). 
        */

        private static bool farmIsProducing = true; //init to true
        private static bool priceCut = false; //wait for a priceCut event
        private static int prevPrice;
        private static int newPrice;
        private static int[] VALID_CC_NUMS = 
        {
            2147483643,
            2147483644,
            2147483645,
            2147483646,
            2147483647
        };
        private Encoder orderEncoder = new Encoder(); //init new encoder

        public void ChickenRetailer() //thread continues until ChickenFarm thread terminates
        {
            ChickenFarm chicken = new ChickenFarm();
            while (farmIsProducing)
            {
                if (priceCut) //if there has been a price cut, have thread create order, then wait 
                {
                    createOrder(Thread.CurrentThread.Name, newPrice);
                    Thread.Sleep(1000); //so retailers can't continuously place orders during a price cut
                }
                //else wait for price cut
            }
            //terminate thread
            Console.WriteLine("Store {0} has closed for the day.", Thread.CurrentThread.Name);
        }
        //event handler for when PricingModel determines a price cut (emits priceCut event)
        public void chickenPriceCut(int _prevPrice, int _newPrice)
        {
            Console.WriteLine("Price cut to ${0}. Previous price was ${1}", _newPrice, _prevPrice);
            prevPrice = _prevPrice;
            newPrice = _newPrice;
            priceCut = true;
        }
        public void chickenPriceRaise()
        {
            priceCut = false; //if price is raised, do not order more chickens
        }
        public void setFarmIsProducing(bool _farmIsProducing)
        {
            //does not need to be syncronized because value should change for all threads
            farmIsProducing = _farmIsProducing;
        }
        public void orderConfirmed(OrderClass confirmedOrder)
        {
            Console.WriteLine("\tOrder has been confirmed. Here is your reciept of purchase:");
            Console.WriteLine("\t----------------------------------------------------------");
            Console.WriteLine(confirmedOrder.ToString());
        }
        private void createOrder(string storeName, int price)
        {
            int senderId = int.Parse(storeName); //store senderId as string

            OrderClass newOrder = new OrderClass(senderId, getCardNo(senderId), suggestedOrderCalc(price), price); //create new order 
            string encodedObject = orderEncoder.encode(newOrder); //encode order before sending
            ChickenEcommerceApplication.mb.setOneCell(encodedObject); //write order to the buffer
        }
        private int suggestedOrderCalc(int currentPrice)
        {
            //simple calculator to find how many chickens a retailer should order.
            //if price is low, buy a lot of chickens and vice versa
            int suggestedOrderAmount = 0;
            switch (currentPrice)
            {
                case 1:
                    suggestedOrderAmount = 10;
                    break;
                case 2:
                    suggestedOrderAmount = 9;
                    break;
                case 3:
                    suggestedOrderAmount = 8;
                    break;
                case 4:
                    suggestedOrderAmount = 7;
                    break;
                case 5:
                    suggestedOrderAmount = 6;
                    break;
                case 6:
                    suggestedOrderAmount = 5;
                    break;
                case 7:
                    suggestedOrderAmount = 4;
                    break;
                case 8:
                    suggestedOrderAmount = 3;
                    break;
                case 9:
                    suggestedOrderAmount = 2;
                    break;
                case 10:
                    suggestedOrderAmount = 1;
                    break;
            }
            return suggestedOrderAmount;
        }
        private int getCardNo(int _senderId)
        {
            //simply returns 1 of the 5 stored VALID_CC_NUMS from retailer
            return VALID_CC_NUMS[_senderId - 1];
        }
    }
}


