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
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using Word = Microsoft.Office.Interop.Word;

namespace AutopaintWPF
{
	/// <summary>
	/// Логика взаимодействия для SupplyManagerWindow.xaml
	/// </summary>
	public partial class SupplyManagerWindow : Window
	{
		bool exit_program = true; // Переменная на выход из всей программы или только из аккаунта
		public User current_user;
		public AuthWindow FirstWindow;
		Tables current_table = Tables.supplies;
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
			"Части машины",
			"Машины",
			"Города",
			"Клиенты",
			"Цвета",
			"Пол",
			"Единицы измерения",
			"Типы краски",
			"Изображения",
			"Продукция",
			"Статусы заявок",
			"Заявки",
			"Роли",
			"Тип обслуживания",
			"Склад",
			"Поставщики",
			"Поставки",
			"Пользователи"
		};

		string current_primary_key_name;
		//Текущая и стартовая таблица
		public static Dictionary<string, string> fields = new Dictionary<string, string>
		{
			//Поля таблиц без повторов
			//Машины
			{ "vin", "VIN"},
			{ "number", "Номер"},
			{ "owner_mail", "Почта владельца"},
			{ "color", "Цвет"},
			{ "model", "Модель"},
			//Части машины
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
			{ "cost_ratio", "Коэффициент цены"},
			//Изображения
			{ "image", "Изображение"},
			//Поставки
			{ "user_mail", "Менеджер"},
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
			{ "service_type", "Тип обслуживания"},
			{ "parts_to_paint", "Части на покраску"},
			{ "picture_name", "Название изображения"},
			{ "request_status", "Статус заявки"},
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
		public SupplyManagerWindow(User user, AuthWindow parent_window)
		{
			InitializeComponent();
			FirstWindow = parent_window;
			current_user = user;
			Title = $"Режим работы: {current_user.role}";
			button_storage_report.Visibility = Visibility.Collapsed;
			ComboBoxTables.SelectedIndex = 2;
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
			string table = "supplies";
			current_table = Tables.supplies;
			switch(ComboBoxTables.SelectedIndex)
			{
				case 0: table = "storage";
					current_table = Tables.storage;
					break;
				case 1: table = "suppliers";
					current_table = Tables.suppliers;
					break;
				case 2: table = "supplies";
					current_table = Tables.supplies;
					break;
			}
			try
			{
				connection.Open();
				MySqlCommand command = new MySqlCommand("SELECT * FROM `" + table + "`;", connection);
				MySqlDataReader data = command.ExecuteReader();
				current_primary_key_name = data.GetName(0);
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
		}

		//Смена таблицы при изменении ComboBox'а
		private void ComboBoxTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Border_color.Visibility = Visibility.Collapsed;
			button_delivery_supply.Visibility = Visibility.Collapsed;
			button_supply_report.Visibility = Visibility.Collapsed;
			button_storage_report.Visibility = Visibility.Collapsed;
			Button_add.Visibility = Visibility.Visible;
			Button_change.Visibility = Visibility.Visible;
			Button_delete.Visibility = Visibility.Visible;
			switch (ComboBoxTables.SelectedIndex)
			{
				case 0: current_table = Tables.storage;
					Button_add.Visibility = Visibility.Collapsed;
					Button_change.Visibility = Visibility.Collapsed;
					Button_delete.Visibility = Visibility.Collapsed;
					button_storage_report.Visibility = Visibility.Visible;
					break;
				case 1: current_table = Tables.suppliers; 
					break;
				case 2: current_table = Tables.supplies;
					button_delivery_supply.Visibility = Visibility.Visible;
					button_supply_report.Visibility = Visibility.Visible;
					break;
			}
			TextBox_search.Text = "";
			fill_table();
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
				string item_name = "";
				switch(current_table)
				{
					case Tables.suppliers: item_name = ((Supplier)item).name; break;
					case Tables.supplies:
						if (((Supply)item).user_mail != current_user.mail)
						{
							MessageBox.Show("Вы не можете удалять чужие поставки!", "Запрещено", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							return;
						}
						if (((Supply)item).delivery_date != "")
						{
							MessageBox.Show("Нельзя удалить зачисленную поставку!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
							return;
						}
						item_name = ((Supply)item).id.ToString(); break;
				}
				if (confirm_action("Вы хотите удалить текущий объект?", "Удаление"))
				{
					Shortcuts.execute_command($@"DELETE FROM `{tables[(int)current_table]}` where `{current_primary_key_name}` = '{item_name}';", connection);
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
				string item_name = "";
				switch (current_table)
				{
					case Tables.suppliers:
						item_name = ((Supplier)item).name;
						WindowSuppliers wsupplier = new WindowSuppliers(QueryMode.change, this, item_name);
						wsupplier.Show();
						break;
					case Tables.supplies:
						if (((Supply)item).user_mail != current_user.mail)
						{
							MessageBox.Show("Вы не можете изменять чужие поставки!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							return;
						}
						if (((Supply)item).delivery_date != "")
						{
							MessageBox.Show("Нельзя изменить зачисленную поставку!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							return;
						}
						item_name = ((Supply)item).id.ToString();
						WindowSupplies wsupply = new WindowSupplies(QueryMode.change, this, item_name);
						wsupply.Show();
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
				case Tables.suppliers: WindowSuppliers wsupplier = new WindowSuppliers(QueryMode.add, this);
					wsupplier.Show();
					break;
				case Tables.supplies: WindowSupplies wsupply = new WindowSupplies(QueryMode.add, this);
					wsupply.Show();
					break;
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
				switch (current_table)
				{
					case Tables.storage:
						List<Storage> storages = new List<Storage>();
						foreach (Storage storage in DataGrid.Items)
						{
							if (storage.ToString().ToLower().Contains(search_text))
							{
								storages.Add(storage);
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
				}
			}
		}

		private void button_delivery_supply_Click(object sender, RoutedEventArgs e)
		{
			if (DataGrid.SelectedItem == null)
			{
				MessageBox.Show("Выберите мышью запись перед совершением поставки.");
			}
			else
			{
				bool success = true;
				Supply sup = (Supply)DataGrid.SelectedItem;
				if (sup.delivery_date != "")
				{
					MessageBox.Show("Данная поставка уже зачислена!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					return;
				}
				int product_storage_count = int.Parse(Shortcuts.get_one_string_data_from($"SELECT count(*) FROM " +
					$"`storage` WHERE `product_name` = '{sup.product_name}' AND " +
					$"`supplier` = '{sup.supplier}';", connection));
				if (product_storage_count == 1)
				{
					//Записать в существующего поставщика
					Storage st = new Storage();
					try
					{
						connection.Open();
						MySqlCommand comm = new MySqlCommand("SELECT * FROM " +
						$"`storage` WHERE `product_name` = '{sup.product_name}' AND " +
						$"`supplier` = '{sup.supplier}';", connection);
						MySqlDataReader data = comm.ExecuteReader();
						data.Read();
						string[] values = new string[data.FieldCount];
						for (int i = 0; i < data.FieldCount; i++)
						{
							values[i] = data[i].ToString();
						}
						st = (Storage)Container_controller.Create_struct(Tables.storage, values);

					}
					catch (Exception ex)
					{
						success = false;
						MessageBox.Show(ex.Message);
					}
					finally
					{
						connection.Close();
					}
					decimal new_prod_amount = st.product_amount + sup.product_amount;
					decimal new_price = ((st.average_purchase_price * st.product_amount) + sup.price*sup.product_amount) / (new_prod_amount);
					success = Shortcuts.execute_command("UPDATE `storage` SET " +
						$"`average_purchase_price` = {new_price.ToString().Replace(',', '.')}, " +
						$"`product_amount` = {new_prod_amount.ToString().Replace(',', '.')} " +
						$"WHERE `product_name` = '{sup.product_name.ToString().Replace(',', '.')}' AND " +
						$"`supplier` = '{sup.supplier}';", connection);

					success = Shortcuts.execute_command("UPDATE `supplies` SET " +
						$"`delivery_date` = '{DateTime.Now:yyyy-MM-dd}' " +
						$"WHERE `id` = {sup.id};", connection);
					if (success)
						MessageBox.Show("Поставка на склад произведена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
					else
						MessageBox.Show("Поставка не произведена!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				else if (product_storage_count == 0)
				{
					success = Shortcuts.execute_command("INSERT INTO `storage` " +
						"(`product_name`, `product_amount`, " +
						"`measurement`, `supplier`, `average_purchase_price`) VALUES " +
						$"('{sup.product_name}', {sup.product_amount.ToString().Replace(',','.')}, " +
						$"'{sup.measurement}', '{sup.supplier}', {(sup.price).ToString().Replace(',', '.')});", connection);
					success = Shortcuts.execute_command("UPDATE `supplies` SET " +
						$"`delivery_date` = '{DateTime.Now:yyyy-MM-dd}' " +
						$"WHERE `id` = {sup.id};", connection);
					if (success)
						MessageBox.Show("Поставка на склад произведена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
					else
						MessageBox.Show("Поставка не произведена!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				else
				{
					MessageBox.Show("Произошла ошибка в базе!\nПовторяющиеся записи продукта на одного поставщика. " +
						"Обратитесь к администратору", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				fill_table();
			}
		}

		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (DataGrid.SelectedItem != null)
			{

				switch (current_table)
				{
					case Tables.supplies:
					Border_color.Visibility = Visibility.Visible;
					Supply sup = (Supply)DataGrid.SelectedItem;
					Border_color.Visibility = Visibility.Visible;
					string supply_color_code = Shortcuts.get_one_string_data_from($"SELECT `color_code` FROM `products` WHERE `name` = '{sup.product_name}';", connection);
					Border_color.Background = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#" + supply_color_code));
						break;
					case Tables.storage:
						Border_color.Visibility = Visibility.Visible;
						Storage st = (Storage)DataGrid.SelectedItem;
						Border_color.Visibility = Visibility.Visible;
						string st_color_code = Shortcuts.get_one_string_data_from($"SELECT `color_code` FROM `products` WHERE `name` = '{st.product_name}';", connection);
						Border_color.Background = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#" + st_color_code));
						break;
				}
			}
		}

		private void button_supply_report_Click(object sender, RoutedEventArgs e)
		{
			WindowSupplyReport wsr = new WindowSupplyReport();
			wsr.Show();
		}

		private void button_storage_report_Click(object sender, RoutedEventArgs e)
		{
			WindowStorageReport wsr = new WindowStorageReport();
			wsr.Show();
		}
	}
}