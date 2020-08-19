using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KontrastIyilestirmeProgrami
{
    public delegate double FonksiyonKDA(double[] parametreler);

    class KaraDelikAlgoritmasi
    {
        private Random rastgele;
        private int parametreSayisi;
        private int yildizSayisi;
        private double[,] parametreAraliklari;
        private FonksiyonKDA degerlendirmeFonksiyonu;
        private double[,] konumlar;
        private double[] enIyiKonum;
        private double[] uygunluklar;
        private int[] komsular;
        private double enIyiUygunluk;
        private double karaDelikUygunluk;
        private double[] karaDelikKonumu;
        private int yutulanYildizSayisi;
        private int[] yutulanYildizlar;
        private double karaDelikCapi;
        private double[] sonIterasyonlar;
        private int hassasiyet;

        public KaraDelikAlgoritmasi()
        {
            this.parametreAraliklari = new double[3, 2] { { 0, 10 }, { -50, 50 }, { 0, 10 } };
            this.degerlendirmeFonksiyonu = this.UygunlukFonksiyonu;
            this.rastgele = new Random();
            this.parametreSayisi = 3;
            this.yildizSayisi = 200;//100-200-400
            this.konumlar = new double[this.yildizSayisi, this.parametreSayisi];            
            this.uygunluklar = new double[this.yildizSayisi];
            this.komsular = new int[this.yildizSayisi];           
            this.enIyiKonum = new double[this.parametreSayisi];            
            this.karaDelikKonumu = new double[this.parametreSayisi];
            this.yutulanYildizSayisi = 0;
            this.yutulanYildizlar = new int[this.yildizSayisi];
            this.karaDelikCapi = 0;
            this.hassasiyet = 100;//50-75-100
            this.sonIterasyonlar = new double[this.hassasiyet];
        }

        public double[] AlgoritmaCalistir()
        {
            double[] sonuc = new double[4];
            DateTime startTime=DateTime.Now;
            int iterasyonSayisi = 0;
            YildizOlustur();
            while (!this.endCondition() || iterasyonSayisi == 0)
            {
                UygunluklariHesapla();
                KaraDelikBelirle();
                KaraDeligeCek();
                UygunluklariHesapla();
                KaraDelikBelirle();
                CapBelirle();
                YildizYut();
                YeniYildizBelirle(); //Yutulan yıldızlar yerine yeni random yildizlar
                iterasyonSayisi++;
                if(enIyiUygunluk < karaDelikUygunluk)
                {
                    enIyiKonum = karaDelikKonumu;
                    enIyiUygunluk = karaDelikUygunluk;
                }
                sonIterasyonlar[iterasyonSayisi % this.hassasiyet] = enIyiUygunluk;
            }
            TimeSpan timeElapsed = DateTime.Now - startTime;
            sonuc[0] = enIyiKonum[0];
            sonuc[1] = enIyiKonum[1];
            sonuc[2] = enIyiKonum[2];
            sonuc[3] = timeElapsed.TotalMilliseconds;
            return sonuc;

        }



        public void YildizOlustur()
        {
            for (int i = 0; i < this.yildizSayisi; i++)
            {
                for (int j = 0; j < this.parametreSayisi; j++)
                {
                    this.konumlar[i, j] = this.rastgele.NextDouble() * (this.parametreAraliklari[j, 1] - this.parametreAraliklari[j, 0]) + this.parametreAraliklari[j, 0];                    
                }
            }
        }

        public void UygunluklariHesapla()
        {
            int i, j;
            double[] parametreler = new double[this.parametreSayisi];

            for (i = 0; i < this.yildizSayisi; i++)
            {
                for (j = 0; j < this.parametreSayisi; j++)
                    parametreler[j] = this.konumlar[i, j];
                this.uygunluklar[i] = this.degerlendirmeFonksiyonu(parametreler);
            }
        }

        public void KaraDelikBelirle()
        {
            int i, j;
            double[] karaDelik = new double[this.parametreSayisi];
            double karaDelikUygunluk = this.uygunluklar[0];
            for (j = 0; j < this.parametreSayisi; j++)
            {
                karaDelik[j] = this.konumlar[0, j];
            }
            for (i = 1; i < this.yildizSayisi; i++)
            {
                if (karaDelikUygunluk < this.uygunluklar[i])
                {
                    karaDelikUygunluk = this.uygunluklar[i];
                    for (j = 0; j < this.parametreSayisi; j++)
                    {
                        karaDelik[j] = this.konumlar[i, j];
                    }
                }
            }
            this.karaDelikKonumu = karaDelik;
            this.karaDelikUygunluk = karaDelikUygunluk;
        }

        public void KaraDeligeCek()
        {
            for(int i = 0; i < yildizSayisi; i++)
            {
                for(int j = 0; j < parametreSayisi; j++)
                {
                    konumlar[i, j] = konumlar[i, j] + rastgele.NextDouble() * (karaDelikKonumu[j] - konumlar[i, j]);
                }
            }
        }

        public void CapBelirle()
        {
            double r=this.karaDelikCapi,sum=0;
            for(int i = 0; i < this.yildizSayisi; i++)
            {
                sum += uygunluklar[i];
            }
            r = karaDelikUygunluk / sum;           
            this.karaDelikCapi = r;
        }

        public void YildizYut()
        {
            this.yutulanYildizSayisi = 0;
            double x, y, z,distanceToBlackHole;
            for (int i = 0; i < this.yildizSayisi; i++)
            {
                this.yutulanYildizlar[i] = -1;
            }
            for (int i = 0; i < this.yildizSayisi; i++)
            {
                x = karaDelikKonumu[0] - konumlar[i, 0];
                y = karaDelikKonumu[1] - konumlar[i, 1];
                z = karaDelikKonumu[2] - konumlar[i, 2];
                distanceToBlackHole = Math.Sqrt(x * x + y * y + z * z);
                if(distanceToBlackHole < this.karaDelikCapi)
                {
                    this.yutulanYildizlar[yutulanYildizSayisi] = i;
                    this.yutulanYildizSayisi++;
                }
            }
        }

        public void YeniYildizBelirle()
        {
            int i = 0;
            while (i < this.yutulanYildizSayisi)
            {
                for (int j = 0; j < this.parametreSayisi; j++)
                {
                    this.konumlar[this.yutulanYildizlar[i], j] = this.rastgele.NextDouble() * (this.parametreAraliklari[j, 1] - this.parametreAraliklari[j, 0]) + this.parametreAraliklari[j, 0];
                }
                i++;
            }
        }

        public Boolean endCondition()
        {
            for(int i = 1; i < this.hassasiyet; i++)
            {
                if (sonIterasyonlar[0] != sonIterasyonlar[i])
                    return false;
            }
            return true;
        }



        public FonksiyonKDA DegerlendirmeFonksiyonu
        {
            get { return this.degerlendirmeFonksiyonu; }
            set { this.degerlendirmeFonksiyonu = value; }
        }

        private double UygunlukFonksiyonu(double[] parametreler)
        {
            return 0;
        }

    }
}
