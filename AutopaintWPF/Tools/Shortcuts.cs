using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MySql.Data.MySqlClient;
using System.Windows.Media;
using Word = Microsoft.Office.Interop.Word;

namespace AutopaintWPF
{
	static class Shortcuts
	{
		/// <returns>Возвращается, исполнилась ли команда</returns>
		public static bool execute_command(string command, MySqlConnection connection)
		{
			bool result = true;
			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand(command, connection);
				comm.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				result = false;
				if (ex.Message.Contains("Cannot delete or update a parent row"))
				{
					int begin = ex.Message.IndexOf("`autopaint`.`") + 13;
					string msg = ex.Message.Substring(begin);
					int end = msg.IndexOf("`");
					string table_name = ex.Message.Substring(begin, end);
					for (int i = 0; i < MainWindow.tables.Length; i++)
					{
						if (table_name == MainWindow.tables[i])
						{
							table_name = MainWindow.ru_tables[i];
						}
					}
					MessageBox.Show("Нельзя удалить запись. Она используется в таблице '" + table_name + "'", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				else if (ex.Message.Contains("Incorrect decimal value"))
					MessageBox.Show("Неверный ввод дробного числа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				else if (ex.Message.Contains("Out of range value"))
					MessageBox.Show("Введено недопустимое значение", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				else if (ex.Message.Contains("Data too long"))
					MessageBox.Show("Введено слишком большое значение", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				else if (ex.Message.Contains("Duplicate entry"))
					MessageBox.Show("Изменение записи невозможно! Такая запись уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				else
					MessageBox.Show(ex.Message);
			}
			finally
			{
				connection.Close();
			}
			return result;
		}

		/// <summary>
		/// Получить дату в одну строку из команды
		/// </summary>
		public static string get_one_string_data_from(string command, MySqlConnection connection)
		{
			string result = "";
			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand(command, connection);
				MySqlDataReader data = comm.ExecuteReader();
				data.Read();
				result = data[0].ToString();
			}
			catch
			{
				return result;
			}
			finally
			{
				connection.Close();
			}
			return result;
		}

		public static List<string> get_full_column_from(string table, string column, MySqlConnection connection)
		{
			List<string> l = new List<string>();
			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand($"SELECT `{column}` FROM `{table}`;", connection);
				MySqlDataReader data = comm.ExecuteReader();
				while (data.Read())
				{
					l.Add(data[0].ToString());
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
			return l;
		}

		public static List<string> get_full_column_from(string table, string column, string condition, MySqlConnection connection)
		{
			List<string> l = new List<string>();
			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand($"SELECT `{column}` FROM `{table}`" +
					$"WHERE {condition};", connection);
				MySqlDataReader data = comm.ExecuteReader();
				while (data.Read())
				{
					l.Add(data[0].ToString());
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
			return l;
		}

		public static bool add(string table, string[] fields, string[] values, MySqlConnection connection)
		{
			string unique_item_count = get_one_string_data_from($"SELECT count(*) FROM `{table}`" +
				$"where `{fields[0]}` = '{values[0]}';", connection);
			if (unique_item_count != "0")
			{
				MessageBox.Show($"Запись с таким значением в поле '{MainWindow.fields[fields[0]]}' существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
			string command_string = $"INSERT INTO `{table}`(";
			for (int i = 0; i < fields.Length; i++)
			{
				command_string += $"`{fields[i]}`" + ((i == fields.Length - 1) ? "" : ", ");
			}
			command_string += ") VALUES (";
			for (int i = 0; i < fields.Length; i++)
			{
				command_string += $"'{values[i]}'" + ((i == values.Length - 1) ? ")" : ", ");
			}
			return execute_command(command_string, connection);
		}

		public static bool change(string table, string[] fields, string[] values, string old_value, MySqlConnection connection)
		{
			string unique_item_count = get_one_string_data_from($"SELECT count(*) FROM `{table}`" +
				$"where `{fields[0]}` = '{values[0]}' and `{fields[0]}` != '{old_value}';", connection);
			if (unique_item_count != "0")
			{
				MessageBox.Show($"Запись с таким значением в поле '{MainWindow.fields[fields[0]]}' существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
			string command_string = $"UPDATE `{table}` SET ";

			for (int i = 0; i < fields.Length; i++)
			{
				command_string += $"`{fields[i]}` = '{values[i]}'" + ((i == fields.Length - 1) ? "" : ", ");
			}
			command_string += $" where `{fields[0]}` = '{old_value}';";

			return execute_command(command_string, connection);
		}

		/// <summary>
		/// Получить передаваемую структуру, хранящую данные пользователя
		/// </summary>
		public static User get_user(string mail, string password, MySqlConnection connection)
		{
			string user_count = get_one_string_data_from($@"select count(*) from `users`
								 where `mail` = '{mail}'
								 and `password` = '{password}';",
								 connection);
			if (user_count == "1")
			{
				User user = new User();
				try
				{
					connection.Open();
					MySqlCommand command = new MySqlCommand($@"select * from `users`
																   where `mail` = '{mail}'
																   and `password` = '{password}';", connection);
					MySqlDataReader data = command.ExecuteReader();
					data.Read();
					user.mail = data[data.GetOrdinal("mail")].ToString();
					user.password = data[data.GetOrdinal("password")].ToString();
					user.surname = data[data.GetOrdinal("surname")].ToString();
					user.first_name = data[data.GetOrdinal("first_name")].ToString();
					user.second_name = data[data.GetOrdinal("second_name")].ToString();
					user.phone = data[data.GetOrdinal("phone")].ToString();
					user.role = data[data.GetOrdinal("role")].ToString();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
				finally
				{
					connection.Close();
				}
				return user;
			}
			else
				return new User();
		}

		public static byte[] get_image(string table, string field, string primary_key_value, MySqlConnection connection)
		{
			byte[] result = null;
			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand($"SELECT `image` FROM `{table}` " +
				$"where `{field}` = '{primary_key_value}';", connection);
				MySqlDataReader data = comm.ExecuteReader();
				data.Read();
				result = (byte[])data[0];
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				connection.Close();
			}
			return result;
		}

		public static void set_image(Image im, byte[] image_data)
		{
			using (MemoryStream ms = new MemoryStream(image_data))
			{
				var imageSource = new BitmapImage();
				imageSource.BeginInit();
				imageSource.StreamSource = ms;
				imageSource.CacheOption = BitmapCacheOption.OnLoad;
				imageSource.EndInit();
				im.Source = imageSource;
			}
		}

		public static void replace_word(string original, string new_text, Word.Document word_document)
		{
			Word.Range range = word_document.Content;
			range.Find.ClearFormatting();
			range.Find.Execute(FindText: original, ReplaceWith: new_text);
		}
		public static string make_column_from(List<string> list)
		{
			string result = "";
			for(int i = 0; i < list.Count; i++)
			{
				result += list[i];
				if (i != list.Count - 1)
					result += "\v";
			}
			return result;
		}

		public static ComboBoxItem create_color_box(string text, string color)
		{
			ComboBoxItem item = new ComboBoxItem();
			item.Tag = text;
			StackPanel panel = new StackPanel();
			panel.Orientation = Orientation.Horizontal;
			Border border = new Border();
			border.Width = 12;
			border.Height = 12;
			border.Background = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#" + color));
			border.Margin = new Thickness(0d, 0d, 5d, 0d);
			TextBlock textBlock = new TextBlock();
			textBlock.Text = text;

			panel.Children.Add(border);
			panel.Children.Add(textBlock);
			item.Content = panel;
			return item;
		}
	}
}
