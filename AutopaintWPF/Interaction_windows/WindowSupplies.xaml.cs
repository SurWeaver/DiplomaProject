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
	/// Логика взаимодействия для WindowSupplies.xaml
	/// </summary>
	public partial class WindowSupplies : Window
	{
		object[] old_values;
		string primary_key_value;
		QueryMode mode;
		Window parent;
		int old_color_index = 0;
		MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user id = root; password = 1234; port = 3306; persistsecurityinfo = True; sslmode = None; database = autopaint");
		public WindowSupplies(QueryMode mode, Window parent, string primary_key_value = "")
		{
			InitializeComponent();
			this.mode = mode;
			this.parent = parent;
			this.primary_key_value = primary_key_value;

			DatePicker_order.DisplayDateStart = DateTime.Today;
			DatePicker_order.DisplayDateEnd = DateTime.Today.AddDays(90);

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
					MySqlCommand comm = new MySqlCommand($"SELECT * FROM `supplies` " +
						$"WHERE `id` = '{primary_key_value}';", connection);
					MySqlDataReader data = comm.ExecuteReader();
					data.Read();
					ComboBox_supplier.Text = data[2].ToString();
					
					TextBox_product_amount.Text = float.Parse(data[4].ToString()).ToString();
					TextBox_product_amount.Text = TextBox_product_amount.Text.Replace(",", ".");

					TextBox_price.Text = float.Parse(data[6].ToString()).ToString();
					TextBox_price.Text = TextBox_price.Text.Replace(",", ".");

					DatePicker_order.SelectedDate = (DateTime)data[7];
					//data[5] - невидимая пользователю единица измерения
					old_values = new object[6]{ 
						/*0*/data[2].ToString(),
						/*1*/data[3].ToString(),
						/*2*/data[4].ToString(),
						/*3*/data[5].ToString(),//measurement
						/*4*/data[6].ToString(),
						/*5*/data[7]};
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
				finally
				{
					connection.Close();
				}
				ComboBox_paint_type.SelectedIndex = (old_values[3].ToString() == "литр") ? 0 : 1;
				for (int i = 0; i < ComboBox_product_name.Items.Count; i++)
				{
					if ((string)(ComboBox_product_name.Items[i] as ComboBoxItem).Tag == old_values[1].ToString())
					{
						ComboBox_product_name.SelectedIndex = i;
						old_color_index = i;
						break;
					}
				}
			}
		}

		private void Button_accept_Click(object sender, RoutedEventArgs e)
		{
			//Количество точек в введённых числах
			int dot_count1 = TextBox_product_amount.Text.Split('.').Length - 1;
			int dot_count2 = TextBox_price.Text.Split('.').Length - 1;
			if (ComboBox_supplier.Text != "" && (string)(ComboBox_product_name.Items[ComboBox_product_name.SelectedIndex] as ComboBoxItem).Tag != "" &&
				TextBox_product_amount.Text != "" && TextBox_price.Text != "" && DatePicker_order.SelectedDate.HasValue &&
				dot_count1 <= 1 && dot_count2 <= 1)
			{
				string mail = "";
				if (parent is MainWindow)
				{
					mail = ((MainWindow)parent).current_user.mail;
				}
				else if (parent is SupplyManagerWindow)
				{
					mail = ((SupplyManagerWindow)parent).current_user.mail;
				}
				string measurement = Shortcuts.get_one_string_data_from($"SELECT `measurement` FROM `products` where `name` = '{(string)(ComboBox_product_name.Items[ComboBox_product_name.SelectedIndex] as ComboBoxItem).Tag}';", connection);
				bool success = true;
				switch (mode)
				{
					case QueryMode.add:
						success = Shortcuts.execute_command($"INSERT INTO `supplies` (`id`, `user_mail`, `supplier`, " +
							$"`product_name`, `product_amount`, `measurement`, `price`, " +
							$"`order_date`, `delivery_date`) " +
							$"VALUES (DEFAULT, '{mail}', '{ComboBox_supplier.Text}', " +
							$"'{(string)(ComboBox_product_name.Items[ComboBox_product_name.SelectedIndex] as ComboBoxItem).Tag}', {TextBox_product_amount.Text}, '{measurement}', {TextBox_price.Text}, " +
							$"'{DatePicker_order.SelectedDate.Value:yyyy-MM-dd}', " +
							$"NULL);", connection);
						break;
					case QueryMode.change:
						success = Shortcuts.execute_command($"UPDATE `supplies` " +
							$"SET " +
							$"`user_mail` = '{mail}', " +
							$"`supplier` = '{ComboBox_supplier.Text}', " +
							$"`product_name` = '{(string)(ComboBox_product_name.Items[ComboBox_product_name.SelectedIndex] as ComboBoxItem).Tag}', " +
							$"`product_amount` = '{TextBox_product_amount.Text}'," +
							$"`measurement` = '{measurement}', " +
							$"`price` = '{TextBox_price.Text}', " +
							$"`order_date` = '{DatePicker_order.SelectedDate.Value:yyyy-MM-dd}' " +
							$"WHERE `id` = {primary_key_value}", connection);
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
				MessageBox.Show("Заполните корректно все числовые поля, поля с выбором и даты!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void Button_reset_Click(object sender, RoutedEventArgs e)
		{
			ComboBox_supplier.Text = (string)old_values[0];
			ComboBox_paint_type.SelectedIndex = ((string)old_values[3] == "литр") ? 0 : 1;
			ComboBox_product_name.SelectedIndex = old_color_index;
			TextBox_product_amount.Text = float.Parse((string)old_values[2]).ToString();
			TextBox_product_amount.Text = TextBox_product_amount.Text.Replace(",", ".");
			TextBox_price.Text = float.Parse((string)old_values[4]).ToString();
			TextBox_price.Text = TextBox_price.Text.Replace(",", ".");
			DatePicker_order.SelectedDate = (DateTime)old_values[5];
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

		private void ComboBox_paint_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			switch(ComboBox_paint_type.SelectedIndex)
			{
				case 0:
					fill_product_with("литр");
					Label_measurement.Content = "литров";
					break;
				case 1:
					fill_product_with("рулон");
					Label_measurement.Content = "рулонов";
					break;
			}
			ComboBox_product_name.SelectedIndex = -1;
		}

		private void fill_product_with(string measurement)
		{
			ComboBox_product_name.IsEnabled = true;
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

		private void TextBox_price_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				Label_full_price.Content = (decimal.Parse(TextBox_price.Text.Replace('.',',')) * decimal.Parse(TextBox_product_amount.Text.Replace('.', ',')));
			}
			catch
			{
				Label_full_price.Content = "Введите количество и цену";
			}
		}
	}
}
