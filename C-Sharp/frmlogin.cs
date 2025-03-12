using Microsoft.Win32;
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
    public partial class frmlogin : Form //เข้าสู่ระบบ
    {
        public string Username { get; set; }
        
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=admin;charset=utf8;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        public frmlogin()
        {
            InitializeComponent();
        }

        private void frmlogin_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MySqlConnection con = databaseConnection();  
            con.Open();
            //โดยตรวจสอบค่าของช่อง username และ password ในฐานข้อมูล MySQL ว่าตรงกับข้อมูลที่ผู้ใช้ป้อน
            string login = "SELECT * FROM users WHERE username= '" + textBox1usernameeee.Text + "' and password= '" + textBox2passssss.Text + "'";
            MySqlCommand cmd = new MySqlCommand(login, con);
            MySqlDataReader dr = cmd.ExecuteReader();  

            if (dr.Read() == true) 
            {
                string username = dr["username"].ToString();                                          

                Form2 form2 = new Form2();
                form2.Username = username; 
                form2.Show();
                this.Hide();


            }
            else
            {
                MessageBox.Show("ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง กรุณาลองอีกครั้ง","การเข้าสู่ระบบล้มเหลว", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1usernameeee.Text = ""; 
                textBox2passssss.Text = "";
                textBox1usernameeee.Focus(); 
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1usernameeee.Text = ""; //ลบ
            textBox2passssss.Text = "";
            textBox1usernameeee.Focus();
        }

        private void checkBox1showpass_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1showpass.Checked) //ถูกติ๊กหรือไม่
            {
                textBox2passssss.PasswordChar = '\0'; //ให้เป็นข้อความปกติ

            }
            else
            {
                textBox2passssss.PasswordChar = '*';  
                
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            new login().Show();
            this.Hide();

        }

        private void buttonback89_Click(object sender, EventArgs e)
        {
            new Form1().Show();
            this.Hide();
        }
    }
}
