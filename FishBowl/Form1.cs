using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FishBowl.OtherFish;
using SevenZip;
using SubEnc;
using System.Threading;

namespace FishBowl
{

    public partial class Form1 : Form
    {
        Thread t;
        bool canClose = true;
        public Form1()
        {
            InitializeComponent();
        }

        private void EncBtn_Click(object sender, EventArgs e)
        {
            string Oname="",Sname="";
            bool cm;
            OpenFileDialog at = new OpenFileDialog();
            if(at.ShowDialog()==DialogResult.OK)
            {
                Oname = at.FileName;
            }
            SaveFileDialog b = new SaveFileDialog();
            if (b.ShowDialog() == DialogResult.OK)
            {
                Sname = b.FileName;
            }
            string a = Prompt.ShowDialog("비밀번호를 입력해주세요   ", "");
            if (a != Prompt.ShowDialog("비밀번호를 다시 입력해주세요    ", ""))
            {
                MessageBox.Show("비밀번호가 다릅니다");
                return;
            }
            /*
            if (DialogResult.Yes==MessageBox.Show("압축을해서 크기를 줄일까요?(암호화 복호화시간이 더 증가)","묻는다", MessageBoxButtons.YesNo))
            {
                cm=true;
            }
            else
            {
                cm=false;
            }
            */
            if (Oname != "" && Sname != "")
            {
                
                t = new Thread(() => Encrypter.encrypt(Oname, a, Sname));
                t.Start();
                waitForThread();
            }
        }

        private void DecBtn_Click(object sender, EventArgs e)
        {
            string Oname = "", Sname = "";
            OpenFileDialog at = new OpenFileDialog();
            if (at.ShowDialog() == DialogResult.OK)
            {
                Oname = at.FileName;
            }
            SaveFileDialog b = new SaveFileDialog();
            if (b.ShowDialog() == DialogResult.OK)
            {
                Sname = b.FileName;
            }
            string a = Prompt.ShowDialog("비밀번호를 입력해주세요   ", "");
            /*
            if (DialogResult.Yes == MessageBox.Show("압축을해서 크기를 줄일까요?(암호화 복호화시간이 더 증가)", "묻는다", MessageBoxButtons.YesNo))
            {
                cm = true;
            }
            else
            {
                cm = false;
            }
            */
            if (Oname != "" && Sname != "")
            {
                t = new Thread(() => Decrypter.decrypt(Oname, a, Sname));
                t.Start();
                waitForThread();
            }
        }
        void waitForThread()
        {
            canClose = false;
            Bitmap bmp = Screenshot.TakeSnapshot(panel1);
            BitmapFilter.GaussianBlur(bmp, 1);
            PictureBox pb = new PictureBox();
            panel1.Controls.Add(pb);
            pb.Image = bmp;
            pb.Dock = DockStyle.Fill;
            pb.BringToFront();
            pictureBox1.BringToFront();
            pictureBox1.Visible = true;
            DecBtn.Enabled = false;
            EncBtn.Enabled = false;
            GifImage ima = new GifImage(Properties.Resources.onWork);
            while (t.IsAlive)
            {
                Application.DoEvents();
                pictureBox1.Image = ima.GetNextFrame();
            }
            pb.Hide();
            DecBtn.Enabled = true;
            EncBtn.Enabled = true;
            pictureBox1.Visible = false;
            canClose = true;
            MessageBox.Show("fin");
        }
        public string MD5HashFunc(string str)
        {
            StringBuilder MD5Str = new StringBuilder();
            byte[] byteArr = Encoding.ASCII.GetBytes(str);
            byte[] resultArr = (new MD5CryptoServiceProvider()).ComputeHash(byteArr);
            //for (int cnti = 1; cnti < resultArr.Length; cnti++) (2010.06.27)
            for (int cnti = 0; cnti < resultArr.Length; cnti++)
            {
                MD5Str.Append(resultArr[cnti].ToString("X2"));
            }
            return MD5Str.ToString();
        }
        private void compress(string filename,string svfn)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                 SevenZipCompressor.SetLibraryPath("7z.dll");
                 SevenZipCompressor compressor = new SevenZipCompressor();
                 compressor.CompressionLevel = SevenZip.CompressionLevel.Ultra;
                 compressor.CompressionMethod = CompressionMethod.Lzma;
                 compressor.CompressFiles(filename, svfn);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text, Width = 400 };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
