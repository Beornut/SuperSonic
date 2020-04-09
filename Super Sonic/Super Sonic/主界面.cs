using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Super_Sonic
{
    public partial class 主界面 : Form
    {
        登录界面 f;
        String ID, n;
        int g;
        int prop1 = 0, prop2 = 0, prop3 = 0, cost = 90;
        int ord = 0;

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 主界面_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
            label4.Text = "ID:" + ID;
            label5.Text = "昵称:" + n;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = @"server=DESKTOP-URK2BE5\SQLEXPRESS;database=Sonic;integrated security=true";
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                //更新积分
                cmd.CommandText = "select goal from Player where playerID='" + ID + "'";
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                g = (int)dr[0];
                label6.Text = "积分:" + g.ToString();
                dr.Close();

                //更新排行榜
                DataTable dt1 = new DataTable();
                dataGridView1.DataSource = dt1;
                cmd.CommandText = "select name, goal from Player order by goal desc";
                dr = cmd.ExecuteReader();
                dt1.Columns.Add("排行");
                dt1.Columns.Add("昵称");
                dt1.Columns.Add("积分");
                int i = 1;
                while(dr.Read() && i < 16)
                {
                    dt1.Rows.Add(i++, dr[0], dr[1]);
                }
                dr.Close();

                //更新战绩
                DataTable dt2 = new DataTable();
                dataGridView2.DataSource = dt2;
                cmd.CommandText = "select ord, goal from Play where playerID='" + ID + "' order by ord desc";
                dr = cmd.ExecuteReader();
                dt2.Columns.Add("场次");
                dt2.Columns.Add("积分");
                if(dr.Read())
                {
                    ord = (int)dr[0] + 1;
                    dt2.Rows.Add(dr[0], dr[1]);
                }
                while (dr.Read())
                {
                    dt2.Rows.Add(dr[0], dr[1]);
                }
                dr.Close();

                //更新背包
                cmd.CommandText = "select num from Own where playerID='" + ID + "' and propID='01'";
                dr = cmd.ExecuteReader();
                if(dr.Read())
                    prop1 = (int)dr[0];
                dr.Close();
                cmd.CommandText = "select num from Own where playerID='" + ID + "' and propID='02'";
                dr = cmd.ExecuteReader();
                if (dr.Read())
                    prop2 = (int)dr[0];
                dr.Close();
                cmd.CommandText = "select num from Own where playerID='" + ID + "' and propID='03'";
                dr = cmd.ExecuteReader();
                if (dr.Read())
                    prop3 = (int)dr[0];
                dr.Close();
                label13.Text = "数量:" + prop1.ToString();
                label14.Text = "数量:" + prop2.ToString();
                label15.Text = "数量:" + prop3.ToString();
                con.Close();
                return;
            }
            catch (SqlException SQL)
            {
                MessageBox.Show(SQL.Message);
                return;
            }
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            cost = 30 * (int)numericUpDown1.Value + 10 * (int)numericUpDown2.Value + 50 * (int)numericUpDown3.Value;
            label22.Text = "总价:" + cost.ToString() + "积分";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int l = comboBox1.SelectedIndex + 1;
            游戏 f2 = new 游戏(this, ID, g, prop1, prop2, prop3, l, ord);
            this.Hide();
            f2.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(cost > g)
            {
                MessageBox.Show("积分不足！");
                return;
            }
            g -= cost;
            prop1 += (int)numericUpDown1.Value;
            prop2 += (int)numericUpDown2.Value;
            prop3 += (int)numericUpDown3.Value;
            label6.Text = "积分:" + g.ToString();
            label13.Text = "数量:" + prop1.ToString();
            label14.Text = "数量:" + prop2.ToString();
            label15.Text = "数量:" + prop3.ToString();
            SqlConnection con = new SqlConnection();
            con.ConnectionString = @"server=DESKTOP-URK2BE5\SQLEXPRESS;database=Sonic;integrated security=true";
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "update Player set goal = "+ g.ToString() + " where playerID='" + ID + "'";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "update Own set num = " + prop1.ToString() + " where playerID='" + ID + "' and propID='01'";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "update Own set num = " + prop2.ToString() + " where playerID='" + ID + "' and propID='02'";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "update Own set num = " + prop3.ToString() + " where playerID='" + ID + "' and propID='03'";
                cmd.ExecuteNonQuery();
                con.Close();
                return;
            }
            catch (SqlException SQL)
            {
                MessageBox.Show(SQL.Message);
                return;
            }
        }

        private void 主界面_Activated(object sender, EventArgs e)
        {
            主界面_Load(null, null);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 主界面_FormClosed(object sender, FormClosedEventArgs e)
        {
            f.Show();
        }

        public 主界面(登录界面 f2, String id, String name)
        {
            InitializeComponent();
            f = f2;
            ID = id;
            n = name;
        }
        
        public void BackToShow()
        {
            this.主界面_Load(null, null);
            this.Show();
        }
    }
}
