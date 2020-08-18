using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ChickenEcommerceSystem
{
	public class OrderClass
	{
        private int senderId; //id of sender (thread_id)
        private int cardNo; //credit card number 
        private int amount; //number of chickens to order
        private int price; //price of chickens when order was placed
        private DateTime orderDate = DateTime.Now; //time order was originally created
        private DateTime orderConfirmedDate; //date when order is confirmed by farmer
        private double totalAmount; //final amount + tax + S&H calculated by OrderProcessor


        public OrderClass(int _senderId, int _cardNo, int _amount, int _price)
        {
            senderId = _senderId;
            cardNo = _cardNo;
            amount = _amount;
            price = _price;
        }

        public int getSenderId()
        {
            return this.senderId;
        }
        public void setSenderId(int _senderId)
        {
            this.senderId = _senderId;
        }
        public int getCardNo()
        {
            return this.cardNo;
        }
        public void setCardNo(int _cardNo)
        {
            this.cardNo = _cardNo;
        }
        public int getAmount()
        {
            return this.amount;
        }
        public void setAmount(int _amount)
        {
            this.amount = _amount;
        }
        public int getPrice()
        {
            return price;
        }
        public void setPrice(int _price)
        {
            price = _price;
        }
        public DateTime getOrderDate()
        {
            return orderDate;
        }
        public void setOrderDate(DateTime _orderDate)
        {
            orderDate = _orderDate;
        }
        public void setOrderConfirmedDate(DateTime _orderDate)
        {
            orderConfirmedDate = _orderDate;
        }
        public void setTotalAmount(double _totalAmount)
        {
            totalAmount = _totalAmount;
        }
        override public string ToString()
        {
            return "\tsenderId: Store " + senderId + "\n\tcardNo: " + cardNo + "\n\tamount: " + amount + 
                "\n\torderDate: " + orderDate + "\n\tprice: $" + price + "\n\torderConfirmedDate: " + 
                orderConfirmedDate + "\n\ttotalAmountDue: " + String.Format("{0:C2}", totalAmount);
        }
    }
}
