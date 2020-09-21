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
	/// Логика взаимодействия для WindowIncomeReport.xaml
	/// </summary>
	public partial class WindowIncomeReport : Window
	{
		MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user id = root; password = 1234; port = 3306; persistsecurityinfo = True; sslmode = None; database = autopaint");
		public WindowIncomeReport()
		{
			InitializeComponent();
			combobox_service_type.SelectedIndex = 0;
			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand("SELECT DISTINCT `service_type` FROM `requests`;", connection);
				MySqlDataReader data = comm.ExecuteReader();
				while (data.Read())
				{
					combobox_service_type.Items.Add(data[0].ToString());
				}
			}
			finally
			{
				connection.Close();
			}

		}

		private void button_make_report_Click(object sender, RoutedEventArgs e)
		{
			//Отчёт по приходу денежных средств
			string info = " ";
			string service_type_condition = "";
			if (combobox_service_type.Text != "все")
			{
				info += "по услуге \"" + combobox_service_type.Text + "\" ";
				service_type_condition = $"`requests`.`service_type` = '{combobox_service_type.Text}'";
			}
			string date_condition = "";
			if (date_start.SelectedDate.HasValue && date_end.SelectedDate.HasValue)
			{
				if (date_start.SelectedDate.Value <= date_end.SelectedDate.Value)
				{
					info += "в промежутке с " + date_start.SelectedDate.Value.ToString("dd.MM.yyyy") + " по " + date_end.SelectedDate.Value.ToString("dd.MM.yyyy");
					date_condition = $"DATE(paint_date) BETWEEN '{date_start.SelectedDate.Value:yyyy.MM.dd}' AND '{date_end.SelectedDate.Value:yyyy.MM.dd}'";
				}
				else
				{
					MessageBox.Show("Начальная дата не может быть позже конечной!");
					return;
				}
			}
			
			string command = "SELECT `service_type`, `parts_to_paint`, `paint_cost`, `picture_name`, `paint_date` FROM `requests` WHERE";
			List<string> condition_list = new List<string>();
			if (service_type_condition != "")
				condition_list.Add(service_type_condition);
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
			command += " `request_status` = 'Обработано'";
			List<string> service_types = new List<string>();
			List<string> parts_to_paint = new List<string>();
			List<string> paint_costs = new List<string>();
			List<string> picture_names = new List<string>();
			List<string> paint_dates = new List<string>();
			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand(command, connection);
				MySqlDataReader data = comm.ExecuteReader();
				while (data.Read())
				{
					service_types.Add(data[0].ToString());
					parts_to_paint.Add(data[1].ToString());
					paint_costs.Add(data[2].ToString());
					picture_names.Add(data[3].ToString());
					paint_dates.Add(((DateTime)data[4]).ToString("dd.MM.yyyy"));
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
			if (service_types.Count == 0)
			{
				MessageBox.Show("По заданным фильтрам нет записей!");
				return;
			}
			List<string> prices = new List<string>();
			for (int i = 0; i < service_types.Count; i++)
			{
				switch(service_types[i])
				{
					case "Аэрография": prices.Add(Shortcuts.get_one_string_data_from($"SELECT `price` FROM `pictures` WHERE `name` = '{picture_names[i]}'", connection)); break;
					default:
						decimal price = decimal.Parse(Shortcuts.get_one_string_data_from($"SELECT SUM(cost) FROM `car_parts` WHERE `id` & {parts_to_paint[i]};", connection));
						prices.Add((price + decimal.Parse(paint_costs[i])).ToString());
						break;
				}
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
					foreach(string cost in prices)
					{
						price += decimal.Parse(cost.Replace('.',','));
					}
					Word.Document word_doc = WordApp.Documents.Open(Directory.GetCurrentDirectory() + $@"\income_report.docx");
					Shortcuts.replace_word("{info}", info, word_doc);
					Shortcuts.replace_word("{service_type}", Shortcuts.make_column_from(service_types), word_doc);
					Shortcuts.replace_word("{cost}", Shortcuts.make_column_from(prices).Replace(',', '.'), word_doc);
					Shortcuts.replace_word("{date}", Shortcuts.make_column_from(paint_dates), word_doc);
					Shortcuts.replace_word("{price}", price.ToString().Replace(',', '.'), word_doc);
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
			combobox_service_type.SelectedIndex = 0;
			date_start.SelectedDate = null;
			date_end.SelectedDate = null;
		}
	}
}
