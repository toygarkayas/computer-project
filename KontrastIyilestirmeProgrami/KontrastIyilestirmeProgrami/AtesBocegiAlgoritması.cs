using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace KontrastIyilestirmeProgrami
{
    public delegate double FonksiyonABA(double[] parametreler);
    class AtesBocegiAlgoritması
    {
        private Random rastgele;
        private int parametreSayisi;
        private int atesBocegiSayisi;
        private double[,] parametreAraliklari;
        private FonksiyonABA degerlendirmeFonksiyonu;
        private double[,] konumlar;
        private double[] enIyiKonum;
        private double[] yerelEnIyiKonum;
        private double[] uygunluklar;
        private double[] isikYogunluklari;
        private double[] sonIterasyonlar;
        private int hassasiyet;
        private double enIyiUygunluk;
        private double yerelEnIyiUygunluk;



        public AtesBocegiAlgoritması()
        {
            this.parametreAraliklari = new double[3, 2] { { 0, 10 }, { -50, 50 }, { 0, 10 } };
            this.degerlendirmeFonksiyonu = this.UygunlukFonksiyonu;
            this.rastgele = new Random();
            this.parametreSayisi = 3;
            this.atesBocegiSayisi = 125;
            this.konumlar = new double[this.atesBocegiSayisi, this.parametreSayisi]; 
            this.uygunluklar = new double[this.atesBocegiSayisi]; 
            this.isikYogunluklari = new double[this.atesBocegiSayisi];
            this.enIyiKonum = new double[this.parametreSayisi];
            this.yerelEnIyiKonum = new double[this.parametreSayisi];
            this.hassasiyet = 100;
            this.sonIterasyonlar = new double[this.hassasiyet];
        }

        public Double[] AlgoritmaCalistir()
        {
            DateTime startTime = DateTime.Now;
            double[] sonuc = new double[4];
            int iterasyonSayisi = 0,i,j;
            double g, b0, a, ad, dag, db, m, b, eag, eb, r, x, y, z, e;
            double[] deger = new double[this.parametreSayisi];
            g = 0.75;//light absorption
            b0 = 0.5;//attraction coefficient base value
            a = 0.2;//mutation coefficient
            ad = 0.98;//mutation coefficient damping ratio
            dag = 0.05 * 10;
            db = 0.05 * 100; 
            AtesBocegiOlustur();
            while (!this.endCondition() || iterasyonSayisi == 0)
            {
                UygunluklariHesapla();
                IsikYogunluguHesapla();
                yerelEnIyiUygunluk = 0;
                for (i = 0; i < atesBocegiSayisi; i++) 
                {  
                    for (j = 0; j < atesBocegiSayisi; j++)
                    {
                        if (this.uygunluklar[j] > this.uygunluklar[i])
                        {
                            x = konumlar[i, 0] - konumlar[j, 0];
                            y = konumlar[i, 1] - konumlar[j, 1];
                            z = konumlar[i, 2] - konumlar[j, 2];
                            r = Math.Sqrt(x * x + y * y + z * z) / 12;
                            b = (b0 - 0.2) * Math.Exp(-g * r * r)/1.7;                         
                            eag = dag * rastgele.NextDouble();
                            e = db * rastgele.NextDouble();
                            this.konumlar[i, 0] = this.konumlar[i, 0] * (1 - b) + b * (this.konumlar[j, 0] - this.konumlar[i, 0]) + a * eag * (rastgele.NextDouble() - 0.5);
                            this.konumlar[i, 1] = this.konumlar[i, 1] * (1 - b) + b * (this.konumlar[j, 1] - this.konumlar[i, 1]) + a * e * (rastgele.NextDouble() - 0.5);
                            this.konumlar[i, 2] = this.konumlar[i, 2] * (1 - b) + b * (this.konumlar[j, 2] - this.konumlar[i, 2]) + a * eag * (rastgele.NextDouble() - 0.5);

                            this.konumlar[i, 0] = Math.Max(this.konumlar[i, 0], 0);
                            this.konumlar[i, 0] = Math.Min(this.konumlar[i, 0], 10);
                            deger[0] = this.konumlar[i, 0];
                            this.konumlar[i, 2] = Math.Max(this.konumlar[i, 2], 0);
                            this.konumlar[i, 2] = Math.Min(this.konumlar[i, 2], 10);
                            deger[1] = this.konumlar[i, 1];
                            this.konumlar[i, 1] = Math.Max(this.konumlar[i, 1], -50);
                            this.konumlar[i, 1] = Math.Min(this.konumlar[i, 1], 50); 
                            deger[2] = this.konumlar[i, 2];
                            this.uygunluklar[i] = this.degerlendirmeFonksiyonu(deger);
                        }
                    }
                    deger[0] = this.konumlar[i, 0]; 
                    deger[1] = this.konumlar[i, 1];
                    deger[2] = this.konumlar[i, 2];
                    this.uygunluklar[i] = this.degerlendirmeFonksiyonu(deger);
                    if (this.uygunluklar[i] > yerelEnIyiUygunluk)
                    {
                        yerelEnIyiUygunluk = this.uygunluklar[i];
                        yerelEnIyiKonum[0] = this.konumlar[i, 0];
                        yerelEnIyiKonum[1] = this.konumlar[i, 1];
                        yerelEnIyiKonum[2] = this.konumlar[i, 2];
                    }
                }
                if (enIyiUygunluk < yerelEnIyiUygunluk)
                {
                    enIyiKonum = yerelEnIyiKonum;
                    enIyiUygunluk = yerelEnIyiUygunluk;
                }
                sonIterasyonlar[iterasyonSayisi % this.hassasiyet] = enIyiUygunluk;
                iterasyonSayisi++;
            }
            TimeSpan timeElapsed = DateTime.Now - startTime;
            sonuc[0] = enIyiKonum[0];
            sonuc[1] = enIyiKonum[1];
            sonuc[2] = enIyiKonum[2];
            sonuc[3] = timeElapsed.TotalMilliseconds;
            return sonuc;
        }
        public void AtesBocegiOlustur()
        {
            for (int i = 0; i < this.atesBocegiSayisi; i++)
            {
                for (int j = 0; j < this.parametreSayisi; j++)
                {
                    this.konumlar[i, j] = this.rastgele.NextDouble() * (this.parametreAraliklari[j, 1] - this.parametreAraliklari[j, 0]) + this.parametreAraliklari[j, 0];
                }
            }
        }


        public void IsikYogunluguHesapla()
        {
            double x, y, z, distance; 
            double max = this.uygunluklar[0];
            int indis = 0;
            for (int i = 1; i < this.atesBocegiSayisi; i++)
            {
                if(this.uygunluklar[i] > max)
                {
                    max = this.uygunluklar[i];
                    indis = i;
                }
            }
            for(int i = 0; i < this.atesBocegiSayisi; i++)
            {
                x = konumlar[i, 0] - konumlar[indis, 0];
                y = konumlar[i, 1] - konumlar[indis, 1];
                z = konumlar[i, 2] - konumlar[indis, 2];
                distance = Math.Sqrt(x * x + y * y + z * z);
                this.isikYogunluklari[i] = this.uygunluklar[i] * Math.Exp(distance*distance*-1);
            }
        }

        public void UygunluklariHesapla()
        {
            int i, j;
            double[] parametreler = new double[this.parametreSayisi];

            for (i = 0; i < this.atesBocegiSayisi; i++)
            {
                for (j = 0; j < this.parametreSayisi; j++)
                    parametreler[j] = this.konumlar[i, j];
                this.uygunluklar[i] = this.degerlendirmeFonksiyonu(parametreler);
            }
        }


        public Boolean endCondition()
        {
            for (int i = 1; i < this.hassasiyet; i++)
            {
                if (sonIterasyonlar[0] != sonIterasyonlar[i])
                    return false;
            }
            return true;
        }

        public FonksiyonABA DegerlendirmeFonksiyonu
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
