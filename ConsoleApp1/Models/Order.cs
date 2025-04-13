using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Models
{
	internal class Order
	{
		public int OrderId { get; set; }
		public DateTime OrderDate { get; set; }
		public decimal TotalAmount { get; set; }

		// Foreign key
		public int CustomerId { get; set; }

		// Navigation properties
		public Customer Customer { get; set; }
		public ICollection<OrderDetail> OrderDetails { get; set; }
	}
}
