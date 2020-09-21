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
	/// Логика взаимодействия для WindowPictures.xaml
	/// </summary>
	public partial class WindowPictures : Window
	{
		object[] old_values;
		string primary_key_value;
		byte[] new_image;
		QueryMode mode;
		MainWindow parent;
		MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user id = root; password = 1234; port = 3306; persistsecurityinfo = True; sslmode = None; database = autopaint");
		public WindowPictures(QueryMode mode, MainWindow parent, string primary_key_value = "")
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
				TextBox_name.Text = primary_key_value;
				byte[] image_bytes = Shortcuts.get_image("pictures", "name", primary_key_value, connection);
				new_image = image_bytes;
				Shortcuts.set_image(Image, image_bytes);
				TextBox_price.Text = Shortcuts.get_one_string_data_from($"select `price` from `pictures` where `name` = '{primary_key_value}'", connection);
				TextBox_price.Text = TextBox_price.Text.Replace(",", ".");
				old_values = new object[2]{ 
					primary_key_value,
					image_bytes};
			}
		}

		private void Button_accept_Click(object sender, RoutedEventArgs e)
		{
			if (TextBox_name.Text != "" && Image.Source != null)
			{
				bool success = true;
				switch (mode)
				{
					case QueryMode.add:
						try
						{
							connection.Open();
							MySqlCommand comm = new MySqlCommand("INSERT INTO `pictures` (`name`, `price`, `image`) " +
								$"VALUES ('{TextBox_name.Text}', {TextBox_price.Text}, @image);", connection);
							MySqlParameter img_param = new MySqlParameter("@image", new_image);
							comm.Parameters.Add(img_param);
							comm.ExecuteNonQuery();
						}
						catch (Exception ex)
						{
							success = false;
							MessageBox.Show(ex.Message);
						}
						finally
						{
							connection.Close();
						}
						break;
					case QueryMode.change:
						try
						{
							connection.Open();
							MySqlCommand comm = new MySqlCommand("UPDATE `pictures` SET " +
								$"`name` = '{TextBox_name.Text}', " +
								$"`price` = {TextBox_price.Text}, " +
								"`image` = @image " +
								$"WHERE `name` = '{primary_key_value}';", connection);
							MySqlParameter img_param = new MySqlParameter("@image", new_image);
							comm.Parameters.Add(img_param);
							comm.ExecuteNonQuery();
						}
						catch (Exception ex)
						{
							success = false;
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
			new_image = (byte[])old_values[1];
			TextBox_name.Text = (string)old_values[0];
			Shortcuts.set_image(Image, (byte[])old_values[1]);
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

		private void TextBox_price_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9.]+");
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
	}
}
