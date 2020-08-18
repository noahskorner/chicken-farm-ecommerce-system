using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChickenEcommerceSystem
{
	public delegate void orderConfirmationEvent(OrderClass confirmedOrder); //define priceCutEvent delegate
	class OrderProcessing
	{
		public static event orderConfirmationEvent orderConfirmation; //link event to delegate
		private const double TAX_RATE = 0.07; 
		private const double SH_PER_CHICKEN = 1; //$1 S&H per chicken
		private static int[] VALID_CC_NUMS =
		{
			2147483643,
			2147483644,
			2147483645,
			2147483646,
			2147483647
		};
		public static void processOrder(object _order)
		{
			OrderClass order = (OrderClass)_order; //cast to order object

			if (validateCreditCard(order.getSenderId(), order.getCardNo())){
				order.setTotalAmount(totalChargeAmount(order.getAmount(), order.getPrice()));
				order.setOrderConfirmedDate(DateTime.Now);
				orderConfirmation(order); //confirms the orders
			}
			else
			{
				Console.WriteLine("Invalid Credit Card Number. Order not confirmed.");
			}
		}
		private static bool validateCreditCard(int storeNum, int ccNum)
		{
			//returns true if credit card is from correct store
			bool isValid = false;

			if(VALID_CC_NUMS[storeNum - 1] == ccNum)
			{
				isValid = true;
			}

			return isValid;
		}
		private static double totalChargeAmount(int amount, int price)
		{
			int totalPreTax = amount * price;
			double totalTax = totalPreTax * TAX_RATE;
			double totalSH = amount * SH_PER_CHICKEN;
			double totalAmount = totalPreTax + totalTax + totalSH;
			return totalAmount;
		}

	}
}
