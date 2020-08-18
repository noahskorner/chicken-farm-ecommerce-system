using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChickenEcommerceSystem
{
	class Encoder
	{
		public string encode(OrderClass order)
		{
			//Converts Order Object into a string
			string encodedString = "";
			encodedString += order.getSenderId();
			encodedString += "+"; //terminal token
			encodedString += order.getAmount();
			encodedString += "+"; //terminal token
			encodedString += order.getCardNo();
			encodedString += "+"; //terminal token
			encodedString += order.getOrderDate();
			encodedString += "+"; //terminal token
			encodedString += order.getPrice();
			return encodedString;
		}
	}
}
