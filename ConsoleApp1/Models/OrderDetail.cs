﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Models
{
	internal class OrderDetail
	{
		public int OrderDetailId { get; set; }
		public int Quantity { get; set; }
		public decimal UnitPrice { get; set; }

		// Foreign keys
		public int OrderId { get; set; }
		public int ProductId { get; set; }

		// Navigation properties
		public Order Order { get; set; }
		public Product Product { get; set; }
	}
}
