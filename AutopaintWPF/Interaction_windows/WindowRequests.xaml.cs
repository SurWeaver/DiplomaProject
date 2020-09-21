using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Globalization;

namespace AutopaintWPF
{
	/// <summary>
	/// Логика взаимодействия для WindowRequests.xaml
	/// </summary>
	public partial class WindowRequests : Window
	{
		object[] old_values;
		string primary_key_value;
		bool initialized = false;
		DateTime request_date;
		int old_color_index = 0;
		CheckBox[] checks;
		List<int> ids;
		QueryMode mode;
		Window parent;
		MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user id = root; password = 1234; port = 3306; persistsecurityinfo = True; sslmode = None; database = autopaint");
		public WindowRequests(QueryMode mode, Window parent, string primary_key_value = "")
		{
			InitializeComponent();
			checks = new CheckBox[]
			{
				check_lpk,
				check_lzk,
				check_ppk,
				check_pzk,
				check_lpd,
				check_lzd,
				check_ppd,
				check_pzd,
				check_roof,
				check_hood,
				check_kb,
				check_pb,
				check_zb
			};

			ids = new List<int>();
			for (int i = 1; i <= 4096; i *= 2)
			{
				ids.Add(i);
			}
			this.mode = mode;
			this.parent = parent;
			this.primary_key_value = primary_key_value;

			ComboBox_vin.ItemsSource = Shortcuts.get_full_column_from("cars", "vin", connection);
			ComboBox_service_type.ItemsSource = Shortcuts.get_full_column_from("service_types", "service_type", connection);
			ComboBox_picture.ItemsSource = Shortcuts.get_full_column_from("pictures", "name", connection);
			ComboBox_supplier.ItemsSource = Shortcuts.get_full_column_from("suppliers", "name", connection);

			if (mode == QueryMode.add)
			{
				Button_reset.Visibility = Visibility.Collapsed;
				initialized = true;
				Button_accept.Content = "Добавить";
				change_checkbox_ability(false);
			}
			else
			{
				Button_accept.Content = "Изменить";
				try
				{
					connection.Open();
					MySqlCommand comm = new MySqlCommand($"SELECT * FROM `requests` " +
						$"WHERE `id` = '{primary_key_value}';", connection);
					MySqlDataReader data = comm.ExecuteReader();
					data.Read();
					ComboBox_vin.Text = data[1].ToString();
					request_date = (DateTime)data[3];
					ComboBox_service_type.Text = data[4].ToString();
					ComboBox_supplier.Text = data[8].ToString();
					old_values = new object[8]{ 
						/*0*/data[1].ToString(),//VIN
						/*1*/data[2].ToString(),//Product_name
						/*2*/data[3],//DateTime
						/*3*/data[4].ToString(),//service_type
						/*4*/data[5],//int parts_to_paint
						/*5*/data[6].ToString(),//pic_name
						/*6*/data[7].ToString(),//request_status
						/*7*/data[8].ToString()};
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message.ToString());
				}
				finally
				{
					connection.Close();
					initialized = true;
					ComboBox_service_type.Text = ComboBox_service_type.Text;
					if (ComboBox_service_type.Text == "Оклейка плёнкой")
						fill_product_with("рулон");
					else
						fill_product_with("литр");
					
					if (ComboBox_service_type.Text != "Аэрография")
					{
						ComboBox_product_name.IsEnabled = true;
						ComboBox_supplier.IsEnabled = true;
						for (int i = 0; i < ComboBox_product_name.Items.Count; i++)
						{
							if ((string)(ComboBox_product_name.Items[i] as ComboBoxItem).Tag == (string)old_values[1])
							{
								ComboBox_product_name.SelectedIndex = i;
								old_color_index = i;
								break;
							}
						}
					}
					else
					{
						ComboBox_picture.IsEnabled = true;
						ComboBox_picture.Text = (string)old_values[5];
					}
					change_checkbox_ability(ComboBox_service_type.Text == "Детальная");
					if (ComboBox_service_type.Text == "Детальная")
						set_checkbox_value((int)old_values[4]);
					else
					{
						change_checkbox_value(false);
						change_checkbox_ability(false);
					}
				}
			}
		}

		private void Button_accept_Click(object sender, RoutedEventArgs e)
		{
			if (ComboBox_vin.Text != "" && ComboBox_service_type.Text != "" &&
				((ComboBox_service_type.Text == "Детальная" && get_checkbox_value() != 0 && ComboBox_product_name.SelectedItem != null) ||
				((ComboBox_service_type.Text == "Полная" || ComboBox_service_type.Text == "Оклейка плёнкой") &&
				ComboBox_product_name.SelectedItem != null && ComboBox_supplier.SelectedItem != null) ||
				(ComboBox_service_type.Text == "Аэрография" && ComboBox_picture.Text != "")))
			{
				bool success = true;
				string prod_name;
				if (ComboBox_product_name.SelectedItem != null)
					prod_name = "'" + (string)(ComboBox_product_name.Items[ComboBox_product_name.SelectedIndex] as ComboBoxItem).Tag + "'";
				else
					prod_name = "NULL";
				string supplier;
				if (ComboBox_supplier.SelectedItem != null)
					supplier = "'" + ComboBox_supplier.Items[ComboBox_supplier.SelectedIndex] + "'";
				else
					supplier = "NULL";
				string pic_name = (ComboBox_picture.Text != "") ? $"'{ComboBox_picture.Text}'" : "NULL";
				switch (mode)
				{
					case QueryMode.add:
						success = Shortcuts.execute_command($"INSERT INTO `requests` (`id`, `vin`, `product_name`, " +
							$"`date_order`, `service_type`, `parts_to_paint`, `picture_name`, " +
							$"`request_status`, `supplier`) " +
							$"VALUES (DEFAULT, '{ComboBox_vin.Text}', {prod_name}, " +
							$"'{DateTime.Now:yyyy-MM-dd HH:mm:ss}', '{ComboBox_service_type.Text}', {get_checkbox_value()}, {pic_name}, " +
							$"'Ожидает обработки', {supplier});", connection);
						break;
					case QueryMode.change:
						success = Shortcuts.execute_command("UPDATE `requests` " +
								$"SET `id` = {primary_key_value}, " +
								$"`vin` = '{ComboBox_vin.Text}', " +
								$"`product_name` = {prod_name}, " +
								$"`date_order` = '{request_date:yyyy-MM-dd HH:mm:ss}', " +
								$"`service_type` = '{ComboBox_service_type.Text}', " +
								$"`parts_to_paint` = {get_checkbox_value()}, " +
								$"`picture_name` = {pic_name}, " +
								$"`request_status` = '{old_values[6]}', " +
								$"`supplier` = {supplier} " +
								$"WHERE `id` = '{primary_key_value}';", connection);
						break;
				}
				if (success)
				{
					parent.Focus();
					if (parent is MainWindow)
						((MainWindow)parent).fill_table();
					if (parent is RequestManagerWindow)
						((RequestManagerWindow)parent).fill_table();
					Close();
				}
			}
			else
			{
				MessageBox.Show("Заполните все поля верно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void Button_reset_Click(object sender, RoutedEventArgs e)
		{
			ComboBox_vin.Text = (string)old_values[0];
			ComboBox_service_type.Text = (string)old_values[3];
			ComboBox_product_name.SelectedIndex = old_color_index;
			request_date = (DateTime)old_values[2];
			change_checkbox_ability((string)old_values[3] == "Детальная");
			if ((string)old_values[3] == "Детальная")
				set_checkbox_value((int)old_values[4]);
			ComboBox_picture.Text = old_values[5].ToString();
			ComboBox_supplier.Text = old_values[7].ToString();
		}

		private void Button_cancel_Click(object sender, RoutedEventArgs e)
		{
			parent.Focus();
			Close();
		}

		private void ComboBox_service_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!initialized)
				return;
			switch(ComboBox_service_type.SelectedItem)
			{
				case "Аэрография":
					ComboBox_picture.IsEnabled = true;
					ComboBox_product_name.IsEnabled = false;
					ComboBox_product_name.Text = "";
					ComboBox_supplier.IsEnabled = false;
					ComboBox_supplier.Text = "";
					change_checkbox_ability(false);
					break;
				case "Детальная":
					ComboBox_picture.IsEnabled = false;
					ComboBox_picture.Text = "";
					ComboBox_product_name.IsEnabled = true;
					ComboBox_supplier.IsEnabled = true;
					fill_product_with("литр");
					change_checkbox_ability(true);
					break;
				case "Оклейка плёнкой":
					ComboBox_picture.IsEnabled = false;
					ComboBox_picture.Text = "";
					ComboBox_product_name.IsEnabled = true;
					ComboBox_supplier.IsEnabled = true;
					fill_product_with("рулон");
					change_checkbox_ability(false);
					break;
				case "Полная":
					ComboBox_picture.IsEnabled = false;
					ComboBox_picture.Text = "";
					ComboBox_product_name.IsEnabled = true;
					ComboBox_supplier.IsEnabled = true;
					fill_product_with("литр");
					change_checkbox_ability(false);
					break;
			}
		}

		private void fill_product_with(string measurement)
		{
			ComboBox_product_name.Items.Clear();
			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand("SELECT `name`, `color_code` FROM `products`" +
					$"WHERE `measurement` = '{measurement}';", connection);
				MySqlDataReader data = comm.ExecuteReader();
				while (data.Read())
				{
					ComboBox_product_name.Items.Add(Shortcuts.create_color_box(data[0].ToString(), data[1].ToString()));
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

		
		private void change_checkbox_ability(bool enable)
		{
			foreach (CheckBox check in checks)
			{
				if (!enable)
					check.IsChecked = false;
				check.IsEnabled = enable;
			}
		}
		private void change_checkbox_value(bool value)
		{
			foreach(CheckBox check in checks)
			{
				check.IsChecked = value;
			}
		}
		private void set_checkbox_value(int number)
		{
			for (int i = 0; i < checks.Length; i++)
			{
				checks[i].IsChecked = ((number & ids[i]) != 0);
			}
		}
		private int get_checkbox_value()
		{
			int number = 0;
			for (int i = 0; i < checks.Length; i++)
			{
				if (checks[i].IsChecked.Value)
					number += ids[i];
			}
			return (number == 0) ? 8191 : number;
		}

		private void ComboBox_picture_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ComboBox_picture.SelectedItem == null)
			{
				Image_pic.Source = null;
				return;
			}
			else
			{
				byte[] byte_image = Shortcuts.get_image("pictures", "name", (string)ComboBox_picture.SelectedItem, connection);
				Shortcuts.set_image(Image_pic, byte_image);
			}
		}
	}
}
