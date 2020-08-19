using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KontrastIyilestirmeProgrami
{
    public delegate double FonksiyonYA(double[] parametreler);

    class YusufcukAlgoritmasi
    {
        private Random rastgele;
        private int parametreSayisi;
        private int yusufcukSayisi;
        private double[,] parametreAraliklari;
        private FonksiyonYA degerlendirmeFonksiyonu;
        private double maxHizAlfaGama;
        private double minHizAlfaGama;
        private double maxHizBeta;
        private double minHizBeta;
        private double hizSinirSabiti;
        private double[,] konumlar;
        private double[,] hizlar;
        private double[] enIyiKonum;
        private double[] uygunluklar;
        private double[] dusmanKonumu;
        private double[] yemekKonumu;
        private int[] komsular;
        //private double[,] komsularHiz;
        private int donusHassasiyeti;
        private double enIyiUygunluk;
        private double yemekUygunluk;
        private double[] sonIterasyonlar;
        private int hassasiyet;
        private double r;
        private double w;
        private double c;
        private double s;
        private double a;
        private double f;
        private double e;
        private double[] S;
        private double[] A;
        private double[] C;
        private double[] F;
        private double[] E;



        public YusufcukAlgoritmasi()
        {
            this.parametreAraliklari = new double[3, 2] { { 0, 10 }, { -50, 50 }, { 0, 10 } };
            this.degerlendirmeFonksiyonu = this.UygunlukFonksiyonu;
            this.rastgele = new Random();
            this.parametreSayisi = 3;
            this.yusufcukSayisi = 800;//400-800-1200
            this.hizSinirSabiti = 0.4;
            this.konumlar = new double[this.yusufcukSayisi, this.parametreSayisi];
            this.hizlar = new double[this.yusufcukSayisi, this.parametreSayisi];
            this.uygunluklar = new double[this.yusufcukSayisi];
            this.komsular = new int[this.yusufcukSayisi];
            //this.komsularHiz= new double[this.yusufcukSayisi, this.parametreSayisi];
            this.enIyiKonum = new double[this.parametreSayisi];
            this.dusmanKonumu = new double[this.parametreSayisi];
            this.yemekKonumu = new double[this.parametreSayisi];
            this.donusHassasiyeti = 1000;//500-1000-2000
            this.hassasiyet = 100;//50-75-100
            this.sonIterasyonlar = new double[this.hassasiyet];
            this.S = new double[this.parametreSayisi];
            this.A = new double[this.parametreSayisi];
            this.C = new double[this.parametreSayisi];
            this.F = new double[this.parametreSayisi];
            this.E = new double[this.parametreSayisi];
        }

        public Double[] AlgoritmaCalistir()
        {
            DateTime startTime = DateTime.Now;
            double[] sonuc = new double[4];
            int iterasyonSayisi = 0;
            //r1 alfa ve gama için , r2 beta için   r=(ub-lb)/4+((ub-lb)*(iter/Max_iteration)*2);      
            YusufcukOlustur();
            HizSinirlariniBelirle();
            HizlariOlustur();          
            while (!this.endCondition() || iterasyonSayisi == 0)
            {
                ParametreleriHesapla(iterasyonSayisi);
                UygunluklariHesapla();
                DusmanBelirle();
                YemekBelirle();
                DurumlariHesapla();
                KonumlariKontrolEt();                
                if (enIyiUygunluk < yemekUygunluk)
                {
                    enIyiKonum = yemekKonumu;
                    enIyiUygunluk = yemekUygunluk;
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

        public void YusufcukOlustur()
        {
            /* Yusufcuklarin konumlari this.rastgele atanir ve hizlari sıfırlanır. */
            for (int i = 0; i < this.yusufcukSayisi; i++)
            {
                for (int j = 0; j < this.parametreSayisi; j++)
                {
                    this.konumlar[i, j] = this.rastgele.NextDouble() * (this.parametreAraliklari[j, 1] - this.parametreAraliklari[j, 0]) + this.parametreAraliklari[j, 0];
                    this.hizlar[i, j] = 0.0;  // Baslangic hizlari 0 olarak ilklendirilir
                }
            }
        }

        public void ParametreleriHesapla(int iterasyonSayisi)
        {
            r = 10 / 4 + (10 * (iterasyonSayisi / this.donusHassasiyeti) * 2); // r=(ub-lb)/4+((ub-lb)*(iter/Max_iteration)*2);
            w = 0.9 - iterasyonSayisi * ((0.9 - 0.4) / this.donusHassasiyeti);
            c = 0.1 - iterasyonSayisi * ((0.1 - 0) / (this.donusHassasiyeti / 2));
            if (c < 0)
                c = 0;
            s = 2 * rastgele.NextDouble() * c;
            a = 2 * rastgele.NextDouble() * c;
            c = 2 * rastgele.NextDouble() * c;
            f = 2 * rastgele.NextDouble();
            e = c;
        }

        public void HizSinirlariniBelirle()
        {
            this.maxHizAlfaGama = this.hizSinirSabiti * (this.parametreAraliklari[0, 1]);
            this.minHizAlfaGama = -1 * (maxHizAlfaGama);
            this.maxHizBeta = this.hizSinirSabiti * (this.parametreAraliklari[1, 1]);
            this.minHizBeta = this.hizSinirSabiti * (this.parametreAraliklari[1, 0]);
        }

        public void UygunluklariHesapla()
        {
            int i, j;
            double[] parametreler = new double[this.parametreSayisi];

            for (i = 0; i < this.yusufcukSayisi; i++)
            {
                for (j = 0; j < this.parametreSayisi; j++)
                    parametreler[j] = this.konumlar[i, j];
                this.uygunluklar[i] = this.degerlendirmeFonksiyonu(parametreler);
            }
        }

        public void DusmanBelirle()
        {
            int i, j;
            double[] dusman = new double[this.parametreSayisi];
            double dusmanUygunluk = this.uygunluklar[0];
            for (j = 0; j < this.parametreSayisi; j++)
            {
                dusman[j] = this.konumlar[0, j];
            }

            for (i = 1; i < this.yusufcukSayisi; i++)
            {
                if (dusmanUygunluk > this.uygunluklar[i])
                {
                    dusmanUygunluk = this.uygunluklar[i];
                    for (j = 0; j < this.parametreSayisi; j++)
                    {
                        dusman[j] = this.konumlar[i, j];
                    }
                }
            }
            this.dusmanKonumu = dusman;
        }

        public void YemekBelirle()
        {
            int i, j;
            double[] yemek = new double[this.parametreSayisi];
            double yemekUygunluk = this.uygunluklar[0];
            for (j = 0; j < this.parametreSayisi; j++)
            {
                yemek[j] = this.konumlar[0, j];
            }
            for (i = 1; i < this.yusufcukSayisi; i++)
            {
                if (yemekUygunluk < this.uygunluklar[i])
                {
                    yemekUygunluk = this.uygunluklar[i];
                    for (j = 0; j < this.parametreSayisi; j++)
                    {
                        yemek[j] = this.konumlar[i, j];
                    }
                }
            }
            this.yemekKonumu = yemek;
            this.yemekUygunluk = yemekUygunluk;
        }

        public void HizlariOlustur()
        {
            int i, j;
            double random;
            for (i = 0; i < this.yusufcukSayisi; i++)
            {
                for (j = 0; j < this.parametreSayisi; j++)
                {
                    random = this.rastgele.NextDouble();
                    hizlar[i, 0] = random * this.maxHizAlfaGama;
                    random = this.rastgele.NextDouble();
                    hizlar[i, 2] = random * this.maxHizAlfaGama;
                    random = this.rastgele.NextDouble();
                    hizlar[i, 1] = random * this.maxHizBeta;
                }
            }
        }

        public void DurumlariHesapla()
        {
            int i, j, k;
            double distanceToNeighbour, distanceToFood, distanceToEnemy, x, y, z;
            for (i = 0; i < this.yusufcukSayisi; i++)
            {
                int index = 0, neighbour_no = 0;
                for (j = 0; j < this.yusufcukSayisi; j++)
                {
                    komsular[j] = -1;
                    /*komsularHiz[j, 0] = -1;
                    komsularHiz[j, 1] = -1;
                    komsularHiz[j, 2] = -1;*/
                }
                for (j = 0; j < this.yusufcukSayisi; j++)
                {
                    x = konumlar[i, 0] - konumlar[j, 0];
                    y = konumlar[i, 1] - konumlar[j, 1];
                    z = konumlar[i, 2] - konumlar[j, 2];
                    distanceToNeighbour = Math.Sqrt(x * x + y * y + z * z);
                    if (distanceToNeighbour <= r && distanceToNeighbour != 0)
                    {
                        komsular[index] = j;
                        index++;
                        neighbour_no++;
                    }
                }
                //SEPERATION
                S[0] = 0;
                S[1] = 0;
                S[2] = 0;
                if (neighbour_no > 0)
                {
                    for (j = 0; j < neighbour_no; j++)
                    {
                        S[0] = S[0] + (konumlar[komsular[j], 0] - konumlar[i, 0]);
                        S[1] = S[1] + (konumlar[komsular[j], 1] - konumlar[i, 1]);
                        S[2] = S[2] + (konumlar[komsular[j], 2] - konumlar[i, 2]);
                    }
                    S[0] = S[0] * -1;
                    S[1] = S[1] * -1;
                    S[2] = S[2] * -1;
                }
                else
                {
                    S[0] = 0;
                    S[1] = 0;
                    S[2] = 0;
                }
                A[0] = 0;
                A[1] = 0;
                A[2] = 0;
                //ALIGNMENT
                if (neighbour_no > 0)
                {
                    for (j = 0; j < neighbour_no; j++)
                    {
                        A[0] = A[0] + hizlar[komsular[j], 0];
                        A[1] = A[1] + hizlar[komsular[j], 1];
                        A[2] = A[2] + hizlar[komsular[j], 2];
                    }
                    A[0] = A[0] / neighbour_no;
                    A[1] = A[1] / neighbour_no;
                    A[2] = A[2] / neighbour_no;
                }
                else
                {
                    A[0] = hizlar[i, 0];
                    A[1] = hizlar[i, 1];
                    A[2] = hizlar[i, 2];
                }
                C[0] = 0;
                C[1] = 0;
                C[2] = 0;
                //COHESION
                if (neighbour_no > 0)
                {
                    for (j = 0; j < neighbour_no; j++)
                    {
                        C[0] = C[0] + konumlar[komsular[j], 0];
                        C[1] = C[1] + konumlar[komsular[j], 1];
                        C[2] = C[2] + konumlar[komsular[j], 2];
                    }
                    C[0] = C[0] / neighbour_no;
                    C[1] = C[1] / neighbour_no;
                    C[2] = C[2] / neighbour_no;
                }
                else
                {
                    C[0] = konumlar[i, 0];
                    C[1] = konumlar[i, 1];
                    C[2] = konumlar[i, 2];
                }
                C[0] = C[0] - konumlar[i, 0];
                C[1] = C[1] - konumlar[i, 1];
                C[2] = C[2] - konumlar[i, 2];

                F[0] = 0;
                F[1] = 0;
                F[2] = 0;
                //ATTRACTION TO FOOD
                x = yemekKonumu[0] - konumlar[i, 0];
                y = yemekKonumu[1] - konumlar[i, 1];
                z = yemekKonumu[2] - konumlar[i, 2];
                distanceToFood = Math.Sqrt(x * x + y * y + z * z);
                if (distanceToFood <= r)
                {
                    F[0] = yemekKonumu[0] - konumlar[i, 0];
                    F[1] = yemekKonumu[1] - konumlar[i, 1];
                    F[2] = yemekKonumu[2] - konumlar[i, 2];
                }
                else
                {
                    F[0] = 0;
                    F[1] = 0;
                    F[2] = 0;
                }
                E[0] = 0;
                E[1] = 0;
                E[2] = 0;
                //DISTRACTION FROM ENEMY
                x = dusmanKonumu[0] - konumlar[i, 0];
                y = dusmanKonumu[1] - konumlar[i, 1];
                z = dusmanKonumu[2] - konumlar[i, 2];
                distanceToEnemy = Math.Sqrt(x * x + y * y + z * z);
                if (distanceToEnemy <= r)
                {
                    E[0] = dusmanKonumu[0] + konumlar[i, 0];
                    E[1] = dusmanKonumu[1] + konumlar[i, 1];
                    E[2] = dusmanKonumu[2] + konumlar[i, 2];
                }
                else
                {
                    E[0] = 0;
                    E[1] = 0;
                    E[2] = 0;
                }
                if (r < distanceToFood)
                {
                    if (0 < neighbour_no)
                    {
                        for (j = 0; j < parametreSayisi; j++)
                        {
                            hizlar[i, j] = w * hizlar[i, j] + this.rastgele.NextDouble() * A[j] + this.rastgele.NextDouble() * C[j] + this.rastgele.NextDouble() * S[j];
                            if (j == 0 || j == 2)
                            {
                                if (hizlar[i, j] < minHizAlfaGama)
                                {
                                    hizlar[i, j] = minHizAlfaGama;
                                }
                                else if (maxHizAlfaGama < hizlar[i, j])
                                {
                                    hizlar[i, j] = maxHizAlfaGama;
                                }
                            }
                            else
                            {
                                if (hizlar[i, j] < minHizBeta)
                                {
                                    hizlar[i, j] = minHizBeta;
                                }
                                else if (maxHizBeta < hizlar[i, j])
                                {
                                    hizlar[i, j] = maxHizBeta;
                                }
                            }
                            konumlar[i, j] = konumlar[i, j] + hizlar[i, j];
                        }
                    }
                    else
                    {//LEVY /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        konumlar[i, 0] = konumlar[i, 0] + rastgele.NextDouble();
                        konumlar[i, 1] = konumlar[i, 1] + rastgele.NextDouble();
                        konumlar[i, 2] = konumlar[i, 2] + rastgele.NextDouble();
                        hizlar[i, 0] = 0;
                        hizlar[i, 1] = 0;
                        hizlar[i, 2] = 0;
                    }
                }
                else
                {
                    for (j = 0; j < parametreSayisi; j++)
                    {
                        hizlar[i, j] = (a * A[j] + c * C[j] + s * S[j] + f * F[j] + e * E[j]) + w * hizlar[i, j];
                        if (j == 0 || j == 2)
                        {
                            if (hizlar[i, j] < minHizAlfaGama)
                            {
                                hizlar[i, j] = minHizAlfaGama;
                            }
                            else if (maxHizAlfaGama < hizlar[i, j])
                            {
                                hizlar[i, j] = maxHizAlfaGama;
                            }
                        }
                        else
                        {
                            if (hizlar[i, j] < minHizBeta)
                            {
                                hizlar[i, j] = minHizBeta;
                            }
                            else if (maxHizBeta < hizlar[i, j])
                            {
                                hizlar[i, j] = maxHizBeta;
                            }
                        }
                    }
                }
            }
        }

        public void KonumlariKontrolEt()
        {
            int j, k;
            for (j = 0; j < yusufcukSayisi; j++)
            {
                for (k = 0; k < parametreSayisi; k++)
                {
                    if (k == 0 || k == 2)
                    {
                        if (10 < konumlar[j, k])
                            konumlar[j, k] = 10;
                        else if (konumlar[j, k] < 0)
                            konumlar[j, k] = 0;
                    }
                    else
                    {
                        if (50 < konumlar[j, k])
                        {
                            konumlar[j, k] = 50;
                        }
                        else if (konumlar[j, k] < -50)
                        {
                            konumlar[j, k] = -50;
                        }
                    }
                }
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


        public FonksiyonYA DegerlendirmeFonksiyonu
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
