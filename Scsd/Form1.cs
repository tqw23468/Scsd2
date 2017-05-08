using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scsd
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
        public List<Dingdan> ls;
        public List<Scpc> lspc;
       // public List<Scpc> lspc2;
       // public List<Scpc> lspc3;
        private void button1_Click(object sender, EventArgs e)
        {
           

            richTextBox1.Text = "";
            ls = new List<Dingdan>();
            ls.Add(new Dingdan(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text), comboBox1.Text));
            ls.Add(new Dingdan(Convert.ToInt32(textBox3.Text), Convert.ToInt32(textBox4.Text), comboBox2.Text));
            ls.Add(new Dingdan(Convert.ToInt32(textBox5.Text), Convert.ToInt32(textBox6.Text), comboBox3.Text));
            ls.Add(new Dingdan(Convert.ToInt32(textBox7.Text), Convert.ToInt32(textBox8.Text), comboBox4.Text));
            ls.Sort();//排序


            //去重
            for (int i = 1; i < ls.Count; i++) {
                if (ls[i - 1].tian == ls[i].tian && ls[i - 1].chanping == ls[i].chanping) {
                    ls[i - 1].liang += ls[i].liang;
                    ls.RemoveAt(i);
                    i--;
                }
            }
            

            //去库存
            int kucunA = Convert.ToInt32(textBox9.Text);
            int kucunB = Convert.ToInt32(textBox11.Text);
            int kucunC = Convert.ToInt32(textBox12.Text);
            int shenA = kucunA;
            int shenB = kucunB;
            int shenC = kucunC;

            for (int i=0; i < ls.Count; i++) {
                if (ls[i].liang >= shenA && ls[i].chanping=="A") { ls[i].liang = ls[i].liang - shenA; break; }
                else if(ls[i].chanping == "A") { shenA -= ls[i].liang; ls[i].liang = 0; }
            }
            for (int i = 0; i < ls.Count; i++)
            {
                if (ls[i].liang >= shenB && ls[i].chanping == "B") { ls[i].liang = ls[i].liang - shenB; break; }
                else if (ls[i].chanping == "B") { shenB -= ls[i].liang; ls[i].liang = 0;  }
            }
            for (int i = 0; i < ls.Count; i++)
            {
                if (ls[i].liang >= shenC && ls[i].chanping == "C") { ls[i].liang = ls[i].liang - shenC; break; }
                else if (ls[i].chanping == "C") { shenC -= ls[i].liang; ls[i].liang = 0;  }
            }
            
            String ss = "\n";
            foreach (Dingdan d in ls) {
                ss += d.ToString() + "\n";
            }
            richTextBox1.Text += ss;

            

            lspc = new List<Scpc>();

           
            int pp = 0;
            foreach (IGrouping<int, Dingdan> group in ls.GroupBy(c => c.tian))
            {
                int s, end;
                s = pp;
                end = group.Key;
                pp = end;
                List<TianAP> lsT = new List<TianAP>();
                double zs = 0;
                foreach (Dingdan d in group)
                {
                    double s1 = d.liang / Convert.ToDouble(end - s);
                    lsT.Add(new TianAP(d.chanping,s1));
                    zs += s1;
                }
                lspc.Add(new Scpc(s, end, zs, lsT));
            }

            int maxchan = Convert.ToInt32(textBox10.Text);

            List<Scpc> lsp = you(lspc, maxchan, 0);
            ss = "\n最优排程\n";
            foreach (Scpc spc in lsp)
            {
                ss += spc.ToString() + "\n";
            }
            richTextBox1.Text += ss;


            ss = "\n结果：";
            bool flag = true;
            foreach (Scpc spc in lsp)
            {
                if (spc.sudu > maxchan) {
                    flag = false;
                    break;
                }
            }
            if (!flag) ss += "新接的订单不能按时完成。请延缓订单，减少订货数量或增加生产线";
            else  ss += "新接的订单能按时完成。";
            richTextBox1.Text += ss;

        }
        public List<Scpc> you(List<Scpc> lss,int maxchan,int p) {
            bool flag=false;
            for (int i= 1;i< lss.Count; i++)
            {
                
                if (lss[i].sudu > maxchan)
                {
                    double yu = (lss[i].end - lss[i].strart) * (lss[i].sudu - maxchan);
                    lss[i].sudu = maxchan;
                    lss[i - 1].sudu += yu / Convert.ToDouble(lss[i - 1].end - lss[i - 1].strart);
                    for (int j =0; j < lss[i].tianls.Count; j++) {
                        if (lss[i].tianls[j].sudu * (lss[i].end - lss[i].strart) >yu)
                        {
                            lss[i].tianls[j].sudu = lss[i].tianls[j].sudu - yu / Convert.ToDouble(lss[i].end - lss[i].strart);
                            int ppp = -1;
                            for (int h = 0; h < lss[i - 1].tianls.Count; h++)
                            {
                                if (lss[i - 1].tianls[h].chanping == lss[i].tianls[j].chanping) { ppp = h; break; }
                            }
                            if (ppp == -1) lss[i - 1].tianls.Add(new TianAP(lss[i].tianls[j].chanping, yu / Convert.ToDouble(lss[i-1].end - lss[i-1].strart)));
                            else lss[i - 1].tianls[ppp].sudu += yu / Convert.ToDouble(lss[i-1].end - lss[i-1].strart);
                            break;
                        }
                        else {
                            
                            int ppp = -1;

                            for (int h = 0; h < lss[i - 1].tianls.Count; h++)
                            {
                                if (lss[i - 1].tianls[h].chanping == lss[i].tianls[j].chanping) { ppp = h; break; }
                            }
                            if (ppp == -1) lss[i - 1].tianls.Add(new TianAP(lss[i].tianls[j].chanping, lss[i].tianls[j].sudu * (lss[i].end - lss[i].strart) / Convert.ToDouble(lss[i - 1].end - lss[i - 1].strart)));
                            else lss[i - 1].tianls[ppp].sudu += lss[i].tianls[j].sudu * (lss[i].end - lss[i].strart) / Convert.ToDouble(lss[i - 1].end - lss[i - 1].strart);
                            yu -= lss[i].tianls[j].sudu * (lss[i].end - lss[i].strart);
                            lss[i].tianls.RemoveAt(j);
                            j--;

                        }
                        if (yu == 0) break;
                    }                               
                    flag = true;
                    if(i==1) flag = false;
                }
                else flag = false;
                if (i > p) p = i;
                if (flag) break;
            }
            if (p == lss.Count - 1 && !flag) return lss;
            else return you(lss, maxchan, p);
        }
        public class Dingdan : IComparable<Dingdan>
        {
            public int tian;
            public int liang;
            public String chanping;
            public Dingdan(int t, int l,String c) {
                this.tian = t;
                this.liang = l;
                this.chanping = c;
            }
            public int CompareTo(Dingdan other)
            {
                if (other == null)
                    return 1;
                int value = this.tian - other.tian;
                if (value == 0)
                    value = string.Compare(this.chanping, other.chanping);
                return value;
            }
            public override string ToString()
            {
                return tian + "天后需生产"+chanping+"产品" + liang+"件";

            }

        }
        public class Scpc
        {
            public int strart;
            public int end;
            public double sudu;
            public List<TianAP> tianls;
            public Scpc(int s, int e, double sd, List<TianAP> ls)
            {
                this.strart = s;
                this.end = e;
                this.sudu = sd;
                this.tianls = ls;
            }
            public override string ToString()
            {
                String s= strart + "-" + end + "天，总生产速度为：" + sudu + "/天,明细如下：\n";
                foreach(TianAP t in this.tianls) {
                    s+= "     生产"+t.chanping+"商品，速度为："+t.sudu+ "/天\n";

                }
                return s;
            }

        }
        public class TianAP{
            public String chanping;
            public double sudu;
            public TianAP(string s ,double d) {
                this.chanping = s;
                this.sudu = d;
            }
        }


        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }
    }

}
