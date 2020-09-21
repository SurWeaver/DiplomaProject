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
	/// Логика взаимодействия для WindowPaint_types.xaml
	/// </summary>
	public partial class WindowPaint_types : Window
	{
		string[] old_values;
		string primary_key_value;
		QueryMode mode;
		MainWindow parent;
		MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user id = root; password = 1234; port = 3306; persistsecurityinfo = True; sslmode = None; database = autopaint");
		public WindowPaint_types(QueryMode mode, MainWindow parent, string primary_key_value = "")
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
					MySqlCommand comm = new MySqlCommand($"SELECT * FROM `paint_types` " +
						$"WHERE `paint_type` = '{primary_key_value}';", connection);
					MySqlDataReader data = comm.ExecuteReader();
					data.Read();
					TextBox_paint_type.Text = primary_key_value;
					TextBox_cost_ratio.Text = data[1].ToString();
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
			}
		}

		private void Button_accept_Click(object sender, RoutedEventArgs e)
		{
			if (TextBox_paint_type.Text != "" && TextBox_cost_ratio.Text != "")
			{
				bool success = true;
				switch (mode)
				{
					case QueryMode.add:
						success = Shortcuts.add("paint_types", new string[] { "paint_type", "cost_ratio" },
							new string[] { TextBox_paint_type.Text, TextBox_cost_ratio.Text },
							connection);
						break;
					case QueryMode.change:
						success = Shortcuts.change("paint_types", new string[] { "paint_type", "cost_ratio" },
							new string[] { TextBox_paint_type.Text, TextBox_cost_ratio.Text },
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
			TextBox_paint_type.Text = old_values[0];
			TextBox_cost_ratio.Text = old_values[1];
		}

		private void Button_cancel_Click(object sender, RoutedEventArgs e)
		{
			parent.Focus();
			Close();
		}

		private void TextBox_number_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		private void TextBox_ru_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^А-Яа-яЁё]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		private void TextBox_mail_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^a-z0-9._%+@-]");
			e.Handled = regex.IsMatch(e.Text);
		}
	}
}
