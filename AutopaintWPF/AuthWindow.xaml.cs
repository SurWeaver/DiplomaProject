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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.IO;
using System.Security.Cryptography;

namespace AutopaintWPF
{
	/// <summary>
	/// Логика взаимодействия для AuthWindow.xaml
	/// </summary>
	public partial class AuthWindow : Window
	{
		User current_user;
		const string remember_file_path = "remember.txt";
		int try_count = 3;
		MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user id = root; password = 1234; port = 3306; persistsecurityinfo = True; sslmode = None; database = autopaint");
		public AuthWindow()
		{
			InitializeComponent();
			textbox_mail.Focus();
			//Вспоминание пользователя
			if (File.Exists(remember_file_path))
			{
				using (StreamReader sr = new StreamReader(remember_file_path))
				{
					textbox_mail.Text = sr.ReadLine();
					passwordbox_password.Password = sr.ReadLine();
					sr.Close();
				}
			}
		}

		private void button_enter_Click(object sender, RoutedEventArgs e)
		{
			string mail = textbox_mail.Text.ToLower();
			string pass = passwordbox_password.Password;
			if (mail != "" && pass != "")
			{
				string user_count = Shortcuts.get_one_string_data_from($@"select count(*) from `users`
																 where `mail` = '{mail}'
																 and `password` = '{pass}';",
																 connection);
				
				if (user_count == "1")
				{
					try_count = 3;
					current_user = Shortcuts.get_user(mail, pass, connection);
					//Запоминание пользователя
					if (Checkbox_remember.IsChecked.Value)
					{
						if (File.Exists(remember_file_path))
						{
							File.Delete(remember_file_path);
						}
						using (StreamWriter sw = new StreamWriter(remember_file_path))
						{
							sw.WriteLine(textbox_mail.Text);
							sw.WriteLine(passwordbox_password.Password);
							sw.Close();
						}
					}
					int hours = DateTime.Now.Hour;
					string greeting;
					if (hours >= 6 && hours <= 11)
						greeting = "Доброе утро, ";
					else if (hours >= 12 && hours <= 17)
						greeting = "Добрый день, ";
					else if (hours >= 18 && hours <= 21)
						greeting = "Добрый вечер, ";
					else
						greeting = "Доброй ночи, ";
					greeting += current_user.first_name + " " + current_user.second_name + "!";
					MessageBox.Show(greeting);
					switch(current_user.role)
					{
						case "администратор":
							MainWindow window = new MainWindow(current_user, this);
							window.Show();
							Hide();
							break;
						case "менеджер по заявкам":
							RequestManagerWindow rmw = new RequestManagerWindow(current_user, this);
							rmw.Show();
							Hide();
							break;
						case "менеджер по поставкам":
							SupplyManagerWindow smw = new SupplyManagerWindow(current_user, this);
							smw.Show();
							Hide();
							break;
					}
				}
				else
				{
					MessageBox.Show("Неверный логин или пароль.");
					try_count--;
					if (try_count <= 0)
					{
						MessageBox.Show("Количество попыток превышено, введите проверочный текст!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
						textbox_mail.Text = "";
						passwordbox_password.Password = "";
						change_login_ability(false);
						generate_captcha();
						change_captcha_visibility(Visibility.Visible);
					}
				}
			}
			else
			{
				MessageBox.Show("Введите логин и пароль.");
			}
		}

		/// <summary>
		/// При нажатии ENTER приложение исполнит событие нажатие на кнопку "Войти"
		/// </summary>
		private void passwordbox_password_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				button_enter_Click(new object(), new RoutedEventArgs());
			}
		}

		private void generate_captcha()
		{
			Random random = new Random();
			string question = "";
			List<char> symbols = new List<char>();
			for (char i = 'a'; i <= 'z'; i++)
				symbols.Add(i);

			for (char i = 'A'; i <= 'Z'; i++)
				symbols.Add(i);

			for (char i = '0'; i <= '9'; i++)
				symbols.Add(i);
			for (int i = 0; i < 5; i++)
			{
				question += symbols[random.Next(symbols.Count)];
			}
			label_captcha.Content = question;
		}

		private void change_captcha_visibility(Visibility visibility)
		{
			StackPanel_captcha.Visibility = visibility;
			label_captcha.Visibility = visibility;
			if (visibility != Visibility.Visible)
				label_captcha.Content = "";
		}

		private void change_login_ability(bool ability)
		{
			textbox_mail.IsEnabled = ability;
			passwordbox_password.IsEnabled = ability;
		}

		private void button_accept_captcha_Click(object sender, RoutedEventArgs e)
		{
			if (textbox_captcha.Text == (string)label_captcha.Content)
			{
				change_captcha_visibility(Visibility.Hidden);
				change_login_ability(true);
				try_count = 3;
			}
			else
			{
				generate_captcha();
				textbox_captcha.Text = "";
			}
		}

		private void passwordbox_password_GotFocus(object sender, RoutedEventArgs e)
		{
			passwordbox_password.SelectAll();
		}
	}
}
