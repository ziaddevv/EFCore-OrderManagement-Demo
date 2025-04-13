using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Models
{
	internal class Customer
	{
		public int CustomerId { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }

		// Navigation property
		public ICollection<Order> Orders { get; set; }
	}
}
