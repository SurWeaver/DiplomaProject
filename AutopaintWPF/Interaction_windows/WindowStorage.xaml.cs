﻿using System;
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

namespace AutopaintWPF
{
	/// <summary>
	/// Логика взаимодействия для WindowStorage.xaml
	/// </summary>
	public partial class WindowStorage : Window
	{
		string[] old_values;
		string primary_key_value;
		QueryMode mode;
		Window parent;
		MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user id = root; password = 1234; port = 3306; persistsecurityinfo = True; sslmode = None; database = autopaint");
		public WindowStorage(QueryMode mode, Window parent, string primary_key_value = "")
		{
			InitializeComponent();
			this.mode = mode;
			this.parent = parent;
			this.primary_key_value = primary_key_value;

			ComboBox_product_name.ItemsSource = Shortcuts.get_full_column_from("products", "name", connection);
			ComboBox_supplier.ItemsSource = Shortcuts.get_full_column_from("suppliers", "name", connection);


			if (mode == QueryMode.add)
			{
				Button_reset.Visibility = Visibility.Collapsed;
				Button_accept.Content = "Добавить";
			}
			else
			{
				Button_accept.Content = "Изменить";
				try
				{
					connection.Open();
					MySqlCommand comm = new MySqlCommand($"SELECT * FROM `storage` " +
						$"WHERE `id` = '{primary_key_value}';", connection);
					MySqlDataReader data = comm.ExecuteReader();
					data.Read();
					ComboBox_product_name.Text = data[1].ToString();
					TextBox_product_amount.Text = float.Parse(data[2].ToString()).ToString();
					TextBox_product_amount.Text = TextBox_product_amount.Text.Replace(",", ".");
					ComboBox_supplier.Text = data[4].ToString();
					TextBox_average_purchase_price.Text = float.Parse(data[5].ToString()).ToString();
					TextBox_average_purchase_price.Text = TextBox_average_purchase_price.Text.Replace(",", ".");
					old_values = new string[4]{ 
						data[1].ToString(),
						data[2].ToString(),
						data[4].ToString(),
						data[5].ToString()};
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

		private void Button_accept_Click(object sender, RoutedEventArgs e)
		{
			int dot_count1 = TextBox_product_amount.Text.Split('.').Length - 1;
			int dot_count2 = TextBox_average_purchase_price.Text.Split('.').Length - 1;
			if (ComboBox_product_name.Text != "" && TextBox_product_amount.Text != "" &&
			ComboBox_supplier.Text != "" && TextBox_average_purchase_price.Text != "" &&
			dot_count1 <= 1 && dot_count2 <= 1)
			{
				string count = Shortcuts.get_one_string_data_from($"SELECT count(*) FROM " +
					$"`storage` WHERE `product_name` = '{ComboBox_product_name.Text}' AND " +
					$"`supplier` = '{ComboBox_supplier.Text}';", connection);
				if (int.Parse(count) >= 1 && mode == QueryMode.add)
				{
					MessageBox.Show("Уже существует запись с такой краской и поставщиком!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				bool success = true;
				string measurement = Shortcuts.get_one_string_data_from($"SELECT `measurement` FROM `products` where `name` = '{ComboBox_product_name.Text}';", connection);
				switch (mode)
				{
					case QueryMode.add:
						success = Shortcuts.execute_command($"INSERT INTO `storage` (`id`, `product_name`, `product_amount`, `measurement`, `supplier`, `average_purchase_price`) " +
							$"VALUES (DEFAULT, '{ComboBox_product_name.Text}', {TextBox_product_amount.Text}, '{measurement}', " +
							$"'{ComboBox_supplier.Text}', {TextBox_average_purchase_price.Text});", connection);
						break;
					case QueryMode.change:
						success = Shortcuts.change("storage", new string[] { "id", "product_name", "product_amount", "measurement", "supplier", "average_purchase_price" },
							new string[] { primary_key_value, ComboBox_product_name.Text, TextBox_product_amount.Text, measurement,
							ComboBox_supplier.Text, TextBox_average_purchase_price.Text },
							primary_key_value,
							connection);
						break;
				}
				if (success)
				{
					parent.Focus();
					if (parent is MainWindow)
						((MainWindow)parent).fill_table();
					if (parent is SupplyManagerWindow)
						((SupplyManagerWindow)parent).fill_table();
					Close();
				}
			}
			else
			{
				MessageBox.Show("Заполните корректно все числовые поля и поля с выбором!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void Button_reset_Click(object sender, RoutedEventArgs e)
		{
			ComboBox_product_name.Text = old_values[0];
			TextBox_product_amount.Text = float.Parse(old_values[1].ToString()).ToString();
			TextBox_product_amount.Text = TextBox_product_amount.Text.Replace(",", ".");
			ComboBox_supplier.Text = old_values[2];
			TextBox_average_purchase_price.Text = float.Parse(old_values[3].ToString()).ToString();
			TextBox_average_purchase_price.Text = TextBox_average_purchase_price.Text.Replace(",", ".");
		}

		private void Button_cancel_Click(object sender, RoutedEventArgs e)
		{
			parent.Focus();
			Close();
		}

		private void TextBox_amount_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9.]+");
			e.Handled = regex.IsMatch(e.Text);
		}
	}
}
