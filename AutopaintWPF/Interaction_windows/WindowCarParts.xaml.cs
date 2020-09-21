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
	/// Логика взаимодействия для WindowCarParts.xaml
	/// </summary>
	public partial class WindowCarParts : Window
	{
		string[] old_values;
		string primary_key_value;
		MainWindow parent;
		MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user id = root; password = 1234; port = 3306; persistsecurityinfo = True; sslmode = None; database = autopaint");
		public WindowCarParts(MainWindow parent, string primary_key_value)
		{
			InitializeComponent();
			this.parent = parent;
			this.primary_key_value = primary_key_value;
			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand($"SELECT * FROM `car_parts` " +
					$"WHERE `id` = '{primary_key_value}';", connection);
				MySqlDataReader data = comm.ExecuteReader();
				data.Read();
				primary_key_value = data[0].ToString();
				Label_name.Content = data[1].ToString();
				TextBox_surface_size.Text = data[2].ToString();
				TextBox_cost.Text = data[3].ToString();
				TextBox_cost.Text = TextBox_cost.Text.Replace(",", ".");
				old_values = new string[2];
				old_values[0] = data[2].ToString();
				old_values[1] = data[3].ToString();
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

		private void Button_accept_Click(object sender, RoutedEventArgs e)
		{
			if (TextBox_surface_size.Text != "" && TextBox_cost.Text != "")
			{
				if (Shortcuts.change("car_parts", new string[] { "id", "name", "surface_size", "cost" },
					new string[] { primary_key_value, Label_name.Content.ToString(), TextBox_surface_size.Text, TextBox_cost.Text },
					primary_key_value, connection))
				{
					parent.Focus();
					parent.fill_table();
					Close();
				}
			}
			else
			{
				MessageBox.Show("Введите правильные значения и не оставляйте пустых полей!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void Button_reset_Click(object sender, RoutedEventArgs e)
		{
			TextBox_surface_size.Text = old_values[0];
			TextBox_cost.Text = float.Parse((string)old_values[1]).ToString();
			TextBox_cost.Text = TextBox_cost.Text.Replace(",", ".");
		}

		private void Button_cancel_Click(object sender, RoutedEventArgs e)
		{
			parent.Focus();
			Close();
		}

		private void TextBox_CarParts_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		private void TextBox_cost_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9.]+");
			e.Handled = regex.IsMatch(e.Text);
		}
	}
}
