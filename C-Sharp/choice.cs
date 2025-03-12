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
    public partial class choice : Form
    {
        public choice()
        {
            InitializeComponent();
        }


        private void button3three_Click(object sender, EventArgs e)
        {
            orderhistory orderhistory = new orderhistory();
            orderhistory.Show();
        }

        private void buttonback2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form1 = (Form1)Application.OpenForms["form1"];
            form1.Show();
        }
        

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // ถ้ารหัสผ่านถูกต้อง ให้เปิดหน้าฟอร์มถัดไป
            admin admin = new admin();
            admin.Show();
            this.Hide(); // ซ่อนหน้าปัจจุบัน
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            memberr memberr = new memberr();
            memberr.Show();
            this.Hide();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            orderhistory orderhistory = new orderhistory();
            orderhistory.Show();
        }
    }
}
