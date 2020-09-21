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
	/// Логика взаимодействия для WindowClients.xaml
	/// </summary>
	public partial class WindowClients : Window
	{
		string[] old_values;
		string primary_key_value;
		QueryMode mode;
		Window parent;
		MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user id = root; password = 1234; port = 3306; persistsecurityinfo = True; sslmode = None; database = autopaint");
		public WindowClients(QueryMode mode, Window parent, string primary_key_value = "")
		{
			InitializeComponent();
			this.mode = mode;
			this.parent = parent;
			this.primary_key_value = primary_key_value;

			ComboBox_gender.ItemsSource = Shortcuts.get_full_column_from("genders", "gender", connection);

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
					MySqlCommand comm = new MySqlCommand($"SELECT * FROM `clients` " +
						$"WHERE `mail` = '{primary_key_value}';", connection);
					MySqlDataReader data = comm.ExecuteReader();
					data.Read();
					TextBox_mail.Text = primary_key_value;
					TextBox_phone.Text = data[1].ToString();
					TextBox_surname.Text = data[2].ToString();
					TextBox_first_name.Text = data[3].ToString();
					TextBox_second_name.Text = data[4].ToString();
					ComboBox_gender.Text = data[5].ToString();
					old_values = new string[6]{ 
						data[0].ToString(),
						data[1].ToString(),
						data[2].ToString(),
						data[3].ToString(),
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
			if (TextBox_mail.Text != "" && TextBox_phone.Text != "" && 
			TextBox_surname.Text != "" && TextBox_first_name.Text != "" &&
			TextBox_second_name.Text != "" && ComboBox_gender.Text != "")
			{
				string m = TextBox_mail.Text.ToLower();
				if (m.Split('@').Length != 2)
				{
					MessageBox.Show("Неправильный ввод почты!");
					return;
				}
				if (!(m.Contains('.') && m.Split('@')[1].Split('.')[1].Length >= 2 &&
					m.Split('@')[1].Split('.')[1].Length <= 4))
				{
					MessageBox.Show("Неправильный ввод почты!");
					return;
				}
				if (m.LastIndexOf('.') - m.IndexOf('@') <= 1)
				{
					MessageBox.Show("Неправильный ввод почты!");
					return;
				}
				if (m.IndexOf('@') == 0)
				{
					MessageBox.Show("Неправильный ввод почты!");
					return;
				}
				/*Regex reg = new Regex("^[A-Za-z0-9._-]+@[A-Za-z0-9.-]+.[A-Za-z]{2,4}$");
				if (!reg.IsMatch(TextBox_mail.Text.ToLower()))
				{
					MessageBox.Show("Неправильный ввод почты!");
					return;
				}*/
				bool success = true;
				switch (mode)
				{
					case QueryMode.add:
						success = Shortcuts.add("clients", new string[] { "mail", "phone", "surname", "first_name", "second_name", "gender" },
							new string[] { TextBox_mail.Text.ToLower(), TextBox_phone.Text,
							TextBox_surname.Text, TextBox_first_name.Text,
							TextBox_second_name.Text, ComboBox_gender.Text},
							connection);
						break;
					case QueryMode.change:
						success = Shortcuts.change("clients", new string[] { "mail", "phone", "surname", "first_name", "second_name", "gender" },
							new string[] { TextBox_mail.Text.ToLower(), TextBox_phone.Text,
							TextBox_surname.Text, TextBox_first_name.Text,
							TextBox_second_name.Text, ComboBox_gender.Text},
							primary_key_value,
							connection);
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
			TextBox_mail.Text = old_values[0];
			TextBox_phone.Text = old_values[1];
			TextBox_surname.Text = old_values[2];
			TextBox_first_name.Text = old_values[3];
			TextBox_second_name.Text = old_values[4];
			ComboBox_gender.Text = old_values[5];
		}

		private void Button_cancel_Click(object sender, RoutedEventArgs e)
		{
			parent.Focus();
			Close();
		}

		private void TextBox_phone_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
