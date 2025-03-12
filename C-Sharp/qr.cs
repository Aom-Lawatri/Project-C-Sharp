using MySql.Data.MySqlClient;
using Saladpuk.PromptPay.Facades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing.Common;
using ZXing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Font = iTextSharp.text.Font;
using iTextSharp.text.pdf.draw;

namespace WindowsFormsApp3
{
    public partial class qr : Form
    {
        public string Username { get; set; }
        // สร้างคอนสตรักเตอร์ที่รับพารามิเตอร์ username
        private string tick = DateTime.Now.Ticks.ToString();

        public qr(string username)
        {
            InitializeComponent();
            Username = username;
            

        }
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=admin;charset=utf8;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        
        public qr()
        {
            InitializeComponent();
        }
        decimal totalAmount = 0;
        private void UpdateTotalLabel()
        {
            try
            {
                using (MySqlConnection conn = databaseConnection())
                {
                    conn.Open();
                    string query = "SELECT total FROM carts";  //ดึงข้อมูลยอดรวมจากตาราง carts และแสดงผลยอดรวมใน labelTotalAmount
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        totalAmount = Convert.ToDecimal(result);
                    }
                    else
                    {
                        MessageBox.Show("No result returned from database.");
                    }
                }

                labelTotalAmount.Text = totalAmount.ToString("N2"); // แสดงผลในรูปแบบสกุลเงิน
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            
        }

        private void buttonreceipt_Click(object sender, EventArgs e)
        {
            DialogResult re = MessageBox.Show("ยืนยันที่จะออกใบเสร็จหรือไม่", "ยืนยัน", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (re == DialogResult.OK)
            {
                try
                {
                    using (MySqlConnection conn = databaseConnection())
                    {
                        conn.Open();

                        
                        string pdfFilePath = @"c://bill//" + tick + ".pdf";
                        Document document = new Document(PageSize.A4, 30, 30, 40, 40);
                        
                        PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(@"c://bill//" + tick + ".pdf", FileMode.Create));
                        document.Open();
                        
                        string thaiFontPath = @"C:\USERS\555\APPDATA\LOCAL\MICROSOFT\WINDOWS\FONTS\THSARABUN.TTF";

                        
                        BaseFont baseFont = BaseFont.CreateFont(thaiFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                        // ดึงข้อมูลจาก carts เพื่อเตรียมลดจำนวนสต้อก
                        string selectQuery = "SELECT id, qty FROM carts";
                        MySqlCommand selectCommand = new MySqlCommand(selectQuery, conn);
                        MySqlDataReader reader = selectCommand.ExecuteReader();

                        Dictionary<int, int> cartItems = new Dictionary<int, int>();

                        while (reader.Read())
                        {
                            int productId = reader.GetInt32("id");
                            int quantity = reader.GetInt32("qty");
                            cartItems[productId] = quantity;
                        }

                        reader.Close();

                        // ลดจำนวนสินค้าในตาราง product ตามที่อยู่ใน carts
                        foreach (var item in cartItems)
                        {
                            string updateQuery = "UPDATE product SET quantity = quantity - @quantity WHERE id = @id";
                            MySqlCommand updateCommand = new MySqlCommand(updateQuery, conn);
                            updateCommand.Parameters.AddWithValue("@quantity", item.Value);
                            updateCommand.Parameters.AddWithValue("@id", item.Key);
                            updateCommand.ExecuteNonQuery();
                        }

                        // ใส่ภาพโลโก้
                        string logoPath = @"D:\c#\รูป\75.png"; 
                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                        logo.ScaleToFit(130f, 130f); 
                        logo.Alignment = Element.ALIGN_CENTER;
                        document.Add(logo);

                        
                        Font thaiFont = new Font(baseFont, 16);
                        Paragraph paragraph = new Paragraph("123 หมู่ที่ 16 ถ.มิตรภาพ ตำบลในเมือง\n อำเภอเมืองขอนแก่น ขอนแก่น 40002\n " ,thaiFont);
                        paragraph.Alignment = Element.ALIGN_CENTER;
                        paragraph.SpacingAfter = 5;
                        paragraph.SpacingBefore = 5;
                        document.Add(paragraph);

                        
                        Paragraph userParagraph = new Paragraph("ลูกค้า : " + Username, thaiFont);
                        userParagraph.Alignment = Element.ALIGN_CENTER;
                        userParagraph.SpacingAfter = 10;
                        document.Add(userParagraph);


                        // ดึงข้อมูลจาก carts เพื่อแสดงใน PDF
                        string selectCartsQuery = "SELECT name, qty, price, totalprice FROM carts";
                        MySqlCommand selectCartsCommand = new MySqlCommand(selectCartsQuery, conn);
                        MySqlDataReader cartsReader = selectCartsCommand.ExecuteReader();

                        // สร้างตารางสำหรับข้อมูลสินค้า
                        PdfPTable table = new PdfPTable(4);
                        table.WidthPercentage = 100;
                        table.SetWidths(new float[] { 20, 15, 15, 15 });

                        
                        PdfPCell cell;
                        cell = new PdfPCell(new Phrase("สินค้า", thaiFont));
                        cell.BorderWidthTop = 1f; 
                        cell.BorderWidthBottom = 1f; 
                        cell.BorderWidthLeft = 0f; 
                        cell.BorderWidthRight = 0f; 
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase("จำนวน", thaiFont));
                        cell.BorderWidthTop = 1f;
                        cell.BorderWidthBottom = 1f;
                        cell.BorderWidthLeft = 0f;
                        cell.BorderWidthRight = 0f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase("ราคา", thaiFont));
                        cell.BorderWidthTop = 1f;
                        cell.BorderWidthBottom = 1f;
                        cell.BorderWidthLeft = 0f;
                        cell.BorderWidthRight = 0f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase("ราคารวม", thaiFont));
                        cell.BorderWidthTop = 1f;
                        cell.BorderWidthBottom = 1f;
                        cell.BorderWidthLeft = 0f;
                        cell.BorderWidthRight = 0f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table.AddCell(cell);

                        
                        while (cartsReader.Read()) // ดึงข้อมูลจาก carts
                        {
                            cell = new PdfPCell(new Phrase(cartsReader["name"].ToString(), thaiFont));
                            cell.Border = PdfPCell.NO_BORDER;
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            table.AddCell(cell);

                            cell = new PdfPCell(new Phrase(cartsReader["qty"].ToString(), thaiFont));
                            cell.Border = PdfPCell.NO_BORDER;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(cell);

                            cell = new PdfPCell(new Phrase(cartsReader["price"].ToString(), thaiFont));
                            cell.Border = PdfPCell.NO_BORDER;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(cell);

                            cell = new PdfPCell(new Phrase(cartsReader["totalprice"].ToString(), thaiFont));
                            cell.Border = PdfPCell.NO_BORDER;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(cell);
                        }
                        cartsReader.Close();
                        document.Add(table);

                        // ดึงข้อมูลยอดรวม ภาษี ส่วนลด และยอดรวมทั้งสิ้นจาก carts
                        string selectTotalsQuery = "SELECT subtotal, vat, discount, total FROM carts LIMIT 1";
                        MySqlCommand selectTotalsCommand = new MySqlCommand(selectTotalsQuery, conn);
                        MySqlDataReader totalsReader = selectTotalsCommand.ExecuteReader();

                        decimal subtotal = 0;
                        decimal vat = 0;
                        decimal discount = 0;
                        decimal grandTotal = 0;

                        if (totalsReader.Read())
                        {
                            subtotal = decimal.Parse(totalsReader["subtotal"].ToString());
                            vat = decimal.Parse(totalsReader["vat"].ToString());
                            discount = decimal.Parse(totalsReader["discount"].ToString());
                            grandTotal = decimal.Parse(totalsReader["total"].ToString());
                        }
                        totalsReader.Close();

                        float rightIndent = 60f; 
                                                 
                        LineSeparator line = new LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_CENTER, -2);
                        document.Add(new Chunk(line));
                        // เพิ่มยอดรวม ภาษี ส่วนลด และยอดรวมทั้งสิ้น โดยไม่ทำเป็นตาราง
                        Paragraph subtotalParagraph = new Paragraph("ยอดรวม:    " + subtotal.ToString("N2"), thaiFont);
                        subtotalParagraph.Alignment = Element.ALIGN_RIGHT;
                        subtotalParagraph.IndentationRight = rightIndent;
                        document.Add(subtotalParagraph);

                        Paragraph discountParagraph = new Paragraph("ส่วนลด:    " + discount.ToString("N2"), thaiFont);
                        discountParagraph.Alignment = Element.ALIGN_RIGHT;
                        discountParagraph.IndentationRight = rightIndent;
                        document.Add(discountParagraph);

                        Paragraph vatParagraph = new Paragraph("ภาษีมูลค่าเพิ่ม (vat 7%):    " + vat.ToString("N2"), thaiFont);
                        vatParagraph.Alignment = Element.ALIGN_RIGHT;
                        vatParagraph.IndentationRight = rightIndent;
                        document.Add(vatParagraph);

                        Paragraph grandTotalParagraph = new Paragraph("ยอดรวมทั้งสิ้น:    " + grandTotal.ToString("N2"), thaiFont);
                        grandTotalParagraph.Alignment = Element.ALIGN_RIGHT;
                        grandTotalParagraph.IndentationRight = rightIndent;
                        document.Add(grandTotalParagraph);
                        
                        document.Close();

                        
                        byte[] pdfBytes = File.ReadAllBytes(pdfFilePath);

                        // คัดลอกข้อมูลจาก carts ไปยัง history พร้อมเพิ่มวันที่และเวลาในรูปแบบ DD - MM - YYYY HH:MM:SS
                        string copyQuery = "INSERT INTO history (id, name, qty, price, totalprice, subtotal, vat, discount, total, datetime, username, receipt_ad)" +
                                            "SELECT id, name, qty, price, totalprice, subtotal, vat, discount, total, DATE_FORMAT(NOW(), '%d-%m-%Y %H:%i:%s'), @username, @receipt_ad FROM carts";
                        MySqlCommand copyCommand = new MySqlCommand(copyQuery, conn);
                        copyCommand.Parameters.AddWithValue("@username", Username);
                        copyCommand.Parameters.AddWithValue("@receipt_ad", pdfFilePath);

                        copyCommand.ExecuteNonQuery();

                        // ลบข้อมูลจากตาราง carts หลังจากคัดลอกไปยัง history
                        string deleteQuery = "DELETE FROM carts";
                        MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, conn);
                        deleteCommand.ExecuteNonQuery();


                        frmlogin frmlogin = new frmlogin();
                        frmlogin.Show();
                        this.Close();
                        
                    }
                    
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message, "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void btnbacktoorder_Click(object sender, EventArgs e)
        {
            this.Hide();
            // แสดง Form1 ที่เปิดอยู่แล้ว
            Form2 form2 = (Form2)Application.OpenForms["Form2"];
            form2.Show();
        }

        private void labelTotalAmount_Click(object sender, EventArgs e)
        {

        }

        private void qr_Load(object sender, EventArgs e)
        {
            UpdateTotalLabel(); 
            //ดึงยอด สร้าง QR 
            double total = (double)totalAmount;
            string qr = PPay.DynamicQR.MobileNumber("0934758624").Amount(total).CreateCreditTransferQrCode();
            BarcodeWriter barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Width = 326,
                    Height = 316,
                    PureBarcode = true
                }
            };
            Bitmap barCodeBitmap = barcodeWriter.Write(qr);
            pictureBox1.Image = barCodeBitmap;
        }
        
        
    }
}
