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
	/// Логика взаимодействия для WindowPicReport.xaml
	/// </summary>
	public partial class WindowPicReport : Window
	{
		MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user id = root; password = 1234; port = 3306; persistsecurityinfo = True; sslmode = None; database = autopaint");
		public WindowPicReport()
		{
			InitializeComponent();
			combobox_picture_name.SelectedIndex = 0;
			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand("SELECT DISTINCT `picture_name` FROM `requests` " +
					"WHERE `service_type` = 'Аэрография';", connection);
				MySqlDataReader data = comm.ExecuteReader();
				while (data.Read())
				{
					combobox_picture_name.Items.Add(data[0].ToString());
				}
			}
			finally
			{
				connection.Close();
			}

		}

		private void button_make_report_Click(object sender, RoutedEventArgs e)
		{
			//Отчёт о работах по аэрографии
			string info = " ";
			string picture_name_condition = "";
			if (combobox_picture_name.Text != "все")
			{
				info += "с наименованием \"" + combobox_picture_name.Text + "\" ";
				picture_name_condition = $"`requests`.`picture_name` = '{combobox_picture_name.Text}'";
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
			
			string command = "SELECT `picture_name`, `pictures`.`price`, `paint_date` FROM `requests` " +
				"JOIN `pictures` ON `pictures`.`name` = `requests`.`picture_name` " +
				"WHERE";
			List<string> condition_list = new List<string>();
			if (picture_name_condition != "")
				condition_list.Add(picture_name_condition);
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
			command += " `request_status` = 'Обработано' AND `service_type` = 'Аэрография'";
			List<string> picture_names = new List<string>();
			List<string> costs = new List<string>();
			List<string> paint_dates = new List<string>();
			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand(command, connection);
				MySqlDataReader data = comm.ExecuteReader();
				while (data.Read())
				{
					picture_names.Add(data[0].ToString());
					costs.Add(data[1].ToString());
					paint_dates.Add(((DateTime)data[2]).ToString("dd.MM.yyyy"));
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
			if (picture_names.Count == 0)
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
					foreach(string cost in costs)
					{
						price += decimal.Parse(cost);
					}
					Word.Document word_doc = WordApp.Documents.Open(Directory.GetCurrentDirectory() + $@"\pic_report.docx");
					Shortcuts.replace_word("{info}", info, word_doc);
					Shortcuts.replace_word("{picture_name}", Shortcuts.make_column_from(picture_names), word_doc);
					Shortcuts.replace_word("{cost}", Shortcuts.make_column_from(costs).Replace(',', '.'), word_doc);
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
			combobox_picture_name.SelectedIndex = 0;
			date_start.SelectedDate = null;
			date_end.SelectedDate = null;
		}
	}
}
