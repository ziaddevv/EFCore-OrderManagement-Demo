using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ConsoleApp1.Data;
using ConsoleApp1.Models;
using System.Collections.Generic;

namespace ConsoleApp1
{
	class Program
	{
		static void Main(string[] args)
		{
			 
			using (var context = new ConsoleDbContext())
			{
				context.Database.EnsureCreated();

				 
				if (!context.Customers.Any())
				{
					SeedDatabase(context);
				}

				 
				DisplayAllCustomers(context);
				DisplayAllProducts(context);
				CreateNewOrder(context);
				DisplayOrdersWithDetails(context);

				Console.WriteLine("Press any key to exit...");
				Console.ReadKey();
			}
		}

		static void SeedDatabase(ConsoleDbContext context)
		{
			Console.WriteLine("Seeding database...");

			 
			var customers = new List<Customer>
			{
				new Customer { Name = "John Smith", Email = "john@example.com", Phone = "555-1234" },
				new Customer { Name = "Jane Doe", Email = "jane@example.com", Phone = "555-5678" }
			};
			context.Customers.AddRange(customers);

			 
			var products = new List<Product>
			{
				new Product { Name = "Laptop", Description = "High-performance laptop", Price = 1200.00m, StockQuantity = 10 },
				new Product { Name = "Smartphone", Description = "Latest model smartphone", Price = 800.00m, StockQuantity = 15 },
				new Product { Name = "Headphones", Description = "Noise-cancelling headphones", Price = 150.00m, StockQuantity = 20 }
			};
			context.Products.AddRange(products);
 
			context.SaveChanges();

		 
			var order = new Order
			{
				CustomerId = customers[0].CustomerId,
				OrderDate = DateTime.Now,
				TotalAmount = products[0].Price + products[2].Price
			};
			context.Orders.Add(order);
			context.SaveChanges();
 
			var orderDetails = new List<OrderDetail>
			{
				new OrderDetail { OrderId = order.OrderId, ProductId = products[0].ProductId, Quantity = 1, UnitPrice = products[0].Price },
				new OrderDetail { OrderId = order.OrderId, ProductId = products[2].ProductId, Quantity = 1, UnitPrice = products[2].Price }
			};
			context.OrderDetails.AddRange(orderDetails);

			context.SaveChanges();
			Console.WriteLine("Database seeded successfully!");
		}

		static void DisplayAllCustomers(ConsoleDbContext context)
		{
			Console.WriteLine("\n--- All Customers ---");
			var customers = context.Customers.ToList();
			foreach (var customer in customers)
			{
				Console.WriteLine($"ID: {customer.CustomerId}, Name: {customer.Name}, Email: {customer.Email}");
			}
		}

		static void DisplayAllProducts(ConsoleDbContext context)
		{
			Console.WriteLine("\n--- All Products ---");
			var products = context.Products.ToList();
			foreach (var product in products)
			{
				Console.WriteLine($"ID: {product.ProductId}, Name: {product.Name}, Price: ${product.Price}, In Stock: {product.StockQuantity}");
			}
		}

		static void CreateNewOrder(ConsoleDbContext context)
		{
			Console.WriteLine("\n--- Creating a New Order ---");

			 
			var customer = context.Customers.First();

		 
			var products = context.Products.Take(2).ToList();

			 
			var order = new Order
			{
				CustomerId = customer.CustomerId,
				OrderDate = DateTime.Now,
				TotalAmount = products.Sum(p => p.Price)
			};

			context.Orders.Add(order);
			context.SaveChanges();

			 
			foreach (var product in products)
			{
				var orderDetail = new OrderDetail
				{
					OrderId = order.OrderId,
					ProductId = product.ProductId,
					Quantity = 1,
					UnitPrice = product.Price
				};
				context.OrderDetails.Add(orderDetail);

				 
				product.StockQuantity -= 1;
				context.Products.Update(product);
			}

			context.SaveChanges();
			Console.WriteLine($"Order created with ID: {order.OrderId} for customer: {customer.Name}");
		}

		static void DisplayOrdersWithDetails(ConsoleDbContext context)
		{
			Console.WriteLine("\n--- Orders with Details ---");

			var orders = context.Orders
				.Include(o => o.Customer)
				.Include(o => o.OrderDetails)
					.ThenInclude(od => od.Product)
				.ToList();

			foreach (var order in orders)
			{
				Console.WriteLine($"Order ID: {order.OrderId}, Date: {order.OrderDate.ToShortDateString()}, Customer: {order.Customer.Name}");
				Console.WriteLine("Order Details:");

				foreach (var detail in order.OrderDetails)
				{
					Console.WriteLine($"  - Product: {detail.Product.Name}, Qty: {detail.Quantity}, Price: ${detail.UnitPrice}");
				}

				Console.WriteLine($"Total Amount: ${order.TotalAmount}\n");
			}
		}
		static void UpdateCustomerInformation(ConsoleDbContext context)
		{
			Console.WriteLine("\n--- Updating Customer Information ---");

			 
			var customer = context.Customers.FirstOrDefault();
			if (customer == null)
			{
				Console.WriteLine("No customers found to update.");
				return;
			}

			 
			Console.WriteLine($"Current info - Name: {customer.Name}, Email: {customer.Email}, Phone: {customer.Phone}");

			 
			customer.Phone = "555-9999";
			customer.Email = $"updated.{customer.Email}";

			 
			context.SaveChanges();

			 
			var updatedCustomer = context.Customers.Find(customer.CustomerId);
			Console.WriteLine($"Updated info - Name: {updatedCustomer.Name}, Email: {updatedCustomer.Email}, Phone: {updatedCustomer.Phone}");
		}
		static void UpdateProductPrice(ConsoleDbContext context)
		{
			Console.WriteLine("\n--- Updating Product Prices ---");

			// Find products to update
			var products = context.Products.ToList();
			if (!products.Any())
			{
				Console.WriteLine("No products found to update.");
				return;
			}

			foreach (var product in products)
			{
				 
				Console.WriteLine($"Product: {product.Name}, Current Price: ${product.Price}");

				 
				decimal oldPrice = product.Price;
				product.Price = Math.Round(product.Price * 1.1m, 2);

				 
				context.Products.Update(product);

				Console.WriteLine($"  - Updated price from ${oldPrice} to ${product.Price}");
			}

			 
			context.SaveChanges();
			Console.WriteLine("All product prices updated successfully!");
		}

		static void DeleteOrder(ConsoleDbContext context)
		{
			Console.WriteLine("\n--- Deleting an Order ---");

			// Find an order to delete
			var order = context.Orders
				.Include(o => o.OrderDetails)
				.Include(o => o.Customer)
				.FirstOrDefault();

			if (order == null)
			{
				Console.WriteLine("No orders found to delete.");
				return;
			}

			Console.WriteLine($"Found Order ID: {order.OrderId} from Customer: {order.Customer.Name} with {order.OrderDetails.Count} items");

	 
		 
			Console.WriteLine("Removing associated order details...");
			context.OrderDetails.RemoveRange(order.OrderDetails);

			 
			 
			context.Orders.Remove(order);

			 
			context.SaveChanges();

			 
			var deletedOrder = context.Orders.Find(order.OrderId);
			if (deletedOrder == null)
			{
				Console.WriteLine($"Order ID: {order.OrderId} was successfully deleted!");
			}
			else
			{
				Console.WriteLine("Failed to delete the order.");
			}
		}
		static void DeleteProduct(ConsoleDbContext context)
		{
			Console.WriteLine("\n--- Deleting a Product ---");

			 
			var productToDelete = context.Products
				.Include(p => p.OrderDetails)
				.LastOrDefault();

			if (productToDelete == null)
			{
				Console.WriteLine("No products found to delete.");
				return;
			}

			Console.WriteLine($"Found Product ID: {productToDelete.ProductId}, Name: {productToDelete.Name}");

			 
			if (productToDelete.OrderDetails.Any())
			{
				Console.WriteLine($"Cannot delete product '{productToDelete.Name}' because it has {productToDelete.OrderDetails.Count} order references.");
				Console.WriteLine("Would you like to delete it anyway? (y/n)");

				string response = Console.ReadLine().ToLower();
				if (response != "y")
				{
					Console.WriteLine("Product deletion cancelled.");
					return;
				}

				 
				context.OrderDetails.RemoveRange(productToDelete.OrderDetails);
			}

	 
			context.Products.Remove(productToDelete);
 
			context.SaveChanges();

	 
			var deletedProduct = context.Products.Find(productToDelete.ProductId);
			if (deletedProduct == null)
			{
				Console.WriteLine($"Product '{productToDelete.Name}' was successfully deleted!");
			}
			else
			{
				Console.WriteLine("Failed to delete the product.");
			}
		}

		static void BulkUpdateProductInventory(ConsoleDbContext context)
		{
			Console.WriteLine("\n--- Bulk Updating Product Inventory ---");

			 
			var lowStockProducts = context.Products
				.Where(p => p.StockQuantity < 10)
				.ToList();

			if (!lowStockProducts.Any())
			{
				Console.WriteLine("No products with low stock found.");
				return;
			}

			Console.WriteLine($"Found {lowStockProducts.Count} products with low stock:");

			foreach (var product in lowStockProducts)
			{
				int oldStock = product.StockQuantity;
				 
				Random rand = new Random();
				int addedStock = rand.Next(5, 16);
				product.StockQuantity += addedStock;

				Console.WriteLine($"  - {product.Name}: Stock increased from {oldStock} to {product.StockQuantity} (+{addedStock})");
			}

			 
			context.SaveChanges();
			Console.WriteLine("Inventory update completed successfully!");
		}
	}
}