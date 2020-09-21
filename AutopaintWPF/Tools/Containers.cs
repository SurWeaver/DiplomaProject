using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AutopaintWPF
{
	public static class Container_controller
	{
		static public object Create_struct(Tables table, string[] fields)
		{
			object structure = new object();
			switch (table)
			{
				case Tables.car_parts: structure = new Car_part(fields); break;
				case Tables.cars: structure = new Car(fields); break;
				case Tables.cities: structure = new City(fields); break;
				case Tables.clients: structure = new Client(fields); break;
				case Tables.colors: structure = new Color(fields); break;
				case Tables.genders: structure = new Gender(fields); break;
				case Tables.measurements: structure = new Measurement(fields); break;
				case Tables.paint_types: structure = new Paint_type(fields); break;
				case Tables.pictures: structure = new Picture(fields); break;
				case Tables.products: structure = new Product(fields); break;
				case Tables.request_statuses: structure = new Request_status(fields); break;
				case Tables.requests: structure = new Request(fields); break;
				case Tables.roles: structure = new Role(fields); break;
				case Tables.service_types: structure = new Service_type(fields); break;
				case Tables.storage: structure = new Storage(fields); break;
				case Tables.suppliers: structure = new Supplier(fields); break;
				case Tables.supplies: structure = new Supply(fields); break;
				case Tables.users: structure = new User(fields); break;
			}
			return structure;
		}
	}
	
	public enum Tables
	{
		car_parts,
		cars,
		cities,
		clients,
		colors,
		genders,
		measurements,
		paint_types,
		pictures,
		products,
		request_statuses,
		requests,
		roles,
		service_types,
		storage,
		suppliers,
		supplies,
		users
	};

	public enum QueryMode
	{
		add,
		change
	};
	
	public struct Car_part
	{
		public int id { get; set; }
		public string name { get; set; }
		public int surface_size { get; set; }
		public decimal cost { get; set; }
		public Car_part(string[] a)
		{
			id = int.Parse(a[0]);
			name = a[1];
			surface_size = int.Parse(a[2]);
			cost = decimal.Parse(a[3]);
		}
		public string ToString()
		{
			return $"{name} {surface_size} {cost}";
		}
	}
	
	public struct Car
	{
		public string vin { get; set; }
		public string number { get; set; }
		public string owner_mail { get; set; }
		public string color { get; set; }
		public string model { get; set; }

		public Car(string[] a)
		{
			vin = a[0];
			number = a[1];
			owner_mail = a[2];
			color = a[3];
			model = a[4];
		}
		public string ToString()
		{
			return $"{vin} {number} {owner_mail} {color} {model}";
		}
	}

	public struct City
	{
		public string city { get; set; }

		public City(string[] a)
		{
			city = a[0];
		}
		public string ToString()
		{
			return $"{city}";
		}
	}

	public struct Client
	{
		public string mail { get; set; }
		public string phone { get; set; }
		public string surname { get; set; }
		public string first_name { get; set; }
		public string second_name { get; set; }
		public string gender { get; set; }

		public Client(string[] a)
		{
			mail = a[0];
			phone = a[1];
			surname = a[2];
			first_name = a[3];
			second_name = a[4];
			gender = a[5];
		}
		public string ToString()
		{
			return $"{mail} {phone} {surname} {first_name} {second_name} {gender}";
		}
	}

	public struct Color
	{
		public string color_code { get; set; }
		public string description { get; set; }
		public Color(string[] a)
		{
			color_code = a[0];
			description = a[1];
		}
		public string ToString()
		{
			return $"{color_code} {description}";
		}
	}

	public struct Gender
	{
		public string gender { get; set; }
		
		public Gender(string[] a)
		{
			gender = (a[0]);
		}
		public string ToString()
		{
			return $"{gender}";
		}
	}

	public struct Measurement
	{
		public string measurement { get; set; }
		public Measurement(string[] a)
		{
			measurement = a[0];
		}
		public string ToString()
		{
			return $"{measurement}";
		}
	}

	public struct Paint_type
	{
		public string paint_type { get; set; }
		public Paint_type(string[] a)
		{
			paint_type = a[0];
		}
		public string ToString()
		{
			return $"{paint_type}";
		}
	}

	public struct Picture
	{
		public string name { get; set; }
		public decimal price { get; set; }
		public string image { get; set; }
		public Picture(string[] a)
		{
			name = a[0];
			price = decimal.Parse(a[1]);
			image = a[2];
		}
		public string ToString()
		{
			return $"{name} {price}";
		}
	}

	public struct Product
	{
		public string name { get; set; }
		public string paint_type { get; set; }
		public string color_code { get; set; }
		public string measurement { get; set; }
		public Product(string[] a)
		{
			name = a[0];
			paint_type = a[1];
			color_code = a[2];
			measurement = a[3];
		}
		public string ToString()
		{
			return $"{name} {paint_type} {color_code} {measurement}";
		}
	}
	
	public struct Request_status
	{
		public string request_status { get; set; }
		public Request_status(string[] a)
		{
			request_status = a[0];
		}
		public string ToString()
		{
			return $"{request_status}";
		}
	}

	public struct Request
	{
		public int id { get; set; }
		public string vin { get; set; }
		public string product_name { get; set; }
		public string date_order { get; set; }
		public string service_type { get; set; }
		public int parts_to_paint { get; set; }
		public string picture_name { get; set; }
		public string request_status { get; set; }
		public string supplier { get; set; }
		public string paint_date { get; set; }
		public string paint_amount { get; set; }
		public string measurement { get; set; }
		public string paint_cost { get; set; }
		public Request(string[] a)
		{
			id = int.Parse(a[0]);
			vin = a[1];
			product_name = a[2];
			date_order = a[3];
			service_type = a[4];
			parts_to_paint = int.Parse(a[5]);
			picture_name = a[6];
			request_status = a[7];
			supplier = a[8];
			paint_date = a[9];
			paint_amount = a[10].Replace(',','.');
			measurement = a[11];
			paint_cost = a[12].Replace(',', '.');
		}
		public string ToString()
		{
			return $"{vin} {product_name} {date_order} {service_type} {picture_name} {request_status} {supplier} {paint_date} {paint_amount} {paint_cost}";
		}
	}

	public struct Role
	{
		public string role { get; set; }
		public Role(string[] a)
		{
			role = a[0];
		}
		public string ToString()
		{
			return $"{role}";
		}
	}

	public struct Service_type
	{
		public string service_type { get; set; }
		public Service_type(string[] a)
		{
			service_type = a[0];
		}
		public string ToString()
		{
			return $"{service_type}";
		}
	}

	public struct Storage
	{
		public int id { get; set; }
		public string product_name { get; set; }
		public decimal product_amount { get; set; }
		public string measurement { get; set; }
		public string supplier { get; set; }
		public decimal average_purchase_price { get; set; }
		public Storage(string[] a)
		{
			id = int.Parse(a[0]);
			product_name = a[1];
			product_amount = decimal.Parse(a[2]);
			measurement = a[3];
			supplier = a[4];
			average_purchase_price = decimal.Parse(a[5]);
		}
		public string ToString()
		{
			return $"{product_name} {product_amount} {measurement} {supplier} {average_purchase_price}";
		}
	}
	
	public struct Supplier
	{
		public string name { get; set; }
		public string city { get; set; }
		public string address { get; set; }
		public string phone { get; set; }
		public Supplier(string[] a)
		{
			name = a[0];
			city = a[1];
			address = a[2];
			phone = a[3];
		}
		public string ToString()
		{
			return $"{name} {city} {address} {phone}";
		}
	}

	public struct Supply
	{
		public int id { get; set; }
		public string user_mail { get; set; }
		public string supplier { get; set; }
		public string product_name { get; set; }
		public decimal product_amount { get; set; }
		public string measurement { get; set; }
		public decimal price { get; set; }
		public string order_date { get; set; }
		public string delivery_date { get; set; }
		public Supply(string[] a)
		{
			id = int.Parse(a[0]);
			user_mail = a[1];
			supplier = a[2];
			product_name = a[3];
			product_amount = decimal.Parse(a[4]);
			measurement = a[5];
			price = decimal.Parse(a[6]);
			order_date = a[7];
			delivery_date = a[8];
		}
		public string ToString()
		{
			return $"{user_mail} {supplier} {product_name} {product_amount} {measurement} {price} {order_date} {delivery_date}";
		}
	}

	public struct User
	{
		public string mail { get; set; }
		public string password { get; set; }
		public string surname { get; set; }
		public string first_name { get; set; }
		public string second_name { get; set; }
		public string phone { get; set; }
		public string role { get; set; }
		public string gender { get; set; }
		public string image { get; set; }


		public User(string[] a)
		{
			mail = a[0];
			password = a[1];
			surname = a[2];
			first_name = a[3];
			second_name = a[4];
			phone = a[5];
			role = a[6];
			gender = a[7];
			image = a[8];
		}
		public string ToString()
		{
			return $"{mail} {password} {surname} {first_name} {second_name} {phone} {role} {gender}";
		}
	}
}