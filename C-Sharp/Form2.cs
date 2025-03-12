using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using ZXing;
using ZXing.QrCode;
using System.Collections;



namespace WindowsFormsApp3
{
    public partial class Form2 : Form //เลือกซื้อ
    {
        public string Username { get; set; }

        private MySqlConnection con; // ประกาศตัวแปร con ในคลาส Form2
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=admin ;charset=utf8;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }

        public Form2()
        {
            InitializeComponent();
            con = databaseConnection(); // กำหนดค่าให้กับตัวแปร con โดยเรียกใช้ฟังก์ชัน databaseConnection
        }
        //กำหนดขนาดของปุ่ม
        private const int BUTTON_WIDTH = 170;
        private const int BUTTON_HEIGHT = 170;
        private const int BUTTON_PADDING = 40;
        
        private void Form2_Load(object sender, EventArgs e)
        {
            LoadProductButtons();
            dataGridViewCart.Refresh();
            textBox1user.Text = Username;
            showCart();
        }
        private void LoadProductButtons(string query = null) //ดึงข้อมูลจากproductมาแสดงให้เลือกซื้อ
        {
            string connectionString = "server=localhost;user=root;password=;database=admin"; //เชื่อมต่อกับฐานข้อมูล
            if (query == null)
            {
                query = "SELECT id, name, price, picture FROM product";
            }

            int buttonCount = 0;  //เก็บจำนวนปุ่มที่ถูกสร้างขึ้น
            int currentRow = 0;  //เก็บค่าของแถวปัจจุบันที่ปุ่มถูกแสดงอยู่

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    

                    while (reader.Read())
                    {
                        int productId = reader.GetInt32(0); //ดึงของสินค้าแต่ละรายการ
                        string productName = reader.GetString(1);   
                        decimal productPrice = reader.GetDecimal(2);
                        byte[] productImageBytes = (byte[])reader["picture"];  

                        //สร้าง PictureBox สำหรับพื้นหลังของสินค้า
                        PictureBox bg_product = new PictureBox();
                        bg_product.Size = new Size(BUTTON_WIDTH + 20, BUTTON_HEIGHT + 70); 
                        bg_product.BackColor = Color.White; 

                        //ปุ่มเลือกซื้อ
                        Button productButton = new Button();
                        productButton.Size = new Size(100, 25);
                        productButton.Text = "เพิ่ม";
                        productButton.Tag = new ProductInfo { Id = productId, Name = productName, Price = productPrice };
                        productButton.Click += ProductButton_Click;  
                        productButton.ForeColor = Color.Red;
                        productButton.BackColor = Color.White;

                        PictureBox pictureBox = new PictureBox(); //ภาพสินค้าจริง
                        pictureBox.Size = new Size(BUTTON_WIDTH, BUTTON_WIDTH);
                        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;  //ปรับขนาดของภาพให้พอดี
                        pictureBox.BackColor = Color.Black;

                        //เช็คว่าตัวแปร productImageBytes มีข้อมูลหรือไม่ ถ้ามีก็แปลงเป็นรูปภาพ
                        if (productImageBytes != null && productImageBytes.Length > 0)
                        {
                            using (MemoryStream ms = new MemoryStream(productImageBytes))
                            {
                                Image productImage = Image.FromStream(ms);
                                pictureBox.Image = productImage;
                            }
                        }
                        // สร้าง Label สำหรับแสดงชื่อสินค้า
                        Label productNameLabel = new Label();
                        productNameLabel.Size = new Size(120, 25);
                        productNameLabel.Text = productName;
                        productNameLabel.BackColor = Color.White;

                        //สร้าง TextBox สำหรับกรอกจำนวน
                        TextBox amountTextBox = new TextBox();
                        amountTextBox.Size = new Size(50, 25);
                        amountTextBox.Tag = productId; // กำหนด Tag เพื่อระบุสินค้า
                        amountTextBox.Text = "1"; // กำหนดค่าเริ่มต้นเป็น 1

                        //กำหนดตำแหน่งของpictureBox
                        int x = (BUTTON_WIDTH + BUTTON_PADDING) * (buttonCount % 3);  //คำนวณx
                        int y = (BUTTON_WIDTH + BUTTON_PADDING + 25) * currentRow;   //คำนวณy currentRow คือตัวนับแถว
                        pictureBox.Location = new Point(x + 40, y + 30);
                        panel1.Controls.Add(pictureBox); //เพิ่ม pictureBox ลงใน panel1

                        // กำหนดตำแหน่ง Label สำหรับชื่อสินค้า
                        int nameLabelX = x + 40;
                        int nameLabelY = y + BUTTON_WIDTH + 30;
                        productNameLabel.Location = new Point(nameLabelX, nameLabelY + 10);
                        panel1.Controls.Add(productNameLabel);

                        //กำหนดตำแหน่ง TextBox สำหรับจำนวน
                        int textBoxX = x + 40;
                        int textBoxY = y + BUTTON_WIDTH + 55;
                        amountTextBox.Location = new Point(textBoxX, textBoxY + 10);
                        panel1.Controls.Add(amountTextBox);

                        //กำหนดตำแหน่งปุ่ม
                        int buttonX = x + 110;
                        int buttonY = y + BUTTON_WIDTH + 55;
                        productButton.Location = new Point( buttonX, buttonY + 10);
                        panel1.Controls.Add(productButton);

                        //กำหนดตำแหน่งและเพิ่ม bg_product (PictureBox ที่เป็นพื้นหลัง) เข้าไปใน panel1
                        int bgx = (BUTTON_WIDTH + BUTTON_PADDING ) * (buttonCount % 3);
                        int bgy = (BUTTON_WIDTH + BUTTON_PADDING ) * currentRow;
                        bg_product.Location = new Point(x + 30, y + 20);
                        panel1.Controls.Add(bg_product);

                        panel1.AutoScroll = true;

                        //เพิ่มจำนวนปุ่มถ้าครบ 3 ปุ่ม ให้เลื่อนการสร้างปุ่มไปยังแถวถัดไป 
                        buttonCount++;
                        if (buttonCount % 3 == 0)
                        {
                            currentRow++;
                        }
                    }

                    reader.Close();
                }
                catch (Exception ex)  //จัดการข้อผิดพลาด
                {
                    MessageBox.Show("Error: " + ex.Message);  //ตามด้วยข้อความข้อผิดพลาด
                }
            }
        }

        
        private class ProductInfo // ใช้เก็บข้อมูลของสินค้า
        {
            public int Id { get; set; } 
            public string Name { get; set; }
            public decimal Price { get; set; }
        }
        
        private void ProductButton_Click(object sender, EventArgs e) 
        {
            Button clickedButton = sender as Button; //ถูกคลิกMessageBoxว่าเพิ่ม
            MessageBox.Show(clickedButton.Text);
            ProductInfo productInfo = clickedButton.Tag as ProductInfo;
            
            if (productInfo != null)
            {
                // ดึง TextBox ของจำนวนตาม Tag ที่ตรงกับ ProductId
                TextBox amountTextBox = panel1.Controls.OfType<TextBox>().FirstOrDefault(tb => (int)tb.Tag == productInfo.Id);
                if (amountTextBox != null)
                {
                    int amount; //ตรวจสอบความถูกต้องของจำนวน ถ้าผช กรอกไม่ถูกต้อง มีแจ้งเตือน
                    if (int.TryParse(amountTextBox.Text, out amount) && amount > 0)
                    {
                        
                        AddProductToCart(productInfo.Id, productInfo.Name, amount, productInfo.Price);
                    }
                    else
                    {
                        MessageBox.Show("กรุณาใส่จำนวนที่ถูกต้อง", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                    }
                }
                else
                {
                    MessageBox.Show("ไม่พบ TextBox สำหรับจำนวนสินค้า", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("ข้อมูลสินค้าไม่ถูกต้อง", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }


        private void AddProductToCart(int productId, string productName, int amount, decimal productPrice)
        {
            try
            {
                // ดำเนินการเพิ่มสินค้าลงในตะกร้าของลูกค้า
                // เพิ่มข้อมูลลงในตาราง carts ในฐานข้อมูล
                string connectionString = "server=localhost;user=root;password=;database=admin";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // ตรวจสอบสต็อกสินค้า
                    MySqlCommand checkStockCommand = new MySqlCommand("SELECT quantity FROM product WHERE id = @productId", connection);
                    checkStockCommand.Parameters.AddWithValue("@productId", productId);
                    int stock = Convert.ToInt32(checkStockCommand.ExecuteScalar());

                    // ตรวจสอบว่าสินค้าในสต็อกเพียงพอกับจำนวนที่ลูกค้ากรอกหรือไม่
                    if (stock == 0)
                    {
                        MessageBox.Show("สินค้าหมด ไม่สามารถเพิ่มสินค้าในตะกร้าได้", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else if (stock < amount)
                    {
                        MessageBox.Show($"สินค้าไม่พอ สินค้าที่มีในสต็อกคือ: " + stock + " ชิ้น", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; 
                    }

                    // ตรวจสอบว่ามีสินค้าอยู่ในตะกร้าแล้วหรือไม่
                    MySqlCommand checkCommand = new MySqlCommand("SELECT COUNT(*) FROM carts WHERE id = @productId", connection);
                    checkCommand.Parameters.AddWithValue("@productId", productId);
                    int existingCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (existingCount > 0)
                    {
                        // หากสินค้ามีอยู่ในตะกร้าแล้ว ให้เพิ่มจำนวนและราคารวม
                        MySqlCommand updateCommand = new MySqlCommand("UPDATE carts SET qty = qty + @amount, totalprice = price * qty WHERE id = @productId", connection);
                        updateCommand.Parameters.AddWithValue("@productId", productId);
                        updateCommand.Parameters.AddWithValue("@amount", amount);

                        updateCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        // หากสินค้ายังไม่มีในตะกร้า ให้เพิ่มรายการใหม่
                        MySqlCommand priceCommand = new MySqlCommand("SELECT price FROM product WHERE id = @productId", connection);
                        priceCommand.Parameters.AddWithValue("@productId", productId);
                        decimal retrievedPrice = (decimal)priceCommand.ExecuteScalar();

                        MySqlCommand insertCommand = connection.CreateCommand();
                        insertCommand.CommandText = "INSERT INTO carts (id, name, qty, price, totalprice) VALUES (@productId, @productName, @amount, @retrievedPrice, @totalprice)";
                        insertCommand.Parameters.AddWithValue("@productId", productId);
                        insertCommand.Parameters.AddWithValue("@productName", productName);
                        insertCommand.Parameters.AddWithValue("@amount", amount);
                        insertCommand.Parameters.AddWithValue("@retrievedPrice", retrievedPrice);
                        decimal totalll = retrievedPrice * amount;
                        insertCommand.Parameters.AddWithValue("@totalprice", totalll);

                        insertCommand.ExecuteNonQuery();

                    }
                    // คำนวณค่า subtotal, vat, และ total ใหม่
                    MySqlCommand sumCommand = new MySqlCommand("SELECT SUM(totalprice) FROM carts", connection);
                    decimal subtotal = Convert.ToDecimal(sumCommand.ExecuteScalar());
                    decimal discount = 0;

                    if (subtotal >= 200)
                    {
                        discount = subtotal * 0.05m; 
                    }

                    decimal subtotalAfterDiscount = subtotal - discount;
                    decimal vat = subtotalAfterDiscount * 0.07m;
                    decimal totalAfterDiscountAndVat = subtotalAfterDiscount + vat;

                    // อัพเดตค่า subtotal, vat, และ total ในตาราง carts
                    MySqlCommand updateSumCommand = new MySqlCommand("UPDATE carts SET subtotal = @subtotal, vat = @vat, total = @total, discount = @discount", connection);
                    updateSumCommand.Parameters.AddWithValue("@subtotal", subtotal);
                    updateSumCommand.Parameters.AddWithValue("@discount", discount);
                    updateSumCommand.Parameters.AddWithValue("@vat", vat);
                    updateSumCommand.Parameters.AddWithValue("@total", totalAfterDiscountAndVat);
                    updateSumCommand.ExecuteNonQuery();

                }
                // อัพเดท DataGridView
                showCart();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public void showCart()
        {
            try
            {
                string connectionString = "server=localhost;user=root;password=;database=admin";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("SELECT name, qty, price, totalprice FROM carts", connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Clear เพื่อเตรียมแสดงข้อมูลใหม่
                    dataGridViewCart.Columns.Clear();
                    dataGridViewCart.Rows.Clear();

                    // Add columns 
                    dataGridViewCart.Columns.Add("ProductName", "สินค้า"); 
                    dataGridViewCart.Columns.Add("ProductQuantity", "จำนวน"); 
                    dataGridViewCart.Columns.Add("ProductPrice", "ราคา"); 
                    dataGridViewCart.Columns.Add("ProductTotal", "รวม");

                    // กำหนดความกว้าง
                    dataGridViewCart.Columns["ProductName"].Width = 144; 
                    dataGridViewCart.Columns["ProductQuantity"].Width = 45; 
                    dataGridViewCart.Columns["ProductPrice"].Width = 45; 
                    dataGridViewCart.Columns["ProductTotal"].Width = 45; 
                    //set เซนเตอร์
                    dataGridViewCart.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // คอลัมน์ที่ 2
                    dataGridViewCart.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // คอลัมน์ที่ 3
                    dataGridViewCart.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // คอลัมน์ที่ 4

                    decimal subtotal = 0; // ยอดรวมราคาสินค้าก่อนคิดภาษี
                    foreach (DataRow row in dataTable.Rows) //ดึงข้อมูลแต่ละแถว
                    {
                        string productName = row["name"].ToString();
                        int productQuantity = Convert.ToInt32(row["qty"]); 
                        decimal productPrice = Convert.ToDecimal(row["price"]);
                        decimal productTotal = Convert.ToDecimal(row["totalprice"]); // ราคารวมสินค้า

                        // เพิ่มรายการสินค้าลงใน DataGridView
                        dataGridViewCart.Rows.Add(productName, productQuantity, productPrice, productTotal);

                        // บวก productTotal ของแต่ละรายการเข้าไปใน subtotal
                        subtotal += productTotal; 
                    }

                    // คำนวณส่วนลด 5% ถ้า subtotal >= 200
                    decimal discount = 0;
                    if (subtotal >= 200)
                    {
                        discount = subtotal * 0.05m; // คำนวณส่วนลดจากยอดรวม
                    }

                    decimal subtotalAfterDiscount = subtotal - discount;

                    decimal vat = subtotalAfterDiscount * 0.07m;

                    decimal totalAfterDiscountAndVat = subtotalAfterDiscount + vat;

                    // แสดงยอดรวมและภาษีมูลค่าเพิ่ม (VAT) ใน TextBox
                    textBoxSubtotal.Text = subtotal.ToString("N2");
                    textBoxVAT.Text = vat.ToString("N2");
                    textBoxTotal.Text = totalAfterDiscountAndVat.ToString("N2");
                    textBoxDiscount.Text = discount.ToString("N2");
                    
                    //อัพเดทข้อมูลส่วนลด
                    MySqlCommand updateDiscountCommand = new MySqlCommand("UPDATE carts SET discount = @discount", connection);
                    updateDiscountCommand.Parameters.AddWithValue("@discount", discount);
                    updateDiscountCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                
            }

        }

        private void buttonback_Click(object sender, EventArgs e)
        {
            // ปิด Form2
            this.Hide();
            ClearCart();
            // แสดง Form1 ที่เปิดอยู่แล้ว
            Form1 form1 = (Form1)Application.OpenForms["Form1"];
            form1.Show();
        }
        
        private void buttonpaynow_Click(object sender, EventArgs e)
        {
            DialogResult re = MessageBox.Show("ต้องการชำระเงินหรือไม่", "ยืนยัน", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (re == DialogResult.OK)
            {
                qr qrForm = new qr(Username);
                this.Hide();
                qrForm.Show();
                //new qr().Show();
                

            }    
            
        }
        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString("===================================================", new Font("Arial", 18, FontStyle.Regular), Brushes.Black, new Point(40, 80));
        }

        



        private void dataCartsInOrder_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void buttondelete_Click(object sender, EventArgs e)
        {
            int selectedRow = dataGridViewCart.CurrentCell.RowIndex; //ดึงแถวที่ถูกเลือก
            string deleteName = dataGridViewCart.Rows[selectedRow].Cells[0].Value.ToString(); // อ้างอิงชื่อคอลัมน์โดยตำแหน่ง (index) ของคอลัมน์แรก
            MySqlConnection conn = databaseConnection();
            String sql = "DELETE FROM carts WHERE name = @deleteName";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@deleteName", deleteName);

            conn.Open();
            int rows = cmd.ExecuteNonQuery();
            conn.Close();
            if (rows > 0)
            {
                MessageBox.Show("ลบข้อมูลสำเร็จ");
                // อัพเดท dataGridViewCartให้แสดงข้อมูลล่าสุด
                showCart();
            }
        }

        private void ClearCart()
        {
            try
            {
                // ลบข้อมูลในตาราง carts
                string connectionString = "server=localhost;user=root;password=;database=admin";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("DELETE FROM carts", connection);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        
                        showCart();
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาดในการลบรายการสินค้าในตะกร้า: " + ex.Message);
            }
        }
        

        private void dataGridViewshow_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void buttonclear_Click(object sender, EventArgs e)
        {
            ClearCart();
        }
        

    }

}
