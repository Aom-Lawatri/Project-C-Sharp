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

namespace WindowsFormsApp3
{
    public partial class login : Form
    {
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=admin;charset=utf8;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        public login()
        {
            InitializeComponent();
        }

        private void login_Load(object sender, EventArgs e)
        {
            textBox1first.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1first.Text == "" || textBox2last.Text == "" || textBox3phone.Text == "" || textBox1username.Text == "" || textBox2pass.Text == "" || textBox2confirmpass.Text == "")
            {
                MessageBox.Show("กรุณากรอกข้อมูลให้ครบทุกช่อง", "การลงทะเบียนล้มเหลว", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else if (textBox2pass.Text != textBox2confirmpass.Text)
            {
                MessageBox.Show("รหัสผ่านไม่ตรงกัน กรุณากรอกรหัสผ่านใหม่", "การลงทะเบียนล้มเหลว", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2pass.Text = "";
                textBox2confirmpass.Text = "";
                textBox2pass.Focus();
            }
            else if (textBox3phone.Text.Length != 10 || !long.TryParse(textBox3phone.Text, out _))
            {
                
                MessageBox.Show("เบอร์โทรศัพท์ต้องเป็นตัวเลข 10 หลัก", "การลงทะเบียนล้มเหลว", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox3phone.Focus();
            }
            else
            {
                MySqlConnection con = databaseConnection();
                con.Open();
                string register = "INSERT INTO users (first_name, last_name, phone_number, username, password) VALUES ('" + textBox1first.Text + "','" + textBox2last.Text + "','" + textBox3phone.Text + "','" + textBox1username.Text + "','" + textBox2pass.Text + "')";
                MySqlCommand cmd = new MySqlCommand(register, con);
                cmd.ExecuteNonQuery();
                con.Close();

                // เคลียร์ข้อมูลใน TextBox
                textBox1first.Text = "";
                textBox2last.Text = "";
                textBox3phone.Text = "";
                textBox1username.Text = "";
                textBox2pass.Text = "";
                textBox2confirmpass.Text = "";

                MessageBox.Show("สร้างบัญชีสำเร็จ", "การลงทะเบียนสำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
            new frmlogin().Show();
            this.Hide();
        }

        private void textBox2confirmpass_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void checkBox1showpass_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1showpass.Checked) 
            {
                textBox2pass.PasswordChar = '\0';
                textBox2confirmpass.PasswordChar = '\0';
            }
            else
            {
                textBox2pass.PasswordChar = '*';
                textBox2confirmpass.PasswordChar = '*';
            }
        }

        

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
