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
	/// Логика взаимодействия для WindowProducts.xaml
	/// </summary>
	public partial class WindowProducts : Window
	{
		string[] old_values;
		string primary_key_value;
		int old_color_index = 0;
		QueryMode mode;
		MainWindow parent;
		MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user id = root; password = 1234; port = 3306; persistsecurityinfo = True; sslmode = None; database = autopaint");
		public WindowProducts(QueryMode mode, MainWindow parent, string primary_key_value = "")
		{
			InitializeComponent();
			this.mode = mode;
			this.parent = parent;
			this.primary_key_value = primary_key_value;

			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand("SELECT `color_code` FROM `colors`;", connection);
				MySqlDataReader data = comm.ExecuteReader();
				while (data.Read())
				{
					ComboBox_color_code.Items.Add(Shortcuts.create_color_box(data[0].ToString(), data[0].ToString()));
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

			ComboBox_paint_type.ItemsSource = Shortcuts.get_full_column_from("paint_types", "paint_type", connection);

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
					MySqlCommand comm = new MySqlCommand($"SELECT * FROM `products` " +
						$"WHERE `name` = '{primary_key_value}';", connection);
					MySqlDataReader data = comm.ExecuteReader();
					data.Read();
					TextBox_name.Text = primary_key_value;
					for (int i = 0; i < ComboBox_color_code.Items.Count; i++)
					{
						if ((string)(ComboBox_color_code.Items[i] as ComboBoxItem).Tag == data[2].ToString())
						{
							ComboBox_color_code.SelectedIndex = i;
							old_color_index = i;
							break;
						}
					}
					ComboBox_paint_type.Text = data[1].ToString();
					old_values = new string[3]{ 
						data[0].ToString(),
						data[1].ToString(),
						data[2].ToString()};
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
			if (TextBox_name.Text != "" && ComboBox_paint_type.Text != "" &&
				ComboBox_color_code.SelectedIndex != -1)
			{
				bool success = true;
				switch (mode)
				{
					case QueryMode.add:
						success = Shortcuts.add("products", new string[] { "name", "paint_type", "color_code", "measurement" },
							new string[] { TextBox_name.Text, ComboBox_paint_type.Text,
							(string)(ComboBox_color_code.Items[ComboBox_color_code.SelectedIndex] as ComboBoxItem).Tag,
							(ComboBox_paint_type.Text == "Плёнка")? "рулон": "литр"},
							connection);
						break;
					case QueryMode.change:
						success = Shortcuts.change("products", new string[] { "name", "paint_type", "color_code", "measurement" },
							new string[] { TextBox_name.Text, ComboBox_paint_type.Text,
							(string)(ComboBox_color_code.Items[ComboBox_color_code.SelectedIndex] as ComboBoxItem).Tag,
							(ComboBox_paint_type.Text == "Плёнка")? "рулон": "литр"},
							primary_key_value,
							connection);
						break;
				}
				if (success)
				{
					parent.Focus();
					parent.fill_table();
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
			TextBox_name.Text = old_values[0];
			ComboBox_paint_type.Text = old_values[1];
			ComboBox_color_code.SelectedIndex = old_color_index;
		}

		private void Button_cancel_Click(object sender, RoutedEventArgs e)
		{
			parent.Focus();
			Close();
		}

		private void TextBox_ru_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^А-Яа-яЁё]+");
			e.Handled = regex.IsMatch(e.Text);
		}
	}
}
