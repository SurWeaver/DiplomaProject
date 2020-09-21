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
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.IO;
using Word = Microsoft.Office.Interop.Word;

namespace AutopaintWPF.Report_windows
{
	/// <summary>
	/// Логика взаимодействия для WindowSpentReport.xaml
	/// </summary>
	public partial class WindowSpentReport : Window
	{
		MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user id = root; password = 1234; port = 3306; persistsecurityinfo = True; sslmode = None; database = autopaint");
		public WindowSpentReport()
		{
			InitializeComponent();
			combobox_paint_name.SelectedIndex = 0;
			combobox_paint_type.SelectedIndex = 0;
			combobox_supplier.SelectedIndex = 0;
			//supplier
			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand("SELECT DISTINCT `supplier` FROM `requests` " +
					"WHERE `service_type` != 'Аэрография';", connection);
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
			//product_name
			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand("SELECT DISTINCT `product_name` FROM `requests` " +
					"WHERE `service_type` != 'Аэрография';", connection);
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
			//Отчёт о расходе
			string info = " ";
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
				product_name_condition = $"`requests`.`product_name` = '{combobox_paint_name.Text}'";
			}
			string supplier_condition = "";
			if (combobox_supplier.Text != "все")
			{
				info += "от поставщика с названием \"" + combobox_supplier.Text + "\" ";
				supplier_condition = $"`requests`.`supplier` = '{combobox_supplier.Text}'";
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
			
			string command = "SELECT `product_name`, `measurement`, `paint_amount`, `paint_cost`, `paint_date`, `supplier` FROM `requests` WHERE";
			List<string> condition_list = new List<string>();
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
			command += " `request_status` = 'Обработано' AND `service_type` != 'Аэрография'";
			List<string> product_names = new List<string>();
			List<string> measurements = new List<string>();
			List<string> paint_amounts = new List<string>();
			List<string> paint_costs = new List<string>();
			List<string> paint_dates = new List<string>();
			List<string> suppliers = new List<string>();
			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand(command, connection);
				MySqlDataReader data = comm.ExecuteReader();
				while (data.Read())
				{
					product_names.Add(data[0].ToString());
					measurements.Add(data[1].ToString());
					paint_amounts.Add(data[2].ToString());
					paint_costs.Add(data[3].ToString());
					paint_dates.Add(((DateTime)data[4]).ToString("dd.MM.yyyy"));
					suppliers.Add(data[5].ToString());
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
			if (product_names.Count == 0)
			{
				MessageBox.Show("По заданным фильтрам нет записей!");
				return;
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
					foreach(string cost in paint_costs)
					{
						price += decimal.Parse(cost);
					}
					Word.Document word_doc = WordApp.Documents.Open(Directory.GetCurrentDirectory() + $@"\spent_report.docx");
					Shortcuts.replace_word("{info}", info, word_doc);
					Shortcuts.replace_word("{product_name}", Shortcuts.make_column_from(product_names), word_doc);
					Shortcuts.replace_word("{supplier}", Shortcuts.make_column_from(suppliers), word_doc);
					Shortcuts.replace_word("{paint_type}", Shortcuts.make_column_from(measurements), word_doc);
					Shortcuts.replace_word("{amount}", Shortcuts.make_column_from(paint_amounts).Replace(',', '.'), word_doc);
					Shortcuts.replace_word("{cost}", Shortcuts.make_column_from(paint_costs).Replace(',', '.'), word_doc);
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
			combobox_paint_name.SelectedIndex = 0;
			combobox_paint_type.SelectedIndex = 0;
			combobox_supplier.SelectedIndex = 0;
			date_start.SelectedDate = null;
			date_end.SelectedDate = null;
		}
	}
}
