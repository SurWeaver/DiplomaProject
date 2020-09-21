using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using MySql.Data.MySqlClient;

namespace AutopaintWPF
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		bool exit_program = true; // Переменная на выход из всей программы или только из аккаунта
		public User current_user;
		public AuthWindow FirstWindow;

		public static string[] tables = {
			"car_parts",
			"cars",
			"cities",
			"clients",
			"colors",
			"genders",
			"measurements",
			"paint_types",
			"pictures",
			"products",
			"request_statuses",
			"requests",
			"roles",
			"service_types",
			"storage",
			"suppliers",
			"supplies",
			"users"
		};
		public static string[] ru_tables = {
			"Части автомобиля",
			"Автомобили",
			"Город",
			"Клиенты",
			"Цвета",
			"Пол",
			"Единица измерения",
			"Тип краски",
			"Изображения",
			"Продукция",
			"Статус заявки",
			"Заявки",
			"Роль",
			"Тип обслуживания",
			"Склад",
			"Поставщики",
			"Поставки",
			"Пользователи"
		};

		string current_primary_key_name;
		List<string> primary_key_values = new List<string>();
		//Текущая и стартовая таблица
		Tables current_table = Tables.users;
		public static Dictionary<string, string> fields = new Dictionary<string, string>
		{
			//Поля таблиц без повторов
			//Автомобили
			{ "vin", "VIN"},
			{ "number", "Регистрационный номер"},
			{ "owner_mail", "Почта владельца"},
			{ "color", "Цвет"},
			{ "model", "Модель"},
			//Части автомобиля
			{ "id", "Код идентификации"},
			{ "name", "Название"},
			{ "surface_size", "Кол-во краски на деталь (мл.)"},
			{ "cost", "Стоимость на покраску (руб.)"},
			//Цвета
			{ "description", "Описание"},
			{ "color_code", "Код цвета"},
			//Единицы измерения
			{ "measurement", "Единица измерения"},
			//Типы краски
			{ "paint_type", "Тип краски"},
			//Изображения
			{ "image", "Изображение"},
			//Поставки
			{ "user_mail", "Почта заказчика"},
			{ "supplier", "Поставщик"},
			{ "product_name", "Наименование продукции"},
			{ "product_amount", "Количество продукции"},
			{ "price", "Цена(руб.)"},
			{ "order_date", "Дата заказа"},
			{ "delivery_date", "Дата привоза"},
			//Поставщики/Города
			{ "city", "Город"},
			//Заявки
			{ "date", "Дата/Время" },
			{ "date_order", "Дата/Время заказа" },
			{ "paint_date", "Дата/Время покраски" },
			{ "service_type", "Тип обслуживания"},
			{ "parts_to_paint", "Части на покраску"},
			{ "picture_name", "Название изображения"},
			{ "request_status", "Статус заявки"},
			{ "paint_amount", "Количество затраченной краски" },
			{ "paint_cost", "Стоимость затраченной краски (руб.)" },
			//Склад
			{ "average_purchase_price", "Средняя закупочная цена(руб.)"},
			// Пользователи/Клиенты
			{ "role", "Роль"},
			{ "address", "Адрес"},
			{ "phone", "Телефон"},
			{ "surname", "Фамилия"},
			{ "first_name", "Имя"},
			{ "second_name", "Отчество"},
			{ "password", "Пароль"},
			{ "mail", "Почта"},
			{ "gender", "Пол"}
		};

		MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user id = root; password = 1234; port = 3306; persistsecurityinfo = True; sslmode = None; database = autopaint");
		public MainWindow(User user, AuthWindow parent_window)
		{
			InitializeComponent();

			FirstWindow = parent_window;
			current_user = user;
			Title = $"Режим работы: {current_user.role}";
			
			//Заполнение списка таблиц
			foreach (string i in ru_tables)
			{
				ComboBoxTables.Items.Add(i);
			}
			ComboBoxTables.SelectedIndex = (int)current_table;
			
			//Заполнение таблицы
			fill_table();
		}
		//Используемые методы
		private bool confirm_action(string message, string title)
		{
			MessageBoxResult result = MessageBox.Show(message, title, MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes)
			{
				return true;
			}
			else
				return false;
		}
		public void fill_table()
		{
			clear_table();
			try
			{
				connection.Open();
				MySqlCommand command = new MySqlCommand("SELECT * FROM `" + tables[(int)current_table] + "`;", connection);
				MySqlDataReader data = command.ExecuteReader();
				current_primary_key_name = data.GetName(0);
				primary_key_values.Clear();
				//Создание и именование столбцов
				for (int i = 0; i < data.FieldCount; i++)
				{
					DataGridTextColumn column = new DataGridTextColumn();
					column.Binding = new Binding(data.GetName(i));
					column.Header = fields[data.GetName(i)];
					DataGrid.Columns.Add(column);
				}
				//Заполнение строк данными из базы
				while (data.Read())
				{
					primary_key_values.Add(data[0].ToString());
					string[] values = new string[data.FieldCount];
					for (int i = 0; i < data.FieldCount; i++)
					{
						values[i] = data[i].ToString();
					}
					DataGrid.Items.Add(Container_controller.Create_struct(current_table, values));
				}
				if (data.GetName(0) == "id")
					DataGrid.Columns[0].Visibility = Visibility.Collapsed;
				if (data.GetName(data.FieldCount-1) == "image")
				{
					DataGrid.Columns.Remove(DataGrid.Columns[DataGrid.Columns.Count - 1]);
				}
				if (current_table == Tables.requests)
				{
					DataGrid.Columns.Remove(DataGrid.Columns[data.GetOrdinal("parts_to_paint")]);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				connection.Close();
			}
		}
		private void clear_table()
		{
			DataGrid.Items.Clear();
			DataGrid.Columns.Clear();
			DataImage.Source = null;
			Border_color.Visibility = Visibility.Collapsed;
		}

		//Смена таблицы при изменении ComboBox'а
		private void ComboBoxTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			current_table = (Tables)ComboBoxTables.SelectedIndex;
			TextBox_search.Text = "";
			TextBlock_car_parts.Text = "";
			TextBlock_car_parts.Visibility = Visibility.Collapsed;
			fill_table();
			Button_image_change.Visibility = Visibility.Collapsed;
			Button_add.Visibility = Visibility.Visible;
			Button_delete.Visibility = Visibility.Visible;
			Button_change.Visibility = Visibility.Visible;
			Border_color.Visibility = Visibility.Collapsed;
			MainPanel.Orientation = (current_table == Tables.users || current_table == Tables.requests) ? Orientation.Vertical : Orientation.Horizontal;

			switch (current_table)
			{
				case Tables.car_parts:
					Button_add.Visibility = Visibility.Collapsed;
					Button_delete.Visibility = Visibility.Collapsed;
					break;
				case Tables.requests:
					TextBlock_car_parts.Visibility = Visibility.Visible;
					TextBlock_car_parts.Text = "";
					break;
			}
		}

		//Полное закрытие программы
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (exit_program)
			{
				if (confirm_action("Вы хотите выйти из программы?", "Закрытие программы"))
				{
					FirstWindow.Close();
				}
				else
					e.Cancel = true;
			}
			else
			{
				FirstWindow.Show();
				FirstWindow.textbox_mail.Text = "";
				FirstWindow.passwordbox_password.Password = "";
			}
		}

		//Выход из программы по нижней кнопке
		private void Button_exit_program_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		//Удаление выбранной записи
		private void Button_delete_Click(object sender, RoutedEventArgs e)
		{
			object item = DataGrid.SelectedItem;
			if (item != null)
			{
				string item_name = primary_key_values[DataGrid.Items.IndexOf(item)];
				if (current_table == Tables.users && item_name == current_user.mail)
				{
					MessageBox.Show("Вы не можете удалить свой аккаунт.");
					return;
				}
				if (current_table == Tables.measurements)
				{
					switch(((Measurement)item).measurement)
					{
						case "литр":
						case "рулон":
							MessageBox.Show("Вы не можете удалять записи, используемые в системе.");
							return;
					}
				}
				if (current_table == Tables.request_statuses)
				{
					switch (((Request_status)item).request_status)
					{
						case "Обработано":
						case "Ожидает обработки":
							MessageBox.Show("Вы не можете удалять записи, используемые в системе.");
							return;
					}
				}
				if (current_table == Tables.roles)
				{
					switch (((Role)item).role)
					{
						case "администратор":
						case "менеджер по заявкам":
						case "менеджер по поставкам":
							MessageBox.Show("Вы не можете удалять записи, используемые в системе.");
							return;
					}
				}
				if (current_table == Tables.service_types)
				{
					switch (((Service_type)item).service_type)
					{
						case "Аэрография":
						case "Детальная":
						case "Оклейка плёнкой":
						case "Полная":
							MessageBox.Show("Вы не можете удалять записи, используемые в системе.");
							return;
					}
				}
				if (confirm_action("Вы хотите удалить текущий объект?", "Удаление"))
				{
					string primary_key_value = DataGrid.SelectedItem.ToString().Split(' ')[0];
					switch(current_table)
					{
						case Tables.cars: primary_key_value = ((Car)item).vin; break;
						case Tables.cities: primary_key_value = ((City)item).city; break;
						case Tables.clients: primary_key_value = ((Client)item).mail; break;
						case Tables.colors: primary_key_value = ((Color)item).color_code; break;
						case Tables.genders: primary_key_value = ((Gender)item).gender; break;
						case Tables.measurements: primary_key_value = ((Measurement)item).measurement; break;
						case Tables.paint_types: primary_key_value = ((Paint_type)item).paint_type; break;
						case Tables.pictures: primary_key_value = ((Picture)item).name; break;
						case Tables.products: primary_key_value = ((Product)item).name; break;
						case Tables.requests: primary_key_value = ((Request)item).id.ToString(); break;
						case Tables.request_statuses: primary_key_value = ((Request_status)item).request_status; break;
						case Tables.roles: primary_key_value = ((Role)item).role; break;
						case Tables.service_types: primary_key_value = ((Service_type)item).service_type; break;
						case Tables.storage: primary_key_value = ((Storage)item).id.ToString(); break;
						case Tables.suppliers: primary_key_value = ((Supplier)item).name; break;
						case Tables.supplies: primary_key_value = ((Supply)item).id.ToString(); break;
						case Tables.users: primary_key_value = ((User)item).mail; break;
					}
					Shortcuts.execute_command($@"DELETE FROM `{tables[(int)current_table]}` where `{current_primary_key_name}` = '{primary_key_value}';", connection);
					TextBox_search.Text = "";
					fill_table();
				}
			}
			else
			{
				MessageBox.Show("Выберите мышью запись перед удалением.");
			}
		}
		//ИЗМЕНЕНИЕ
		private void Button_change_Click(object sender, RoutedEventArgs e)
		{
			object item = DataGrid.SelectedItem;
			if (item != null)
			{
				if (current_table == Tables.measurements)
				{
					switch (((Measurement)item).measurement)
					{
						case "литр":
						case "рулон":
							MessageBox.Show("Вы не можете изменять записи, используемые в системе.");
							return;
					}
				}
				if (current_table == Tables.request_statuses)
				{
					switch (((Request_status)item).request_status)
					{
						case "Обработано":
						case "Ожидает обработки":
							MessageBox.Show("Вы не можете изменять записи, используемые в системе.");
							return;
					}
				}
				if (current_table == Tables.roles)
				{
					switch (((Role)item).role)
					{
						case "администратор":
						case "менеджер по заявкам":
						case "менеджер по поставкам":
							MessageBox.Show("Вы не можете изменять записи, используемые в системе.");
							return;
					}
				}
				if (current_table == Tables.service_types)
				{
					switch (((Service_type)item).service_type)
					{
						case "Аэрография":
						case "Детальная":
						case "Оклейка плёнкой":
						case "Полная":
							MessageBox.Show("Вы не можете изменять записи, используемые в системе.");
							return;
					}
				}
				string item_name = primary_key_values[DataGrid.Items.IndexOf(item)];
				switch (current_table)
				{
					case Tables.genders:
					case Tables.cities:
					case Tables.measurements:
					case Tables.request_statuses:
					case Tables.roles:
					case Tables.service_types:
					case Tables.paint_types:
						DictionaryWindow dw = new DictionaryWindow(QueryMode.change,
						ru_tables[(int)current_table],
						tables[(int)current_table],
						fields[current_primary_key_name],
						current_primary_key_name,
						this, item_name);
						dw.Show();
						break;
					case Tables.car_parts:
						WindowCarParts wcp = new WindowCarParts(this, item_name);
						wcp.Show();
						break;
					case Tables.cars:
						WindowCars wcar = new WindowCars(QueryMode.change, this, item_name);
						wcar.Show();
						break;
					case Tables.clients:
						WindowClients wclient = new WindowClients(QueryMode.change, this, item_name);
						wclient.Show();
						break;
					case Tables.colors:
						WindowColors wcolor = new WindowColors(QueryMode.change, this, item_name);
						wcolor.Show();
						break;
					/*case Tables.paint_types: WindowPaint_types wpt = new WindowPaint_types(QueryMode.change, this, item_name);
						wpt.Show();
						break;*/
					case Tables.pictures: WindowPictures wpic = new WindowPictures(QueryMode.change, this, item_name);
						wpic.Show();
						break;
					case Tables.products: WindowProducts wproduct = new WindowProducts(QueryMode.change, this, item_name);
						wproduct.Show();
						break;
					case Tables.requests: WindowRequests wrequest = new WindowRequests(QueryMode.change, this, item_name);
						wrequest.Show();
						break;
					case Tables.storage: WindowStorage wstor = new WindowStorage(QueryMode.change, this, item_name);
						wstor.Show();
						break;
					case Tables.suppliers: WindowSuppliers wsupplier = new WindowSuppliers(QueryMode.change, this, item_name);
						wsupplier.Show();
						break;
					case Tables.supplies: WindowSupplies wsupply = new WindowSupplies(QueryMode.change, this, item_name);
						wsupply.Show();
						break;
					case Tables.users: WindowUsers wuser = new WindowUsers(QueryMode.change, this, item_name);
						wuser.Show();
						break;
				}
			}
			else
			{
				MessageBox.Show("Выберите мышью запись перед изменением.");
			}
		}

		private void Button_update_Click(object sender, RoutedEventArgs e)
		{
			fill_table();
		}	
		//ДОБАВЛЕНИЕ
		private void Button_add_Click(object sender, RoutedEventArgs e)
		{
			switch (current_table)
			{
				case Tables.genders:
				case Tables.cities:
				case Tables.measurements:
				case Tables.request_statuses:
				case Tables.roles:
				case Tables.service_types:
					DictionaryWindow dw = new DictionaryWindow(QueryMode.add,
						ru_tables[(int)current_table],
						tables[(int)current_table],
						fields[current_primary_key_name],
						current_primary_key_name,
						this);
					dw.Show();
					break;
				case Tables.cars:
					WindowCars wcar = new WindowCars(QueryMode.add, this);
					wcar.Show();
					break;
				case Tables.clients:
					WindowClients wclient = new WindowClients(QueryMode.add, this);
					wclient.Show();
					break;
				case Tables.colors:
					WindowColors wcolor = new WindowColors(QueryMode.add, this);
					wcolor.Show();
					break;
				case Tables.paint_types:
					WindowPaint_types wpt = new WindowPaint_types(QueryMode.add, this);
					wpt.Show();
					break;
				case Tables.pictures:
					WindowPictures wpic = new WindowPictures(QueryMode.add, this);
					wpic.Show();
					break;
				case Tables.products: WindowProducts wproduct = new WindowProducts(QueryMode.add, this);
					wproduct.Show();
					break;
				case Tables.requests: WindowRequests wrequest = new WindowRequests(QueryMode.add, this);
					wrequest.Show();
					break;
				case Tables.storage: WindowStorage wstor = new WindowStorage(QueryMode.add, this);
					wstor.Show();
					break;
				case Tables.suppliers: WindowSuppliers wsupplier = new WindowSuppliers(QueryMode.add, this);
					wsupplier.Show();
					break;
				case Tables.supplies: WindowSupplies wsupply = new WindowSupplies(QueryMode.add, this);
					wsupply.Show();
					break;
				case Tables.users: WindowUsers wuser = new WindowUsers(QueryMode.add, this);
					wuser.Show();
					break;
			}
		}

		//Отображение изображения или цвета из некоторых таблиц
		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TextBlock_car_parts.Text = "";
			object item = DataGrid.SelectedItem;
			if (item != null)
			{ 
				switch (current_table)
				{
					case Tables.users:
						Button_image_change.Visibility = Visibility.Visible;
						Shortcuts.set_image(DataImage,
							Shortcuts.get_image(tables[(int)current_table], current_primary_key_name, ((User)item).mail, connection));
						break;
					case Tables.pictures:
						Button_image_change.Visibility = Visibility.Visible;
						Shortcuts.set_image(DataImage,
							Shortcuts.get_image(tables[(int)current_table], current_primary_key_name, ((Picture)item).name, connection));
						break;
					case Tables.colors:
						Border_color.Visibility = Visibility.Visible;
						string color_code = "#" + ((Color)item).color_code;
						Border_color.Background = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString(color_code));
						break;
					case Tables.products:
						Border_color.Visibility = Visibility.Visible;
						string color = "#" + ((Product)item).color_code;
						Border_color.Background = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString(color));
						break;
					case Tables.supplies:
						Supply sup = (Supply)item;
						Border_color.Visibility = Visibility.Visible;
						string supply_color_code = Shortcuts.get_one_string_data_from($"SELECT `color_code` FROM `products` WHERE `name` = '{sup.product_name}';", connection);
						Border_color.Background = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#" + supply_color_code));
						break;
					case Tables.requests:
						int numb = ((Request)item).parts_to_paint;
						TextBlock_car_parts.Text = "Части на покраску:\n";
						if (numb == 8191)
							TextBlock_car_parts.Text += "Все";
						else
						{
							try
							{
								connection.Open();
								MySqlCommand comm = new MySqlCommand($"SELECT `name` FROM `car_parts` WHERE `id` & {numb};", connection);
								MySqlDataReader data = comm.ExecuteReader();
								while (data.Read())
								{
									TextBlock_car_parts.Text += data[0].ToString() + "\n";
								}/*
							if (TextBlock_car_parts.Text == "Части на покраску:\n")
							{
								TextBlock_car_parts.Text += "Все";
							}*/
							}
							catch (Exception ex)
							{
								MessageBox.Show(ex.Message);
							}
							finally
							{
								connection.Close();
							}
						}
						break;
				}
			}
		}

		private void Button_image_change_Click(object sender, RoutedEventArgs e)
		{
			object item = DataGrid.SelectedItem;
			if (item != null)
			{
				OpenFileDialog ofd = new OpenFileDialog();
				if (ofd.ShowDialog() == true)
				{
					string image_path = ofd.FileName;
					FileStream fs = new FileStream(image_path, FileMode.Open, FileAccess.Read);
					BinaryReader br = new BinaryReader(fs);
					byte[] image_bytes = br.ReadBytes((int)fs.Length);
					try
					{
						MySqlCommand comm = new MySqlCommand($"UPDATE `{tables[(int)current_table]}` " +
							$"SET `image` = @image_bytes " +
							$"where `{current_primary_key_name}` = '{primary_key_values[DataGrid.Items.IndexOf(item)]}';", connection);
						connection.Open();
						MySqlParameter image_parameter = new MySqlParameter("@image_bytes", image_bytes);
						comm.Parameters.Add(image_parameter);
						comm.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message);
					}
					finally
					{
						connection.Close();
					}
				}
			}
		}

		private void Button_exit_account_Click(object sender, RoutedEventArgs e)
		{
			if (confirm_action("Вы хотите выйти из аккаунта?", "Выход из аккаунта"))
			{
				exit_program = false;
				Close();
			}
		}

		private void TextBox_search_TextChanged(object sender, TextChangedEventArgs e)
		{
			fill_table();
			string search_text = TextBox_search.Text.ToLower();
			if (search_text != "")
			{
				primary_key_values.Clear();
				switch (current_table)
				{
					case Tables.car_parts:
						List<Car_part> car_parts = new List<Car_part>();
						foreach (Car_part car_part in DataGrid.Items)
						{
							if (car_part.ToString().ToLower().Contains(search_text))
							{
								car_parts.Add(car_part);
								primary_key_values.Add(car_part.id.ToString());
							}
						}
						if (car_parts.Count > 0)
						{
							DataGrid.Items.Clear();
							foreach (Car_part car_part in car_parts)
							{
								DataGrid.Items.Add(car_part);
							}
						}
						else
						{
							MessageBox.Show("Ни одна запись не содержит схождений со строкой поиска!", "Нет результатов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							TextBox_search.Text = "";
						}
						break;
					case Tables.cars:
						List<Car> cars = new List<Car>();
						foreach (Car car in DataGrid.Items)
						{
							if (car.ToString().ToLower().Contains(search_text))
							{
								cars.Add(car);
								primary_key_values.Add(car.vin);
							}
						}
						if (cars.Count > 0)
						{
							DataGrid.Items.Clear();
							foreach (Car car in cars)
							{
								DataGrid.Items.Add(car);
							}
						}
						else
						{
							MessageBox.Show("Ни одна запись не содержит схождений со строкой поиска!", "Нет результатов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							TextBox_search.Text = "";
						}
						break;
					case Tables.cities:
						List<City> cities = new List<City>();
						foreach (City city in DataGrid.Items)
						{
							if (city.ToString().ToLower().Contains(search_text))
							{
								cities.Add(city);
								primary_key_values.Add(city.city.ToString());
							}
						}
						if (cities.Count > 0)
						{
							DataGrid.Items.Clear();
							foreach (City city in cities)
							{
								DataGrid.Items.Add(city);
							}
						}
						else
						{
							MessageBox.Show("Ни одна запись не содержит схождений со строкой поиска!", "Нет результатов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							TextBox_search.Text = "";
						}
						break;
					case Tables.clients:
						List<Client> clients = new List<Client>();
						foreach (Client client in DataGrid.Items)
						{
							if (client.ToString().ToLower().Contains(search_text))
							{
								clients.Add(client);
								primary_key_values.Add(client.mail);
							}
						}
						if (clients.Count > 0)
						{
							DataGrid.Items.Clear();
							foreach (Client client in clients)
							{
								DataGrid.Items.Add(client);
							}
						}
						else
						{
							MessageBox.Show("Ни одна запись не содержит схождений со строкой поиска!", "Нет результатов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							TextBox_search.Text = "";
						}
						break;
					case Tables.colors:
						List<Color> colors = new List<Color>();
						foreach (Color color in DataGrid.Items)
						{
							if (color.ToString().ToLower().Contains(search_text))
							{
								colors.Add(color);
								primary_key_values.Add(color.color_code);
							}
						}
						if (colors.Count > 0)
						{
							DataGrid.Items.Clear();
							foreach (Color color in colors)
							{
								DataGrid.Items.Add(color);
							}
						}
						else
						{
							MessageBox.Show("Ни одна запись не содержит схождений со строкой поиска!", "Нет результатов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							TextBox_search.Text = "";
						}
						break;
					case Tables.genders:
						List<Gender> genders = new List<Gender>();
						foreach (Gender gender in DataGrid.Items)
						{
							if (gender.ToString().ToLower().Contains(search_text))
							{
								genders.Add(gender);
								primary_key_values.Add(gender.gender);
							}
						}
						if (genders.Count > 0)
						{
							DataGrid.Items.Clear();
							foreach (Gender gender in genders)
							{
								DataGrid.Items.Add(gender);
							}
						}
						else
						{
							MessageBox.Show("Ни одна запись не содержит схождений со строкой поиска!", "Нет результатов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							TextBox_search.Text = "";
						}
						break;
					case Tables.measurements:
						List<Measurement> measurements = new List<Measurement>();
						foreach (Measurement measurement in DataGrid.Items)
						{
							if (measurement.ToString().ToLower().Contains(search_text))
							{
								measurements.Add(measurement);
								primary_key_values.Add(measurement.measurement);
							}
						}
						if (measurements.Count > 0)
						{
							DataGrid.Items.Clear();
							foreach (Measurement measurement in measurements)
							{
								DataGrid.Items.Add(measurement);
							}
						}
						else
						{
							MessageBox.Show("Ни одна запись не содержит схождений со строкой поиска!", "Нет результатов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							TextBox_search.Text = "";
						}
						break;
					case Tables.paint_types:
						List<Paint_type> paint_types = new List<Paint_type>();
						foreach (Paint_type paint_type in DataGrid.Items)
						{
							if (paint_type.ToString().ToLower().Contains(search_text))
							{
								paint_types.Add(paint_type);
								primary_key_values.Add(paint_type.paint_type);
							}
						}
						if (paint_types.Count > 0)
						{
							DataGrid.Items.Clear();
							foreach (Paint_type paint_type in paint_types)
							{
								DataGrid.Items.Add(paint_type);
							}
						}
						else
						{
							MessageBox.Show("Ни одна запись не содержит схождений со строкой поиска!", "Нет результатов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							TextBox_search.Text = "";
						}
						break;
					case Tables.pictures:
						List<Picture> pictures = new List<Picture>();
						foreach (Picture picture in DataGrid.Items)
						{
							if (picture.ToString().ToLower().Contains(search_text))
							{
								pictures.Add(picture);
								primary_key_values.Add(picture.name);
							}
						}
						if (pictures.Count > 0)
						{
							DataGrid.Items.Clear();
							foreach (Picture picture in pictures)
							{
								DataGrid.Items.Add(picture);
							}
						}
						else
						{
							MessageBox.Show("Ни одна запись не содержит схождений со строкой поиска!", "Нет результатов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							TextBox_search.Text = "";
						}
						break;
					case Tables.products:
						List<Product> products = new List<Product>();
						foreach (Product product in DataGrid.Items)
						{
							if (product.ToString().ToLower().Contains(search_text))
							{
								products.Add(product);
								primary_key_values.Add(product.name);
							}
						}
						if (products.Count > 0)
						{
							DataGrid.Items.Clear();
							foreach (Product product in products)
							{
								DataGrid.Items.Add(product);
							}
						}
						else
						{
							MessageBox.Show("Ни одна запись не содержит схождений со строкой поиска!", "Нет результатов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							TextBox_search.Text = "";
						}
						break;
					case Tables.request_statuses:
						List<Request_status> request_statuses = new List<Request_status>();
						foreach (Request_status request_status in DataGrid.Items)
						{
							if (request_status.ToString().ToLower().Contains(search_text))
							{
								request_statuses.Add(request_status);
								primary_key_values.Add(request_status.request_status);
							}
						}
						if (request_statuses.Count > 0)
						{
							DataGrid.Items.Clear();
							foreach (Request_status request_status in request_statuses)
							{
								DataGrid.Items.Add(request_status);
							}
						}
						else
						{
							MessageBox.Show("Ни одна запись не содержит схождений со строкой поиска!", "Нет результатов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							TextBox_search.Text = "";
						}
						break;
					case Tables.requests:
						List<Request> requests = new List<Request>();
						foreach (Request request in DataGrid.Items)
						{
							if (request.ToString().ToLower().Contains(search_text))
							{
								requests.Add(request);
								primary_key_values.Add(request.id.ToString());
							}
						}
						if (requests.Count > 0)
						{
							DataGrid.Items.Clear();
							foreach (Request request in requests)
							{
								DataGrid.Items.Add(request);
							}
						}
						else
						{
							MessageBox.Show("Ни одна запись не содержит схождений со строкой поиска!", "Нет результатов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							TextBox_search.Text = "";
						}
						break;
					case Tables.roles:
						List<Role> roles = new List<Role>();
						foreach (Role role in DataGrid.Items)
						{
							if (role.ToString().ToLower().Contains(search_text))
							{
								roles.Add(role);
								primary_key_values.Add(role.role);
							}
						}
						if (roles.Count > 0)
						{
							DataGrid.Items.Clear();
							foreach (Role role in roles)
							{
								DataGrid.Items.Add(role);
							}
						}
						else
						{
							MessageBox.Show("Ни одна запись не содержит схождений со строкой поиска!", "Нет результатов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							TextBox_search.Text = "";
						}
						break;
					case Tables.service_types:
						List<Service_type> service_types = new List<Service_type>();
						foreach (Service_type service_type in DataGrid.Items)
						{
							if (service_type.ToString().ToLower().Contains(search_text))
							{
								service_types.Add(service_type);
								primary_key_values.Add(service_type.service_type);
							}
						}
						if (service_types.Count > 0)
						{
							DataGrid.Items.Clear();
							foreach (Service_type service_type in service_types)
							{
								DataGrid.Items.Add(service_type);
							}
						}
						else
						{
							MessageBox.Show("Ни одна запись не содержит схождений со строкой поиска!", "Нет результатов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							TextBox_search.Text = "";
						}
						break;
					case Tables.storage:
						List<Storage> storages = new List<Storage>();
						foreach (Storage storage in DataGrid.Items)
						{
							if (storage.ToString().ToLower().Contains(search_text))
							{
								storages.Add(storage);
								primary_key_values.Add(storage.id.ToString());
							}
						}
						if (storages.Count > 0)
						{
							DataGrid.Items.Clear();
							foreach (Storage storage in storages)
							{
								DataGrid.Items.Add(storage);
							}
						}
						else
						{
							MessageBox.Show("Ни одна запись не содержит схождений со строкой поиска!", "Нет результатов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							TextBox_search.Text = "";
						}
						break;
					case Tables.suppliers:
						List<Supplier> suppliers = new List<Supplier>();
						foreach (Supplier supplier in DataGrid.Items)
						{
							if (supplier.ToString().ToLower().Contains(search_text))
							{
								suppliers.Add(supplier);
								primary_key_values.Add(supplier.name);
							}
						}
						if (suppliers.Count > 0)
						{
							DataGrid.Items.Clear();
							foreach (Supplier supplier in suppliers)
							{
								DataGrid.Items.Add(supplier);
							}
						}
						else
						{
							MessageBox.Show("Ни одна запись не содержит схождений со строкой поиска!", "Нет результатов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							TextBox_search.Text = "";
						}
						break;
					case Tables.supplies:
						List<Supply> supplies = new List<Supply>();
						foreach (Supply supply in DataGrid.Items)
						{
							if (supply.ToString().ToLower().Contains(search_text))
							{
								supplies.Add(supply);
								primary_key_values.Add(supply.id.ToString());
							}
						}
						if (supplies.Count > 0)
						{
							DataGrid.Items.Clear();
							foreach (Supply supply in supplies)
							{
								DataGrid.Items.Add(supply);
							}
						}
						else
						{
							MessageBox.Show("Ни одна запись не содержит схождений со строкой поиска!", "Нет результатов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							TextBox_search.Text = "";
						}
						break;
					case Tables.users:
						List<User> users = new List<User>();
						foreach (User user in DataGrid.Items)
						{
							if (user.ToString().ToLower().Contains(search_text))
							{
								users.Add(user);
								primary_key_values.Add(user.mail);
							}
						}
						if (users.Count > 0)
						{
							DataGrid.Items.Clear();
							foreach (User user in users)
							{
								DataGrid.Items.Add(user);
							}
						}
						else
						{
							MessageBox.Show("Ни одна запись не содержит схождений со строкой поиска!", "Нет результатов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							TextBox_search.Text = "";
						}
						break;
				}
			}
		}
	}
}