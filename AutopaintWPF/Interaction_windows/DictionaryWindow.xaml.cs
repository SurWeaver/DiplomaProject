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

namespace AutopaintWPF
{
	/// <summary>
	/// Логика взаимодействия для DictionaryWindow.xaml
	/// </summary>
	public partial class DictionaryWindow : Window
	{
		public string table_name, field_name, old_value;
		public MainWindow parent_window;
		public QueryMode mode;

		MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user id = root; password = 1234; port = 3306; persistsecurityinfo = True; sslmode = None; database = autopaint");
		public DictionaryWindow(QueryMode current_mode, string ru_table, string table, string ru_field, string field, MainWindow parent, string old_value = "")
		{
			InitializeComponent();
			Textbox_item_value.Focus();
			mode = current_mode;
			table_name = table;
			field_name = field;
			Label_field_name.Content = ru_field;
			parent_window = parent;
			this.old_value = old_value;
			Textbox_item_value.Text = old_value;
			if (mode == QueryMode.add)
			{
				Title = "Добавление в таблицу '" + ru_table + "'";
				Button_cancel.Visibility = Visibility.Collapsed;
				Button_action.Content = "Добавить";
			}
			else
			{

				Title = "Изменение в таблице '" + ru_table + "'";
			}
		}

		private void Button_exit_Click(object sender, RoutedEventArgs e)
		{
			Close();
			parent_window.Focus();
		}

		private void Button_action_Click(object sender, RoutedEventArgs e)
		{
			bool success = true;
			if (mode == QueryMode.change)
			{
				success = Shortcuts.change(table_name, new string[] { field_name },
					new string[] { Textbox_item_value.Text },
					old_value,
					connection);
			}
			else
			{
				success = Shortcuts.add(table_name, new string[] { field_name }, new string[] { Textbox_item_value.Text }, connection);
			}
			if (success)
			{
				Close();
				parent_window.fill_table();
				parent_window.Focus();
			}
		}

		private void Button_cancel_Click(object sender, RoutedEventArgs e)
		{
			Textbox_item_value.Text = old_value;
		}
	}
}