using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;

namespace KontrastIyilestirmeProgrami
{
    public partial class FormKontrastIyilestirme : Form
    {
        private GoruntuIslemleri goruntuIslemleri;
        private Bitmap resim;
        private double alfa, beta, gama, uygunluk, maxUygunluk_GA = 0, maxUygunluk_DGA = 0, maxUygunluk_BTA = 0, maxUygunluk_PSOA = 0, maxUygunluk_ARI = 0, maxUygunluk_YA = 0, maxUygunluk_KDA = 0, maxUygunluk_BBA = 0, maxUygunluk_KSOA = 0, maxUygunluk_ABA = 0;
        private bool islemAktifMi;
        private int SimdikiWidth = 1920, SimdikiHeight = 1080;

        public FormKontrastIyilestirme()
        {
            this.InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void FormKontrastIyilestirme_Load(object sender, EventArgs e)
        {
            System.Drawing.Rectangle ClientCozunurluk = new System.Drawing.Rectangle();
            ClientCozunurluk = Screen.GetBounds(ClientCozunurluk);
            float OranWidth = ((float)ClientCozunurluk.Width / (float)SimdikiWidth);
            float OranHeight = ((float)ClientCozunurluk.Height / (float)SimdikiHeight);
            this.Scale(OranWidth, OranHeight);
            this.goruntuIslemleri = new GoruntuIslemleri();
            this.pictureBoxResimX.Controls.Add(this.labelAciklamaX);
            this.labelAciklamaX.BackColor = Color.Transparent;
            this.labelAciklamaX.Location = new System.Drawing.Point(30, 310);
            this.labelAciklamaX.Text = "";
            this.labelAciklamaX.Visible = false;
            this.pictureBoxResimY.Controls.Add(this.labelAciklamaY);
            this.labelAciklamaY.BackColor = Color.Transparent;
            this.labelAciklamaY.Location = new System.Drawing.Point(30, 310);
            this.labelAciklamaY.Text = "";
            this.labelAciklamaY.Visible = false;
            this.pictureBoxResimZ.Controls.Add(this.labelAciklamaZ);
            this.labelAciklamaZ.BackColor = Color.Transparent;
            this.labelAciklamaZ.Location = new System.Drawing.Point(30, 310);
            this.labelAciklamaZ.Text = "";
            this.labelAciklamaZ.Visible = false;
        }

        private void pictureBoxResimX_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.islemAktifMi)
                return;
            if (e.Button == MouseButtons.Left)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.DefaultExt = "bmp";
                openFileDialog.Filter = "Resim Dosyaları|*.bmp;*.jpg;*.jpeg;*.png;*.tif";
                openFileDialog.Title = "Resim Yükle";
                openFileDialog.ShowDialog();
                if (openFileDialog.FileName == "")
                    return;
                Bitmap resimX = new Bitmap(Image.FromFile(openFileDialog.FileName));
                if ((this.pictureBoxResimY.Image != null) && ((resimX.Width != this.pictureBoxResimY.Image.Width) || (resimX.Height != this.pictureBoxResimY.Image.Height)))
                {
                    this.pictureBoxResimY.Image = null;
                    this.labelMeanYDeger.Text = "";
                    this.labelMedianYDeger.Text = "";
                    this.labelStdSapmaYDeger.Text = "";
                }
                this.resim = resimX;
                this.pictureBoxResimX.Image = null;
                this.pictureBoxResimX.SizeMode = PictureBoxSizeMode.CenterImage;
                this.pictureBoxResimX.Image = Properties.Resources.Animasyon;
                this.labelMeanXDeger.Text = "";
                this.labelMedianXDeger.Text = "";
                this.labelStdSapmaXDeger.Text = "";
                this.labelMSEDeger.Text = "";
                this.labelPSNRDeger.Text = "";
                this.labelSSIMDeger.Text = "";
                this.islemAktifMi = true;
                konsolBilgi.Text = "Resim yükleniyor.";
                new Thread(new ThreadStart(this.ResimXYukle)).Start();
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (this.pictureBoxResimX.Image == null)
                {
                    MessageBox.Show("Kaynak resim alanında resim yok.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = "bmp";
                saveFileDialog.Filter = "Resim Dosyaları|*.bmp;*.jpg;*.jpeg;*.png;*.tif";
                saveFileDialog.Title = "Resmi Kaydet";
                saveFileDialog.ShowDialog();
                if (saveFileDialog.FileName != "")
                    this.pictureBoxResimX.Image.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                this.pictureBoxResimX.Image = null;
                this.labelMeanXDeger.Text = "";
                this.labelMedianXDeger.Text = "";
                this.labelStdSapmaXDeger.Text = "";
                this.labelMSEDeger.Text = "";
                this.labelPSNRDeger.Text = "";
                this.labelSSIMDeger.Text = "";
            }
        }

        private void pictureBoxResimY_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.islemAktifMi)
                return;
            if (e.Button == MouseButtons.Left)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.DefaultExt = "bmp";
                openFileDialog.Filter = "Resim Dosyaları|*.bmp;*.jpg;*.jpeg;*.png;*.tif";
                openFileDialog.Title = "Resim Yükle";
                openFileDialog.ShowDialog();
                if (openFileDialog.FileName == "")
                    return;
                Bitmap resimY = new Bitmap(Image.FromFile(openFileDialog.FileName));
                if ((this.pictureBoxResimX.Image != null) && ((resimY.Width != this.pictureBoxResimX.Image.Width) || (resimY.Height != this.pictureBoxResimX.Image.Height)))
                {
                    this.pictureBoxResimX.Image = null;
                    this.labelMeanXDeger.Text = "";
                    this.labelMedianXDeger.Text = "";
                    this.labelStdSapmaXDeger.Text = "";
                }
                this.resim = resimY;
                this.pictureBoxResimY.Image = null;
                this.pictureBoxResimY.SizeMode = PictureBoxSizeMode.CenterImage;
                this.pictureBoxResimY.Image = Properties.Resources.Animasyon;
                this.labelMeanYDeger.Text = "";
                this.labelMedianYDeger.Text = "";
                this.labelStdSapmaYDeger.Text = "";
                this.labelMSEDeger.Text = "";
                this.labelPSNRDeger.Text = "";
                this.labelSSIMDeger.Text = "";
                this.islemAktifMi = true;
                new Thread(new ThreadStart(this.ResimYYukle)).Start();
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (this.pictureBoxResimY.Image == null)
                {
                    MessageBox.Show("Hedef resim alanında resim yok.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = "bmp";
                saveFileDialog.Filter = "Resim Dosyaları|*.bmp;*.jpg;*.jpeg;*.png;*.tif";
                saveFileDialog.Title = "Resmi Kaydet";
                saveFileDialog.ShowDialog();
                if (saveFileDialog.FileName != "")
                    this.pictureBoxResimY.Image.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                this.pictureBoxResimY.Image = null;
                this.labelMeanYDeger.Text = "";
                this.labelMedianYDeger.Text = "";
                this.labelStdSapmaYDeger.Text = "";
                this.labelMSEDeger.Text = "";
                this.labelPSNRDeger.Text = "";
                this.labelSSIMDeger.Text = "";
            }
        }

        private void pictureBoxResimZ_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.islemAktifMi)
                return;
            if (e.Button == MouseButtons.Left)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.DefaultExt = "bmp";
                openFileDialog.Filter = "Resim Dosyaları|*.bmp;*.jpg;*.jpeg;*.png;*.tif";
                openFileDialog.Title = "Resim Yükle";
                openFileDialog.ShowDialog();
                if (openFileDialog.FileName == "")
                    return;
                Bitmap resimZ = new Bitmap(Image.FromFile(openFileDialog.FileName));                
                if ((this.pictureBoxResimY.Image != null) && ((resimZ.Width != this.pictureBoxResimY.Image.Width) || (resimZ.Height != this.pictureBoxResimY.Image.Height)))
                {
                    this.pictureBoxResimY.Image = null;
                    this.labelMeanYDeger.Text = "";
                    this.labelMedianYDeger.Text = "";
                    this.labelStdSapmaYDeger.Text = "";
                }
                this.resim = resimZ;
                this.pictureBoxResimZ.Image = null;
                this.pictureBoxResimZ.SizeMode = PictureBoxSizeMode.CenterImage;
                this.pictureBoxResimZ.Image = Properties.Resources.Animasyon;
                this.labelMeanZDeger.Text = "";
                this.labelMedianZDeger.Text = "";
                this.labelStdSapmaZDeger.Text = "";
                this.labelMSEZDeger.Text = "";
                this.labelPSNRZDeger.Text = "";
                this.labelSSIMZDeger.Text = "";
                this.islemAktifMi = true;
                konsolBilgi.Text = "Resim yükleniyor.";
                new Thread(new ThreadStart(this.ResimZYukle)).Start();
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (this.pictureBoxResimZ.Image == null)
                {
                    MessageBox.Show("Kaynak resim alanında resim yok.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = "bmp";
                saveFileDialog.Filter = "Resim Dosyaları|*.bmp;*.jpg;*.jpeg;*.png;*.tif";
                saveFileDialog.Title = "Resmi Kaydet";
                saveFileDialog.ShowDialog();
                if (saveFileDialog.FileName != "")
                    this.pictureBoxResimZ.Image.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                this.pictureBoxResimZ.Image = null;
                this.labelMeanZDeger.Text = "";
                this.labelMedianZDeger.Text = "";
                this.labelStdSapmaZDeger.Text = "";
                this.labelMSEZDeger.Text = "";
                this.labelPSNRZDeger.Text = "";
                this.labelSSIMZDeger.Text = "";
            }
        }

        private void buttonResimYukle_Click(object sender, EventArgs e)
        {
            if (this.islemAktifMi)
                return;
            this.label2.Visible = false;
            this.timer1Label.Visible = false;
            this.label1.Visible = false;
            this.progressBar1.Visible = false;
            konsolBilgi.Text = "";
            progressBar1.Value = 0;
            this.pictureBoxResimX_MouseDoubleClick(new object(), new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0));
        }

        private void buttonKontrastIyilestir_Click(object sender, EventArgs e)
        {
            if (this.islemAktifMi)
                return;

            this.label1.Visible = false;
            this.progressBar1.Visible = false;
            konsolBilgi.Text = "";
            progressBar1.Value = 0;
            timer1Label.Text = "0";
            if (this.pictureBoxResimX.Image == null)
            {
                MessageBox.Show("Kaynak resim alanında resim yok.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (this.comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Çalışacak algoritmayı seçin.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            this.pictureBoxResimY.Image = null;
            this.pictureBoxResimY.SizeMode = PictureBoxSizeMode.CenterImage;
            this.pictureBoxResimY.Image = Properties.Resources.Animasyon;
            this.labelMeanYDeger.Text = "";
            this.labelMedianYDeger.Text = "";
            this.labelStdSapmaYDeger.Text = "";
            this.labelMSEDeger.Text = "";
            this.labelPSNRDeger.Text = "";
            this.labelSSIMDeger.Text = "";
            this.labelMSEZDeger.Text = "";
            this.labelPSNRZDeger.Text = "";
            this.labelSSIMZDeger.Text = "";
            this.islemAktifMi = true;
            konsolBilgi.Text = "Kontrast iyileştiriliyor.";
            timer1Label.Text = "";
            new Thread(new ThreadStart(this.KontrastIyilestir)).Start();
            this.label2.Visible = true;
            this.timer1Label.Visible = true;
        }

        private void buttonParametreleriUygula_Click(object sender, EventArgs e)
        {
            if (this.islemAktifMi)
                return;
            if (this.pictureBoxResimX.Image == null)
            {
                MessageBox.Show("Kaynak resim alanında resim yok.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            this.alfa = (double)this.numericUpDownAlfaDeger.Value;
            this.beta = (double)this.numericUpDownBetaDeger.Value;
            this.gama = (double)this.numericUpDownGamaDeger.Value;
            this.pictureBoxResimY.Image = null;
            this.pictureBoxResimY.SizeMode = PictureBoxSizeMode.CenterImage;
            this.pictureBoxResimY.Image = Properties.Resources.Animasyon;
            this.labelMeanYDeger.Text = "";
            this.labelMedianYDeger.Text = "";
            this.labelStdSapmaYDeger.Text = "";
            this.labelMSEDeger.Text = "";
            this.labelPSNRDeger.Text = "";
            this.labelSSIMDeger.Text = "";
            this.islemAktifMi = true;
            new Thread(new ThreadStart(this.ParametreleriUygula)).Start();
        }

        private void buttonResmiKaydet_Click(object sender, EventArgs e)
        {
            if (this.islemAktifMi)
                return;

            this.label2.Visible = false;
            this.timer1Label.Visible = false;
            this.label1.Visible = false;
            this.progressBar1.Visible = false;
            progressBar1.Value = 0;
            this.pictureBoxResimY_MouseDoubleClick(new object(), new MouseEventArgs(MouseButtons.Right, 2, 0, 0, 0));
        }

        private double DegerlendirmeFonksiyonu(double[] parametreler)
        {
            return this.goruntuIslemleri.ParametreleriDegerlendir(parametreler[0], parametreler[1], parametreler[2]);
        }

        private void ResimXYukle()
        {
            this.labelAciklamaX.Visible = true;
            this.labelAciklamaX.Text = "Resim Bilgileri Hesaplanıyor";
            this.goruntuIslemleri.ResimX = this.resim;
            if (this.pictureBoxResimY.Image == null)
            {
                this.labelAciklamaX.Text = "";
                this.labelAciklamaX.Visible = false;
                this.pictureBoxResimX.Image = null;
                this.pictureBoxResimX.SizeMode = PictureBoxSizeMode.Zoom;
                this.pictureBoxResimX.Image = this.resim;
                this.labelMeanXDeger.Text = this.goruntuIslemleri.MeanX.ToString("f2");
                this.labelMedianXDeger.Text = this.goruntuIslemleri.MedianX.ToString();
                this.labelStdSapmaXDeger.Text = this.goruntuIslemleri.StdSapmaX.ToString("f2");
                this.islemAktifMi = false;
                this.konsolBilgi.Text = "Resim başarıyla yüklendi.";
                return;
            }
            this.labelAciklamaX.Text = "Resimler Karşılaştırılıyor";
            this.goruntuIslemleri.ResimleriKarsilastir();
            this.labelAciklamaX.Text = "";
            this.labelAciklamaX.Visible = false;
            this.pictureBoxResimX.Image = null;
            this.pictureBoxResimX.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxResimX.Image = this.resim;
            this.labelMeanXDeger.Text = this.goruntuIslemleri.MeanX.ToString("f2");
            this.labelMedianXDeger.Text = this.goruntuIslemleri.MedianX.ToString();
            this.labelStdSapmaXDeger.Text = this.goruntuIslemleri.StdSapmaX.ToString("f2");
            this.labelMSEDeger.Text = this.goruntuIslemleri.MSE.ToString("f2");
            this.labelPSNRDeger.Text = this.goruntuIslemleri.PSNR.ToString("f2");
            this.labelSSIMDeger.Text = this.goruntuIslemleri.SSIM.ToString("f2");
            this.numericUpDownAlfaDeger.Value = 1;
            this.numericUpDownBetaDeger.Value = 0;
            this.numericUpDownGamaDeger.Value = 1;
            this.islemAktifMi = false;
            this.konsolBilgi.Text = "Resim başarıyla yüklendi.";
        }

        private void ResimZYukle()
        {
            this.labelAciklamaZ.Visible = true;
            this.labelAciklamaZ.Text = "Resim Bilgileri Hesaplanıyor";
            this.goruntuIslemleri.ResimZ = this.resim;
            if (this.pictureBoxResimY.Image == null)
            {
                this.labelAciklamaZ.Text = "";
                this.labelAciklamaZ.Visible = false;
                this.pictureBoxResimZ.Image = null;
                this.pictureBoxResimZ.SizeMode = PictureBoxSizeMode.Zoom;
                this.pictureBoxResimZ.Image = this.resim;
                this.labelMeanZDeger.Text = this.goruntuIslemleri.MeanZ.ToString("f2");
                this.labelMedianZDeger.Text = this.goruntuIslemleri.MedianZ.ToString();
                this.labelStdSapmaZDeger.Text = this.goruntuIslemleri.StdSapmaZ.ToString("f2");
                this.islemAktifMi = false;
                this.konsolBilgi.Text = "Resim başarıyla yüklendi.";
                return;
            }
            this.labelAciklamaZ.Text = "Resimler Karşılaştırılıyor";
            this.goruntuIslemleri.ResimleriKarsilastir2();
            this.labelAciklamaZ.Text = "";
            this.labelAciklamaZ.Visible = false;
            this.pictureBoxResimZ.Image = null;
            this.pictureBoxResimZ.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxResimZ.Image = this.resim;
            this.labelMeanZDeger.Text = this.goruntuIslemleri.MeanZ.ToString("f2");
            this.labelMedianZDeger.Text = this.goruntuIslemleri.MedianZ.ToString();
            this.labelStdSapmaZDeger.Text = this.goruntuIslemleri.StdSapmaZ.ToString("f2");
            this.labelMSEZDeger.Text = this.goruntuIslemleri.MSE.ToString("f2");
            this.labelPSNRZDeger.Text = this.goruntuIslemleri.PSNR.ToString("f2");
            this.labelSSIMZDeger.Text = this.goruntuIslemleri.SSIM.ToString("f2");
            this.numericUpDownAlfaDeger.Value = 1;
            this.numericUpDownBetaDeger.Value = 0;
            this.numericUpDownGamaDeger.Value = 1;
            this.islemAktifMi = false;
            konsolBilgi.Text = "Resim başarıyla yüklendi.";
        }

        private void ResimYYukle()
        {
            this.labelAciklamaY.Visible = true;
            this.labelAciklamaY.Text = "Resim Bilgileri Hesaplanıyor";
            this.goruntuIslemleri.ResimY = this.resim;
            if (this.pictureBoxResimX.Image == null)
            {
                this.labelAciklamaY.Text = "";
                this.labelAciklamaY.Visible = false;
                this.pictureBoxResimY.Image = null;
                this.pictureBoxResimY.SizeMode = PictureBoxSizeMode.Zoom;
                this.pictureBoxResimY.Image = this.resim;
                this.labelMeanYDeger.Text = this.goruntuIslemleri.MeanY.ToString("f2");
                this.labelMedianYDeger.Text = this.goruntuIslemleri.MedianY.ToString();
                this.labelStdSapmaYDeger.Text = this.goruntuIslemleri.StdSapmaY.ToString("f2");
                this.islemAktifMi = false;
                this.konsolBilgi.Text = "Resim başarıyla yüklendi.";
                return;
            }
            this.labelAciklamaY.Text = "Resimler Karşılaştırılıyor";
            this.goruntuIslemleri.ResimleriKarsilastir();
            this.labelAciklamaY.Text = "";
            this.labelAciklamaY.Visible = false;
            this.pictureBoxResimY.Image = null;
            this.pictureBoxResimY.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxResimY.Image = this.resim;
            this.labelMeanYDeger.Text = this.goruntuIslemleri.MeanY.ToString("f2");
            this.labelMedianYDeger.Text = this.goruntuIslemleri.MedianY.ToString();
            this.labelStdSapmaYDeger.Text = this.goruntuIslemleri.StdSapmaY.ToString("f2");
            this.labelMSEDeger.Text = this.goruntuIslemleri.MSE.ToString("f2");
            this.labelPSNRDeger.Text = this.goruntuIslemleri.PSNR.ToString("f2");
            this.labelSSIMDeger.Text = this.goruntuIslemleri.SSIM.ToString("f2");
            this.labelMeanZDeger.Text = this.goruntuIslemleri.MeanZ.ToString("f2");
            this.labelMedianZDeger.Text = this.goruntuIslemleri.MedianZ.ToString();
            this.labelStdSapmaZDeger.Text = this.goruntuIslemleri.StdSapmaZ.ToString("f2");
            this.numericUpDownAlfaDeger.Value = 1;
            this.numericUpDownBetaDeger.Value = 0;
            this.numericUpDownGamaDeger.Value = 1;
            this.islemAktifMi = false;
            konsolBilgi.Text = "Resim başarıyla yüklendi.";
        }

        private void KontrastIyilestir()
        {
            this.labelAciklamaY.Visible = true;
            this.labelAciklamaY.Text = "Parametreler Aranıyor";
            Double[] degerler;
            DateTime startTime = DateTime.Now;
            TimeSpan elapsedTime;
            double sure;
            //double alfa = 9.9, beta = 49.9, gama = 9.9;

            if (comboBox1.SelectedIndex == 0)
            {                
                GenetikAlgoritma genetikAlgoritma = new GenetikAlgoritma();
                genetikAlgoritma.DegerlendirmeFonksiyonu = this.DegerlendirmeFonksiyonu;
                startTime = DateTime.Now;
                degerler = genetikAlgoritma.IterasyonBaslat();
                elapsedTime = DateTime.Now - startTime;
                alfa = Math.Round(degerler[0], 1);
                beta = Math.Round(degerler[1], 0);
                gama = Math.Round(degerler[2], 1);
                sure = Math.Round(elapsedTime.TotalSeconds, 2);
                this.label2.Visible = true;
                this.timer1Label.Visible = true;
                timer1Label.Text = sure.ToString()+" saniye";
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                ParcacikSuruOptAlgoritmasi parcacikSuruOptAlgoritmasi = new ParcacikSuruOptAlgoritmasi();
                parcacikSuruOptAlgoritmasi.DegerlendirmeFonksiyonu = this.DegerlendirmeFonksiyonu;
                startTime = DateTime.Now;
                degerler = parcacikSuruOptAlgoritmasi.AlgoritmaCalistir();
                elapsedTime = DateTime.Now - startTime;
                alfa = Math.Round(degerler[0], 1);
                beta = Math.Round(degerler[1], 0);
                gama = Math.Round(degerler[2], 1);
                sure = Math.Round(elapsedTime.TotalSeconds, 2);
                this.label2.Visible = true;
                this.timer1Label.Visible = true;
                timer1Label.Text = sure.ToString() + " saniye";
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                BenzetimliTavlamaAlgoritmasi benzetimliTavlamaAlgoritmasi = new BenzetimliTavlamaAlgoritmasi();
                benzetimliTavlamaAlgoritmasi.DegerlendirmeFonksiyonu = this.DegerlendirmeFonksiyonu;
                startTime = DateTime.Now;
                degerler = benzetimliTavlamaAlgoritmasi.AlgoritmaCalistir();
                elapsedTime = DateTime.Now - startTime;
                alfa = Math.Round(degerler[0], 1);
                beta = Math.Round(degerler[1], 0);
                gama = Math.Round(degerler[2], 1);
                sure = Math.Round(elapsedTime.TotalSeconds, 2);
                this.label2.Visible = true;
                this.timer1Label.Visible = true;
                timer1Label.Text = sure.ToString() + " saniye";

            }
            else if (comboBox1.SelectedIndex == 3)
            {
                DiferansiyelGelisimAlgoritmasi diferansiyelGelisimAlgoritmasi = new DiferansiyelGelisimAlgoritmasi();
                diferansiyelGelisimAlgoritmasi.DegerlendirmeFonksiyonu = this.DegerlendirmeFonksiyonu;
                startTime = DateTime.Now;
                degerler = diferansiyelGelisimAlgoritmasi.AlgoritmaCalistir();
                elapsedTime = DateTime.Now - startTime;
                alfa = Math.Round(degerler[0], 1);
                beta = Math.Round(degerler[1], 0);
                gama = Math.Round(degerler[2], 1);
                sure = Math.Round(elapsedTime.TotalSeconds, 2);
                this.label2.Visible = true;
                this.timer1Label.Visible = true;
                timer1Label.Text = sure.ToString() + " saniye";
            }
            else if (comboBox1.SelectedIndex == 4)
            {
                YapayAriKoloniAlgoritmasi yapayAriKoloniAlgoritmasi = new YapayAriKoloniAlgoritmasi();
                yapayAriKoloniAlgoritmasi.EvaluationFonksiyonu = this.DegerlendirmeFonksiyonu;
                startTime = DateTime.Now;
                degerler = yapayAriKoloniAlgoritmasi.IterasyonBaslat(1000);
                elapsedTime = DateTime.Now - startTime;
                alfa = Math.Round(degerler[0], 1);
                beta = Math.Round(degerler[1], 0);
                gama = Math.Round(degerler[2], 1);
                sure = Math.Round(elapsedTime.TotalSeconds, 2);
                this.label2.Visible = true;
                this.timer1Label.Visible = true;
                timer1Label.Text = sure.ToString() + " saniye";
            }
            else if (comboBox1.SelectedIndex == 5)
            {
                YusufcukAlgoritmasi yusufcukAlgoritmasi = new YusufcukAlgoritmasi();
                yusufcukAlgoritmasi.DegerlendirmeFonksiyonu = this.DegerlendirmeFonksiyonu;                
                degerler = yusufcukAlgoritmasi.AlgoritmaCalistir();
                alfa = Math.Round(degerler[0], 1);
                beta = Math.Round(degerler[1], 0);
                gama = Math.Round(degerler[2], 1);
                sure = Math.Round(degerler[3] / 1000, 2);
                this.label2.Visible = true;
                this.timer1Label.Visible = true;
                timer1Label.Text = sure.ToString() + " saniye";
            }
            else if (comboBox1.SelectedIndex == 6)
            {
                KaraDelikAlgoritmasi karaDelikAlgoritmasi = new KaraDelikAlgoritmasi();
                karaDelikAlgoritmasi.DegerlendirmeFonksiyonu = this.DegerlendirmeFonksiyonu;
                degerler = karaDelikAlgoritmasi.AlgoritmaCalistir();
                alfa = Math.Round(degerler[0], 1);
                beta = Math.Round(degerler[1], 0);
                gama = Math.Round(degerler[2], 1);
                sure = Math.Round(degerler[3] / 1000, 2);
                timer1Label.Text = sure.ToString() + " saniye";
            }
            else if (comboBox1.SelectedIndex == 7)
            {
                BakteriyelBesinArama bakteriyelBesinArama = new BakteriyelBesinArama(this.goruntuIslemleri);
                bakteriyelBesinArama.DegerlendirmeFonksiyonu = this.DegerlendirmeFonksiyonu;
                startTime = DateTime.Now;
                degerler = bakteriyelBesinArama.aramaBaslat(30);
                elapsedTime = DateTime.Now - startTime;
                alfa = Math.Round(degerler[0], 1);
                beta = Math.Round(degerler[1], 0);
                gama = Math.Round(degerler[2], 1);
                sure = Math.Round(elapsedTime.TotalSeconds, 2);
                this.label2.Visible = true;
                this.timer1Label.Visible = true;
                timer1Label.Text = sure.ToString() + " saniye";
            }
            else if (comboBox1.SelectedIndex == 8)
            {
                KediSurusuOptimizasyonu kediSurusuOptimizasyonu = new KediSurusuOptimizasyonu(this.goruntuIslemleri);
                kediSurusuOptimizasyonu.DegerlendirmeFonksiyonu = this.DegerlendirmeFonksiyonu;
                startTime = DateTime.Now;
                degerler = kediSurusuOptimizasyonu.aramaBaslat(5);
                elapsedTime = DateTime.Now - startTime;
                alfa = Math.Round(degerler[0], 1);
                beta = Math.Round(degerler[1], 0);
                gama = Math.Round(degerler[2], 1);
                sure = Math.Round(elapsedTime.TotalSeconds, 2);
                this.label2.Visible = true;
                this.timer1Label.Visible = true;
                timer1Label.Text = sure.ToString() + " saniye";
            }
            else if (comboBox1.SelectedIndex == 9)
            {
                AtesBocegiAlgoritması atesBocegiAlgoritmasi = new AtesBocegiAlgoritması();
                atesBocegiAlgoritmasi.DegerlendirmeFonksiyonu = this.DegerlendirmeFonksiyonu;
                degerler = atesBocegiAlgoritmasi.AlgoritmaCalistir();
                alfa = Math.Round(degerler[0], 1);
                beta = Math.Round(degerler[1], 0);
                gama = Math.Round(degerler[2], 1);
                sure = Math.Round(degerler[3] / 1000, 2);
                this.label2.Visible = true;
                this.timer1Label.Visible = true;
                timer1Label.Text = sure.ToString() + " saniye";
            }
            Console.WriteLine("alfa: " + alfa + " beta: " + beta + " gama: " + gama);

            Bitmap resimY = this.goruntuIslemleri.ParametreleriUygula(alfa, beta, gama);

            this.labelAciklamaY.Text = "Resim Bilgileri Hesaplanıyor";
            this.goruntuIslemleri.ResimY = resimY;
            this.labelAciklamaY.Text = "Resimler Karşılaştırılıyor";
            this.goruntuIslemleri.ResimleriKarsilastir();
            this.goruntuIslemleri.ResimleriKarsilastir2();
            this.labelAciklamaY.Text = "";
            this.labelAciklamaY.Visible = false;
            this.pictureBoxResimY.Image = null;
            this.pictureBoxResimY.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxResimY.Image = resimY;
            this.labelMeanYDeger.Text = this.goruntuIslemleri.MeanY.ToString("f2");
            this.labelMedianYDeger.Text = this.goruntuIslemleri.MedianY.ToString();
            this.labelStdSapmaYDeger.Text = this.goruntuIslemleri.StdSapmaY.ToString("f2");
            this.labelMSEDeger.Text = this.goruntuIslemleri.MSE.ToString("f2");
            this.labelPSNRDeger.Text = this.goruntuIslemleri.PSNR.ToString("f2");
            this.labelSSIMDeger.Text = this.goruntuIslemleri.SSIM.ToString("f2");
            this.labelMSEZDeger.Text = this.goruntuIslemleri.MSEZ.ToString("f2");
            this.labelPSNRZDeger.Text = this.goruntuIslemleri.PSNRZ.ToString("f2");
            this.labelSSIMZDeger.Text = this.goruntuIslemleri.SSIMZ.ToString("f2");
            this.numericUpDownAlfaDeger.Value = (decimal)alfa;
            this.numericUpDownBetaDeger.Value = (decimal)beta;
            this.numericUpDownGamaDeger.Value = (decimal)gama;
            konsolBilgi.Text = "Kontrast iyileştirme işlemi tamamlandı.";
            MessageBox.Show("Kontrast iyileştirme işlemi tamamlandı.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);            
            this.islemAktifMi = false;
        }

        private void ParametreleriUygula()
        {
            this.labelAciklamaY.Visible = true;
            this.labelAciklamaY.Text = "Parametreler Uygulanıyor";
            Bitmap resimY = this.goruntuIslemleri.ParametreleriUygula(this.alfa, this.beta, this.gama);
            this.labelAciklamaY.Text = "Resim Bilgileri Hesaplanıyor";
            this.goruntuIslemleri.ResimY = resimY;
            this.labelAciklamaY.Text = "Resimler Karşılaştırılıyor";
            this.goruntuIslemleri.ResimleriKarsilastir();
            this.goruntuIslemleri.ResimleriKarsilastir2();
            this.labelAciklamaY.Text = "";
            this.labelAciklamaY.Visible = false;
            this.pictureBoxResimY.Image = null;
            this.pictureBoxResimY.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxResimY.Image = resimY;
            this.labelMeanYDeger.Text = this.goruntuIslemleri.MeanY.ToString("f2");
            this.labelMedianYDeger.Text = this.goruntuIslemleri.MedianY.ToString();
            this.labelStdSapmaYDeger.Text = this.goruntuIslemleri.StdSapmaY.ToString("f2");
            this.labelMSEDeger.Text = this.goruntuIslemleri.MSE.ToString("f2");
            this.labelPSNRDeger.Text = this.goruntuIslemleri.PSNR.ToString("f2");
            this.labelSSIMDeger.Text = this.goruntuIslemleri.SSIM.ToString("f2");
            this.labelMSEZDeger.Text = this.goruntuIslemleri.MSEZ.ToString("f2");
            this.labelPSNRZDeger.Text = this.goruntuIslemleri.PSNRZ.ToString("f2");
            this.labelSSIMZDeger.Text = this.goruntuIslemleri.SSIMZ.ToString("f2");
            MessageBox.Show("İşlem tamamlandı.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            konsolBilgi.Text = "Parametreler uygulandı.";
            this.islemAktifMi = false;
        }

        private void labelBilgi_MouseEnter(object sender, EventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            label.ForeColor = Color.Lime;
            if (label.Name == "labelMeanXBilgi")
            {
                labelBilgiBaslik.Text = "Mean";
                labelBilgiMesaj.Text = "Orjinal görüntünün ortalama renk tonunu gösterir.";
                pictureBoxBilgiFormul.Image = Properties.Resources.Mean;
                pictureBoxBilgiFormul.SizeMode = PictureBoxSizeMode.Zoom;
                panelBilgi.Size = new Size(groupBoxResimXBilgileri.Width, pictureBoxBilgiFormul.Top + pictureBoxBilgiFormul.Height + 2);
                panelBilgi.Location = new System.Drawing.Point(groupBoxResimXBilgileri.Left, groupBoxResimXBilgileri.Top - panelBilgi.Height + 5);
            }
            else if (label.Name == "labelMedianXBilgi")
            {
                labelBilgiBaslik.Text = "Median";
                labelBilgiMesaj.Text = "Orjinal görüntünün ortanca renk tonunu gösterir.";
                pictureBoxBilgiFormul.Image = null;
                panelBilgi.Size = new Size(groupBoxResimXBilgileri.Width, labelBilgiMesaj.Top + labelBilgiMesaj.Height + 2);
                panelBilgi.Location = new System.Drawing.Point(groupBoxResimXBilgileri.Left, groupBoxResimXBilgileri.Top - panelBilgi.Height + 5);
            }
            else if (label.Name == "labelStdSapmaXBilgi")
            {
                labelBilgiBaslik.Text = "Standart Sapma";
                labelBilgiMesaj.Text = "Orjinal görüntünün ortalama renk dağılımını verir.";
                pictureBoxBilgiFormul.Image = Properties.Resources.SS;
                pictureBoxBilgiFormul.SizeMode = PictureBoxSizeMode.Zoom;
                panelBilgi.Size = new Size(groupBoxResimXBilgileri.Width, pictureBoxBilgiFormul.Top + pictureBoxBilgiFormul.Height + 2);
                panelBilgi.Location = new System.Drawing.Point(groupBoxResimXBilgileri.Left, groupBoxResimXBilgileri.Top - panelBilgi.Height + 5);
            }
            else if (label.Name == "labelMeanYBilgi")
            {
                labelBilgiBaslik.Text = "Mean";
                labelBilgiMesaj.Text = "İyileştirilmiş görüntünün ortalama renk tonunu gösterir.";
                pictureBoxBilgiFormul.Image = Properties.Resources.Mean;
                pictureBoxBilgiFormul.SizeMode = PictureBoxSizeMode.Zoom;
                panelBilgi.Size = new Size(groupBoxResimYBilgileri.Width, pictureBoxBilgiFormul.Top + pictureBoxBilgiFormul.Height + 2);
                panelBilgi.Location = new System.Drawing.Point(groupBoxResimYBilgileri.Left, groupBoxResimYBilgileri.Top - panelBilgi.Height + 5);
            }
            else if (label.Name == "labelMedianYBilgi")
            {
                labelBilgiBaslik.Text = "Median";
                labelBilgiMesaj.Text = "İyileştirilmiş görüntünün ortanca renk tonunu gösterir.";
                pictureBoxBilgiFormul.Image = null;
                panelBilgi.Size = new Size(groupBoxResimYBilgileri.Width, labelBilgiMesaj.Top + labelBilgiMesaj.Height + 2);
                panelBilgi.Location = new System.Drawing.Point(groupBoxResimYBilgileri.Left, groupBoxResimYBilgileri.Top - panelBilgi.Height + 5);
            }
            else if (label.Name == "labelStdSapmaYBilgi")
            {
                labelBilgiBaslik.Text = "Standart Sapma";
                labelBilgiMesaj.Text = "İyileştirilmiş görüntünün ortalama renk dağılımını verir.";
                pictureBoxBilgiFormul.Image = Properties.Resources.SS;
                pictureBoxBilgiFormul.SizeMode = PictureBoxSizeMode.Zoom;
                panelBilgi.Size = new Size(groupBoxResimYBilgileri.Width, pictureBoxBilgiFormul.Top + pictureBoxBilgiFormul.Height + 2);
                panelBilgi.Location = new System.Drawing.Point(groupBoxResimYBilgileri.Left, groupBoxResimYBilgileri.Top - panelBilgi.Height + 5);
            }
            else if (label.Name == "labelMSEBilgi")
            {
                labelBilgiBaslik.Text = "MSE (Mean Square Error)";
                labelBilgiMesaj.Text = "Orjinal görüntü ile iyileştirilmiş görüntünün piksel değerlerinin farklarının karesinin toplamının toplam piksel sayısına bölümüdür.";
                pictureBoxBilgiFormul.Image = Properties.Resources.MSE;
                pictureBoxBilgiFormul.SizeMode = PictureBoxSizeMode.Zoom;
                panelBilgi.Size = new Size(groupBoxResimBilgileri.Width, pictureBoxBilgiFormul.Top + pictureBoxBilgiFormul.Height + 2);
                panelBilgi.Location = new System.Drawing.Point(groupBoxResimBilgileri.Left, groupBoxResimBilgileri.Top - panelBilgi.Height + 5);
            }
            else if (label.Name == "labelPSNRBilgi")
            {
                labelBilgiBaslik.Text = "PSNR (Peak Signal to Noise Ratio)";
                labelBilgiMesaj.Text = "Orjinal görüntü ile iyileştirilmiş görüntünün maksimum renk tonunun karesinin MSE'ye oranının logaritmasının 10 katıdır.";
                pictureBoxBilgiFormul.Image = Properties.Resources.PSNR;
                pictureBoxBilgiFormul.SizeMode = PictureBoxSizeMode.Zoom;
                panelBilgi.Size = new Size(groupBoxResimBilgileri.Width, pictureBoxBilgiFormul.Top + pictureBoxBilgiFormul.Height + 2);
                panelBilgi.Location = new System.Drawing.Point(groupBoxResimBilgileri.Left, groupBoxResimBilgileri.Top - panelBilgi.Height + 5);
            }
            else if (label.Name == "labelSSIMBilgi")
            {
                labelBilgiBaslik.Text = "SSIM (Structural Similarity)";
                labelBilgiMesaj.Text = "Orjinal görüntü ile iyileştirilmiş görüntünün aritmetik ortalaması (µx, µy), varyansı (σx^2, σy^2) ve kovaryansı (σxy) kullanılarak hesaplanır.";
                pictureBoxBilgiFormul.Image = Properties.Resources.SSIM;
                pictureBoxBilgiFormul.SizeMode = PictureBoxSizeMode.Zoom;
                panelBilgi.Size = new Size(groupBoxResimBilgileri.Width, pictureBoxBilgiFormul.Top + pictureBoxBilgiFormul.Height + 2);
                panelBilgi.Location = new System.Drawing.Point(groupBoxResimBilgileri.Left, groupBoxResimBilgileri.Top - panelBilgi.Height + 5);
            }
            else if (label.Name == "labelAlfaBilgi")
            {
                labelBilgiBaslik.Text = "Alfa";
                labelBilgiMesaj.Text = "Orjinal görüntünün histogramına kontrast işleminin hangi oranda uygulanacağını belirler.\r\n0.1 ile 10 arasında değer alır.\r\n1'in altında görüntüyü karartır, üstünde açar.";
                pictureBoxBilgiFormul.Image = null;
                panelBilgi.Size = new Size(groupBoxParametreBilgileri.Width, labelBilgiMesaj.Top + labelBilgiMesaj.Height + 2);
                panelBilgi.Location = new System.Drawing.Point(groupBoxParametreBilgileri.Left, groupBoxParametreBilgileri.Top - panelBilgi.Height + 5);
            }
            else if (label.Name == "labelBetaBilgi")
            {
                labelBilgiBaslik.Text = "Beta";
                labelBilgiMesaj.Text = "Orjinal görüntünün histogramına parlaklık işleminin hangi oranda uygulanacağını belirler.\r\n-50 ile 50 arasında değer alır.\r\n0'ın altında görüntüyü karartır, üstünde açar.";
                pictureBoxBilgiFormul.Image = null;
                panelBilgi.Size = new Size(groupBoxParametreBilgileri.Width, labelBilgiMesaj.Top + labelBilgiMesaj.Height + 2);
                panelBilgi.Location = new System.Drawing.Point(groupBoxParametreBilgileri.Left, groupBoxParametreBilgileri.Top - panelBilgi.Height + 5);
            }
            else if (label.Name == "labelGamaBilgi")
            {
                labelBilgiBaslik.Text = "Gama";
                labelBilgiMesaj.Text = "Orjinal görüntünün histogramına gama işleminin hangi oranda uygulanacağını belirler.\r\n0.1 ile 10 arasında değer alır.\r\n1'in altında görüntüyü karartır, üstünde açar.";
                pictureBoxBilgiFormul.Image = null;
                panelBilgi.Size = new Size(groupBoxParametreBilgileri.Width, labelBilgiMesaj.Top + labelBilgiMesaj.Height + 2);
                panelBilgi.Location = new System.Drawing.Point(groupBoxParametreBilgileri.Left, groupBoxParametreBilgileri.Top - panelBilgi.Height + 5);
            }
            else if(label.Name== "labelMeanZBilgi")
            {
                labelBilgiBaslik.Text = "Mean";
                labelBilgiMesaj.Text = "Orjinal görüntünün ortalama renk tonunu gösterir.";
                pictureBoxBilgiFormul.Image = Properties.Resources.Mean;
                pictureBoxBilgiFormul.SizeMode = PictureBoxSizeMode.Zoom;
                panelBilgi.Size = new Size(groupBoxResimZBilgileri.Width, pictureBoxBilgiFormul.Top + pictureBoxBilgiFormul.Height + 2);
                panelBilgi.Location = new System.Drawing.Point(groupBoxResimZBilgileri.Left, groupBoxResimZBilgileri.Top - panelBilgi.Height + 5);
            }
            else if (label.Name == "labelMedianZBilgi")
            {
                labelBilgiBaslik.Text = "Median";
                labelBilgiMesaj.Text = "Orjinal görüntünün ortanca renk tonunu gösterir.";
                pictureBoxBilgiFormul.Image = null;
                panelBilgi.Size = new Size(groupBoxResimZBilgileri.Width, labelBilgiMesaj.Top + labelBilgiMesaj.Height + 2);
                panelBilgi.Location = new System.Drawing.Point(groupBoxResimZBilgileri.Left, groupBoxResimZBilgileri.Top - panelBilgi.Height + 5);
            }
            else if (label.Name == "labelStdSapmaZBilgi")
            {
                labelBilgiBaslik.Text = "Standart Sapma";
                labelBilgiMesaj.Text = "Orjinal görüntünün ortalama renk dağılımını verir.";
                pictureBoxBilgiFormul.Image = Properties.Resources.SS;
                pictureBoxBilgiFormul.SizeMode = PictureBoxSizeMode.Zoom;
                panelBilgi.Size = new Size(groupBoxResimZBilgileri.Width, pictureBoxBilgiFormul.Top + pictureBoxBilgiFormul.Height + 2);
                panelBilgi.Location = new System.Drawing.Point(groupBoxResimZBilgileri.Left, groupBoxResimZBilgileri.Top - panelBilgi.Height + 5);
            }
            else if (label.Name == "labelMSEZBilgi")
            {
                labelBilgiBaslik.Text = "MSE (Mean Square Error)";
                labelBilgiMesaj.Text = "Orjinal görüntü ile iyileştirilmiş görüntünün piksel değerlerinin farklarının karesinin toplamının toplam piksel sayısına bölümüdür.";
                pictureBoxBilgiFormul.Image = Properties.Resources.MSE;
                pictureBoxBilgiFormul.SizeMode = PictureBoxSizeMode.Zoom;
                panelBilgi.Size = new Size(groupBoxResimBilgileri2.Width, pictureBoxBilgiFormul.Top + pictureBoxBilgiFormul.Height + 2);
                panelBilgi.Location = new System.Drawing.Point(groupBoxResimBilgileri2.Left, groupBoxResimBilgileri2.Top - panelBilgi.Height + 5);
            }
            else if (label.Name == "labelPSNRZBilgi")
            {
                labelBilgiBaslik.Text = "PSNR (Peak Signal to Noise Ratio)";
                labelBilgiMesaj.Text = "Orjinal görüntü ile iyileştirilmiş görüntünün maksimum renk tonunun karesinin MSE'ye oranının logaritmasının 10 katıdır.";
                pictureBoxBilgiFormul.Image = Properties.Resources.PSNR;
                pictureBoxBilgiFormul.SizeMode = PictureBoxSizeMode.Zoom;
                panelBilgi.Size = new Size(groupBoxResimBilgileri2.Width, pictureBoxBilgiFormul.Top + pictureBoxBilgiFormul.Height + 2);
                panelBilgi.Location = new System.Drawing.Point(groupBoxResimBilgileri2.Left, groupBoxResimBilgileri2.Top - panelBilgi.Height + 5);
            }
            else if (label.Name == "labelSSIMZBilgi")
            {
                labelBilgiBaslik.Text = "SSIM (Structural Similarity)";
                labelBilgiMesaj.Text = "Orjinal görüntü ile iyileştirilmiş görüntünün aritmetik ortalaması (µx, µy), varyansı (σx^2, σy^2) ve kovaryansı (σxy) kullanılarak hesaplanır.";
                pictureBoxBilgiFormul.Image = Properties.Resources.SSIM;
                pictureBoxBilgiFormul.SizeMode = PictureBoxSizeMode.Zoom;
                panelBilgi.Size = new Size(groupBoxResimBilgileri2.Width, pictureBoxBilgiFormul.Top + pictureBoxBilgiFormul.Height + 2);
                panelBilgi.Location = new System.Drawing.Point(groupBoxResimBilgileri2.Left, groupBoxResimBilgileri2.Top - panelBilgi.Height + 5);
            }
            panelBilgi.Visible = true;
        }

        private void buttonParametreleriUygula_Click_1(object sender, EventArgs e)
        {
            if (this.islemAktifMi)
                return;

            this.label2.Visible = false;
            this.timer1Label.Visible = false;
            this.label1.Visible = false;
            this.progressBar1.Visible = false;
            konsolBilgi.Text = "";
            progressBar1.Value = 0;
            if (this.pictureBoxResimX.Image == null)
            {
                MessageBox.Show("Kaynak resim alanında resim yok.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            this.alfa = (double)this.numericUpDownAlfaDeger.Value;
            this.beta = (double)this.numericUpDownBetaDeger.Value;
            this.gama = (double)this.numericUpDownGamaDeger.Value;
            this.pictureBoxResimY.Image = null;
            this.pictureBoxResimY.SizeMode = PictureBoxSizeMode.CenterImage;
            this.pictureBoxResimY.Image = Properties.Resources.Animasyon;
            this.labelMeanYDeger.Text = "";
            this.labelMedianYDeger.Text = "";
            this.labelStdSapmaYDeger.Text = "";
            this.labelMSEDeger.Text = "";
            this.labelPSNRDeger.Text = "";
            this.labelSSIMDeger.Text = "";
            this.labelMSEZDeger.Text = "";
            this.labelPSNRZDeger.Text = "";
            this.labelSSIMZDeger.Text = "";
            this.islemAktifMi = true;
            konsolBilgi.Text = "Parametreler uygulanıyor.";
            new Thread(new ThreadStart(this.ParametreleriUygula)).Start();
        }

        private void buttonAlgoritmalariDegerlendir_Click(object sender, EventArgs e)
        {
            if (this.islemAktifMi)
                return;
            konsolBilgi.Text = "Algoritma değerlendirme işlemi başlatılıyor.";
            progressBar1.Value = 0;

            this.label2.Visible = false;
            this.timer1Label.Visible = false;
            this.islemAktifMi = true;
            this.label1.Visible = true;
            this.progressBar1.Visible = true;
            new Thread(new ThreadStart(this.algoritmalariDegerlendir)).Start();
        }
        private void algoritmalariDegerlendir()
        {
            int resimSayisi = 1, bozuntuSayisi = 4, denemeSayisi = 1;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = resimSayisi * bozuntuSayisi * denemeSayisi * 10;//10 algoritma
            string isim;
            DateTime startTime;
            TimeSpan elapsedTime;
            Bitmap psoa_result, ga_result, dga_result, bta_result, ari_result, ya_result, kda_result, bba_result, ksoa_result, aba_result;
            Stopwatch saat = Stopwatch.StartNew();

            Excel.Application ExcelUygulama;
            Excel.Workbook ExcelProje;
            Excel.Worksheet ExcelSayfa;
            object Missing = System.Reflection.Missing.Value;
            Excel.Range ExcelRange;


            Excel.Application ExcelUygulama2;
            Excel.Workbook ExcelProje2;
            Excel.Worksheet ExcelSayfa2;
            object Missing2 = System.Reflection.Missing.Value;
            Excel.Range ExcelRange2;

            string s_dosyaadi = "Algoritmaların MSE , PSNR ve SSIM Değerleri",s_dosyaadi2="Algoritmaların Çalışma Süreleri";
            ExcelUygulama = new Excel.Application();
            ExcelProje = ExcelUygulama.Workbooks.Add(Missing);
            ExcelSayfa = (Excel.Worksheet)ExcelProje.Worksheets.get_Item(1);
            ExcelRange = ExcelSayfa.UsedRange;
            ExcelSayfa = (Excel.Worksheet)ExcelUygulama.ActiveSheet;
            ExcelUygulama.Visible = false;
            ExcelUygulama.AlertBeforeOverwriting = false;
            Excel.Range bolge = (Excel.Range)ExcelSayfa.Cells[1, 1];

            ExcelUygulama2 = new Excel.Application();
            ExcelProje2 = ExcelUygulama2.Workbooks.Add(Missing);
            ExcelSayfa2 = (Excel.Worksheet)ExcelProje2.Worksheets.get_Item(1);
            ExcelRange2 = ExcelSayfa2.UsedRange;
            ExcelSayfa2 = (Excel.Worksheet)ExcelUygulama2.ActiveSheet;
            ExcelUygulama2.Visible = false;
            ExcelUygulama2.AlertBeforeOverwriting = false;
            Excel.Range bolge2 = (Excel.Range)ExcelSayfa2.Cells[1, 1];

            bolge.Value2 = "Resim Adı";
            bolge2.Value2 = "Resim Adı";
            bolge2 = (Excel.Range)ExcelSayfa2.Cells[1, 2];
            bolge2.Value2 = "Genetik Algoritma";
            bolge2 = (Excel.Range)ExcelSayfa2.Cells[1, 3];
            bolge2.Value2 = "Diferansiyel Gelişim Algoritması";
            bolge2 = (Excel.Range)ExcelSayfa2.Cells[1, 4];
            bolge2.Value2 = "Benzetimli Tavlama Algoritması";
            bolge2 = (Excel.Range)ExcelSayfa2.Cells[1, 5];
            bolge2.Value2 = "Parçacık Sürü Optimizasyon Algoritması";
            bolge2 = (Excel.Range)ExcelSayfa2.Cells[1, 6];
            bolge2.Value2 = "Yapay Arı Kolonisi Algoritması";
            bolge2 = (Excel.Range)ExcelSayfa2.Cells[1, 7];
            bolge2.Value2 = "Yusufçuk Algoritması";
            bolge2 = (Excel.Range)ExcelSayfa2.Cells[1, 8];
            bolge2.Value2 = "Kara Delik Algoritması";
            bolge2 = (Excel.Range)ExcelSayfa2.Cells[1, 9];
            bolge2.Value2 = "Bakteriyel Besin Arama Algoritması";
            bolge2 = (Excel.Range)ExcelSayfa2.Cells[1, 10];
            bolge2.Value2 = "Kedi Sürüsü Optimizasyon Algoritması";
            bolge2 = (Excel.Range)ExcelSayfa2.Cells[1, 11];
            bolge2.Value2 = "Ateş Böceği Algoritması";
            string tmp;
            for (int i = 2; i <= 31; i++)
            {
                bolge = (Excel.Range)ExcelSayfa.Cells[1, i];
                if (i % 3 == 2)
                {
                    tmp = "MSE_";
                }
                else if (i % 3 == 0)
                {
                    tmp = "PSNR_";
                }
                else
                {
                    tmp = "SSIM_";
                }
                if (i < 5)
                {
                    tmp += "GA";
                }
                else if (i < 8)
                {
                    tmp += "DGA";
                }
                else if (i < 11)
                {
                    tmp += "BTA";
                }
                else if (i < 14)
                {
                    tmp += "PSOA";
                }
                else if (i < 17)
                {
                    tmp += "ARI";
                }
                else if(i < 20)
                {
                    tmp += "YA";
                }
                else if(i < 23)
                {
                    tmp += "KDA";
                }
                else if (i < 26)
                {
                    tmp += "BBA";
                }
                else if(i < 29)
                {
                    tmp += "KSOA";
                }
                else
                {
                    tmp += "ABA";
                }
                bolge.Value2 = tmp;
            }

            double GA_MSE_ORT = 0, GA_PSNR_ORT = 0, GA_SSIM_ORT = 0, DGA_MSE_ORT = 0, DGA_PSNR_ORT = 0, DGA_SSIM_ORT = 0, BTA_MSE_ORT = 0, BTA_PSNR_ORT = 0, BTA_SSIM_ORT = 0, PSOA_MSE_ORT = 0, PSOA_PSNR_ORT = 0, PSOA_SSIM_ORT = 0,
                   ARI_MSE_ORT = 0, ARI_PSNR_ORT = 0, ARI_SSIM_ORT = 0, YA_MSE_ORT = 0, YA_PSNR_ORT = 0, YA_SSIM_ORT = 0, KDA_MSE_ORT = 0, KDA_PSNR_ORT = 0, KDA_SSIM_ORT = 0, BBA_MSE_ORT = 0, BBA_PSNR_ORT = 0, BBA_SSIM_ORT = 0, KSOA_MSE_ORT = 0, KSOA_PSNR_ORT = 0, KSOA_SSIM_ORT = 0, ABA_MSE_ORT = 0, ABA_PSNR_ORT = 0, ABA_SSIM_ORT = 0;
            double GA_MSE_ORT2, GA_PSNR_ORT2, GA_SSIM_ORT2, DGA_MSE_ORT2, DGA_PSNR_ORT2, DGA_SSIM_ORT2, BTA_MSE_ORT2, BTA_PSNR_ORT2, BTA_SSIM_ORT2, PSOA_MSE_ORT2, PSOA_PSNR_ORT2, PSOA_SSIM_ORT2,
                   ARI_MSE_ORT2, ARI_PSNR_ORT2, ARI_SSIM_ORT2, YA_MSE_ORT2, YA_PSNR_ORT2, YA_SSIM_ORT2, KDA_MSE_ORT2, KDA_PSNR_ORT2, KDA_SSIM_ORT2, BBA_MSE_ORT2, BBA_PSNR_ORT2, BBA_SSIM_ORT2, KSOA_MSE_ORT2, KSOA_PSNR_ORT2, KSOA_SSIM_ORT2, ABA_MSE_ORT2, ABA_PSNR_ORT2, ABA_SSIM_ORT2;
            double GA_TIME = 0, DGA_TIME = 0, BTA_TIME = 0, PSOA_TIME = 0, ARI_TIME = 0, YA_TIME = 0, KDA_TIME = 0, BBA_TIME = 0, KSOA_TIME = 0, ABA_TIME = 0,
                   GA_TIME2, DGA_TIME2, BTA_TIME2, PSOA_TIME2, ARI_TIME2, YA_TIME2, KDA_TIME2, BBA_TIME2, KSOA_TIME2, ABA_TIME2;
            Bitmap enIyiResim_GA = null, enIyiResim_DGA = null, enIyiResim_BTA = null, enIyiResim_PSOA = null, enIyiResim_ARI = null, enIyiResim_YA = null, enIyiResim_KDA = null, enIyiResim_BBA = null, enIyiResim_KSOA = null, enIyiResim_ABA = null;
            for (int i = 1; i <= resimSayisi; i++)
            {
                this.orijinalResmiYukle(i);
                konsolBilgi.Text = i + ".resmin orijinal hali yüklendi.";
                for (int j = 1; j <= bozuntuSayisi; j++)
                {
                    GA_TIME2 = 0; GA_MSE_ORT2 = 0; GA_PSNR_ORT2 = 0; GA_SSIM_ORT2 = 0; maxUygunluk_GA = 0;
                    DGA_TIME2 = 0; DGA_MSE_ORT2 = 0; DGA_PSNR_ORT2 = 0; DGA_SSIM_ORT2 = 0; maxUygunluk_DGA = 0;
                    BTA_TIME2 = 0; BTA_MSE_ORT2 = 0; BTA_PSNR_ORT2 = 0; BTA_SSIM_ORT2 = 0; maxUygunluk_BTA = 0;
                    PSOA_TIME2 = 0; PSOA_MSE_ORT2 = 0; PSOA_PSNR_ORT2 = 0; PSOA_SSIM_ORT2 = 0; maxUygunluk_PSOA = 0;
                    ARI_TIME2 = 0; ARI_MSE_ORT2 = 0; ARI_PSNR_ORT2 = 0; ARI_SSIM_ORT2 = 0; maxUygunluk_ARI = 0;
                    YA_TIME2 = 0; YA_MSE_ORT2 = 0; YA_PSNR_ORT2 = 0; YA_SSIM_ORT2 = 0; maxUygunluk_YA = 0;
                    KDA_TIME2 = 0; KDA_MSE_ORT2 = 0; KDA_PSNR_ORT2 = 0; KDA_SSIM_ORT2 = 0; maxUygunluk_KDA = 0;
                    BBA_TIME2 = 0; BBA_MSE_ORT2 = 0; BBA_PSNR_ORT2 = 0; BBA_SSIM_ORT2 = 0; maxUygunluk_BBA = 0;
                    KSOA_TIME2 = 0; KSOA_MSE_ORT2 = 0; KSOA_PSNR_ORT2 = 0; KSOA_SSIM_ORT2 = 0; maxUygunluk_KSOA = 0;
                    ABA_TIME2 = 0; ABA_MSE_ORT2 = 0; ABA_PSNR_ORT2 = 0; ABA_SSIM_ORT2 = 0; maxUygunluk_ABA = 0;
                    enIyiResim_GA = null; enIyiResim_DGA = null; enIyiResim_BTA = null; enIyiResim_PSOA = null; enIyiResim_ARI = null; enIyiResim_YA = null; enIyiResim_KDA = null; enIyiResim_BBA = null; enIyiResim_KSOA = null; enIyiResim_ABA = null;
                    this.bozulmusResmiYukle(i,j);
                    konsolBilgi.Text = i + ".resmin " + j + " numaralı bozuntusu yüklendi.";
                    if (i < 10)
                    {
                        isim = "I0" + i + "_17_" + j;
                    }
                    else
                    {
                        isim = "I" + i + "_17_" + j;
                    }         
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j -3, 1];
                    bolge.Value2 = isim;
                    bolge2 = (Excel.Range)ExcelSayfa2.Cells[i * bozuntuSayisi + j -3, 1];
                    bolge2.Value2 = isim;





                    for (int k = 0; k < denemeSayisi; k++)
                    {
                        konsolBilgi.Text = i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Genetik Algoritma uygulanıyor.";
                        Console.WriteLine(i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Genetik Algoritma uygulanıyor.");
                        startTime = DateTime.Now;
                        ga_result = KontrastIyilestir_GA();
                        elapsedTime = DateTime.Now - startTime;
                        this.goruntuIslemleri.ResimY = ga_result;
                        this.goruntuIslemleri.ResimleriKarsilastir();
                        this.goruntuIslemleri.ResimleriKarsilastir2();
                        this.labelAciklamaY.Text = "";
                        this.labelAciklamaY.Visible = false;
                        this.pictureBoxResimY.Image = null;
                        this.pictureBoxResimY.SizeMode = PictureBoxSizeMode.Zoom;
                        this.pictureBoxResimY.Image = ga_result;
                        konsolBilgi.Text = i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Genetik Algoritma uygulandı.";
                        this.labelMeanYDeger.Text = this.goruntuIslemleri.MeanY.ToString("f2");
                        this.labelMedianYDeger.Text = this.goruntuIslemleri.MedianY.ToString();
                        this.labelStdSapmaYDeger.Text = this.goruntuIslemleri.StdSapmaY.ToString("f2");
                        this.labelMSEDeger.Text = this.goruntuIslemleri.MSE.ToString("f2");
                        this.labelPSNRDeger.Text = this.goruntuIslemleri.PSNR.ToString("f2");
                        this.labelSSIMDeger.Text = this.goruntuIslemleri.SSIM.ToString("f2");
                        this.labelMSEZDeger.Text = this.goruntuIslemleri.MSEZ.ToString("f2");
                        this.labelPSNRZDeger.Text = this.goruntuIslemleri.PSNRZ.ToString("f2");
                        this.labelSSIMZDeger.Text = this.goruntuIslemleri.SSIMZ.ToString("f2");
                        GA_MSE_ORT2 = GA_MSE_ORT2 + this.goruntuIslemleri.MSEZ;
                        GA_PSNR_ORT2 = GA_PSNR_ORT2 + this.goruntuIslemleri.PSNRZ;
                        GA_SSIM_ORT2 = GA_SSIM_ORT2 + this.goruntuIslemleri.SSIMZ;
                        GA_TIME2 = GA_TIME2 + elapsedTime.TotalSeconds;
                        Console.WriteLine("MSE: " + this.goruntuIslemleri.MSEZ.ToString("f2") + " PSNR: " + this.goruntuIslemleri.PSNRZ.ToString("f2") + " SSIM: " + this.goruntuIslemleri.SSIMZ.ToString("f2") + " " + (elapsedTime.TotalMilliseconds / 1000).ToString());
                        if(this.uygunluk > this.maxUygunluk_GA)
                        {
                            maxUygunluk_GA = uygunluk;
                            enIyiResim_GA = ga_result;
                        }
                        progressBar1.Increment(1);
                    }
                    GA_TIME2 = GA_TIME2 / denemeSayisi;
                    GA_MSE_ORT2 = GA_MSE_ORT2 / denemeSayisi;
                    GA_PSNR_ORT2 = GA_PSNR_ORT2 / denemeSayisi;
                    GA_SSIM_ORT2 = GA_SSIM_ORT2 / denemeSayisi;
                    bolge2 = (Excel.Range)ExcelSayfa2.Cells[i * bozuntuSayisi + j -3, 2];
                    bolge2.Value2 = GA_TIME2;
                    GA_TIME += GA_TIME2;
                    GA_MSE_ORT += GA_MSE_ORT2;
                    GA_PSNR_ORT += GA_PSNR_ORT2;
                    GA_SSIM_ORT += GA_SSIM_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j -3, 2];
                    bolge.Value2 = GA_MSE_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j -3, 3];
                    bolge.Value2 = GA_PSNR_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j -3, 4];
                    bolge.Value2 = GA_SSIM_ORT2;



                    for (int k = 0; k < denemeSayisi; k++)
                    {
                        konsolBilgi.Text = i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Diferansiyel Gelişim Algoritması uygulanıyor.";
                        Console.WriteLine(i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Diferansiyel Gelişim Algoritması uygulanıyor.");
                        startTime = DateTime.Now;
                        dga_result = KontrastIyilestir_DGA();
                        elapsedTime = DateTime.Now - startTime;
                        this.goruntuIslemleri.ResimY = dga_result;
                        this.goruntuIslemleri.ResimleriKarsilastir();
                        this.goruntuIslemleri.ResimleriKarsilastir2();
                        this.labelAciklamaY.Text = "";
                        this.labelAciklamaY.Visible = false;
                        this.pictureBoxResimY.Image = null;
                        this.pictureBoxResimY.SizeMode = PictureBoxSizeMode.Zoom;
                        this.pictureBoxResimY.Image = dga_result;
                        konsolBilgi.Text = i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Diferansiyel Gelişim Algoritması uygulandı.";
                        this.labelMeanYDeger.Text = this.goruntuIslemleri.MeanY.ToString("f2");
                        this.labelMedianYDeger.Text = this.goruntuIslemleri.MedianY.ToString();
                        this.labelStdSapmaYDeger.Text = this.goruntuIslemleri.StdSapmaY.ToString("f2");
                        this.labelMSEDeger.Text = this.goruntuIslemleri.MSE.ToString("f2");
                        this.labelPSNRDeger.Text = this.goruntuIslemleri.PSNR.ToString("f2");
                        this.labelSSIMDeger.Text = this.goruntuIslemleri.SSIM.ToString("f2");
                        this.labelMSEZDeger.Text = this.goruntuIslemleri.MSEZ.ToString("f2");
                        this.labelPSNRZDeger.Text = this.goruntuIslemleri.PSNRZ.ToString("f2");
                        this.labelSSIMZDeger.Text = this.goruntuIslemleri.SSIMZ.ToString("f2");
                        DGA_MSE_ORT2 = DGA_MSE_ORT2 + this.goruntuIslemleri.MSEZ;
                        DGA_PSNR_ORT2 = DGA_PSNR_ORT2 + this.goruntuIslemleri.PSNRZ;
                        DGA_SSIM_ORT2 = DGA_SSIM_ORT2 + this.goruntuIslemleri.SSIMZ;
                        DGA_TIME2 = DGA_TIME2 + elapsedTime.TotalSeconds;
                        Console.WriteLine("MSE: " + this.goruntuIslemleri.MSEZ.ToString("f2") + " PSNR: " + this.goruntuIslemleri.PSNRZ.ToString("f2") + " SSIM: " + this.goruntuIslemleri.SSIMZ.ToString("f2") + " " + (elapsedTime.TotalMilliseconds / 1000).ToString());
                        if(this.uygunluk > this.maxUygunluk_DGA)
                        {
                            maxUygunluk_DGA = uygunluk;
                            enIyiResim_DGA = dga_result;
                        }
                        progressBar1.Increment(1);
                    }
                    DGA_TIME2 = DGA_TIME2 / denemeSayisi;
                    DGA_MSE_ORT2 = DGA_MSE_ORT2 / denemeSayisi;
                    DGA_PSNR_ORT2 = DGA_PSNR_ORT2 / denemeSayisi;
                    DGA_SSIM_ORT2 = DGA_SSIM_ORT2 / denemeSayisi;
                    bolge2 = (Excel.Range)ExcelSayfa2.Cells[i * bozuntuSayisi + j -3, 3];
                    bolge2.Value2 = DGA_TIME2;
                    DGA_TIME += DGA_TIME2;
                    DGA_MSE_ORT += DGA_MSE_ORT2;
                    DGA_PSNR_ORT += DGA_PSNR_ORT2;
                    DGA_SSIM_ORT += DGA_SSIM_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j -3, 5];
                    bolge.Value2 = DGA_MSE_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j -3, 6];
                    bolge.Value2 = DGA_PSNR_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j -3, 7];
                    bolge.Value2 = DGA_SSIM_ORT2;


                    for (int k = 0; k < denemeSayisi; k++)
                    {
                        konsolBilgi.Text = i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Benzetimli Tavlama Algoritması uygulanıyor.";
                        Console.WriteLine(i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Benzetimli Tavlama Algoritması uygulanıyor.");
                        startTime = DateTime.Now;
                        bta_result = KontrastIyilestir_BTA();
                        elapsedTime = DateTime.Now - startTime;
                        this.goruntuIslemleri.ResimY = bta_result;
                        this.goruntuIslemleri.ResimleriKarsilastir();
                        this.goruntuIslemleri.ResimleriKarsilastir2();
                        this.labelAciklamaY.Text = "";
                        this.labelAciklamaY.Visible = false;
                        this.pictureBoxResimY.Image = null;
                        this.pictureBoxResimY.SizeMode = PictureBoxSizeMode.Zoom;
                        this.pictureBoxResimY.Image = bta_result;
                        konsolBilgi.Text = i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Benzetimli Tavlama Algoritması uygulandı.";
                        this.labelMeanYDeger.Text = this.goruntuIslemleri.MeanY.ToString("f2");
                        this.labelMedianYDeger.Text = this.goruntuIslemleri.MedianY.ToString();
                        this.labelStdSapmaYDeger.Text = this.goruntuIslemleri.StdSapmaY.ToString("f2");
                        this.labelMSEDeger.Text = this.goruntuIslemleri.MSE.ToString("f2");
                        this.labelPSNRDeger.Text = this.goruntuIslemleri.PSNR.ToString("f2");
                        this.labelSSIMDeger.Text = this.goruntuIslemleri.SSIM.ToString("f2");
                        this.labelMSEZDeger.Text = this.goruntuIslemleri.MSEZ.ToString("f2");
                        this.labelPSNRZDeger.Text = this.goruntuIslemleri.PSNRZ.ToString("f2");
                        this.labelSSIMZDeger.Text = this.goruntuIslemleri.SSIMZ.ToString("f2");
                        BTA_MSE_ORT2 = BTA_MSE_ORT2 + this.goruntuIslemleri.MSEZ;
                        BTA_PSNR_ORT2 = BTA_PSNR_ORT2 + this.goruntuIslemleri.PSNRZ;
                        BTA_SSIM_ORT2 = BTA_SSIM_ORT2 + this.goruntuIslemleri.SSIMZ;
                        BTA_TIME2 = BTA_TIME2 + elapsedTime.TotalSeconds;
                        Console.WriteLine("MSE: " + this.goruntuIslemleri.MSEZ.ToString("f2") + " PSNR: " + this.goruntuIslemleri.PSNRZ.ToString("f2") + " SSIM: " + this.goruntuIslemleri.SSIMZ.ToString("f2") + " " + (elapsedTime.TotalMilliseconds / 1000).ToString());
                        if (this.uygunluk > this.maxUygunluk_BTA)
                        {
                            maxUygunluk_BTA = uygunluk;
                            enIyiResim_BTA = bta_result;
                        }
                        progressBar1.Increment(1);
                    }
                    BTA_TIME2 = BTA_TIME2 / denemeSayisi;
                    BTA_MSE_ORT2 = BTA_MSE_ORT2 / denemeSayisi;
                    BTA_PSNR_ORT2 = BTA_PSNR_ORT2 / denemeSayisi;
                    BTA_SSIM_ORT2 = BTA_SSIM_ORT2 / denemeSayisi;
                    bolge2 = (Excel.Range)ExcelSayfa2.Cells[i * bozuntuSayisi + j -3, 4];
                    bolge2.Value2 = BTA_TIME2;
                    BTA_TIME += BTA_TIME2;
                    BTA_MSE_ORT += BTA_MSE_ORT2;
                    BTA_PSNR_ORT += BTA_PSNR_ORT2;
                    BTA_SSIM_ORT += BTA_SSIM_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j -3, 8];
                    bolge.Value2 = BTA_MSE_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j -3, 9];
                    bolge.Value2 = BTA_PSNR_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j -3, 10];
                    bolge.Value2 = BTA_SSIM_ORT2;


                    for (int k = 0; k < denemeSayisi; k++)
                    {
                        konsolBilgi.Text = i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Parçacık Sürü Optimizasyon Algoritması uygulanıyor.";
                        Console.WriteLine(i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Parçacık Sürü Optimizasyon Algoritması uygulanıyor.");
                        startTime = DateTime.Now;
                        psoa_result = KontrastIyilestir_PSOA();
                        elapsedTime = DateTime.Now - startTime;
                        this.goruntuIslemleri.ResimY = psoa_result;
                        this.goruntuIslemleri.ResimleriKarsilastir();
                        this.goruntuIslemleri.ResimleriKarsilastir2();
                        this.labelAciklamaY.Text = "";
                        this.labelAciklamaY.Visible = false;
                        this.pictureBoxResimY.Image = null;
                        this.pictureBoxResimY.SizeMode = PictureBoxSizeMode.Zoom;
                        this.pictureBoxResimY.Image = psoa_result;
                        konsolBilgi.Text = i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Parçacık Sürü Optimizasyon Algoritması uygulandı.";
                        this.labelMeanYDeger.Text = this.goruntuIslemleri.MeanY.ToString("f2");
                        this.labelMedianYDeger.Text = this.goruntuIslemleri.MedianY.ToString();
                        this.labelStdSapmaYDeger.Text = this.goruntuIslemleri.StdSapmaY.ToString("f2");
                        this.labelMSEDeger.Text = this.goruntuIslemleri.MSE.ToString("f2");
                        this.labelPSNRDeger.Text = this.goruntuIslemleri.PSNR.ToString("f2");
                        this.labelSSIMDeger.Text = this.goruntuIslemleri.SSIM.ToString("f2");
                        this.labelMSEZDeger.Text = this.goruntuIslemleri.MSEZ.ToString("f2");
                        this.labelPSNRZDeger.Text = this.goruntuIslemleri.PSNRZ.ToString("f2");
                        this.labelSSIMZDeger.Text = this.goruntuIslemleri.SSIMZ.ToString("f2");
                        PSOA_MSE_ORT2 = PSOA_MSE_ORT2 + this.goruntuIslemleri.MSEZ;
                        PSOA_PSNR_ORT2 = PSOA_PSNR_ORT2 + this.goruntuIslemleri.PSNRZ;
                        PSOA_SSIM_ORT2 = PSOA_SSIM_ORT2 + this.goruntuIslemleri.SSIMZ;
                        PSOA_TIME2 = PSOA_TIME2 + elapsedTime.TotalSeconds;
                        Console.WriteLine("MSE: " + this.goruntuIslemleri.MSEZ.ToString("f2") + " PSNR: " + this.goruntuIslemleri.PSNRZ.ToString("f2") + " SSIM: " + this.goruntuIslemleri.SSIMZ.ToString("f2") + " " + (elapsedTime.TotalMilliseconds / 1000).ToString());
                        if (this.uygunluk > this.maxUygunluk_PSOA)
                        {
                            maxUygunluk_PSOA = uygunluk;
                            enIyiResim_PSOA = psoa_result;
                        }
                        progressBar1.Increment(1);
                    }
                    PSOA_TIME2 = PSOA_TIME2 / denemeSayisi;
                    PSOA_MSE_ORT2 = PSOA_MSE_ORT2 / denemeSayisi;
                    PSOA_PSNR_ORT2 = PSOA_PSNR_ORT2 / denemeSayisi;
                    PSOA_SSIM_ORT2 = PSOA_SSIM_ORT2 / denemeSayisi;
                    bolge2 = (Excel.Range)ExcelSayfa2.Cells[i * bozuntuSayisi + j -3, 5];
                    bolge2.Value2 = PSOA_TIME2;
                    PSOA_TIME += PSOA_TIME2;
                    PSOA_MSE_ORT += PSOA_MSE_ORT2;
                    PSOA_PSNR_ORT += PSOA_PSNR_ORT2;
                    PSOA_SSIM_ORT += PSOA_SSIM_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j -3, 11];
                    bolge.Value2 = PSOA_MSE_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j -3, 12];
                    bolge.Value2 = PSOA_PSNR_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j -3, 13];
                    bolge.Value2 = PSOA_SSIM_ORT2;



                    for (int k = 0; k < denemeSayisi; k++)
                    {
                        konsolBilgi.Text = i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Yapay Arı Kolonisi Algoritması uygulanıyor.";
                        Console.WriteLine(i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Yapay Arı Kolonisi Algoritması uygulanıyor.");
                        startTime = DateTime.Now;
                        ari_result = KontrastIyilestir_ARI();
                        elapsedTime = DateTime.Now - startTime;
                        this.goruntuIslemleri.ResimY = ari_result;
                        this.goruntuIslemleri.ResimleriKarsilastir();
                        this.goruntuIslemleri.ResimleriKarsilastir2();
                        this.labelAciklamaY.Text = "";
                        this.labelAciklamaY.Visible = false;
                        this.pictureBoxResimY.Image = null;
                        this.pictureBoxResimY.SizeMode = PictureBoxSizeMode.Zoom;
                        this.pictureBoxResimY.Image = ari_result;
                        konsolBilgi.Text = i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Yapay Arı Kolonisi Algoritması uygulandı.";
                        this.labelMeanYDeger.Text = this.goruntuIslemleri.MeanY.ToString("f2");
                        this.labelMedianYDeger.Text = this.goruntuIslemleri.MedianY.ToString();
                        this.labelStdSapmaYDeger.Text = this.goruntuIslemleri.StdSapmaY.ToString("f2");
                        this.labelMSEDeger.Text = this.goruntuIslemleri.MSE.ToString("f2");
                        this.labelPSNRDeger.Text = this.goruntuIslemleri.PSNR.ToString("f2");
                        this.labelSSIMDeger.Text = this.goruntuIslemleri.SSIM.ToString("f2");
                        this.labelMSEZDeger.Text = this.goruntuIslemleri.MSEZ.ToString("f2");
                        this.labelPSNRZDeger.Text = this.goruntuIslemleri.PSNRZ.ToString("f2");
                        this.labelSSIMZDeger.Text = this.goruntuIslemleri.SSIMZ.ToString("f2");
                        ARI_MSE_ORT2 = ARI_MSE_ORT2 + this.goruntuIslemleri.MSEZ;
                        ARI_PSNR_ORT2 = ARI_PSNR_ORT2 + this.goruntuIslemleri.PSNRZ;
                        ARI_SSIM_ORT2 = ARI_SSIM_ORT2 + this.goruntuIslemleri.SSIMZ;
                        ARI_TIME2 = ARI_TIME2 + elapsedTime.TotalSeconds;
                        Console.WriteLine("MSE: " + this.goruntuIslemleri.MSEZ.ToString("f2") + " PSNR: " + this.goruntuIslemleri.PSNRZ.ToString("f2") + " SSIM: " + this.goruntuIslemleri.SSIMZ.ToString("f2") + " " + (elapsedTime.TotalMilliseconds / 1000).ToString());
                        if (this.uygunluk > this.maxUygunluk_ARI)
                        {
                            maxUygunluk_ARI = uygunluk;
                            enIyiResim_ARI = ari_result;
                        }
                        progressBar1.Increment(1);
                    }
                    ARI_TIME2 = ARI_TIME2 / denemeSayisi;
                    ARI_MSE_ORT2 = ARI_MSE_ORT2 / denemeSayisi;
                    ARI_PSNR_ORT2 = ARI_PSNR_ORT2 / denemeSayisi;
                    ARI_SSIM_ORT2 = ARI_SSIM_ORT2 / denemeSayisi;
                    bolge2 = (Excel.Range)ExcelSayfa2.Cells[i * bozuntuSayisi + j -3, 6];
                    bolge2.Value2 = ARI_TIME2;
                    ARI_TIME += ARI_TIME2;
                    ARI_MSE_ORT += ARI_MSE_ORT2;
                    ARI_PSNR_ORT += ARI_PSNR_ORT2;
                    ARI_SSIM_ORT += ARI_SSIM_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j -3, 14];
                    bolge.Value2 = ARI_MSE_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j -3, 15];
                    bolge.Value2 = ARI_PSNR_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j -3, 16];
                    bolge.Value2 = ARI_SSIM_ORT2;


                    for (int k = 0; k < denemeSayisi; k++)
                    {
                        konsolBilgi.Text = i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Yusufçuk Algoritması uygulanıyor.";
                        Console.WriteLine(i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Yusufçuk Algoritması uygulanıyor.");
                        startTime = DateTime.Now;
                        ya_result = KontrastIyilestir_YA();
                        elapsedTime = DateTime.Now - startTime;
                        this.goruntuIslemleri.ResimY = ya_result;
                        this.goruntuIslemleri.ResimleriKarsilastir();
                        this.goruntuIslemleri.ResimleriKarsilastir2();
                        this.labelAciklamaY.Text = "";
                        this.labelAciklamaY.Visible = false;
                        this.pictureBoxResimY.Image = null;
                        this.pictureBoxResimY.SizeMode = PictureBoxSizeMode.Zoom;
                        this.pictureBoxResimY.Image = ya_result;
                        konsolBilgi.Text = i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Yusufçuk Algoritması uygulandı.";
                        this.labelMeanYDeger.Text = this.goruntuIslemleri.MeanY.ToString("f2");
                        this.labelMedianYDeger.Text = this.goruntuIslemleri.MedianY.ToString();
                        this.labelStdSapmaYDeger.Text = this.goruntuIslemleri.StdSapmaY.ToString("f2");
                        this.labelMSEDeger.Text = this.goruntuIslemleri.MSE.ToString("f2");
                        this.labelPSNRDeger.Text = this.goruntuIslemleri.PSNR.ToString("f2");
                        this.labelSSIMDeger.Text = this.goruntuIslemleri.SSIM.ToString("f2");
                        this.labelMSEZDeger.Text = this.goruntuIslemleri.MSEZ.ToString("f2");
                        this.labelPSNRZDeger.Text = this.goruntuIslemleri.PSNRZ.ToString("f2");
                        this.labelSSIMZDeger.Text = this.goruntuIslemleri.SSIMZ.ToString("f2");
                        YA_MSE_ORT2 = YA_MSE_ORT2 + this.goruntuIslemleri.MSEZ;
                        YA_PSNR_ORT2 = YA_PSNR_ORT2 + this.goruntuIslemleri.PSNRZ;
                        YA_SSIM_ORT2 = YA_SSIM_ORT2 + this.goruntuIslemleri.SSIMZ;
                        YA_TIME2 = YA_TIME2 + elapsedTime.TotalSeconds;
                        Console.WriteLine("MSE: " + this.goruntuIslemleri.MSEZ.ToString("f2") + " PSNR: " + this.goruntuIslemleri.PSNRZ.ToString("f2") + " SSIM: " + this.goruntuIslemleri.SSIMZ.ToString("f2") + " " + (elapsedTime.TotalMilliseconds / 1000).ToString());
                        if (this.uygunluk > this.maxUygunluk_YA)
                        {
                            maxUygunluk_YA = uygunluk;
                            enIyiResim_YA = ya_result;
                        }
                        progressBar1.Increment(1);
                    }
                    YA_TIME2 = YA_TIME2 / denemeSayisi;
                    YA_MSE_ORT2 = YA_MSE_ORT2 / denemeSayisi;
                    YA_PSNR_ORT2 = YA_PSNR_ORT2 / denemeSayisi;
                    YA_SSIM_ORT2 = YA_SSIM_ORT2 / denemeSayisi;
                    bolge2 = (Excel.Range)ExcelSayfa2.Cells[i * bozuntuSayisi + j -3, 7];
                    bolge2.Value2 = YA_TIME2;
                    YA_TIME += YA_TIME2;
                    YA_MSE_ORT += YA_MSE_ORT2;
                    YA_PSNR_ORT += YA_PSNR_ORT2;
                    YA_SSIM_ORT += YA_SSIM_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j -3, 17];
                    bolge.Value2 = YA_MSE_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j -3, 18];
                    bolge.Value2 = YA_PSNR_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j -3, 19];
                    bolge.Value2 = YA_SSIM_ORT2;



                    for (int k = 0; k < denemeSayisi; k++)
                    {
                        konsolBilgi.Text = i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Kara Delik Algoritması uygulanıyor.";
                        Console.WriteLine(i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Kara Delik Algoritması uygulanıyor.");
                        startTime = DateTime.Now;
                        kda_result = KontrastIyilestir_KDA();
                        elapsedTime = DateTime.Now - startTime;
                        this.goruntuIslemleri.ResimY = kda_result;
                        konsolBilgi.Text = i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Kara Delik Algoritması uygulandı.";
                        this.goruntuIslemleri.ResimleriKarsilastir();
                        this.goruntuIslemleri.ResimleriKarsilastir2();
                        this.labelAciklamaY.Text = "";
                        this.labelAciklamaY.Visible = false;
                        this.pictureBoxResimY.Image = null;
                        this.pictureBoxResimY.SizeMode = PictureBoxSizeMode.Zoom;
                        this.pictureBoxResimY.Image = kda_result;
                        this.labelMeanYDeger.Text = this.goruntuIslemleri.MeanY.ToString("f2");
                        this.labelMedianYDeger.Text = this.goruntuIslemleri.MedianY.ToString();
                        this.labelStdSapmaYDeger.Text = this.goruntuIslemleri.StdSapmaY.ToString("f2");
                        this.labelMSEDeger.Text = this.goruntuIslemleri.MSE.ToString("f2");
                        this.labelPSNRDeger.Text = this.goruntuIslemleri.PSNR.ToString("f2");
                        this.labelSSIMDeger.Text = this.goruntuIslemleri.SSIM.ToString("f2");
                        this.labelMSEZDeger.Text = this.goruntuIslemleri.MSEZ.ToString("f2");
                        this.labelPSNRZDeger.Text = this.goruntuIslemleri.PSNRZ.ToString("f2");
                        this.labelSSIMZDeger.Text = this.goruntuIslemleri.SSIMZ.ToString("f2");
                        KDA_MSE_ORT2 = KDA_MSE_ORT2 + this.goruntuIslemleri.MSEZ;
                        KDA_PSNR_ORT2 = KDA_PSNR_ORT2 + this.goruntuIslemleri.PSNRZ;
                        KDA_SSIM_ORT2 = KDA_SSIM_ORT2 + this.goruntuIslemleri.SSIMZ;
                        KDA_TIME2 = KDA_TIME2 + elapsedTime.TotalSeconds;
                        Console.WriteLine("MSE: " + this.goruntuIslemleri.MSEZ.ToString("f2") + " PSNR: " + this.goruntuIslemleri.PSNRZ.ToString("f2") + " SSIM: " + this.goruntuIslemleri.SSIMZ.ToString("f2") + " " + (elapsedTime.TotalMilliseconds / 1000).ToString());
                        if (this.uygunluk > this.maxUygunluk_KDA)
                        {
                            maxUygunluk_KDA = uygunluk;
                            enIyiResim_KDA = kda_result;
                        }
                        progressBar1.Increment(1);
                    }
                    KDA_TIME2 = KDA_TIME2 / denemeSayisi;
                    KDA_MSE_ORT2 = KDA_MSE_ORT2 / denemeSayisi;
                    KDA_PSNR_ORT2 = KDA_PSNR_ORT2 / denemeSayisi;
                    KDA_SSIM_ORT2 = KDA_SSIM_ORT2 / denemeSayisi;
                    bolge2 = (Excel.Range)ExcelSayfa2.Cells[i * bozuntuSayisi + j - 3, 8];
                    bolge2.Value2 = KDA_TIME2;
                    KDA_TIME += KDA_TIME2;
                    KDA_MSE_ORT += KDA_MSE_ORT2;
                    KDA_PSNR_ORT += KDA_PSNR_ORT2;
                    KDA_SSIM_ORT += KDA_SSIM_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j - 3, 20];
                    bolge.Value2 = KDA_MSE_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j - 3, 21];
                    bolge.Value2 = KDA_PSNR_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j - 3, 22];
                    bolge.Value2 = KDA_SSIM_ORT2;



                    for (int k = 0; k < denemeSayisi; k++)
                    {
                        konsolBilgi.Text = i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Bakteriyel Besin Arama Algoritması uygulanıyor.";
                        Console.WriteLine(i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Bakteriyel Besin Arama Algoritması uygulanıyor.");
                        startTime = DateTime.Now;
                        bba_result = KontrastIyilestir_BBA();
                        elapsedTime = DateTime.Now - startTime;
                        this.goruntuIslemleri.ResimY = bba_result;
                        konsolBilgi.Text = i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Bakteriyel Besin Arama Algoritması uygulandı.";
                        this.goruntuIslemleri.ResimleriKarsilastir();
                        this.goruntuIslemleri.ResimleriKarsilastir2();
                        this.labelAciklamaY.Text = "";
                        this.labelAciklamaY.Visible = false;
                        this.pictureBoxResimY.Image = null;
                        this.pictureBoxResimY.SizeMode = PictureBoxSizeMode.Zoom;
                        this.pictureBoxResimY.Image = bba_result;
                        this.labelMeanYDeger.Text = this.goruntuIslemleri.MeanY.ToString("f2");
                        this.labelMedianYDeger.Text = this.goruntuIslemleri.MedianY.ToString();
                        this.labelStdSapmaYDeger.Text = this.goruntuIslemleri.StdSapmaY.ToString("f2");
                        this.labelMSEDeger.Text = this.goruntuIslemleri.MSE.ToString("f2");
                        this.labelPSNRDeger.Text = this.goruntuIslemleri.PSNR.ToString("f2");
                        this.labelSSIMDeger.Text = this.goruntuIslemleri.SSIM.ToString("f2");
                        this.labelMSEZDeger.Text = this.goruntuIslemleri.MSEZ.ToString("f2");
                        this.labelPSNRZDeger.Text = this.goruntuIslemleri.PSNRZ.ToString("f2");
                        this.labelSSIMZDeger.Text = this.goruntuIslemleri.SSIMZ.ToString("f2");
                        BBA_MSE_ORT2 = BBA_MSE_ORT2 + this.goruntuIslemleri.MSEZ;
                        BBA_PSNR_ORT2 = BBA_PSNR_ORT2 + this.goruntuIslemleri.PSNRZ;
                        BBA_SSIM_ORT2 = BBA_SSIM_ORT2 + this.goruntuIslemleri.SSIMZ;
                        BBA_TIME2 = BBA_TIME2 + elapsedTime.TotalSeconds;
                        Console.WriteLine("MSE: " + this.goruntuIslemleri.MSEZ.ToString("f2") + " PSNR: " + this.goruntuIslemleri.PSNRZ.ToString("f2") + " SSIM: " + this.goruntuIslemleri.SSIMZ.ToString("f2") + " " + (elapsedTime.TotalMilliseconds / 1000).ToString());
                        if (this.uygunluk > this.maxUygunluk_BBA)
                        {
                            maxUygunluk_BBA = uygunluk;
                            enIyiResim_BBA = bba_result;
                        }
                        progressBar1.Increment(1);
                    }
                    BBA_TIME2 = BBA_TIME2 / denemeSayisi;
                    BBA_MSE_ORT2 = BBA_MSE_ORT2 / denemeSayisi;
                    BBA_PSNR_ORT2 = BBA_PSNR_ORT2 / denemeSayisi;
                    BBA_SSIM_ORT2 = BBA_SSIM_ORT2 / denemeSayisi;
                    bolge2 = (Excel.Range)ExcelSayfa2.Cells[i * bozuntuSayisi + j - 3, 9];
                    bolge2.Value2 = BBA_TIME2;
                    BBA_TIME += BBA_TIME2;
                    BBA_MSE_ORT += BBA_MSE_ORT2;
                    BBA_PSNR_ORT += BBA_PSNR_ORT2;
                    BBA_SSIM_ORT += BBA_SSIM_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j - 3, 23];
                    bolge.Value2 = BBA_MSE_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j - 3, 24];
                    bolge.Value2 = BBA_PSNR_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j - 3, 25];
                    bolge.Value2 = BBA_SSIM_ORT2;



                    for (int k = 0; k < denemeSayisi; k++)
                    {
                        konsolBilgi.Text = i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Kedi Sürüsü Optimizasyon Algoritması uygulanıyor.";
                        Console.WriteLine(i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Kedi Sürüsü Optimizasyon Algoritması uygulanıyor.");
                        startTime = DateTime.Now;
                        ksoa_result = KontrastIyilestir_KSOA();
                        elapsedTime = DateTime.Now - startTime;
                        this.goruntuIslemleri.ResimY = ksoa_result;
                        konsolBilgi.Text = i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Kedi Sürüsü Optimizasyon Algoritması uygulandı.";
                        this.goruntuIslemleri.ResimleriKarsilastir();
                        this.goruntuIslemleri.ResimleriKarsilastir2();
                        this.labelAciklamaY.Text = "";
                        this.labelAciklamaY.Visible = false;
                        this.pictureBoxResimY.Image = null;
                        this.pictureBoxResimY.SizeMode = PictureBoxSizeMode.Zoom;
                        this.pictureBoxResimY.Image = ksoa_result;
                        this.labelMeanYDeger.Text = this.goruntuIslemleri.MeanY.ToString("f2");
                        this.labelMedianYDeger.Text = this.goruntuIslemleri.MedianY.ToString();
                        this.labelStdSapmaYDeger.Text = this.goruntuIslemleri.StdSapmaY.ToString("f2");
                        this.labelMSEDeger.Text = this.goruntuIslemleri.MSE.ToString("f2");
                        this.labelPSNRDeger.Text = this.goruntuIslemleri.PSNR.ToString("f2");
                        this.labelSSIMDeger.Text = this.goruntuIslemleri.SSIM.ToString("f2");
                        this.labelMSEZDeger.Text = this.goruntuIslemleri.MSEZ.ToString("f2");
                        this.labelPSNRZDeger.Text = this.goruntuIslemleri.PSNRZ.ToString("f2");
                        this.labelSSIMZDeger.Text = this.goruntuIslemleri.SSIMZ.ToString("f2");
                        KSOA_MSE_ORT2 = KSOA_MSE_ORT2 + this.goruntuIslemleri.MSEZ;
                        KSOA_PSNR_ORT2 = KSOA_PSNR_ORT2 + this.goruntuIslemleri.PSNRZ;
                        KSOA_SSIM_ORT2 = KSOA_SSIM_ORT2 + this.goruntuIslemleri.SSIMZ;
                        KSOA_TIME2 = KSOA_TIME2 + elapsedTime.TotalSeconds;
                        Console.WriteLine("MSE: " + this.goruntuIslemleri.MSEZ.ToString("f2") + " PSNR: " + this.goruntuIslemleri.PSNRZ.ToString("f2") + " SSIM: " + this.goruntuIslemleri.SSIMZ.ToString("f2") + " " + (elapsedTime.TotalMilliseconds / 1000).ToString());
                        if (this.uygunluk > this.maxUygunluk_KSOA)
                        {
                            maxUygunluk_KSOA = uygunluk;
                            enIyiResim_KSOA = ksoa_result;
                        }
                        progressBar1.Increment(1);
                    }
                    KSOA_TIME2 = KSOA_TIME2 / denemeSayisi;
                    KSOA_MSE_ORT2 = KSOA_MSE_ORT2 / denemeSayisi;
                    KSOA_PSNR_ORT2 = KSOA_PSNR_ORT2 / denemeSayisi;
                    KSOA_SSIM_ORT2 = KSOA_SSIM_ORT2 / denemeSayisi;
                    bolge2 = (Excel.Range)ExcelSayfa2.Cells[i * bozuntuSayisi + j - 3, 10];
                    bolge2.Value2 = KSOA_TIME2;
                    KSOA_TIME += KSOA_TIME2;
                    KSOA_MSE_ORT += KSOA_MSE_ORT2;
                    KSOA_PSNR_ORT += KSOA_PSNR_ORT2;
                    KSOA_SSIM_ORT += KSOA_SSIM_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j - 3, 26];
                    bolge.Value2 = KSOA_MSE_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j - 3, 27];
                    bolge.Value2 = KSOA_PSNR_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j - 3, 28];
                    bolge.Value2 = KSOA_SSIM_ORT2;



                    for (int k = 0; k < denemeSayisi; k++)
                    {
                        konsolBilgi.Text = i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Ateş Böceği Algoritması uygulanıyor.";
                        Console.WriteLine(i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Ateş Böceği Algoritması uygulanıyor.");
                        startTime = DateTime.Now;
                        aba_result = KontrastIyilestir_ABA();
                        elapsedTime = DateTime.Now - startTime;
                        this.goruntuIslemleri.ResimY = aba_result;
                        konsolBilgi.Text = i + ". resmin " + j + " numaralı bozuntusu için " + (k + 1).ToString() + ". kez Ateş Böceği Algoritması uygulandı.";
                        this.goruntuIslemleri.ResimleriKarsilastir();
                        this.goruntuIslemleri.ResimleriKarsilastir2();
                        this.labelAciklamaY.Text = "";
                        this.labelAciklamaY.Visible = false;
                        this.pictureBoxResimY.Image = null;
                        this.pictureBoxResimY.SizeMode = PictureBoxSizeMode.Zoom;
                        this.pictureBoxResimY.Image = aba_result;
                        this.labelMeanYDeger.Text = this.goruntuIslemleri.MeanY.ToString("f2");
                        this.labelMedianYDeger.Text = this.goruntuIslemleri.MedianY.ToString();
                        this.labelStdSapmaYDeger.Text = this.goruntuIslemleri.StdSapmaY.ToString("f2");
                        this.labelMSEDeger.Text = this.goruntuIslemleri.MSE.ToString("f2");
                        this.labelPSNRDeger.Text = this.goruntuIslemleri.PSNR.ToString("f2");
                        this.labelSSIMDeger.Text = this.goruntuIslemleri.SSIM.ToString("f2");
                        this.labelMSEZDeger.Text = this.goruntuIslemleri.MSEZ.ToString("f2");
                        this.labelPSNRZDeger.Text = this.goruntuIslemleri.PSNRZ.ToString("f2");
                        this.labelSSIMZDeger.Text = this.goruntuIslemleri.SSIMZ.ToString("f2");
                        ABA_MSE_ORT2 = ABA_MSE_ORT2 + this.goruntuIslemleri.MSEZ;
                        ABA_PSNR_ORT2 = ABA_PSNR_ORT2 + this.goruntuIslemleri.PSNRZ;
                        ABA_SSIM_ORT2 = ABA_SSIM_ORT2 + this.goruntuIslemleri.SSIMZ;
                        ABA_TIME2 = ABA_TIME2 + elapsedTime.TotalSeconds;
                        Console.WriteLine("MSE: " + this.goruntuIslemleri.MSEZ.ToString("f2") + " PSNR: " + this.goruntuIslemleri.PSNRZ.ToString("f2") + " SSIM: " + this.goruntuIslemleri.SSIMZ.ToString("f2") + " " + (elapsedTime.TotalMilliseconds / 1000).ToString());
                        if (this.uygunluk > this.maxUygunluk_ABA)
                        {
                            maxUygunluk_ABA = uygunluk;
                            enIyiResim_ABA = aba_result;
                        }
                        progressBar1.Increment(1);
                    }
                    ABA_TIME2 = ABA_TIME2 / denemeSayisi;
                    ABA_MSE_ORT2 = ABA_MSE_ORT2 / denemeSayisi;
                    ABA_PSNR_ORT2 = ABA_PSNR_ORT2 / denemeSayisi;
                    ABA_SSIM_ORT2 = ABA_SSIM_ORT2 / denemeSayisi;
                    bolge2 = (Excel.Range)ExcelSayfa2.Cells[i * bozuntuSayisi + j - 3, 11];
                    bolge2.Value2 = ABA_TIME2;
                    ABA_TIME += ABA_TIME2;
                    ABA_MSE_ORT += ABA_MSE_ORT2;
                    ABA_PSNR_ORT += ABA_PSNR_ORT2;
                    ABA_SSIM_ORT += ABA_SSIM_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j - 3, 29];
                    bolge.Value2 = ABA_MSE_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j - 3, 30];
                    bolge.Value2 = ABA_PSNR_ORT2;
                    bolge = (Excel.Range)ExcelSayfa.Cells[i * bozuntuSayisi + j - 3, 31];
                    bolge.Value2 = ABA_SSIM_ORT2;
                        
                    
                    enIyiResim_GA.Save("C:\\Users\\CASPER\\Desktop\\Results\\Genetik Algoritma\\" + isim + "_GA.bmp");
                    enIyiResim_DGA.Save("C:\\Users\\CASPER\\Desktop\\Results\\Diferansiyel Gelisim Algoritmasi\\" + isim + "_DGA.bmp");
                    enIyiResim_BTA.Save("C:\\Users\\CASPER\\Desktop\\Results\\Benzetimli Tavlama Algoritmasi\\" + isim + "_BTA.bmp");
                    enIyiResim_PSOA.Save("C:\\Users\\CASPER\\Desktop\\Results\\Parcacık Suru Optimizasyon Algoritmasi\\" + isim + "_PSOA.bmp");
                    enIyiResim_ARI.Save("C:\\Users\\CASPER\\Desktop\\Results\\Yapay Ari Kolonisi Algoritmasi\\" + isim + "_ARI.bmp");
                    enIyiResim_YA.Save("C:\\Users\\CASPER\\Desktop\\Results\\Yusufcuk Algoritmasi\\" + isim + "_YA.bmp");
                    enIyiResim_KDA.Save("C:\\Users\\CASPER\\Desktop\\Results\\Kara Delik Algoritmasi\\" + isim + "_KDA.bmp");
                    enIyiResim_BBA.Save("C:\\Users\\CASPER\\Desktop\\Results\\Bakteriyel Besin Algoritmasi\\" + isim + "_BBA.bmp");
                    enIyiResim_KSOA.Save("C:\\Users\\CASPER\\Desktop\\Results\\Kedi Surusu Optimizasyon Algoritmasi\\" + isim + "_KSOA.bmp");
                    enIyiResim_ABA.Save("C:\\Users\\CASPER\\Desktop\\Results\\Ates Bocegi Algoritmasi\\" + isim + "_ABA.bmp");
                }
            }
            
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 1];
            bolge.Value2 = "Ortalamalar";
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 2];
            bolge.Value2 = GA_MSE_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 3];
            bolge.Value2 = GA_PSNR_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 4];
            bolge.Value2 = GA_SSIM_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 5];
            bolge.Value2 = DGA_MSE_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 6];
            bolge.Value2 = DGA_PSNR_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 7];
            bolge.Value2 = DGA_SSIM_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 8];
            bolge.Value2 = BTA_MSE_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 9];
            bolge.Value2 = BTA_PSNR_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 10];
            bolge.Value2 = BTA_SSIM_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 11];
            bolge.Value2 = PSOA_MSE_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 12];
            bolge.Value2 = PSOA_PSNR_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 13];
            bolge.Value2 = PSOA_SSIM_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 14];
            bolge.Value2 = ARI_MSE_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 15];
            bolge.Value2 = ARI_PSNR_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 16];
            bolge.Value2 = ARI_SSIM_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 17];
            bolge.Value2 = YA_MSE_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 18];
            bolge.Value2 = YA_PSNR_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 19];
            bolge.Value2 = YA_SSIM_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 20];
            bolge.Value2 = KDA_MSE_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 21];
            bolge.Value2 = KDA_PSNR_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 22];
            bolge.Value2 = KDA_SSIM_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 23];
            bolge.Value2 = BBA_MSE_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 24];
            bolge.Value2 = BBA_PSNR_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 25];
            bolge.Value2 = BBA_SSIM_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 26];
            bolge.Value2 = KSOA_MSE_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 27];
            bolge.Value2 = KSOA_PSNR_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 28];
            bolge.Value2 = KSOA_SSIM_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 29];
            bolge.Value2 = ABA_MSE_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 30];
            bolge.Value2 = ABA_PSNR_ORT / (resimSayisi * bozuntuSayisi);
            bolge = (Excel.Range)ExcelSayfa.Cells[resimSayisi * bozuntuSayisi + 3, 31];
            bolge.Value2 = ABA_SSIM_ORT / (resimSayisi * bozuntuSayisi);

            bolge2 = (Excel.Range)ExcelSayfa2.Cells[resimSayisi * bozuntuSayisi + 3, 1];
            bolge2.Value2 = "Ortalamalar";
            bolge2 = (Excel.Range)ExcelSayfa2.Cells[resimSayisi * bozuntuSayisi + 3, 2];
            bolge2.Value2 = GA_TIME / (resimSayisi * bozuntuSayisi);
            bolge2 = (Excel.Range)ExcelSayfa2.Cells[resimSayisi * bozuntuSayisi + 3, 3];
            bolge2.Value2 = DGA_TIME / (resimSayisi * bozuntuSayisi);
            bolge2 = (Excel.Range)ExcelSayfa2.Cells[resimSayisi * bozuntuSayisi + 3, 4];
            bolge2.Value2 = BTA_TIME / (resimSayisi * bozuntuSayisi);
            bolge2 = (Excel.Range)ExcelSayfa2.Cells[resimSayisi * bozuntuSayisi + 3, 5];
            bolge2.Value2 = PSOA_TIME / (resimSayisi * bozuntuSayisi);
            bolge2 = (Excel.Range)ExcelSayfa2.Cells[resimSayisi * bozuntuSayisi + 3, 6];
            bolge2.Value2 = ARI_TIME / (resimSayisi * bozuntuSayisi);
            bolge2 = (Excel.Range)ExcelSayfa2.Cells[resimSayisi * bozuntuSayisi + 3, 7];
            bolge2.Value2 = YA_TIME / (resimSayisi * bozuntuSayisi);
            bolge2 = (Excel.Range)ExcelSayfa2.Cells[resimSayisi * bozuntuSayisi + 3, 8];
            bolge2.Value2 = KDA_TIME / (resimSayisi * bozuntuSayisi);
            bolge2 = (Excel.Range)ExcelSayfa2.Cells[resimSayisi * bozuntuSayisi + 3, 9];
            bolge2.Value2 = BBA_TIME / (resimSayisi * bozuntuSayisi);
            bolge2 = (Excel.Range)ExcelSayfa2.Cells[resimSayisi * bozuntuSayisi + 3, 10];
            bolge2.Value2 = KSOA_TIME / (resimSayisi * bozuntuSayisi);
            bolge2 = (Excel.Range)ExcelSayfa2.Cells[resimSayisi * bozuntuSayisi + 3, 11];
            bolge2.Value2 = ABA_TIME / (resimSayisi * bozuntuSayisi);



            if (s_dosyaadi != "")
            {
                ExcelProje.SaveAs("C:\\Users\\CASPER\\Desktop\\" + s_dosyaadi + ".xlsx", Excel.XlFileFormat.xlWorkbookDefault, Missing, Missing, false, Missing, Excel.XlSaveAsAccessMode.xlNoChange);
                ExcelProje.Close(true, Missing, Missing);
                ExcelUygulama.Quit();
            }

            if (s_dosyaadi2 != "")
            {
                ExcelProje2.SaveAs("C:\\Users\\CASPER\\Desktop\\" + s_dosyaadi2 + ".xlsx", Excel.XlFileFormat.xlWorkbookDefault, Missing2, Missing2, false, Missing2, Excel.XlSaveAsAccessMode.xlNoChange);
                ExcelProje2.Close(true, Missing2, Missing2);
                ExcelUygulama2.Quit();
            }
            saat.Stop();
             long gecenSureMs = saat.ElapsedMilliseconds;
             double gecenSure = gecenSureMs / 1000;
             Console.WriteLine("Processing finished");
             Console.WriteLine("Total time elapsed: " + gecenSure);
             konsolBilgi.Text = "Algoritma değerlendirme işlemi tamamlandı , sonuçlar .xlsx formatında kaydedildi.";
            this.pictureBoxResimX.Image = null;
            this.pictureBoxResimY.Image = null;
            this.pictureBoxResimZ.Image = null;
            this.islemAktifMi = false;
        }

        private void orijinalResmiYukle(int resimNo)
        {
            string path = "C:\\Users\\CASPER\\Desktop\\Data";
            if (resimNo < 10)
            {
                string new_path = path + "\\Reference" + "\\I0" + resimNo.ToString() + ".bmp";
                Bitmap resimZ= new Bitmap(Image.FromFile(new_path));
                this.resim = resimZ;
            }
            else
            {
                string new_path = path + "\\Reference" + "\\I" + resimNo.ToString() + ".bmp";
                Bitmap resimZ = new Bitmap(Image.FromFile(new_path));
                this.resim = resimZ;
            }
            this.goruntuIslemleri.ResimZ = resim;
            this.pictureBoxResimZ.Image = null;
            this.pictureBoxResimZ.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxResimZ.Image = resim;
            this.labelMeanZDeger.Text = this.goruntuIslemleri.MeanZ.ToString("f2");
            this.labelMedianZDeger.Text = this.goruntuIslemleri.MedianZ.ToString();
            this.labelStdSapmaZDeger.Text = this.goruntuIslemleri.StdSapmaZ.ToString("f2");
        }

        private void bozulmusResmiYukle(int resimNo,int bozuntuNo)
        {
            string path = "C:\\Users\\CASPER\\Desktop\\Data";
            if(resimNo < 10)
            {
                string new_path = path + "\\Distorted" + "\\I0" + resimNo.ToString() + "_17_" + bozuntuNo + ".bmp";
                Bitmap resimX= new Bitmap(Image.FromFile(new_path));
                this.resim = resimX;
            }
            else
            {
                string new_path = path + "\\Distorted" + "\\I" + resimNo.ToString() + "_17_" + bozuntuNo + ".bmp";
                Bitmap resimX = new Bitmap(Image.FromFile(new_path));
                this.resim = resimX;
            }
            this.goruntuIslemleri.ResimX = resim;
            this.pictureBoxResimX.Image = null;
            this.pictureBoxResimX.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxResimX.Image = resim;
            this.labelMeanXDeger.Text = this.goruntuIslemleri.MeanX.ToString("f2");
            this.labelMedianXDeger.Text = this.goruntuIslemleri.MedianX.ToString();
            this.labelStdSapmaXDeger.Text = this.goruntuIslemleri.StdSapmaX.ToString("f2");
           
        }

        private Bitmap KontrastIyilestir_PSOA()
        {
            ParcacikSuruOptAlgoritmasi parcacikSuruOptAlgoritmasi = new ParcacikSuruOptAlgoritmasi();
            parcacikSuruOptAlgoritmasi.DegerlendirmeFonksiyonu = this.DegerlendirmeFonksiyonu;
            double[] degerler = parcacikSuruOptAlgoritmasi.AlgoritmaCalistir();
            alfa = Math.Round(degerler[0], 1);
            beta = Math.Round(degerler[1], 0);
            gama = Math.Round(degerler[2], 1);
            uygunluk = parcacikSuruOptAlgoritmasi.DegerlendirmeFonksiyonu(degerler);
            Bitmap resimY = this.goruntuIslemleri.ParametreleriUygula(alfa, beta, gama);
            return resimY;
        }

        private Bitmap KontrastIyilestir_DGA()
        {
            DiferansiyelGelisimAlgoritmasi diferansiyelGelisimAlgoritmasi = new DiferansiyelGelisimAlgoritmasi();
            diferansiyelGelisimAlgoritmasi.DegerlendirmeFonksiyonu = this.DegerlendirmeFonksiyonu;
            double[] degerler = diferansiyelGelisimAlgoritmasi.AlgoritmaCalistir();
            alfa = Math.Round(degerler[0], 1);
            beta = Math.Round(degerler[1], 0);
            gama = Math.Round(degerler[2], 1);
            uygunluk = diferansiyelGelisimAlgoritmasi.DegerlendirmeFonksiyonu(degerler);
            Bitmap resimY = this.goruntuIslemleri.ParametreleriUygula(alfa, beta, gama);
            return resimY;
        }

        private Bitmap KontrastIyilestir_GA()
        {
            GenetikAlgoritma genetikAlgoritma = new GenetikAlgoritma();
            genetikAlgoritma.DegerlendirmeFonksiyonu = this.DegerlendirmeFonksiyonu;
            double[] degerler = genetikAlgoritma.IterasyonBaslat();
            alfa = Math.Round(degerler[0], 1);
            beta = Math.Round(degerler[1], 0);
            gama = Math.Round(degerler[2], 1);
            uygunluk = genetikAlgoritma.DegerlendirmeFonksiyonu(degerler);
            Bitmap resimY = this.goruntuIslemleri.ParametreleriUygula(alfa, beta, gama);
            return resimY;
        }

        private Bitmap KontrastIyilestir_BTA()
        {
            BenzetimliTavlamaAlgoritmasi benzetimliTavlamaAlgoritmasi = new BenzetimliTavlamaAlgoritmasi();
            benzetimliTavlamaAlgoritmasi.DegerlendirmeFonksiyonu = this.DegerlendirmeFonksiyonu;
            double[] degerler = benzetimliTavlamaAlgoritmasi.AlgoritmaCalistir();
            alfa = Math.Round(degerler[0], 1);
            beta = Math.Round(degerler[1], 0);
            gama = Math.Round(degerler[2], 1);
            uygunluk = benzetimliTavlamaAlgoritmasi.DegerlendirmeFonksiyonu(degerler);
            Bitmap resimY = this.goruntuIslemleri.ParametreleriUygula(alfa, beta, gama);
            return resimY;
        }

        private Bitmap KontrastIyilestir_ARI()
        {
            YapayAriKoloniAlgoritmasi yapayAriKoloniAlgoritmasi = new YapayAriKoloniAlgoritmasi();
            yapayAriKoloniAlgoritmasi.EvaluationFonksiyonu = this.DegerlendirmeFonksiyonu;
            Double[] degerler = yapayAriKoloniAlgoritmasi.IterasyonBaslat(1000);
            alfa = Math.Round(degerler[0], 1);
            beta = Math.Round(degerler[1], 0);
            gama = Math.Round(degerler[2], 1);
            uygunluk = yapayAriKoloniAlgoritmasi.DegerlendirmeFonksiyonu(degerler);
            Bitmap resimY = this.goruntuIslemleri.ParametreleriUygula(alfa, beta, gama);
            return resimY;
        }

        private Bitmap KontrastIyilestir_YA()
        {
            YusufcukAlgoritmasi yusufcukAlgoritmasi = new YusufcukAlgoritmasi();
            yusufcukAlgoritmasi.DegerlendirmeFonksiyonu = this.DegerlendirmeFonksiyonu;
            Double[] degerler = yusufcukAlgoritmasi.AlgoritmaCalistir();
            alfa = Math.Round(degerler[0], 1);
            beta = Math.Round(degerler[1], 0);
            gama = Math.Round(degerler[2], 1);
            uygunluk = yusufcukAlgoritmasi.DegerlendirmeFonksiyonu(degerler);
            Bitmap resimY = this.goruntuIslemleri.ParametreleriUygula(alfa, beta, gama);
            return resimY;
        }

        private Bitmap KontrastIyilestir_KDA()
        {
            KaraDelikAlgoritmasi karaDelikAlgoritmasi = new KaraDelikAlgoritmasi();
            karaDelikAlgoritmasi.DegerlendirmeFonksiyonu = this.DegerlendirmeFonksiyonu;
            Double[] degerler = karaDelikAlgoritmasi.AlgoritmaCalistir();
            alfa = Math.Round(degerler[0], 1);
            beta = Math.Round(degerler[1], 0);
            gama = Math.Round(degerler[2], 1);
            uygunluk = karaDelikAlgoritmasi.DegerlendirmeFonksiyonu(degerler);
            Bitmap resimY = this.goruntuIslemleri.ParametreleriUygula(alfa, beta, gama);
            return resimY;
        }

        private Bitmap KontrastIyilestir_BBA()
        {
            BakteriyelBesinArama bakteriyelBesinArama = new BakteriyelBesinArama(goruntuIslemleri);
            bakteriyelBesinArama.DegerlendirmeFonksiyonu = this.DegerlendirmeFonksiyonu;
            Double[] degerler = bakteriyelBesinArama.aramaBaslat(30);
            alfa = Math.Round(degerler[0], 1);
            beta = Math.Round(degerler[1], 0);
            gama = Math.Round(degerler[2], 1);
            uygunluk = bakteriyelBesinArama.DegerlendirmeFonksiyonu(degerler);
            Bitmap resimY = this.goruntuIslemleri.ParametreleriUygula(alfa, beta, gama);
            return resimY;
        }

        private Bitmap KontrastIyilestir_KSOA()
        {
            KediSurusuOptimizasyonu kediSurusuOptimizasyonu = new KediSurusuOptimizasyonu(goruntuIslemleri);
            kediSurusuOptimizasyonu.DegerlendirmeFonksiyonu = this.DegerlendirmeFonksiyonu;
            Double[] degerler = kediSurusuOptimizasyonu.aramaBaslat(5);
            alfa = Math.Round(degerler[0], 1);
            beta = Math.Round(degerler[1], 0);
            gama = Math.Round(degerler[2], 1);
            uygunluk = kediSurusuOptimizasyonu.DegerlendirmeFonksiyonu(degerler);
            Bitmap resimY = this.goruntuIslemleri.ParametreleriUygula(alfa, beta, gama);
            return resimY;
        }

        private Bitmap KontrastIyilestir_ABA()
        {
            AtesBocegiAlgoritması atesBocegiAlgoritmasi = new AtesBocegiAlgoritması();
            atesBocegiAlgoritmasi.DegerlendirmeFonksiyonu = this.DegerlendirmeFonksiyonu;
            Double[] degerler = atesBocegiAlgoritmasi.AlgoritmaCalistir();
            alfa = Math.Round(degerler[0], 1);
            beta = Math.Round(degerler[1], 0);
            gama = Math.Round(degerler[2], 1);
            uygunluk = atesBocegiAlgoritmasi.DegerlendirmeFonksiyonu(degerler);
            Bitmap resimY = this.goruntuIslemleri.ParametreleriUygula(alfa, beta, gama);
            return resimY;
        }

        private void pictureBoxResimY_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void buttonResimYukle2_Click(object sender, EventArgs e)
        {
            if (this.islemAktifMi)
                return;

            this.label2.Visible = false;
            this.timer1Label.Visible = false;
            this.label1.Visible = false;
            this.progressBar1.Visible = false;
            konsolBilgi.Text = "";
            progressBar1.Value = 0;
            this.pictureBoxResimZ_MouseDoubleClick(new object(), new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0));
        }

        private void labelMedianYBaslik_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void labelBilgi_MouseLeave(object sender, EventArgs e)
        {
            ((System.Windows.Forms.Label)sender).ForeColor = Color.FromKnownColor(KnownColor.Window);
            panelBilgi.Visible = false;
        }
    }
}