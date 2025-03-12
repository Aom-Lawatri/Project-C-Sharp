using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp3
{
    public partial class orderhistory : Form
    {
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=admin;charset=utf8;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        public orderhistory()
        {
            InitializeComponent();
        }
        //เรียกใช้เมื่อเกิดข้อผิดพลาดในการโหลดข้อมูล
        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        { 
            e.Cancel = true;
        }
        private void orderhistory_Load(object sender, EventArgs e)
        {
            

            MySqlConnection conn = databaseConnection();
            DataSet ds = new DataSet();
            conn.Open();

            MySqlCommand cmd;

            cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM history";

            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(ds);

            conn.Close();
            dataGridView1.DataSource = ds.Tables[0].DefaultView;
        }
        

        private void buttonback2_Click(object sender, EventArgs e)
        { 
            this.Hide();
            choice choice = new choice();
            choice.Show();
        }
       
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 12) 
            {
                DataGridViewCell cell = null;
                foreach (DataGridViewCell selectedCell in dataGridView1.SelectedCells)
                {
                    cell = selectedCell;
                    break;
                }
                if (cell != null)
                {
                    DataGridViewRow row = cell.OwningRow;
                    string selectedId = row.Cells["receipt_ad"].Value.ToString();
                    MessageBox.Show(selectedId);


                    string file = selectedId;
                    Process.Start(file);


                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                textBox1.Text = "username";  
                textBox3.Visible = false;   
                textBox4.Visible = false;
                textBox5.Visible = false;
                textBox1.Visible = true;  

            }
            else if (comboBox1.SelectedIndex == 1)
            {
                textBox3.Text = "31";  
                textBox3.Visible = true;
                textBox4.Text = "01";
                textBox4.Visible = true;
                textBox5.Visible = true;
                textBox5.Text = "2024";
                textBox1.Visible = false;
            }
            else if (comboBox1.SelectedIndex == 2)
            {

                textBox3.Visible = false;    
                textBox4.Visible = true;
                textBox4.Text = "01";
                textBox5.Visible = true;
                textBox5.Text = "2024";
                textBox1.Visible = false;

            }
            else if (comboBox1.SelectedIndex == 3)
            {
                textBox1.Text = "2024";    
                textBox3.Visible = false;
                textBox4.Visible = false;
                textBox5.Visible = true;
                textBox5.Text = "2024";
                textBox1.Visible = false;

            }
            

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        //ค้นหาข้อมูลจากฐานข้อมูลตาม username ที่กรอกใน textBox1 
        //แล้วนำข้อมูลมาแสดงใน dataGridView2 หากข้อมูลไม่ตรงกัน จะแสดงกล่องข้อความแจ้งเตือน
        //คำนวณยอดรวมจากบิลที่ไม่ซ้ำกันและแสดงยอดรวมใน textBox6
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                MySqlConnection conn = databaseConnection();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM history WHERE username = @username", conn);
                cmd.Parameters.AddWithValue("@username", textBox1.Text);
                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                    dataGridView2.DataSource = dt;
                    
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("ไม่มีข้อมูลที่ตรงกัน");
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
                double total = 0;
                HashSet<string> uniqueBills = new HashSet<string>(); // ใช้นับบิลที่ไม่ซ้ำกัน

                foreach (DataGridViewRow r in dataGridView2.Rows)
                {
                    string billName = r.Cells["receipt_ad"].Value.ToString(); 
                    if (!uniqueBills.Contains(billName)) 
                    {
                        uniqueBills.Add(billName); 
                        total += Convert.ToDouble(r.Cells["total"].Value); 
                    }
                }

                textBox6.Text = total.ToString("F2");  
                best_selling();
            }
            else if (comboBox1.SelectedIndex == 1) //รวมข้อมมูลผู้ใช้กรอก แล้วค้นหา
            {
                MySqlConnection conn = databaseConnection();
                string dateString = $"{textBox3.Text.PadLeft(2, '0')}-{textBox4.Text.PadLeft(2, '0')}-{textBox5.Text}"; 
                MySqlCommand cmd = new MySqlCommand(
                    "SELECT * FROM history WHERE DATE(STR_TO_DATE(datetime, '%d-%m-%Y %H:%i:%s')) = STR_TO_DATE(@date, '%d-%m-%Y')",
                    conn
                );
                cmd.Parameters.AddWithValue("@date", dateString);

                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                    dataGridView2.DataSource = dt;
                    // ตรวจสอบว่ามีข้อมูลที่แสดงหรือไม่
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("ไม่มีข้อมูลที่ตรงกัน");
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }

                double total = 0;
                HashSet<string> uniqueBills = new HashSet<string>(); 

                foreach (DataGridViewRow r in dataGridView2.Rows)
                {
                    string billName = r.Cells["receipt_ad"].Value.ToString(); 
                    if (!uniqueBills.Contains(billName)) 
                    {
                        uniqueBills.Add(billName); 
                        total += Convert.ToDouble(r.Cells["total"].Value); 
                    }
                }

                textBox6.Text = total.ToString("F2");  
                best_selling();
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                MySqlConnection conn = databaseConnection();

                if (!int.TryParse(textBox4.Text, out int month) || month < 1 || month > 12)
                {
                    MessageBox.Show("กรุณากรอกเดือนให้ถูกต้อง (1-12)");
                    return;
                }

                if (!int.TryParse(textBox5.Text, out int year))
                {
                    MessageBox.Show("กรุณากรอกปีให้ถูกต้อง");
                    return;
                }

                MySqlCommand cmd = new MySqlCommand(
                    "SELECT * FROM history WHERE MONTH(STR_TO_DATE(datetime, '%d-%m-%Y %H:%i:%s')) = @month AND YEAR(STR_TO_DATE(datetime, '%d-%m-%Y %H:%i:%s')) = @year",
                    conn
                );

                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@year", year);

                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                    dataGridView2.DataSource = dt;

                    // ตรวจสอบว่ามีข้อมูลที่แสดงหรือไม่
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("ไม่มีข้อมูลที่ตรงกัน");
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }

                double total = 0;
                HashSet<string> uniqueBills = new HashSet<string>(); 

                foreach (DataGridViewRow r in dataGridView2.Rows)
                {
                    string billName = r.Cells["receipt_ad"].Value.ToString(); 
                    if (!uniqueBills.Contains(billName)) 
                    {
                        uniqueBills.Add(billName); 
                        total += Convert.ToDouble(r.Cells["total"].Value); 
                    }
                }
                textBox6.Text = total.ToString("F2");  
                best_selling();
            }
            else if (comboBox1.SelectedIndex == 3)
            {
                MySqlConnection conn = databaseConnection();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM history WHERE YEAR(STR_TO_DATE(datetime, '%d-%m-%Y %H:%i:%s')) = @year", conn);
                cmd.Parameters.AddWithValue("@year", textBox5.Text);
                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                    dataGridView2.DataSource = dt;
                    // ตรวจสอบว่ามีข้อมูลที่แสดงหรือไม่
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("ไม่มีข้อมูลที่ตรงกัน");
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
                double total = 0;
                HashSet<string> uniqueBills = new HashSet<string>(); 

                foreach (DataGridViewRow r in dataGridView2.Rows)
                {
                    string billName = r.Cells["receipt_ad"].Value.ToString(); 
                    if (!uniqueBills.Contains(billName)) 
                    {
                        uniqueBills.Add(billName); 
                        total += Convert.ToDouble(r.Cells["total"].Value); 
                    }
                }
                textBox6.Text = total.ToString("F2");  
                best_selling();
            }
        }
        private void best_selling() //ลำดับ
        {
            
            Dictionary<string, int> productQuantities = new Dictionary<string, int>();
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.Cells["name"].Value != null && row.Cells["qty"].Value != null) 
                {
                    string product = row.Cells["name"].Value.ToString(); 
                    int quantity = Convert.ToInt32(row.Cells["qty"].Value);   

                    
                    if (productQuantities.ContainsKey(product))
                    {
                        productQuantities[product] += quantity;
                    }
                    else
                    {
                        productQuantities[product] = quantity;
                    }
                }
            }
            // ส่วนที่เอาไว้สร้างตาราง
            DataTable aggregatedDataTable = new DataTable();
            aggregatedDataTable.Columns.Add("Rank", typeof(int));
            aggregatedDataTable.Columns.Add("name", typeof(string));
            aggregatedDataTable.Columns.Add("TotalQuantity", typeof(int));

            
            int rank = 0;
            foreach (var kvp in productQuantities) // kvp คือของที่ยุใน productQuantities
            {
                aggregatedDataTable.Rows.Add(rank, kvp.Key, kvp.Value);
            }

            
            dataGridView3.DataSource = aggregatedDataTable;
            //เรียงลำดับสินค้าที่ขายได้มากที่สุด และแสดง 3 อันดับแรก
            dataGridView3.Sort(dataGridView3.Columns["TotalQuantity"], System.ComponentModel.ListSortDirection.Descending);
            for (int i = 0; i < dataGridView3.Rows.Count; i++) 
            {
                dataGridView3.Rows[i].Cells["Rank"].Value = (i + 1).ToString();
            }
            int rankLimit = 3;
            for (int i = rankLimit; i < dataGridView3.Rows.Count; i++) 
            {
                dataGridView3.Rows[i].Visible = false;
            }


        }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e) //เปิดpdf
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 12)
            {
                DataGridViewCell cell = null;
                foreach (DataGridViewCell selectedCell in dataGridView2.SelectedCells)
                {
                    cell = selectedCell;
                    break;
                }
                if (cell != null)
                {
                    DataGridViewRow row = cell.OwningRow;
                    string selectedId = row.Cells["receipt_ad"].Value.ToString();
                    MessageBox.Show(selectedId);


                    string file = selectedId;
                    Process.Start(file);
                }
            }
        }

        private void dataGridView2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        
    }
}  