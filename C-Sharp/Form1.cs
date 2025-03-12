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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
          
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // ตั้งค่าขนาดเริ่มต้นของฟอร์ม
            this.Size = new System.Drawing.Size(1000, 600);
        }

        private void button1order_Click(object sender, EventArgs e)
        {
            new frmlogin().Show();
            // ปิด Form1
            this.Hide();

        }

        private void buttonadmin_Click(object sender, EventArgs e)
        {
            this.Hide();
            pass passForm = new pass();
            passForm.FormClosed += (s, args) => this.Show();
            passForm.Show();
            

        }

        

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            new me().Show();
            this.Hide();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // แสดงกล่องข้อความยืนยันการออกจากโปรแกรม
            DialogResult result = MessageBox.Show("คุณต้องการออกจากโปรแกรมหรือไม่?", "ยืนยันการออก", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // ถ้าผู้ใช้คลิก Yes ให้โปรแกรมออก
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
