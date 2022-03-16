using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        List<decimal> price = new List<decimal>();
        string[] menu = null;
        string path = "";
        public Form1()
        {
            InitializeComponent();
            label31.Text = DateTime.Now.ToLongDateString();
            timer2.Start();
            button3_Click(null, null);
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            path = Path.GetFullPath(Path.Combine(path, "..", ".."));
            menu = System.IO.File.ReadAllLines(path + "/properties.txt");
            int i = 2;
            


            foreach (string line in menu)
            {
                string name = "label" + i;
                Label lbl = this.Controls.Find(name, true).FirstOrDefault() as Label;
                lbl.Text = line.Substring(0, line.IndexOf("/"));
                Console.WriteLine(Regex.Match(line, @"\d+.+\d").Value);
                price.Add(System.Convert.ToDecimal(Regex.Match(line, @"\d+.+\d").Value));
                i++;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is TextBox)
                    {
                        (control as TextBox).Text = "0";
                        (control as TextBox).Enabled = false;

                    }
                    else if (control is CheckBox)
                        (control as CheckBox).Checked = false;
                    else
                        func(control.Controls);
            };

            func(Controls);
            label25.Text = "0";
            label26.Text = "0";
            label27.Text = "0";
            label29.Text = "0";
            label28.Text = "0";
            richTextBox1.Text = "";

        }


        private void CheckedChanged(object sender, EventArgs e)
        {

            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is CheckBox && (control as CheckBox).Checked)
                    {
                        string number = control.Name.Remove(0, 8);
                        string textboxname = "textBox" + number;
                        TextBox tbx = this.Controls.Find(textboxname, true).FirstOrDefault() as TextBox;
                        tbx.Enabled = true;
                    }
                    else
                        func(control.Controls);
            };
            func(Controls);
        }



        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "PARAGON FISKALNY\n";
            decimal sum = 0;
            decimal tax = 0;
            decimal drink = 0;
            decimal other = 0;
            decimal number = 0;
            for (int i = 0; i < price.Count; i++)
            {
                string name = "textBox" + (i + 1);
                TextBox txt = this.Controls.Find(name, true).FirstOrDefault() as TextBox;
                try
                {
                   number = decimal.Parse(txt.Text);
                }
                catch (System.FormatException ex)
                {
                    MessageBox.Show("Błędna wartość", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                decimal subprice = Decimal.Multiply(number, price[i]);
                if(subprice > 0 ){
                    sum += subprice;
                    string lab = "label" + (i + 2);
                    Label label = this.Controls.Find(lab, true).FirstOrDefault() as Label;
                    richTextBox1.AppendText(label.Text+"   " +txt.Text +" SZT"+ " * " + price[i] + "="+ subprice + "\n");
                    if (menu[i].Contains("napój") == true)
                    {
                        drink += subprice;
                        tax += Decimal.Multiply(subprice, 0.23m);
                       

                    }
                    else
                    {
                        tax += Decimal.Multiply(subprice, 0.05m);
                        other += subprice;
                    } }
            }
            richTextBox1.AppendText("Data " + label30.Text+"\n " + label31.Text);
            richTextBox1.AppendText("\nSUMA PLN   " + sum);
            label25.Text = sum.ToString();
            label26.Text = tax.ToString();
            label27.Text = (sum - tax).ToString();
            label29.Text = drink.ToString();
            label28.Text = other.ToString();

            Console.WriteLine(sum);
            Console.WriteLine(tax);
            
        }

        private void Color_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog1 = new ColorDialog();
            colorDialog1.Color = panel1.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                panel1.BackColor = colorDialog1.Color;
                Action<Control.ControlCollection> func = null;
                func = (controls) =>
                {
                    foreach (Control control in controls)
                        if (control is Panel)
                        {
                            (control as Panel).BackColor = colorDialog1.Color;

                        }
                        else
                            func(control.Controls);
                };
                func(Controls);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog1 = new FontDialog();
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                Action<Control.ControlCollection> func = null;

                func = (controls) =>
                {
                    foreach (Control control in controls)
                        if (control is TextBox | control is Label)
                        {
                            (control).Font = fontDialog1.Font;

                        }
                        else
                            func(control.Controls);
                };

                func(Controls);


            }
            label1.Font = new Font(label1.Font.FontFamily, 28);

        }



        private void timer2_Tick(object sender, EventArgs e)
        {
            label30.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void nowyToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }

        private void drukujToolStripButton_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.Show();
        }



        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString(richTextBox1.Text, new Font("Arial", 14, FontStyle.Regular), Brushes.Black, 120, 120);
        }

        private void zapiszToolStripButton_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Console.WriteLine(path);
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName+".txt"))
                {
                    sw.Write(richTextBox1.Text);
                    sw.Flush();
                    sw.Close();
                }
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}



