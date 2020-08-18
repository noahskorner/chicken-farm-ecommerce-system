using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChickenEcommerceSystem
{
    class MultiCellBuffer
    {
        private static int numOrders = 0;
        private const int MAX_NUM_CELLS = 2;
        string[] buffer = new string[MAX_NUM_CELLS];
        //define semaphores for keeping track of resources
        Semaphore write = new Semaphore(MAX_NUM_CELLS, MAX_NUM_CELLS);
        Semaphore read = new Semaphore(MAX_NUM_CELLS, MAX_NUM_CELLS);
        public void setOneCell(string encodedOrder)
        {
          write.WaitOne(); //'lock' the semaphore
            lock (buffer) //entering critical section -> lock the buffer
            {
                while (numOrders == MAX_NUM_CELLS && ChickenEcommerceSystem.ChickenFarm.isProducing())
                {
                   Monitor.Wait(buffer); //release the lock and wait until it is released by another thread
                }

                if (ChickenEcommerceSystem.ChickenFarm.isProducing()) //if the farm is still producing chickens we can still place orders
                { 
                    //Console.WriteLine("Thread Store {0} is writing to the buffer", Thread.CurrentThread.Name);
                    buffer[numOrders] = encodedOrder; //place order in the buffer
                    numOrders++; //increment number of orders
                    Console.WriteLine("Store {0} has placed an order.", Thread.CurrentThread.Name);
                }

                write.Release(); //'unlock' the semaphore
                Monitor.Pulse(buffer); //wake up other threads that are waiting
            }
        }
        public string getOneCell()
        {
            string encodedOrder = "";
            read.WaitOne(); //'lock' the semaphore
            lock (buffer)
            {
                while (numOrders == 0 && ChickenEcommerceSystem.ChickenFarm.isProducing()) //wait for an order to be placed into the buffer or until chicken farm is done producing chickens
                {
                    Monitor.Wait(buffer); //release the lock and wait until it is release
                }   
                if (ChickenEcommerceSystem.ChickenFarm.isProducing()) //orders will still be processes
                {
                    //Console.WriteLine("Thread {0} is entering the buffer.", Thread.CurrentThread.Name);
                    encodedOrder = buffer[numOrders - 1]; //take order out of buffer
                    numOrders--; //decrement number of orders
                    //Console.WriteLine("Thread {0} has exited the buffer.", Thread.CurrentThread.Name);
                    read.Release(); //'unlock' the semaphore
                    Monitor.Pulse(buffer); //wake up other threads that are waiting
                }
                else //else we are just emptying the buffer
                {
                    numOrders--; //decrement number of orders
                    read.Release(); //'unlock' the semaphore
                    Monitor.Pulse(buffer); //wake up other threads that are waiting
                }
            }
            return encodedOrder;
        }

    }
}
