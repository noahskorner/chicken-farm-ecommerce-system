using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChickenEcommerceSystem
{
	class Decoder
	{
        public OrderClass decode(string encodedObject)
        {
            //Converts encodedObject string into orderObject
            //Decode object into seperate attributes of object
            string[] attributes = new string[5];
            attributes = encodedObject.Split('+');
            //Store attributes
            int senderId = int.Parse(attributes[0]); //id of sender (thread_id) 
            int amount = int.Parse(attributes[2]); //number of chickens to order
            int cardNo = int.Parse(attributes[1]); //credit card number
            DateTime orderDate = DateTime.Parse(attributes[3]);
            int price = int.Parse(attributes[4]);

            OrderClass decodedObject = new OrderClass(senderId, amount, cardNo, price);
            decodedObject.setOrderDate(orderDate);

            return decodedObject;
        }
    }
}
