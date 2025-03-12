using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp3
{
    public partial class memberr : Form
    {
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=admin;charset=utf8;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        public memberr()
        {
            InitializeComponent();
        }

        private void showEquipment1()
        {
            MySqlConnection conn = databaseConnection();
            DataSet ds = new DataSet();
            conn.Open();

            MySqlCommand cmd;

            cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, first_name, last_name, phone_number, username FROM users";

            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(ds);

            conn.Close();
            dataGridView1.DataSource = ds.Tables[0].DefaultView;
        }

        private void buttonback_Click(object sender, EventArgs e)
        {
            this.Hide();
            choice choice = (choice)Application.OpenForms["choice"];
            choice.Show();
        }

        private void memberr_Load(object sender, EventArgs e)
        {
            showEquipment1();
        }

        private void button111_Click(object sender, EventArgs e)
        {
            // ตรวจสอบหมายเลขโทรศัพท์ว่ามี 10 หลักหรือไม่
            if (textBoxpn.Text.Length != 10)
            {
                MessageBox.Show("หมายเลขโทรศัพท์ต้องมี 10 หลัก", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // หยุดการทำงานหากหมายเลขไม่ถูกต้อง
            }

            int selectedRow = dataGridView1.CurrentCell.RowIndex;  //แถวที่ถูกเลือก
            int editId = Convert.ToInt32(dataGridView1.Rows[selectedRow].Cells["id"].Value);

            MySqlConnection conn = databaseConnection();
            String sql = "UPDATE users SET first_name = '" + textBoxf.Text + "' ,last_name = '" + textBoxl.Text + "' ,phone_number = '" + textBoxpn.Text + "' ,username = '" + textBoxu.Text + "' WHERE id = '" + editId + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            

            conn.Open();
            int rows = cmd.ExecuteNonQuery();
            conn.Close();
            if (rows > 0)
            {
                MessageBox.Show("แก้ไขข้อมูลสำเร็จ");
                showEquipment1();

                // ล้างข้อมูลใน TextBox หลังจากเพิ่มข้อมูลสำเร็จ
                textBoxf.Clear();
                textBoxl.Clear();
                textBoxpn.Clear();
                textBoxu.Clear();
                textBoxs.Clear();
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e) //โหลดข้อมูลจากเซลล์ในแถวที่ถูกเลือก
        {
            dataGridView1.CurrentRow.Selected = true;
            textBoxf.Text = dataGridView1.Rows[e.RowIndex].Cells["first_name"].FormattedValue.ToString();
            textBoxl.Text = dataGridView1.Rows[e.RowIndex].Cells["last_name"].FormattedValue.ToString();
            textBoxpn.Text = dataGridView1.Rows[e.RowIndex].Cells["phone_number"].FormattedValue.ToString();
            textBoxu.Text = dataGridView1.Rows[e.RowIndex].Cells["username"].FormattedValue.ToString();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                textBoxs.Text = "Username";  //กดเลือกชื่อในคอมโบบ็อก จะขึ้นแค่ให้กรอกชื่อ
                textBoxs.Visible = true;  //แสดง

            }
            else if (comboBox1.SelectedIndex == 1)
            {
                textBoxs.Text = "Tel";  //กดเลือกชื่อในคอมโบบ็อก จะขึ้นแค่ให้กรอกชื่อ
                textBoxs.Visible = true;
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                MySqlConnection conn = databaseConnection();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM users WHERE username = @username", conn);
                cmd.Parameters.AddWithValue("@username", textBoxs.Text);
                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                    dataGridView1.DataSource = dt;
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
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                MySqlConnection conn = databaseConnection();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM users WHERE phone_number = @phone_number", conn);
                cmd.Parameters.AddWithValue("@phone_number", textBoxs.Text);
                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                    dataGridView1.DataSource = dt;
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
            }
        }    
    }
}
