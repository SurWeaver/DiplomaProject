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
using System.IO;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using Word = Microsoft.Office.Interop.Word;

namespace AutopaintWPF.Report_windows
{
	/// <summary>
	/// Логика взаимодействия для WindowSupplyReport.xaml
	/// </summary>
	public partial class WindowSupplyReport : Window
	{
		MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user id = root; password = 1234; port = 3306; persistsecurityinfo = True; sslmode = None; database = autopaint");
		public WindowSupplyReport()
		{
			InitializeComponent();
			combobox_user.SelectedIndex = 0;
			combobox_supplier.SelectedIndex = 0;
			combobox_paint_name.SelectedIndex = 0;
			combobox_paint_type.SelectedIndex = 0;
			//user_mail
			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand("SELECT DISTINCT `user_mail` FROM `supplies`", connection);
				MySqlDataReader data = comm.ExecuteReader();
				while (data.Read())
				{
					combobox_user.Items.Add(data[0].ToString());
				}
			}
			finally
			{
				connection.Close();
			}
			//supplier
			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand("SELECT DISTINCT `supplier` FROM `supplies`", connection);
				MySqlDataReader data = comm.ExecuteReader();
				while (data.Read())
				{
					combobox_supplier.Items.Add(data[0].ToString());
				}
			}
			finally
			{
				connection.Close();
			}
			//paint_name
			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand("SELECT DISTINCT `product_name` FROM `supplies`", connection);
				MySqlDataReader data = comm.ExecuteReader();
				while (data.Read())
				{
					combobox_paint_name.Items.Add(data[0].ToString());
				}
			}
			finally
			{
				connection.Close();
			}
		}

		private void button_make_report_Click(object sender, RoutedEventArgs e)
		{
			//Отчёт о поставках
			string info = " ";
			string user_mail_condition = "";
			if (combobox_user.Text != "все")
			{
				string[] full_name = Shortcuts.get_one_string_data_from("SELECT CONCAT(`surname`,' ',`first_name`,' ',`second_name`) FROM `users` " +
					$"WHERE `mail` = '{combobox_user.Text}'", connection).Split(' ');
				string manager_name = full_name[0] + " " + full_name[1][0] + ". " + full_name[2][0] + ".";
				info += "от менеджера " + manager_name + " ";
				user_mail_condition = $"`user_mail` = '{combobox_user.Text}'";
			}
			string paint_type_condition = "";
			switch(combobox_paint_type.Text)
			{
				case "краска": 
					info += "красок ";
					paint_type_condition = "`measurement` = 'литр'";
					break;
				case "плёнка": 
					info += "плёнок ";
					paint_type_condition = "`measurement` = 'рулон'";
					break;
				case "все":
					info += "продукции "; break;
			}
			string product_name_condition = "";
			if (combobox_paint_name.Text != "все")
			{
				info += "с наименованием \"" + combobox_paint_name.Text + "\" ";
				product_name_condition = $"`product_name` = '{combobox_paint_name.Text}'";
			}
			string supplier_condition = "";
			if (combobox_supplier.Text != "все")
			{
				info += "от поставщика с названием \"" + combobox_supplier.Text + "\" ";
				supplier_condition = $"`supplier` = '{combobox_supplier.Text}'";
			}
			string date_condition = "";
			if (date_start.SelectedDate.HasValue && date_end.SelectedDate.HasValue)
			{
				if (date_start.SelectedDate.Value <= date_end.SelectedDate.Value)
				{
					info += "в промежутке с " + date_start.SelectedDate.Value.ToString("dd.MM.yyyy") + " по " + date_end.SelectedDate.Value.ToString("dd.MM.yyyy");
					date_condition = $"DATE(delivery_date) BETWEEN '{date_start.SelectedDate.Value:yyyy.MM.dd}' AND '{date_end.SelectedDate.Value:yyyy.MM.dd}'";
				}
				else
				{
					MessageBox.Show("Начальная дата не может быть позже конечной!");
					return;
				}
			}
			
			string command = "SELECT `user_mail`, `supplier`, `product_name`, `product_amount`, `measurement`, `price`, `delivery_date` FROM `supplies` WHERE";
			List<string> condition_list = new List<string>();
			if (user_mail_condition != "")
				condition_list.Add(user_mail_condition);
			if (paint_type_condition != "")
				condition_list.Add(paint_type_condition);
			if (product_name_condition != "")
				condition_list.Add(product_name_condition);
			if (supplier_condition != "")
				condition_list.Add(supplier_condition);
			if (date_condition != "")
				condition_list.Add(date_condition);
			if (condition_list.Count != 0)
			{
				for (int i = 0; i < condition_list.Count; i++)
				{
					command += " " + condition_list[i];
					if (i != condition_list.Count - 1)
						command += " AND";
				}
				command += " AND";
			}
			command += " `delivery_date` IS NOT NULL";
			List<string> user_mails = new List<string>();
			List<string> suppliers = new List<string>();
			List<string> product_names = new List<string>();
			List<string> product_amounts = new List<string>();
			List<string> measurements = new List<string>();
			List<string> prices = new List<string>();
			List<string> delivery_dates = new List<string>();
			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand(command, connection);
				MySqlDataReader data = comm.ExecuteReader();
				while (data.Read())
				{
					user_mails.Add(data[0].ToString());
					suppliers.Add(data[1].ToString());
					product_names.Add(data[2].ToString());
					product_amounts.Add(data[3].ToString());
					measurements.Add(data[4].ToString());
					prices.Add(data[5].ToString());
					delivery_dates.Add(((DateTime)data[6]).ToString("dd.MM.yyyy"));
				}
			}
			catch
			{
				MessageBox.Show("Нет подключения к базе. Нельзя сформировать отчёт. Попробуйте позже.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				connection.Close();
				return;
			}
			finally
			{
				connection.Close();
			}
			if (user_mails.Count == 0)
			{
				MessageBox.Show("По заданным фильтрам нет записей!");
				return;
			}
			List<string> manager_names = new List<string>();
			foreach (string mail in user_mails)
			{
				string[] full_name = Shortcuts.get_one_string_data_from("SELECT CONCAT(`surname`,' ',`first_name`,' ',`second_name`) FROM `users` " +
					$"WHERE `mail` = '{mail}'", connection).Split(' ');
				string manager_name = full_name[0] + " " + full_name[1][0] + ". " + full_name[2][0] + ".";
				manager_names.Add(manager_name);
			}
			try
			{
				SaveFileDialog SFDialog = new SaveFileDialog();
				SFDialog.Filter = "Microsoft Word Document (*.docx)|*.docx";
				if (SFDialog.ShowDialog() == true)
				{
					Word.Application WordApp = new Word.Application();
					WordApp.Visible = false;
					decimal price = 0;
					for(int i = 0; i < prices.Count; i++)
					{
						price += decimal.Parse(prices[i]) * decimal.Parse(product_amounts[i]);
					}
					string price_in_doc = price.ToString().Replace(',', '.');
					int dot_pos = price_in_doc.IndexOf('.');
					if (dot_pos > 0)
						price_in_doc = price_in_doc.Substring(0, dot_pos + 3);
					Word.Document word_doc = WordApp.Documents.Open(Directory.GetCurrentDirectory() + $@"\supply_report.docx");
					Shortcuts.replace_word("{info}", info, word_doc);
					Shortcuts.replace_word("{product_name}", Shortcuts.make_column_from(product_names), word_doc);
					Shortcuts.replace_word("{supplier}", Shortcuts.make_column_from(suppliers), word_doc);
					Shortcuts.replace_word("{name}", Shortcuts.make_column_from(manager_names), word_doc);
					Shortcuts.replace_word("{measurement}", Shortcuts.make_column_from(measurements), word_doc);
					Shortcuts.replace_word("{amount}", Shortcuts.make_column_from(product_amounts).Replace(',','.'), word_doc);
					Shortcuts.replace_word("{cost}", Shortcuts.make_column_from(prices).Replace(',', '.'), word_doc);
					Shortcuts.replace_word("{date}", Shortcuts.make_column_from(delivery_dates), word_doc);
					Shortcuts.replace_word("{price}", price_in_doc, word_doc);
					Shortcuts.replace_word("{current_date}", DateTime.Now.ToString("dd.MM.yyyy"), word_doc);
					word_doc.SaveAs2(FileName: SFDialog.FileName);
					word_doc.Close();
					MessageBox.Show("Файл успешно сохранён!");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("При сохранении чека возникла ошибка. Документ не сохранён.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void button_cancel_Click(object sender, RoutedEventArgs e)
		{
			combobox_paint_name.SelectedIndex = 0;
			combobox_paint_type.SelectedIndex = 0;
			combobox_supplier.SelectedIndex = 0;
			combobox_user.SelectedIndex = 0;
			date_start.SelectedDate = null;
			date_end.SelectedDate = null;
		}
	}
}
