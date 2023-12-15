using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Xml;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;

namespace WindowsFormsApp5
{
    public partial class MainForm : Form
    {
        private SQLiteConnection SQLiteConn;
        private DataTable dTable;

        public MainForm()
        {
            InitializeComponent();
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            comboBox3.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            radioButton1.Enabled = false;
            radioButton2.Enabled = false;
            radioButton3.Enabled = false;

            comboBox5.Enabled = false;
            comboBox4.Enabled = false;
            button8.Enabled = false;
            textBox2.Enabled = false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SQLiteConn = new SQLiteConnection();
            dTable = new DataTable();
        }

        //
        // Возвращаемые динамические запросы
        //
        private string SQL_AllTable()
        {
            if(comboBox1.SelectedIndex != -1)
                return "SELECT * FROM [" + comboBox1.SelectedItem + "] order by 1";
            else
                return "SELECT * FROM [" + comboBox1.Items[1] + "] order by 1";
        }

        private string SQL_FilterByManufacture()
        {
            return "SELECT * FROM [" + comboBox1.SelectedItem + "] " +
            "WHERE Производитель = \"" + comboBox3.SelectedItem + "\";";
        }

        private string SQL_FilterByProduct()
        {
            return "SELECT * FROM [" + comboBox1.SelectedItem + "] " +
            "WHERE [Количество(Коробки)] <= 23";
        }

        //
        // Взаимодействия с БД (доп инфа)
        //
        private void GetTableNames()
        {
            string SQLQuery = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;";
            SQLiteCommand command = new SQLiteCommand(SQLQuery, SQLiteConn);
            SQLiteDataReader reader = command.ExecuteReader();

            comboBox1.Items.Clear();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader[0].ToString());
            }
        }

        private void GetTableColumns(bool isIndivid = false)
        {
            if (isIndivid == false)
            {
                string SQLQuery = "PRAGMA table_info(\"" + comboBox1.SelectedItem + "\");";
                SQLiteCommand command = new SQLiteCommand(SQLQuery, SQLiteConn);
                SQLiteDataReader read = command.ExecuteReader();

                comboBox2.Items.Clear();
                while (read.Read())
                {
                    comboBox2.Items.Add((string)read[1]);
                }
            }
            else
            {
                string SQLQuery = "PRAGMA table_info(\"" + comboBox1.Items[comboBox1.Items.Count - 1] + "\");";
                SQLiteCommand command = new SQLiteCommand(SQLQuery, SQLiteConn);
                SQLiteDataReader read = command.ExecuteReader();


                comboBox5.Items.Clear();
                while (read.Read())
                {
                    comboBox5.Items.Add((string)read[1]);
                }
            }
        }

        private void GetManufactures()
        {
            int kol = 0;
            string sl, s2;
            comboBox3.Items.Clear();
            for (int row = 0; row < dTable.Rows.Count; row++)
            {
                for (int i = 0; i < comboBox3.Items.Count; i++)
                {
                    sl = (string)dTable.Rows[row].ItemArray[2];
                    s2 = (string)comboBox3.Items[i];

                    if (String.Compare(sl, s2) == 0) kol++;
                }
                if (kol == 0) comboBox3.Items.Add(dTable.Rows[row].ItemArray[2]); else kol = 0;
            }
        }


        //
        // Клики мыши
        //
        private void button1_Click_DataBaseConnect(object sender, EventArgs e)
        {
            if (OpenDBFile() == true)
            {
                GetTableNames(); //nonyue
                GetTableColumns(true);
                comboBox1.Enabled = true;
                button2.Enabled = true;

                comboBox5.Enabled = true;
                comboBox4.Enabled = true;
                button8.Enabled = true;
                textBox2.Enabled = true;
            }
        }

        private void button2_Click_ShowTable(object sender, EventArgs e)
        {
            dTable.Clear();
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите таблицу!", "Owubka", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            comboBox2.Enabled = true;
            comboBox3.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            radioButton1.Enabled = true;
            radioButton2.Enabled = true;
            radioButton3.Enabled = true;


            ShowTable(SQL_AllTable());
            GetTableColumns();
            GetManufactures();
        }

        private void button3_Click_Calculation(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите поле для расчета", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            double max;
            double min;
            double sum = 0;
            double value;
            try
            {
                max = Convert.ToDouble(dTable.Rows[0].ItemArray[comboBox2.SelectedIndex]);
                min = Convert.ToDouble(dTable.Rows[0].ItemArray[comboBox2.SelectedIndex]);
            }
            catch
            {
                MessageBox.Show("Поле не является числовым", "Owubka",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            for (int row = 0; row < dTable.Rows.Count; row++)
            {
                value = Convert.ToDouble(dTable.Rows[row].ItemArray[comboBox2.SelectedIndex]);
                if (value > max) max = value;
                if (value < min) min = value;
                sum = sum + value;
            }

            string MyMessage = "";
            if ((sender as System.Windows.Forms.Button).Name == "button3")

                MyMessage = "Минимальное значениее в поле" + comboBox2.Text + " = " + min.ToString();
            if ((sender as System.Windows.Forms.Button).Name == "button4")

                MyMessage = "Максиамльное значениее в поле" + comboBox2.Text + " = " + max.ToString();
            if ((sender as System.Windows.Forms.Button).Name == "button5")
                MyMessage = "Среднее значениее в поле" + comboBox2.Text + " = " + (sum / dTable.Rows.Count).ToString();
            if ((sender as System.Windows.Forms.Button).Name == "button6")
                MyMessage = "Сумма значениее в поле" + comboBox2.Text + " = " + sum.ToString();

            MessageBox.Show(MyMessage, "Расчеты", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }

        private void button7_Click_Filter(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex == -1 && radioButton2.Checked == true)
            {
                MessageBox.Show("Выберите производителя товара", "Owubka",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (radioButton1.Checked == true)
                ShowTable(SQL_AllTable());
            if (radioButton2.Checked == true)
                ShowTable(SQL_FilterByManufacture());
            if (radioButton3.Checked == true)
                ShowTable(SQL_FilterByProduct());
        }

        private bool OpenDBFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory =
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.Filter = "Текстовые файлы (*.db)|*.db|Все файлы (*.*)|*.*";

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {

                SQLiteConn = new SQLiteConnection("Data Source=" + openFileDialog.FileName + ";Version = 3;");
                SQLiteConn.Open();
                SQLiteCommand command = new SQLiteCommand();
                command.Connection = SQLiteConn;
                return true;
            }
            else return false;
        }

        private void ShowTable(string SQLQuery)
        {
            dTable.Clear();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(SQLQuery, SQLiteConn);
            adapter.Fill(dTable);
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            for (int col = 0; col < dTable.Columns.Count; col++)
            {
                string ColName = dTable.Columns[col].ColumnName;
                dataGridView1.Columns.Add(ColName, ColName);

                dataGridView1.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            for (int row = 0; row < dTable.Rows.Count; row++)
            {
                dataGridView1.Rows.Add(dTable.Rows[row].ItemArray);
            }
        }



        //
        // Индивидуальное 
        //
        private void button8_Click_UnionTables(object sender, EventArgs e)
        {
            if (comboBox4.SelectedIndex != -1 && comboBox5.SelectedIndex != -1 && textBox2.Text != "")
            {
                dTable.Clear();
                string nameTable = "Kukuwka";
                DropTable(nameTable);
                GetTableNames();
                comboBox1.SelectedIndex = 0;


                if ((isNumTable() == true) && (isNum() == true) == true)
                {
                    CreateNewTable(nameTable);
                    comboBox1.SelectedItem = nameTable;
                }
                else if ((isNumTable() == false) && (isNum() == false) == false)
                {
                    CreateNewTable(nameTable);
                    comboBox1.SelectedItem = nameTable;
                }
                else
                {
                    MessageBox.Show("Ошибка значений и параметров", "Owubka");
                }

                GetTableNames();
                ShowTable(SQL_AllTable());

            }
            else
                MessageBox.Show("Не выбраны условия/значения для создания таблицы", "owubka");
        }


        private bool isNumTable()
        {
            string SQLQuery = $"Select * From [{comboBox1.Items[comboBox1.Items.Count - 1]}]";
            SQLiteCommand command = new SQLiteCommand(SQLQuery, SQLiteConn);
            SQLiteDataReader read = command.ExecuteReader();

            try
            {
                string a = read.GetString(comboBox5.SelectedIndex);
                return false;
            }
            catch
            {
                return true;
            }

        }

        private bool isNum()
        {
            double x;
            try
            {
                x = double.Parse(textBox2.Text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool isTableExist(string name)
        {
            foreach (string elem in comboBox1.Items)
            {
                if (elem == name) return true;
            }

            return false;
        }

        private void DropTable(string nameTable)
        {
            if (isTableExist(nameTable) == true)
            {
                string SQLQuery;
                SQLiteCommand command;
                try
                {
                    SQLQuery = $"DROP TABLE {nameTable}";
                    command = new SQLiteCommand(SQLQuery, SQLiteConn);
                    command.ExecuteNonQuery();
                }
                catch
                {

                }
            }
        }





        private void CreateNewTable(string nameTable)
        {
            string SQLQuery;
            SQLiteCommand command;

            SQLQuery = $"CREATE TABLE [{nameTable}] as ";
            foreach (string elem in comboBox1.Items)
            {
                if (elem != nameTable)
                {
                    SQLQuery += $"SELECT * From [{elem}] ";
                    SQLQuery += $"WHERE [{comboBox5.SelectedItem}] ";

                    if (isNum() == false)
                    {
                        SQLQuery += $" = \"{textBox2.Text}\"";
                    }
                    else
                    {
                        SQLQuery += $" {comboBox4.SelectedItem} {textBox2.Text}";
                    }

                    if (comboBox1.Items[comboBox1.Items.Count - 1].ToString() == elem)
                    {
                        SQLQuery += " ;";
                    }
                    else
                    {
                        SQLQuery += " UNION ";
                    }
                }
            }

            try
            {
                command = new SQLiteCommand(SQLQuery, SQLiteConn);
                command.ExecuteNonQuery();
            }
            catch { }
        }

    }
}