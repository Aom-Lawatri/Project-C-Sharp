using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp3
{
    public partial class admin : Form
    {
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=admin;charset=utf8;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        //ดึงข้อมูลจากฐานข้อมูล MySQL และแสดงผลใน DataGridView
        private void showEquipment()   
        {
            MySqlConnection conn = databaseConnection();
            DataSet ds = new DataSet();
            conn.Open();

            MySqlCommand cmd;

            cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM product";

            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(ds);

            conn.Close();
            dataproduct.DataSource = ds.Tables[0].DefaultView;
        }

        public admin()
        {
            InitializeComponent();
        }

        private void admin_Load(object sender, EventArgs e)
        {
            showEquipment();
            this.Size = new System.Drawing.Size(1000, 600);
            
        }

        private void buttonback2_Click(object sender, EventArgs e)
        {
            this.Hide();
            choice choice = (choice)Application.OpenForms["choice"];
            choice.Show();
        }

        private void dataproduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private byte[] imageBytes; // ประกาศตัวแปรชั้น global
        private void buttonpic_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog()) //OpenFileDialog เพื่อให้ผู้ใช้สามารถเลือกไฟล์จากเครื่อง
            {
                openFileDialog1.Filter = "Image Files (.jpg, *.jpeg, *.png, *.gif)|.jpg; *.jpeg; *.png; *.gif"; // ตั้งค่า Filter ให้เลือกไฟล์ภาพเท่านั้น

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    // เมื่อผู้ใช้เลือกไฟล์ภาพแล้ว
                    string imagePath = openFileDialog1.FileName;
                    imageBytes = File.ReadAllBytes(imagePath);
                    // เพื่อดึงชื่อไฟล์ และแสดงใน textBoxpic
                    textBoxpic.Text = Path.GetFileName(imagePath);

                }
            }
        }

        private void buttonadd_Click(object sender, EventArgs e)
        {
            // ตรวจสอบว่า imageBytes ไม่ใช่ค่า null หรือข้อมูลว่างเปล่าก่อนที่จะทำการเพิ่มข้อมูล
            if (imageBytes != null && imageBytes.Length > 0)
            {
                MySqlConnection conn = databaseConnection();
                String sql = "INSERT INTO product (name, price, quantity, picture) VALUES(@name, @price, @quantity, @picture)";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@name", textBoxname.Text);
                cmd.Parameters.AddWithValue("@price", textBoxprice.Text);
                cmd.Parameters.AddWithValue("@quantity", textBoxquantity.Text);
                cmd.Parameters.AddWithValue("@picture", imageBytes);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                conn.Close();
                if (rows > 0)
                {
                    MessageBox.Show("เพิ่มข้อมูลสำเร็จ");
                    showEquipment();

                    textBoxname.Clear();
                    textBoxprice.Clear();
                    textBoxquantity.Clear();
                    textBoxpic.Clear();
                }
            }
            else
            {
                MessageBox.Show("กรุณาเลือกไฟล์รูปภาพ");
            }
        }

        private void dataproduct_CellClick(object sender, DataGridViewCellEventArgs e) //โหลดข้อมูลจากเซลล์ในแถวที่ถูกเลือก
        {
            dataproduct.CurrentRow.Selected = true;
            textBoxname.Text = dataproduct.Rows[e.RowIndex].Cells["name"].FormattedValue.ToString();
            textBoxprice.Text = dataproduct.Rows[e.RowIndex].Cells["price"].FormattedValue.ToString();
            textBoxquantity.Text = dataproduct.Rows[e.RowIndex].Cells["quantity"].FormattedValue.ToString();
            
        }

        private void buttondelete_Click(object sender, EventArgs e)
        {
            int selectedRow = dataproduct.CurrentCell.RowIndex; //แถวที่เลือก
            int deleteId = Convert.ToInt32(dataproduct.Rows[selectedRow].Cells["id"].Value);
            MySqlConnection conn = databaseConnection();
            String sql = "DELETE FROM product WHERE id = '" + deleteId + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);

            conn.Open();
            int rows = cmd.ExecuteNonQuery();
            conn.Close();
            if (rows > 0)
            {
                MessageBox.Show("ลบข้อมูลสำเร็จ");
                showEquipment();

                
                textBoxname.Clear();
                textBoxprice.Clear();
                textBoxquantity.Clear();
                textBoxpic.Clear();
            }

        }

        private void buttonedit_Click(object sender, EventArgs e)
        {
            int selectedRow = dataproduct.CurrentCell.RowIndex;  //แถวที่ถูกเลือก
            int editId = Convert.ToInt32(dataproduct.Rows[selectedRow].Cells["id"].Value);

            MySqlConnection conn = databaseConnection();
            String sql = "UPDATE product SET name = '" + textBoxname.Text + "' ,price = '" + textBoxprice.Text + "' ,quantity = '" + textBoxquantity.Text + "' ,picture = @picture WHERE id = '" + editId + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@picture", imageBytes); // เพิ่ม parameter สำหรับรูปภาพ

            conn.Open();
            int rows = cmd.ExecuteNonQuery();
            conn.Close();
            if (rows > 0)
            {
                MessageBox.Show("แก้ไขข้อมูลสำเร็จ");
                showEquipment();

                // ล้างข้อมูลใน TextBox หลังจากเพิ่มข้อมูลสำเร็จ
                textBoxname.Clear();
                textBoxprice.Clear();
                textBoxquantity.Clear();
                textBoxpic.Clear();
            }

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBoxname_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxpic_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBoxprice_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
