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
    public partial class 登录界面 : Form
    {
        public 登录界面()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

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

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("本游戏通过躲避障碍物和行进获得积分\n" + 
                            "积分既可购买三种强力道具，功能分别为加血、清怪、保护\n" +
                            "又可凭其问鼎排行榜\n" +
                            "共三关,三种级别难度，难度递增，积分递增\n" +
                            "只有通关才能获得该关卡积分\n" +
                            "障碍分为静止障碍和飞行物，碰到障碍则生命值减1\n" +
                            "生命为0时本局比赛结束" , "帮助", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            注册界面 f2 = new 注册界面(this);
            //this.Hide();
            f2.Show();
            this.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("帐号、密码不能为空！");
                return;
            }
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
                if (!dr.HasRows)
                {
                    MessageBox.Show("帐号不存在！");
                }
                else
                {
                    if (textBox2.Text == dr.GetString(2))
                    {
                        主界面 f3 = new 主界面(this, dr.GetString(0), dr.GetString(1));
                        this.Hide();
                        f3.Show();
                    }
                    else
                    {
                        MessageBox.Show("密码错误！");
                    }
                }
                dr.Close();
                con.Close();
                return;
            }
            catch(SqlException SQL)
            {
                MessageBox.Show(SQL.Message);
                return;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text == "")
                return;
            for(int i = 0; i < ((TextBox)sender).Text.Length; i++)
            {
                char c = ((TextBox)sender).Text[i];
                if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_'))
                {
                    ((TextBox)sender).Text = ((TextBox)sender).Text.Remove(i, 1);
                    MessageBox.Show("只能输入数字、字母或下划线！");
                }
            }  
        }

        private void 登录界面_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
        }
    }
}
