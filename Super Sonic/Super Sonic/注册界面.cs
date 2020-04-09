using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Data.SqlClient;

namespace Super_Sonic
{
    public partial class 注册界面 : Form
    {
        登录界面 f;
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

        public 注册界面(登录界面 f1)
        {
            InitializeComponent();
            f = f1;
        }

        private void 注册界面_FormClosed(object sender, FormClosedEventArgs e)
        {
            f.Show();
        }

        private void Form2_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void btn_MouseEnter(object sender, EventArgs e)
        {
            ((Button)sender).Font = new Font(((Button)sender).Font.Name, ((Button)sender).Font.Size + 5);
            ((Button)sender).ForeColor = Color.Blue;

        }

        private void btn_MouseLeave(object sender, EventArgs e)
        {
            ((Button)sender).Font = new Font(((Button)sender).Font.Name, ((Button)sender).Font.Size - 5);
            ((Button)sender).ForeColor = Color.Black;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void text_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("注册信息不能为空！");
            }
            else if(textBox3.Text != textBox2.Text)
            {
                MessageBox.Show("两次密码输入不一致！");
            }
            else
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = @"server=DESKTOP-URK2BE5\SQLEXPRESS;database=Sonic;integrated security=true";
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "select * from Player where name = '" + textBox1.Text + "'";
                    SqlDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        dr.Close();
                        MessageBox.Show("帐号已存在，请换个昵称！");
                    }
                    else
                    {
                        dr.Close();
                        Boolean flag = true;
                        String s = "";
                        while (flag)
                        {
                            Random r = new Random();
                            s = r.Next(10000, 99999).ToString();
                            cmd = new SqlCommand("SELECT * FROM Player WHERE playerID='" + s + "'", con);
                            dr = cmd.ExecuteReader();
                            dr.Read();
                            if (!dr.HasRows)
                                flag = false;
                            dr.Close();
                        }
                        cmd.CommandText = "insert into Player values('" + s + "','" + textBox1.Text + "','" + textBox2.Text + "'," + "0" +")";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "insert into Own values('" + s + "','01',0)";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "insert into Own values('" + s + "','02',0)";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "insert into Own values('" + s + "','03',0)";
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("注册成功！");
                        con.Close();
                        this.Close();
                    }
                    con.Close();
                    return;
                }
                catch (SqlException SQL)
                {
                    MessageBox.Show(SQL.Message);
                    return;
                }
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text == "")
                return;
            for (int i = 0; i < ((TextBox)sender).Text.Length; i++)
            {
                char c = ((TextBox)sender).Text[i];
                if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_'))
                {
                    ((TextBox)sender).Text = ((TextBox)sender).Text.Remove(i, 1);
                    MessageBox.Show("只能输入数字、字母或下划线！");
                }
            }
        }

        private void 注册界面_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
        }
    }
}
