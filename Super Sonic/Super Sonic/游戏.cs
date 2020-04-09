using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Data.SqlClient;
using System.Media;

namespace Super_Sonic
{
    public partial class 游戏 : Form
    {
        主界面 fmain;//主界面
        String ID;
        int goal, prop1, prop2, prop3, level, ord;//已有积分，道具一数量，道具二数量，道具三数量，难度，场次
        SoundPlayer sp = new SoundPlayer();
        int life = 5;//生命值
        int dgoal = 0;//本局积分
        int dis = 0;//行进距离
        int gate = -1; //关卡
        bool bstart = false;//关卡开关
        bool bmusic = true;//音乐开关
        bool bkey = true;//按键开关
        bool bpause = false;//暂停开关
        int bjump = 0;//跳跃开关
        int bdes = 0;//毁灭开关
        int bhurt = 0;//免伤开关
        int binfo = 0;//提示开关
        bool b_r_l = false;//人物位置
        bool b_m_r = false;//飞行物方向
        bool bleft = false;//左障碍开关
        bool bright = false;//右障碍开关
        bool bmid = false;//飞行物开关
        int ibleft = -1;//左爆炸
        int ibright = -1;//右爆炸
        int ibmid = -1;//飞行物爆炸
        int i_m_act = 0;//飞行物索引
        int i_m_pic = 0;//飞行物帧数索引
        int i_r_act = 0;//人物动作索引
        int i_r_pic = 0;//人物帧数索引
        int i_back = 0; //背景索引
        int[] lim = {2, 6, 4, 5, 1, 11, 7};//背景，走，跑，啸，飞, 爆炸
        Bitmap[][] back = new Bitmap[3][];//背景
        Bitmap[][] lrole = new Bitmap[5][];//人物左边动作
        Bitmap[][] rrole = new Bitmap[5][];//人物右边动作
        Bitmap[] lmid =
        {
                new Bitmap(Properties.Resources.左111),
                new Bitmap(Properties.Resources.左112),
                new Bitmap(Properties.Resources.左121),
                new Bitmap(Properties.Resources.左122),
                new Bitmap(Properties.Resources.左211),
                new Bitmap(Properties.Resources.左212),
                new Bitmap(Properties.Resources.左221),
                new Bitmap(Properties.Resources.左222),
                new Bitmap(Properties.Resources.左311),
                new Bitmap(Properties.Resources.左312),
                new Bitmap(Properties.Resources.左321),
                new Bitmap(Properties.Resources.左322),
                new Bitmap(Properties.Resources.左4)
            };//飞行物朝左飞
        Bitmap[] rmid =
        {
                new Bitmap(Properties.Resources.右111),
                new Bitmap(Properties.Resources.右112),
                new Bitmap(Properties.Resources.右121),
                new Bitmap(Properties.Resources.右122),
                new Bitmap(Properties.Resources.右211),
                new Bitmap(Properties.Resources.右212),
                new Bitmap(Properties.Resources.右221),
                new Bitmap(Properties.Resources.右222),
                new Bitmap(Properties.Resources.右311),
                new Bitmap(Properties.Resources.右312),
                new Bitmap(Properties.Resources.右321),
                new Bitmap(Properties.Resources.右322),
                new Bitmap(Properties.Resources.右4)
            };//飞行物朝右飞
        Bitmap[] lbrr =
            {
                new Bitmap(Properties.Resources.左11),
                new Bitmap(Properties.Resources.左12),
                new Bitmap(Properties.Resources.左13),
                new Bitmap(Properties.Resources.左14),
                new Bitmap(Properties.Resources.左15),
                new Bitmap(Properties.Resources.左16),
                new Bitmap(Properties.Resources.左21),
                new Bitmap(Properties.Resources.左22),
                new Bitmap(Properties.Resources.左23),
                new Bitmap(Properties.Resources.左24),
                new Bitmap(Properties.Resources.左25),
                new Bitmap(Properties.Resources.左26),
                new Bitmap(Properties.Resources.左31),
                new Bitmap(Properties.Resources.左32),
                new Bitmap(Properties.Resources.左33),
                new Bitmap(Properties.Resources.左34),
                new Bitmap(Properties.Resources.左35),
                new Bitmap(Properties.Resources.左36),
            };//左边障碍物
        Bitmap[] rbrr =
            {
                new Bitmap(Properties.Resources.右11),
                new Bitmap(Properties.Resources.右12),
                new Bitmap(Properties.Resources.右13),
                new Bitmap(Properties.Resources.右14),
                new Bitmap(Properties.Resources.右15),
                new Bitmap(Properties.Resources.右16),
                new Bitmap(Properties.Resources.右21),
                new Bitmap(Properties.Resources.右22),
                new Bitmap(Properties.Resources.右23),
                new Bitmap(Properties.Resources.右24),
                new Bitmap(Properties.Resources.右25),
                new Bitmap(Properties.Resources.右26),
                new Bitmap(Properties.Resources.右31),
                new Bitmap(Properties.Resources.右32),
                new Bitmap(Properties.Resources.右33),
                new Bitmap(Properties.Resources.右34),
                new Bitmap(Properties.Resources.右35),
                new Bitmap(Properties.Resources.右36),
            };//右边障碍物
        Bitmap[] bang =
            {
                new Bitmap(Properties.Resources.爆炸__1_),
                new Bitmap(Properties.Resources.爆炸__2_),
                new Bitmap(Properties.Resources.爆炸__3_),
                new Bitmap(Properties.Resources.爆炸__4_),
                new Bitmap(Properties.Resources.爆炸__5_),
                new Bitmap(Properties.Resources.爆炸__6_),
                new Bitmap(Properties.Resources.爆炸__7_),
            };//爆炸
        Point p1, p2, p3;//初始障碍物坐标
        int leftlim, rightlim;//飞行物左右边界
        

        String[] stext = { "Ready", "Go", "" };
        

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            提示.Start();
        }

        private void 提示_Tick(object sender, EventArgs e)
        {
            label7.Text = stext[binfo++];
            if(binfo == 3)
            {
                gate++;
                label1.Text = "第" + (gate + 1).ToString() + "关";
                binfo = 0;
                提示.Stop();               
                重绘.Start();
                label7.Visible = false;
                bstart = true;
            }
        }

        private void 重绘_Tick(object sender, EventArgs e)
        {
            //行进、计分
            dis += level;
            dgoal += level;
            label9.Text = "积分*" + dgoal.ToString();

            //绘制
            panel1.BackgroundImage = back[gate][i_back];
            if(!(bjump > 0 && i_r_act == 3))
            {
                if (b_r_l)
                {
                    role.Image = lrole[i_r_act][i_r_pic];
                }
                else
                {
                    role.Image = rrole[i_r_act][i_r_pic];
                }
            }
            else
            {
                if (!b_r_l)
                {
                    role.Image = lrole[i_r_act][i_r_pic];
                }
                else
                {
                    role.Image = rrole[i_r_act][i_r_pic];
                }
            }

            if(bmid)
            {
                if (b_m_r)
                    brr3.Image = rmid[i_m_act + i_m_pic];
                else
                    brr3.Image = lmid[i_m_act + i_m_pic];
            }

            if (ibleft >= 0)
            {
                brr1.Image = bang[ibleft];
            }

            if (ibright >= 0)
            {
                brr2.Image = bang[ibright];
            }

            if (ibmid >= 0)
            {
                brr3.Image = bang[ibmid];
            }

            //帧切换
            if (++i_r_pic == lim[i_r_act + 1])
                i_r_pic = 0;

            if (++i_back == lim[0])
                i_back = 0;

            if (bmid)
            {
                if (i_m_pic == 0)
                    i_m_pic = 1;
                else
                    i_m_pic = 0;
                if (brr3.Location.X < leftlim)
                    b_m_r = true;
                if (brr3.Location.X + brr3.Size.Width > rightlim)
                    b_m_r = false;
            }

            if (ibleft >= 0)
            {
                ibleft++;
                if (ibleft == 7)
                {
                    ibleft = -1;
                    brr1.Location = p1;
                    brr1.Image = null;
                }
            }

            if (ibright >= 0)
            {
                ibright++;
                if (ibright == 7)
                {
                    ibright = -1;
                    brr2.Location = p2;
                    brr2.Image = null;
                }
            }

            if (ibmid >= 0)
            {
                ibmid++;
                if (ibmid == 7)
                {
                    ibmid = -1;
                    brr3.Location = p3;
                    brr3.Image = null;
                }
            }

            //状态切换
            if (bjump > 0)
                bjump++;
            if (bhurt > 0)
                bhurt++;
            if (bdes > 0)
                bdes++;
            if (bjump == 12)
            {
                i_r_act = 0;
                i_r_pic = 0;
                bkey = true;
                b_r_l = !b_r_l;
                label10.Text = "";
                bjump = 0;
            }
            if(bhurt == 31)
            {
                label6.Text = "";
                bhurt = 0;
                if(i_r_act == 3)
                {
                    i_r_act = 0;
                    i_r_pic = 0;
                }
            }
            if (bdes == 6)
            {
                bdes = 0;
                if(i_r_act == 2)
                {
                    i_r_act = 0;
                    i_r_pic = 0;
                }
            }

            //位移
            if (bjump > 0)
            {
                if (b_r_l)
                {
                    role.Location = new Point(role.Location.X + 25, role.Location.Y);
                }
                else
                {
                    role.Location = new Point(role.Location.X - 25, role.Location.Y);
                }
            }

            if(brr1.Image != null)
                brr1.Location = new Point(brr1.Location.X, brr1.Location.Y + level * 10 + gate * 10);

            if (brr2.Image != null)
                brr2.Location = new Point(brr2.Location.X, brr2.Location.Y + level * 10 + gate * 10);

            if (bmid)
            {
                if (b_m_r)
                    brr3.Location = new Point(brr3.Location.X + level * 10 + gate * 10, brr3.Location.Y + level * 10 + gate * 10);
                else
                    brr3.Location = new Point(brr3.Location.X - level * 10 - gate * 10, brr3.Location.Y + level * 10 + gate * 10);
            }

            //判伤
            if(bleft)
            {
                if(ishurt(brr1, role.Location) || ishurt(brr1, new Point(role.Location.X + role.Size.Width, role.Location.Y)) ||
                    ishurt(brr1, new Point(role.Location.X + role.Size.Width, role.Location.Y + role.Size.Height)) || 
                    ishurt(brr1, new Point(role.Location.X, role.Location.Y + role.Size.Height)))
                {
                    bleft = false;
                    ibleft = 0;
                    if(bhurt == 0)
                        life -= 1;
                    label8.Text = "生命*" + life.ToString();
                    if (bmusic)
                    {
                        sp.SoundLocation = @"..\..\Resources\受伤.wav";
                        sp.Play();
                    }
                    if (life <= 0)
                    {
                        label7.Visible = true;
                        label7.Text = "失败！";
                        if (bmusic)
                        {
                            sp.SoundLocation = @"..\..\Resources\失败.wav";
                            sp.Play();
                        }
                        重绘.Stop();
                        bstart = false;
                        return;
                    }
                }
            }

            if (bright)
            {
                if (ishurt(brr2, role.Location) || ishurt(brr2, new Point(role.Location.X + role.Size.Width, role.Location.Y)) ||
                    ishurt(brr2, new Point(role.Location.X + role.Size.Width, role.Location.Y + role.Size.Height)) ||
                    ishurt(brr2, new Point(role.Location.X, role.Location.Y + role.Size.Height)))
                {
                    bright = false;
                    ibright = 0;
                    if (bhurt == 0)
                        life -= 1;
                    label8.Text = "生命*" + life.ToString();
                    if (bmusic)
                    {
                        sp.SoundLocation = @"..\..\Resources\受伤.wav";
                        sp.Play();
                    }
                    if (life <= 0)
                    {
                        label7.Visible = true;
                        label7.Text = "失败！";
                        if (bmusic)
                        {
                            sp.SoundLocation = @"..\..\Resources\失败.wav";
                            sp.Play();
                        }
                        重绘.Stop();
                        bstart = false;
                        return;
                    }
                }
            }

            if (bmid)
            {
                if (ishurt(brr3, role.Location) || ishurt(brr3, new Point(role.Location.X + role.Size.Width, role.Location.Y)) ||
                    ishurt(brr3, new Point(role.Location.X + role.Size.Width, role.Location.Y + role.Size.Height)) ||
                    ishurt(brr3, new Point(role.Location.X, role.Location.Y + role.Size.Height)))
                {
                    bmid = false;
                    ibmid = 0;
                    if (bhurt == 0)
                        life -= 1;
                    label8.Text = "生命*" + life.ToString();
                    if (bmusic)
                    {
                        sp.SoundLocation = @"..\..\Resources\受伤.wav";
                        sp.Play();
                    }
                    if (life <= 0)
                    {
                        label7.Visible = true;
                        label7.Text = "失败！";
                        if (bmusic)
                        {
                            sp.SoundLocation = @"..\..\Resources\失败.wav";
                            sp.Play();
                        }
                        重绘.Stop();
                        bstart = false;
                        return;
                    }
                }
            }

            //刷新障碍
            if (!bleft)
            {
                if(dis % 30 == 0 && brr1.Image == null)
                {
                    bleft = true;
                    Random r = new Random();
                    brr1.Image = lbrr[gate * 6 + r.Next(6)];
                }
            }
            else if (brr1.Location.Y > this.Height)
            {
                bleft = false;
                brr1.Image = null;
                brr1.Location = p1;
                dgoal += 10;
                label9.Text = "积分*" + dgoal.ToString();
            }

            if (!bright)
            {
                if (dis % 50 == 0 && brr2.Image == null)
                {
                    bright = true;
                    Random r = new Random();
                    brr2.Image = rbrr[gate * 6 + r.Next(6)];
                }
            }
            else if (brr2.Location.Y > this.Height)
            {
                bright = false;
                brr2.Image = null;
                brr2.Location = p2;
                dgoal += 10;
                label9.Text = "积分*" + dgoal.ToString();
            }

            if (!bmid)
            {
                if (dis % 100 == 0 && brr3.Image == null)
                {
                    bmid = true;
                    Random r = new Random();
                    i_m_act = gate * 4 + r.Next(2) * 2;
                }

            }
            else if (brr3.Location.Y > this.Height)
            {
                bmid = false;
                brr3.Image = null;
                brr3.Location = p3;
                dgoal += 10;
                label9.Text = "积分*" + dgoal.ToString();
            }

            //流程控制
            if (dis > 1000 && gate < 1)
            {
                brr1.Image = null;
                brr1.Location = p1;
                bleft = false;
                brr2.Image = null;
                brr2.Location = p2;
                bright = false;
                brr3.Image = null;
                brr3.Location = p3;
                bmid = false;
                label7.Visible = true;
                label7.Text = "过关！";
                if (bmusic)
                {
                    sp.SoundLocation = @"..\..\Resources\成功.wav";
                    sp.Play();
                }
                重绘.Stop();
                button1.Enabled = true;
                bstart = false;
                update();
            }

            if (dis > 2000 && gate < 2)
            {
                brr1.Image = null;
                brr1.Location = p1;
                bleft = false;
                brr2.Image = null;
                brr2.Location = p2;
                bright = false;
                brr3.Image = null;
                brr3.Location = p3;
                bmid = false;
                label7.Visible = true;
                label7.Text = "过关！";
                if (bmusic)
                {
                    sp.SoundLocation = @"..\..\Resources\成功.wav";
                    sp.Play();
                }
                重绘.Stop();
                button1.Enabled = true;
                bstart = false;
                update();
            }

            if (dis > 3000 && gate < 3)
            {
                brr1.Image = null;
                brr1.Location = p1;
                bleft = false;
                brr2.Image = null;
                brr2.Location = p2;
                bright = false;
                brr3.Image = null;
                brr3.Location = p3;
                bmid = false;
                label7.Visible = true;
                label7.Text = "通关！";
                if (bmusic)
                {
                    sp.SoundLocation = @"..\..\Resources\成功.wav";
                    sp.Play();
                }
                重绘.Stop();
                bstart = false;
                update();
            }

        }


        private void 游戏_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                this.Close();
            }
            else if (e.KeyValue == 'm' || e.KeyValue == 'M')
            {
                bmusic = !bmusic;
            }
            else if(bstart)
            {
                if ((e.KeyValue == 'p' || e.KeyValue == 'P'))
                {
                    if (bpause)
                    {
                        重绘.Start();
                        bpause = false;
                    }
                    else
                    {
                        重绘.Stop();
                        bpause = true;
                    }
                }
                else if(!bpause)
                {
                    if (e.KeyValue == '1' && prop1 > 0)
                    {
                        prop1--;
                        life++;
                        label8.Text = "生命*" + life.ToString();
                        label3.Text = prop1.ToString();
                        if (bmusic)
                        {
                            sp.SoundLocation = @"..\..\Resources\加血.wav";
                            sp.Play();
                        }
                    }
                    else  if (e.KeyValue == '2' && prop2 > 0)
                    {
                        prop2--;
                        label4.Text = prop2.ToString();
                        bdes = 1;
                        if(bleft)
                        {
                            bleft = false;
                            ibleft = 0;
                        }
                        if (bright)
                        {
                            bright = false;
                            ibright = 0;
                        }
                        if (bmid)
                        {
                            bmid = false;
                            ibmid = 0;
                        }
                        if (bjump == 0)
                        {
                            i_r_act = 2;
                            i_r_pic = 0;
                        }
                        if (bmusic)
                        {
                            sp.SoundLocation = @"..\..\Resources\爆炸.wav";
                            sp.Play();
                        }
                    }
                    else if (e.KeyValue == '3' && prop3 > 0)
                    {
                        prop3--;
                        label5.Text = prop3.ToString();
                        bhurt = 1;
                        i_r_act = 3;
                        i_r_pic = 0;
                        label6.Text = "无敌";
                        if(bmusic)
                        {
                            sp.SoundLocation = @"..\..\Resources\无敌.wav";
                            sp.Play();
                        }
                    }
                    else if (e.KeyValue == 32 && bkey)
                    {
                        i_r_act = 4;
                        i_r_pic = 0;
                        bjump = 1;
                        label10.Text = "跳跃";
                        bkey = false;
                    }
                }
            }
        }


        private void 游戏_FormClosing(object sender, FormClosingEventArgs e)
        {
            fmain.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 游戏_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            label3.Text = prop1.ToString();
            label4.Text = prop2.ToString();
            label5.Text = prop3.ToString();
            label8.Text = "生命*" + life.ToString();
            label9.Text = "积分*" + dgoal.ToString();
        }

        public 游戏(主界面 f, String id, int g, int pr1, int pr2, int pr3, int l, int o)
        {
            InitializeComponent();
            fmain = f;
            ID = id;
            goal = g;
            prop1 = pr1;
            prop2 = pr2;
            prop3 = pr3;
            level = l;
            ord = o;
            p1 = brr1.Location;
            p2 = brr2.Location;
            p3 = brr3.Location;

            //背景初始化
            back[0] = new Bitmap[2];
            back[0][0] = new Bitmap(Properties.Resources.场景1_1);
            back[0][1] = new Bitmap(Properties.Resources.场景1_2);
            back[1] = new Bitmap[2];
            back[1][0] = new Bitmap(Properties.Resources.场景2_1);
            back[1][1] = new Bitmap(Properties.Resources.场景2_2);
            back[2] = new Bitmap[2];
            back[2][0] = new Bitmap(Properties.Resources.场景3_1);
            back[2][1] = new Bitmap(Properties.Resources.场景3_2);
            //人物右初始化
            Bitmap[] rbmp =
            {
                new Bitmap(Properties.Resources.走__1_),
                new Bitmap(Properties.Resources.走__2_),
                new Bitmap(Properties.Resources.走__3_),
                new Bitmap(Properties.Resources.走__4_),
                new Bitmap(Properties.Resources.走__5_),
                new Bitmap(Properties.Resources.走__6_),
                new Bitmap(Properties.Resources.跑__1_),
                new Bitmap(Properties.Resources.跑__2_),
                new Bitmap(Properties.Resources.跑__3_),
                new Bitmap(Properties.Resources.跑__4_),
                new Bitmap(Properties.Resources.状态1__1_),
                new Bitmap(Properties.Resources.状态1__2_),
                new Bitmap(Properties.Resources.状态1__3_),
                new Bitmap(Properties.Resources.状态1__4_),
                new Bitmap(Properties.Resources.状态1__5_),
                new Bitmap(Properties.Resources.状态2),
                new Bitmap(Properties.Resources.跳__1_),
                new Bitmap(Properties.Resources.跳__2_),
                new Bitmap(Properties.Resources.跳__3_),
                new Bitmap(Properties.Resources.跳__4_),
                new Bitmap(Properties.Resources.跳__5_),
                new Bitmap(Properties.Resources.跳__6_),
                new Bitmap(Properties.Resources.跳__5_),
                new Bitmap(Properties.Resources.跳__4_),
                new Bitmap(Properties.Resources.跳__3_),
                new Bitmap(Properties.Resources.跳__2_),
                new Bitmap(Properties.Resources.跳__11_),
            };
            foreach(Bitmap i in rbmp)
            {
                i.MakeTransparent(Color.White);
            }
            for(int i = 1; i <= 5; i++)
            {
                rrole[i - 1] = new Bitmap[lim[i]];
            }
            for(int i = 0; i < rbmp.Length; i++)
            {
                if(i < 6)
                {
                    rrole[0][i] = rbmp[i];
                }
                else if (i < 10)
                {
                    rrole[1][i - 6] = rbmp[i];
                }
                else if (i < 15)
                {
                    rrole[2][i - 10] = rbmp[i];
                }
                else if(i < 16)
                {
                    rrole[3][i - 15] = rbmp[i];
                }
                else
                {
                    rrole[4][i - 16] = rbmp[i];
                }
            }

            Bitmap[] lbmp =
            {
                new Bitmap(Properties.Resources.走__7_),
                new Bitmap(Properties.Resources.走__8_),
                new Bitmap(Properties.Resources.走__9_),
                new Bitmap(Properties.Resources.走__10_),
                new Bitmap(Properties.Resources.走__11_),
                new Bitmap(Properties.Resources.走__12_),
                new Bitmap(Properties.Resources.跑__5_),
                new Bitmap(Properties.Resources.跑__6_),
                new Bitmap(Properties.Resources.跑__7_),
                new Bitmap(Properties.Resources.跑__8_),
                new Bitmap(Properties.Resources.状态1_2__1_),
                new Bitmap(Properties.Resources.状态1_2__2_),
                new Bitmap(Properties.Resources.状态1_2__3_),
                new Bitmap(Properties.Resources.状态1_2__4_),
                new Bitmap(Properties.Resources.状态1_2__5_),
                new Bitmap(Properties.Resources.状态2_2),
                new Bitmap(Properties.Resources.跳__11_),
                new Bitmap(Properties.Resources.跳__2_),
                new Bitmap(Properties.Resources.跳__3_),
                new Bitmap(Properties.Resources.跳__4_),
                new Bitmap(Properties.Resources.跳__5_),
                new Bitmap(Properties.Resources.跳__6_),
                new Bitmap(Properties.Resources.跳__5_),
                new Bitmap(Properties.Resources.跳__4_),
                new Bitmap(Properties.Resources.跳__3_),
                new Bitmap(Properties.Resources.跳__2_),
                new Bitmap(Properties.Resources.跳__1_),
            };
            foreach (Bitmap i in lbmp)
            {
                i.MakeTransparent(Color.White);
            }
            for (int i = 1; i <= 5; i++)
            {
                lrole[i - 1] = new Bitmap[lim[i]];
            }
            for (int i = 0; i < lbmp.Length; i++)
            {
                if (i < 6)
                {
                    lrole[0][i] = lbmp[i];
                }
                else if (i < 10)
                {
                    lrole[1][i - 6] = lbmp[i];
                }
                else if (i < 15)
                {
                    lrole[2][i - 10] = lbmp[i];
                }
                else if (i < 16)
                {
                    lrole[3][i - 15] = lbmp[i];
                }
                else
                {
                    lrole[4][i - 16] = lbmp[i];
                }
            }
            
            foreach (Bitmap i in rmid)
            {
                i.MakeTransparent(Color.White);
            }

            foreach (Bitmap i in lmid)
            {
                i.MakeTransparent(Color.White);
            }

            foreach (Bitmap i in rbrr)
            {
                i.MakeTransparent(Color.White);
            }

            foreach (Bitmap i in lbrr)
            {
                i.MakeTransparent(Color.White);
            }

            leftlim = brr1.Location.X + brr3.Size.Width;
            rightlim = brr2.Location.X;
        }

        private bool ishurt(PictureBox pic, Point p)
        {
            if ((p.X > pic.Location.X + 2) &&
                (p.X < pic.Location.X + pic.Size.Width - 2) &&
                (p.Y > pic.Location.Y + 2) &&
                (p.Y < pic.Location.Y + pic.Size.Height - 2))
                return true;
            return false;
        }

        private void update()
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = @"server=DESKTOP-URK2BE5\SQLEXPRESS;database=Sonic;integrated security=true";
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "update Player set goal=" + (goal + dgoal).ToString() + " where playerID = '" + ID + "'";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "update Own set num=" + prop1 + " where playerID = '" + ID + "' and propID = '01'";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "update Own set num=" + prop2 + " where playerID = '" + ID + "' and propID = '02'";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "update Own set num=" + prop3 + " where playerID = '" + ID + "' and propID = '03'";
                cmd.ExecuteNonQuery();    
                cmd.CommandText = "insert into Play values('" + ID + " '," + ord.ToString() + "," + dgoal.ToString() + ")";
                cmd.ExecuteNonQuery();
                ord++;
                con.Close();
            }
            catch (SqlException SQL)
            {
                MessageBox.Show(SQL.Message);
                return;
            }
        }
    }
}
