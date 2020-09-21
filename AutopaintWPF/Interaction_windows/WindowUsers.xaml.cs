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
using System.IO;
using Microsoft.Win32;

namespace AutopaintWPF
{
	/// <summary>
	/// Логика взаимодействия для WindowUsers.xaml
	/// </summary>
	public partial class WindowUsers : Window
	{
		object[] old_values;
		string primary_key_value;
		byte[] new_image;
		QueryMode mode;
		MainWindow parent;
		MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user id = root; password = 1234; port = 3306; persistsecurityinfo = True; sslmode = None; database = autopaint");
		public WindowUsers(QueryMode mode, MainWindow parent, string primary_key_value = "")
		{
			InitializeComponent();
			this.mode = mode;
			this.parent = parent;
			this.primary_key_value = primary_key_value;

			ComboBox_role.ItemsSource = Shortcuts.get_full_column_from("roles", "role", connection);
			ComboBox_gender.ItemsSource = Shortcuts.get_full_column_from("genders", "gender", connection);

			if (mode == QueryMode.add)
			{
				Button_reset.Visibility = Visibility.Collapsed;
				Button_accept.Content = "Добавить";
			}
			else
			{
				Button_accept.Content = "Изменить";
				byte[] image_bytes = Shortcuts.get_image("users", "mail", primary_key_value, connection);
				new_image = image_bytes;
				Shortcuts.set_image(Image, image_bytes);
				try
				{
					connection.Open();
					MySqlCommand comm = new MySqlCommand($"SELECT * FROM `users` WHERE `mail` = '{primary_key_value}'", connection);
					MySqlDataReader data = comm.ExecuteReader();
					data.Read();
					TextBox_mail.Text = primary_key_value;
					TextBox_password.Text = data[1].ToString();
					TextBox_surname.Text = data[2].ToString();
					TextBox_first_name.Text = data[3].ToString();
					TextBox_second_name.Text = data[4].ToString();
					TextBox_phone.Text = data[5].ToString();
					ComboBox_role.Text = data[6].ToString();
					ComboBox_gender.Text = data[7].ToString();
					old_values = new object[9]{ 
						primary_key_value,
						data[1].ToString(),
						data[2].ToString(),
						data[3].ToString(),
						data[4].ToString(),
						data[5].ToString(),
						data[6].ToString(),
						data[7].ToString(),
						image_bytes};
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

		private void Button_accept_Click(object sender, RoutedEventArgs e)
		{
			if (TextBox_mail.Text != "" && TextBox_password.Text != "" &&
			TextBox_surname.Text != "" && TextBox_first_name.Text != "" &&
			TextBox_second_name.Text != "" && TextBox_phone.Text != "" &&
			ComboBox_role.Text != "" && ComboBox_gender.Text != "" &&
			Image.Source != null)
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
						try
						{
							connection.Open();
							MySqlCommand comm = new MySqlCommand("INSERT INTO `users` (`mail`, `password`, " +
								"`surname`, `first_name`, `second_name`, " +
								"`phone`, `role`, `gender`, `image`) " +
								$"VALUES ('{TextBox_mail.Text.ToLower()}', '{TextBox_password.Text}', " +
								$" '{TextBox_surname.Text}', '{TextBox_first_name.Text}', '{TextBox_second_name.Text}', " +
								$"'{TextBox_phone.Text}', '{ComboBox_role.Text}', '{ComboBox_gender.Text}', @image);", connection);
							MySqlParameter img_param = new MySqlParameter("@image", new_image);
							comm.Parameters.Add(img_param);
							comm.ExecuteNonQuery();
						}
						catch (Exception ex)
						{
							success = false;
							if (ex.Message.Contains("Duplicate entry"))
								MessageBox.Show("Пользователь с такой почтой уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
							else
								MessageBox.Show(ex.Message);
						}
						finally
						{
							connection.Close();
						}
						break;
					case QueryMode.change:
						User current_user = parent.current_user;
						int admin_count = int.Parse(Shortcuts.get_one_string_data_from("SELECT count(*) from `users` " +
							"WHERE `role`='администратор'", connection));
						if (current_user.mail == primary_key_value &&
							ComboBox_role.Text != "администратор" &&
							admin_count <= 1)
						{
							MessageBox.Show("Изменение роли невозможно! Единственный администратор в системе!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
							return;
						}
						//int user_count = int.Parse(Shortcuts.get_one_string_data_from($"SELECT count(*) FROM `users` WHERE `mail` = {TextBox_mail.Text.ToLower()}", connection));
						if (!confirm_action("Вы точно хотите изменить данные пользователя?", "Подтверждение действия"))
						{
							return;
						}
						try
						{
							connection.Open();
							MySqlCommand comm = new MySqlCommand("UPDATE `users` " +
								$"SET `mail` = '{TextBox_mail.Text.ToLower()}', " +
								$"`password` = '{TextBox_password.Text}', " +
								$"`surname` = '{TextBox_surname.Text}', " +
								$"`first_name` = '{TextBox_first_name.Text}', " +
								$"`second_name` = '{TextBox_second_name.Text}', " +
								$"`phone` = '{TextBox_phone.Text}', " +
								$"`gender` = '{ComboBox_gender.Text}', " +
								$"`role` = '{ComboBox_role.Text}', " +
								$"`image` = @image " +
								$"WHERE `mail` = '{primary_key_value}';", connection);
							MySqlParameter img_param = new MySqlParameter("@image", new_image);
							comm.Parameters.Add(img_param);
							comm.ExecuteNonQuery();
						}
						catch (Exception ex)
						{
							success = false;
							if (ex.Message.Contains("Duplicate entry"))
								MessageBox.Show("Пользователь с такой почтой уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
							else
								MessageBox.Show(ex.Message);
						}
						finally
						{
							connection.Close();
						}
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
				MessageBox.Show("Заполните все поля и выберите изображение!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void Button_reset_Click(object sender, RoutedEventArgs e)
		{
			TextBox_mail.Text = (string)old_values[0];
			TextBox_password.Text = (string)old_values[1];
			TextBox_surname.Text = (string)old_values[2];
			TextBox_first_name.Text = (string)old_values[3];
			TextBox_second_name.Text = (string)old_values[4];
			TextBox_phone.Text = (string)old_values[5];
			ComboBox_role.Text = (string)old_values[6];
			ComboBox_gender.Text = (string)old_values[7];
			new_image = (byte[])old_values[8];
			Shortcuts.set_image(Image, (byte[])old_values[8]);
		}

		private void Button_cancel_Click(object sender, RoutedEventArgs e)
		{
			parent.Focus();
			Close();
		}

		private void TextBox_mail_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^a-z0-9._%+@-]");
			e.Handled = regex.IsMatch(e.Text);
		}

		private void TextBox_ru_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^А-Яа-яЁё]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		private void Button_choose_image_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Изображения(*.BMP;*.JPG;*.JPEG;*.PNG)|*.BMP;*.JPG;*.JPEG;*.PNG";
			if (ofd.ShowDialog() == true)
			{
				string image_path = ofd.FileName;
				FileStream fs = new FileStream(image_path, FileMode.Open, FileAccess.Read);
				BinaryReader br = new BinaryReader(fs);
				new_image = br.ReadBytes((int)fs.Length);
				Shortcuts.set_image(Image, new_image);
			}
		}

		private void TextBox_phone_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}
	}
}
