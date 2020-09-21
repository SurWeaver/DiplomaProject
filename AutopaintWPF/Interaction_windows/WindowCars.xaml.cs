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

namespace AutopaintWPF
{
	/// <summary>
	/// Логика взаимодействия для WindowCars.xaml
	/// </summary>
	public partial class WindowCars : Window
	{
		string[] old_values;
		string primary_key_value;
		QueryMode mode;
		Window parent;
		MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user id = root; password = 1234; port = 3306; persistsecurityinfo = True; sslmode = None; database = autopaint");
		public WindowCars(QueryMode mode, Window parent, string primary_key_value = "")
		{
			InitializeComponent();
			this.mode = mode;
			this.parent = parent;
			this.primary_key_value = primary_key_value;

			List<string> mails = Shortcuts.get_full_column_from("clients", "mail", connection);
			ComboBox_owner_mail.ItemsSource = mails;

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
					MySqlCommand comm = new MySqlCommand($"SELECT * FROM `cars` " +
						$"WHERE `vin` = '{primary_key_value}';", connection);
					MySqlDataReader data = comm.ExecuteReader();
					data.Read();
					TextBox_vin.Text = primary_key_value;
					TextBox_number.Text = data[1].ToString();
					ComboBox_owner_mail.Text = data[2].ToString();
					TextBox_color.Text = data[3].ToString();
					TextBox_model.Text = data[4].ToString();
					old_values = new string[5]{ 
						data[0].ToString(),
						data[1].ToString(),
						data[2].ToString(),
						data[3].ToString(),
						data[4].ToString() };
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
			if (TextBox_vin.Text != "" && TextBox_number.Text != "" && ComboBox_owner_mail.Text != "" && TextBox_color.Text != "" && TextBox_model.Text != "")
			{
				bool success = true;
				switch (mode)
				{
					case QueryMode.add:
						success = Shortcuts.add("cars", new string[] { "vin", "number", "owner_mail", "color", "model" },
							new string[] { TextBox_vin.Text.ToUpper(),
							TextBox_number.Text,
							ComboBox_owner_mail.Text,
							TextBox_color.Text,
							TextBox_model.Text},
							connection);
						break;
					case QueryMode.change:
						success = Shortcuts.change("cars", new string[] { "vin", "number", "owner_mail", "color", "model" },
							new string[] { TextBox_vin.Text.ToUpper(),
							TextBox_number.Text,
							ComboBox_owner_mail.Text,
							TextBox_color.Text,
							TextBox_model.Text},
							primary_key_value, connection);
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
				MessageBox.Show("Заполните все пустые поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void Button_reset_Click(object sender, RoutedEventArgs e)
		{
			TextBox_vin.Text = old_values[0];
			TextBox_number.Text = old_values[1];
			ComboBox_owner_mail.Text = old_values[2];
			TextBox_color.Text = old_values[3];
			TextBox_model.Text = old_values[4];
		}

		private void Button_cancel_Click(object sender, RoutedEventArgs e)
		{
			parent.Focus();
			Close();
		}

		private void TextBox_vin_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			//Для обеспечения уникальности из VIN исключены I, O, Q из-за сходства с 1 и 0
			Regex regex = new Regex("[^A-HJ-NPR-Za-hj-npr-z0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		private void TextBox_number_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^АВЕКМНОРСТУХавекмнорстух0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}
	}
}
