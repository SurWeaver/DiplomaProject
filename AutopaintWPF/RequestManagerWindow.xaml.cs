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
using AutopaintWPF.Report_windows;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using Word = Microsoft.Office.Interop.Word;

namespace AutopaintWPF
{
	/// <summary>
	/// Логика взаимодействия для RequestManagerWindow.xaml
	/// </summary>
	public partial class RequestManagerWindow : System.Windows.Window
	{
		bool exit_program = true; // Переменная на выход из всей программы или только из аккаунта
		public User current_user;
		public AuthWindow FirstWindow;
		Tables current_table = Tables.requests;

		public static Dictionary<string, string> fields = new Dictionary<string, string>
		{
			{ "vin", "VIN"},
			{ "number", "Регистрационный номер"},
			{ "owner_mail", "Почта владельца"},
			{ "color", "Цвет"},
			{ "model", "Модель"},
			{ "id", "Код идентификации"},
			{ "product_name", "Наименование продукции"},
			{ "date", "Дата/Время" },
			{ "date_order", "Дата/Время заказа" },
			{ "paint_date", "Дата/Время покраски" },
			{ "service_type", "Тип обслуживания"},
			{ "parts_to_paint", "Части на покраску"},
			{ "picture_name", "Название изображения"},
			{ "request_status", "Статус заявки"},
			{ "paint_amount", "Количество затраченной краски" },
			{ "paint_cost", "Стоимость затраченной краски (руб.)" },
			{ "measurement", "Единица измерения" },
			{ "supplier", "Поставщик" },
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
		public RequestManagerWindow(User user, AuthWindow parent_window)
		{
			InitializeComponent();

			FirstWindow = parent_window;
			current_user = user;
			ComboBoxTables.SelectedIndex = 0;
			Title = $"Режим работы: {current_user.role}";
			
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
				string table = "";
				switch(current_table)
				{
					case Tables.requests: table = "requests"; break;
					case Tables.clients: table = "clients"; break;
					case Tables.cars: table = "cars"; break;
				}
				MySqlCommand command = new MySqlCommand($"SELECT * FROM `{table}`;", connection);
				MySqlDataReader data = command.ExecuteReader();
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
					string[] values = new string[data.FieldCount];
					for (int i = 0; i < data.FieldCount; i++)
					{
						values[i] = data[i].ToString();
					}
					DataGrid.Items.Add(Container_controller.Create_struct(current_table, values));
				}
				if (data.GetName(0) == "id")
					DataGrid.Columns[0].Visibility = Visibility.Collapsed;
				if (current_table == Tables.requests)
					DataGrid.Columns.Remove(DataGrid.Columns[data.GetOrdinal("parts_to_paint")]);
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
		}

		private void ComboBoxTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			switch(ComboBoxTables.SelectedIndex)
			{
				case 0: current_table = Tables.requests;
					break;
				case 1: current_table = Tables.cars; 
					break;
				case 2: current_table = Tables.clients;
					break;
			}
			if (current_table == Tables.requests)
				change_buttons_visibility(Visibility.Visible);
			else
				change_buttons_visibility(Visibility.Collapsed);
			fill_table();
		}

		private void change_buttons_visibility(Visibility visibility)
		{
			button_write_check.Visibility = visibility;
			button_accept_request.Visibility = visibility;
			button_spent_report.Visibility = visibility;
			button_pic_report.Visibility = visibility;
			button_income_report.Visibility = visibility;
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
				switch (current_table)
				{
					case Tables.requests:
						if (((Request)item).request_status == "Обработано")
						{
							MessageBox.Show("Нельзя удалить обработанную заявку!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
							return;
						}
						if (confirm_action("Вы хотите удалить текущий объект?", "Удаление"))
						{
							Shortcuts.execute_command($@"DELETE FROM `requests` where `id` = '{((Request)item).id}';", connection);
							
							fill_table();
						}
						break;
					case Tables.cars:
						if (confirm_action("Вы хотите удалить текущий объект?", "Удаление"))
						{
							Shortcuts.execute_command($@"DELETE FROM `cars` where `vin` = '{((Car)item).vin}';", connection);
							fill_table();
						}
						break;
					case Tables.clients:
						if (confirm_action("Вы хотите удалить текущий объект?", "Удаление"))
						{
							Shortcuts.execute_command($@"DELETE FROM `clients` where `mail` = '{((Client)item).mail}';", connection);
							fill_table();
						}
						break;
				}
				TextBox_search.Text = "";
				fill_table();
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
				string item_name;
				switch(current_table)
				{
					case Tables.requests:
						if (((Request)item).request_status == "Обработано")
						{
							MessageBox.Show("Нельзя изменить обработанную заявку!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
							return;
						}
						item_name = ((Request)item).id.ToString();
						WindowRequests wrequest = new WindowRequests(QueryMode.change, this, item_name);
						wrequest.Show();
						break;
					case Tables.cars:
						item_name = ((Car)item).vin;
						WindowCars wcar = new WindowCars(QueryMode.change, this, item_name);
						wcar.Show();
						break;
					case Tables.clients:
						item_name = ((Client)item).mail.ToString();
						WindowClients wclient = new WindowClients(QueryMode.change, this, item_name);
						wclient.Show();
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
				case Tables.requests:
					WindowRequests wrequest = new WindowRequests(QueryMode.add, this);
					wrequest.Show();
					break;
				case Tables.cars:
					WindowCars wcar = new WindowCars(QueryMode.add, this);
					wcar.Show();
					break;
				case Tables.clients:
					WindowClients wclient = new WindowClients(QueryMode.add, this);
					wclient.Show();
					break;
			}
		}

		//Отображение изображения или цвета из некоторых таблиц
		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TextBlock_car_parts.Text = "";
			object item = DataGrid.SelectedItem;
			if (item != null && current_table == Tables.requests)
			{ 
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
				switch(current_table)
				{
					case Tables.cars:
						List<Car> cars = new List<Car>();
						foreach (Car car in DataGrid.Items)
						{
							if (cars.ToString().ToLower().Contains(search_text))
							{
								cars.Add(car);
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
					case Tables.clients:
						List<Client> clients = new List<Client>();
						foreach (Client client in DataGrid.Items)
						{
							if (client.ToString().ToLower().Contains(search_text))
							{
								clients.Add(client);
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
					case Tables.requests:
						List<Request> requests = new List<Request>();
						foreach (Request request in DataGrid.Items)
						{
							if (request.ToString().ToLower().Contains(search_text))
							{
								requests.Add(request);
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
				}
			}
		}

		private void button_accept_request_Click(object sender, RoutedEventArgs e)
		{
			bool success = true;
			if (DataGrid.SelectedItem != null)
			{
				Request req = (Request)DataGrid.SelectedItem;
				if (req.request_status == "Обработано")
				{
					MessageBox.Show("Данная заявка уже обработана!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				else if (req.service_type == "Аэрография")
				{
					success = Shortcuts.execute_command("UPDATE `requests` " +
						"SET `request_status` = 'Обработано', " +
						$"`paint_date` = '{DateTime.Now:yyyy-MM-dd HH:mm:ss}' " +
						$"WHERE `id` = {req.id};", connection);
					fill_table();
					if (success)
					{
						MessageBox.Show("Заявка успешно обработана!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
					}
				}
				else
				{
					int parts = req.parts_to_paint;
					string measurement = Shortcuts.get_one_string_data_from($"SELECT `measurement`" +
						$"FROM `storage` WHERE `product_name` = '{req.product_name}'", connection);
					decimal paint_amount;
					if (req.service_type == "Детальная")
						paint_amount = decimal.Parse(Shortcuts.get_one_string_data_from($"SELECT SUM(`surface_size`) FROM `car_parts` " +
							$"WHERE `id` & {parts};", connection)) / 1000;
					else
						paint_amount = decimal.Parse(Shortcuts.get_one_string_data_from($"SELECT SUM(`surface_size`) FROM `car_parts`;", connection)) / 1000;
					int prod_available = int.Parse(Shortcuts.get_one_string_data_from($"SELECT count(*) FROM `storage` " +
						$"WHERE `product_name` = '{req.product_name}' AND `supplier` = '{req.supplier}';", connection));
					decimal paint_cost;
					switch (prod_available)
					{
						case 1:
							paint_cost = paint_amount * decimal.Parse(Shortcuts.get_one_string_data_from("SELECT `average_purchase_price` " +
						$"FROM `storage` WHERE `product_name` = '{req.product_name}';", connection));
							//Всё норм
							success = Shortcuts.execute_command("UPDATE `storage` " +
								$"SET `product_amount` = (`product_amount` - {paint_amount.ToString().Replace(',','.')}) " +
								$"WHERE `product_name` = '{req.product_name}' AND `supplier` = '{req.supplier}';", connection);
							success = Shortcuts.execute_command("UPDATE `requests` " +
								$"SET `request_status` = 'Обработано', " +
								$"`paint_amount` = {paint_amount.ToString().Replace(',','.')}, " +
								$"`measurement` = '{measurement}', " +
								$"`paint_cost` = {paint_cost.ToString().Replace(',', '.')}, " +
								$"`paint_date` = '{DateTime.Now:yyyy-MM-dd HH:mm:ss}' " +
								$"WHERE `id` = {req.id};", connection);
							fill_table();
							if (success)
							{
								MessageBox.Show("Заявка успешно обработана!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
							}
							break;
						case 0:
							MessageBox.Show("На складе отсутствует краска соответствующего поставщика!", "Нет краски", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							break;
						default:
							MessageBox.Show("Произошла ошибка в базе!\nПовторяющиеся записи продукта на одного поставщика. " +
								"Обратитесь к администратору", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
							break;
					}
				}
			}
			else
			{
				MessageBox.Show("Выберите мышью запись перед изменением.");
			}
		}

		private void button_write_check_Click(object sender, RoutedEventArgs e)
		{
			if (DataGrid.SelectedItem == null)
			{
				MessageBox.Show("Выберите мышью запись перед созданием чека!");
				return;
			}
			else if (((Request)DataGrid.SelectedItem).paint_date == "")
			{
				MessageBox.Show("Нельзя выписать чек необработанной заявки!");
				return;
			}
			
			SaveFileDialog SFDialog = new SaveFileDialog();
			SFDialog.Filter = "Microsoft Word Document (*.docx)|*.docx";
			if (SFDialog.ShowDialog() == true)
			{
				Request req = (Request)DataGrid.SelectedItem;
				string cashier_name = current_user.surname + " " + current_user.first_name[0] + ". " + current_user.second_name[0] + ".";
				string parts_to_paint = "";
				decimal price = 0;
				//Определение цены услуги и списка покрашенных деталей 
				if (req.parts_to_paint == 8191)
				{
					parts_to_paint = "Полностью";
					//количество краски на всю машину
					decimal paint_amount = decimal.Parse(Shortcuts.get_one_string_data_from("SELECT SUM(surface_size) FROM `car_parts`;", connection)) / 1000;

					if (req.product_name == "")
						//стоимость за аэрографию
						price = decimal.Parse(Shortcuts.get_one_string_data_from("SELECT `price` " +
						$"FROM `pictures` WHERE `name` = '{req.picture_name}';", connection));
					else
					{
						try
						{
							//стоимость покраски всей машины без учёта краски
							price = decimal.Parse(Shortcuts.get_one_string_data_from("SELECT SUM(cost) FROM `car_parts`;", connection));
							//стоимость использованной краски
							decimal paint_price = decimal.Parse(Shortcuts.get_one_string_data_from("SELECT `average_purchase_price` " +
							$"FROM `storage` WHERE `supplier` = '{req.supplier}' AND `product_name` = '{req.product_name}';", connection));
							//надбавка цены на краску
							price = price + paint_price * paint_amount;
						}
						catch
						{
							MessageBox.Show("Невозможно оформить чек. Отсутствуют данные о цене краски.");
							return;
						}
					}
				}
				else//детальная покраска
				{
					//перечисление частей на покраску
					List<string> parts = Shortcuts.get_full_column_from("car_parts", "name", $"`id` & {req.parts_to_paint}", connection);
					for(int i = 0; i < parts.Count; i++)
					{
						parts_to_paint += parts[i];
						if (i != parts.Count - 1)
							parts_to_paint += "; ";
						else
							parts_to_paint += ".";
					}
					try
					{
						//стоимость за покраску частей без учёта краски
						price = decimal.Parse(Shortcuts.get_one_string_data_from($"SELECT SUM(cost) FROM `car_parts` WHERE `id` & {req.parts_to_paint};", connection));
						decimal paint_price = decimal.Parse(Shortcuts.get_one_string_data_from("SELECT `average_purchase_price` " +
							$"FROM `storage` WHERE `supplier` = '{req.supplier}' AND `product_name` = '{req.product_name}';", connection));
						//общая цена за услугу
						decimal paint_amount = decimal.Parse(Shortcuts.get_one_string_data_from($"SELECT SUM(surface_size) FROM `car_parts` WHERE `id` & {req.parts_to_paint};", connection)) / 1000;
						//надбавка цены на краску
						price = price + paint_price * paint_amount;
					}
					catch
					{
						MessageBox.Show("Невозможно оформить чек. Отсутствуют данные о цене краски.");
						return;
					}
				}
				try
				{
					Word.Application WordApp = new Word.Application();
					WordApp.Visible = false;
					string price_in_doc = price.ToString().Replace(',', '.');
					int dot_pos = price_in_doc.IndexOf('.');
					if (dot_pos > 0)
						price_in_doc = price_in_doc.Substring(0, dot_pos + 3);

					Document word_doc = WordApp.Documents.Open(Directory.GetCurrentDirectory() + $@"\check.docx");
					Shortcuts.replace_word("{vin}", req.vin, word_doc);
					Shortcuts.replace_word("{service_type}", req.service_type, word_doc);
					Shortcuts.replace_word("{color}", req.product_name, word_doc);
					Shortcuts.replace_word("{parts_to_paint}", parts_to_paint, word_doc);
					Shortcuts.replace_word("{picture}", req.picture_name, word_doc);
					Shortcuts.replace_word("{price}", price_in_doc, word_doc);
					Shortcuts.replace_word("{cashier_name}", cashier_name, word_doc);
					Shortcuts.replace_word("{current_date}", req.paint_date, word_doc);
					word_doc.SaveAs2(FileName: SFDialog.FileName);
					word_doc.Close();
					MessageBox.Show("Файл успешно сохранён!");
				}
				catch
				{
					MessageBox.Show("При сохранении чека возникла ошибка. Документ не сохранён.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void button_spent_report_Click(object sender, RoutedEventArgs e)
		{
			WindowSpentReport wsr = new WindowSpentReport();
			wsr.Show();
		}

		private void button_pic_report_Click(object sender, RoutedEventArgs e)
		{
			WindowPicReport wpr = new WindowPicReport();
			wpr.Show();
		}

		private void button_income_report_Click(object sender, RoutedEventArgs e)
		{
			WindowIncomeReport wir = new WindowIncomeReport();
			wir.Show();
		}
	}
}