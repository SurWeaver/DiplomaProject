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
	/// Логика взаимодействия для WindowStorageReport.xaml
	/// </summary>
	public partial class WindowStorageReport : Window
	{
		MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user id = root; password = 1234; port = 3306; persistsecurityinfo = True; sslmode = None; database = autopaint");
		public WindowStorageReport()
		{
			InitializeComponent();
			combobox_paint_type.SelectedIndex = 0;
			combobox_paint_name.SelectedIndex = 0;
			combobox_supplier.SelectedIndex = 0;
			//supplier
			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand("SELECT DISTINCT `supplier` FROM `storage`;", connection);
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
				MySqlCommand comm = new MySqlCommand("SELECT DISTINCT `product_name` FROM `storage`;", connection);
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
			//Отчёт об остатках на складе
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
				product_name_condition = $"`product_name` = '{combobox_paint_name.Text}'";
			}
			string supplier_condition = "";
			if (combobox_supplier.Text != "все")
			{
				info += "от поставщика с названием \"" + combobox_supplier.Text + "\" ";
				supplier_condition = $"`supplier` = '{combobox_supplier.Text}'";
			}
			
			string command = "SELECT `product_name`, `product_amount`, `measurement`, `supplier`, `average_purchase_price` FROM `storage`";
			List<string> condition_list = new List<string>();
			if (paint_type_condition != "")
				condition_list.Add(paint_type_condition);
			if (product_name_condition != "")
				condition_list.Add(product_name_condition);
			if (supplier_condition != "")
				condition_list.Add(supplier_condition);
			if (condition_list.Count != 0)
			{
				command += " WHERE";
				for (int i = 0; i < condition_list.Count; i++)
				{
					command += " " + condition_list[i];
					if (i != condition_list.Count - 1)
						command += " AND";
				}
			}
			List<string> product_names = new List<string>();
			List<string> paint_amounts = new List<string>();
			List<string> measurements = new List<string>();
			List<string> suppliers = new List<string>();
			List<string> prices = new List<string>();
			try
			{
				connection.Open();
				MySqlCommand comm = new MySqlCommand(command, connection);
				MySqlDataReader data = comm.ExecuteReader();
				while (data.Read())
				{
					product_names.Add(data[0].ToString());
					paint_amounts.Add(data[1].ToString());
					measurements.Add(data[2].ToString());
					suppliers.Add(data[3].ToString());
					prices.Add(data[4].ToString());
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
					for(int i = 0; i < prices.Count; i++)
					{
						price += decimal.Parse(prices[i]) * decimal.Parse(paint_amounts[i]);
					}
					string price_in_doc = price.ToString().Replace(',', '.');
					int dot_pos = price_in_doc.IndexOf('.');
					if (dot_pos > 0)
						price_in_doc = price_in_doc.Substring(0, dot_pos + 3);
					Word.Document word_doc = WordApp.Documents.Open(Directory.GetCurrentDirectory() + $@"\storage_report.docx");
					Shortcuts.replace_word("{info}", info, word_doc);
					Shortcuts.replace_word("{product_name}", Shortcuts.make_column_from(product_names), word_doc);
					Shortcuts.replace_word("{supplier}", Shortcuts.make_column_from(suppliers), word_doc);
					Shortcuts.replace_word("{amount}", Shortcuts.make_column_from(paint_amounts).Replace(',', '.'), word_doc);
					Shortcuts.replace_word("{measurement}", Shortcuts.make_column_from(measurements), word_doc);
					Shortcuts.replace_word("{cost}", Shortcuts.make_column_from(prices).Replace(',', '.'), word_doc);
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
			combobox_paint_type.SelectedIndex = 0;
			combobox_paint_name.SelectedIndex = 0;
			combobox_supplier.SelectedIndex = 0;
		}
	}
}
