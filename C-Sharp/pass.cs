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
    public partial class pass : Form
    {
        public pass()
        {
            InitializeComponent();
        }

        private void pass_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form1 = (Form1)Application.OpenForms["Form1"];
            form1.Show();
        }

        private void textBoxpass_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonpass_Click(object sender, EventArgs e)
        {
            string correctPassword = "6565"; // กำหนดรหัส

            string enteredPassword = textBoxpass.Text;

            if (enteredPassword == correctPassword)
            {
                // ถ้ารหัสผ่านถูกต้อง ให้เปิดหน้าฟอร์มถัดไป
                choice choice = new choice();
                choice.Show();
                this.Hide(); 
            }
            else
            {
                
                MessageBox.Show("รหัสผ่านไม่ถูกต้อง", "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
