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
	/// Логика взаимодействия для WindowColors.xaml
	/// </summary>
	public partial class WindowColors : Window
	{
		string[] old_values;
		string primary_key_value;
		QueryMode mode;
		MainWindow parent;
		MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user id = root; password = 1234; port = 3306; persistsecurityinfo = True; sslmode = None; database = autopaint");
		public WindowColors(QueryMode mode, MainWindow parent, string primary_key_value = "")
		{
			InitializeComponent();
			this.mode = mode;
			this.parent = parent;
			this.primary_key_value = primary_key_value;

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
					MySqlCommand comm = new MySqlCommand($"SELECT * FROM `colors` " +
						$"WHERE `color_code` = '{primary_key_value}';", connection);
					MySqlDataReader data = comm.ExecuteReader();
					data.Read();
					TextBox_color_code.Text = primary_key_value;
					TextBox_description.Text = data[1].ToString();
					old_values = new string[2]{ 
						data[0].ToString(),
						data[1].ToString()};
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
				finally
				{
					connection.Close();
				}
				set_border_color();
			}
		}

		private void set_border_color()
		{
			if (TextBox_color_code.Text.Length == 6)
			{
				Border_color.Background = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#" + TextBox_color_code.Text));
				Border_color.Visibility = Visibility.Visible;
			}
			else
			{
				Border_color.Visibility = Visibility.Hidden;
			}
		}

		private void Button_accept_Click(object sender, RoutedEventArgs e)
		{
			if (TextBox_color_code.Text != "" && TextBox_description.Text != ""
				&& TextBox_color_code.Text.Length == 6)
			{
				bool success = true;
				switch (mode)
				{
					case QueryMode.add:
						success = Shortcuts.add("colors", new string[] { "color_code", "description" },
							new string[] { TextBox_color_code.Text, TextBox_description.Text },
							connection);
						break;
					case QueryMode.change:
						success = Shortcuts.change("colors", new string[] { "color_code", "description" },
							new string[] { TextBox_color_code.Text, TextBox_description.Text },
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
			TextBox_color_code.Text = old_values[0];
			TextBox_description.Text = old_values[1];
		}

		private void Button_cancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
			parent.Focus();
		}

		private void TextBox_hex_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9abcdefABCDEF]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		private void TextBox_ru_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^А-Яа-яЁё-]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		private void TextBox_color_code_TextChanged(object sender, TextChangedEventArgs e)
		{
			set_border_color();
		}
	}
}