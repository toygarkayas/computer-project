using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KontrastIyilestirmeProgrami
{
    public delegate double FonksiyonBBA(double[] parametreler);
    class Hucre
    {
        Random r;
        private double[] konum;
        private double cost;
        private double uygunluk;
        private double can;
        private double adım_buyukulugu;

        public Hucre(int boyut_sayisi, double adım, double[,] parametreAraliklari, GoruntuIslemleri goruntuIslemleri)
        {
            r = new Random();

            this.konum = new double[boyut_sayisi];
            double aralik;
            double minimum;
            for (int i = 0; i < boyut_sayisi; i++)
            {
                aralik = parametreAraliklari[i, 1] - parametreAraliklari[i, 0];
                minimum = parametreAraliklari[i, 0];
                this.Konum[i] = r.NextDouble() * (aralik) + minimum;
            }
            this.cost = goruntuIslemleri.ParametreleriDegerlendir(this.konum[0], this.konum[1], this.konum[2]);
            this.uygunluk = 0;
            this.can = 0;
            this.adım_buyukulugu = adım;
        }

        public double[] Konum
        {
            get { return this.konum; }
            set { this.konum = value; }
        }

        public double Uygunluk
        {
            get { return this.uygunluk; }
            set { this.uygunluk = value; }
        }

        public double Cost
        {
            get { return this.cost; }
            set { this.cost = value; }
        }

        public double Can
        {
            get { return this.can; }
            set { this.can = value; }
        }

        public double AdımBuyuklugu
        {
            get { return this.adım_buyukulugu; }
            set { this.adım_buyukulugu = value; }
        }
    }

    class BakteriyelBesinArama
    {
        private int populasyonBuyuklugu;
        private int boyutSayisi;
        private Hucre[] populasyon;
        private Hucre enIyiHucre;
        private double[] yuvarAdım; // Yuvarlanma adımları
        private double[] delta;
        //private int bakteriSayisi;
        private double uremeOranı;
        private int uremeSayisi;
        private double adimBuyuklugu;
        private double[,] parametreAraliklari;
        private FonksiyonBBA degerlendirmeFonksiyonu;
        private int yuzmeUzunlugu;
        private double edIhtimali; //Eliminasyon-Dağıtma
        private double dAttract;
        private double wAtrract;
        private double hRepel;
        private double wRepel;
        private Random rastgele;
        private double[] enIyiKonumlar;
        private GoruntuIslemleri goruntuIslemleri;

        public BakteriyelBesinArama(GoruntuIslemleri goruntuIsl)
        {
            goruntuIslemleri = goruntuIsl;
            this.degerlendirmeFonksiyonu = this.UygunlukFonksiyonu;
            populasyonBuyuklugu = 100;
            uremeOranı = 0.4;
            boyutSayisi = 3;
            adimBuyuklugu = 0.8;
            uremeSayisi = Convert.ToInt32(Convert.ToDouble(populasyonBuyuklugu) * uremeOranı);

            yuzmeUzunlugu = 4;
            edIhtimali = 0.10;
            dAttract = 0.1;
            wAtrract = 0.2;
            hRepel = dAttract;
            wRepel = 10.0;

            enIyiKonumlar = new double[boyutSayisi];
            populasyon = new Hucre[populasyonBuyuklugu];

            rastgele = new Random();
            yuvarAdım = new double[boyutSayisi];
            delta = new double[boyutSayisi];
            //this.parametreAraliklari = parametreAra;
            this.parametreAraliklari = new double[3, 2] { { 0, 10 }, { -50, 50 }, { 0, 10 } };

        }


        public double[] aramaBaslat(int sayacLimiti)
        {
            double eski_cost;
            int sayac = sayacLimiti;
            int nesilSayisi = 0;

            populasyonuIlkle();
            enIyiHucre = new Hucre(boyutSayisi, adimBuyuklugu, parametreAraliklari, goruntuIslemleri);

            do
            {
                sayac -= 1;
                nesilSayisi += 1;
                eski_cost = enIyiHucre.Cost;


                //for (int k = 0; k < kemotaksiSayisi; k++)
                //{
                kemotaksi();

                //}
                ureme();
                eliminasyon_dagıtım();


                if (eski_cost != enIyiHucre.Cost)
                {
                    sayac = sayacLimiti; // Reset
                }

                //Console.WriteLine("Nesil sayısı: " + nesilSayisi);
                //Console.WriteLine("En iyi varyans:" + enIyiHucre.Cost);

            } while (sayac > 0);

            enIyiKonumlar[0] = enIyiHucre.Konum[0];
            enIyiKonumlar[1] = enIyiHucre.Konum[1];
            enIyiKonumlar[2] = enIyiHucre.Konum[2];
            return enIyiHucre.Konum;
        }

        public void kemotaksi()
        {
            double Jlast;
            Hucre yenhucre = new Hucre(boyutSayisi, adimBuyuklugu, parametreAraliklari, goruntuIslemleri);

            for (int i = 0; i < populasyonBuyuklugu; i++)
            {
                etkilesim(populasyon[i]);
                Jlast = populasyon[i].Uygunluk;
                yuvarlanma(yenhucre, populasyon[i]);
                populasyon[i].Cost = goruntuIslemleri.ParametreleriDegerlendir(populasyon[i].Konum[0], populasyon[i].Konum[1], populasyon[i].Konum[2]);
                enIyiyiKontrolEt(populasyon[i]);
                etkilesim(yenhucre);

                for (int j = 0; j < boyutSayisi; j++)
                {
                    populasyon[i].Konum[j] = yenhucre.Konum[j];
                }

                populasyon[i].Cost = yenhucre.Cost;
                populasyon[i].Uygunluk = yenhucre.Uygunluk;
                populasyon[i].Can += populasyon[i].Uygunluk;

                for (int m = 0; m < yuzmeUzunlugu; m++)
                {
                    if (yenhucre.Uygunluk < Jlast)
                    {
                        Jlast = yenhucre.Uygunluk;
                        yuzme(yenhucre, populasyon[i]);
                        populasyon[i].Cost = goruntuIslemleri.ParametreleriDegerlendir(populasyon[i].Konum[0], populasyon[i].Konum[1], populasyon[i].Konum[2]);
                        enIyiyiKontrolEt(populasyon[i]);
                        etkilesim(yenhucre);

                        for (int j = 0; j < boyutSayisi; j++)
                        {
                            populasyon[i].Konum[j] = yenhucre.Konum[j];
                        }

                        populasyon[i].Cost = yenhucre.Cost;
                        populasyon[i].Uygunluk = yenhucre.Uygunluk;
                        populasyon[i].Can += populasyon[i].Uygunluk;
                    }
                    else
                        break;
                }
            }
        }

        public void ureme()
        {
            populasyonSirala();

            for (int i = populasyonBuyuklugu - uremeSayisi, j = 0; j < uremeSayisi; i++, j++)
            {
                populasyon[i] = populasyon[j];
            }

            for (int i = 0; i < populasyonBuyuklugu; i++)
            {
                populasyon[i].Can = 0.0;
            }
        }

        public void enIyiyiKontrolEt(Hucre aday)
        {
            if (aday.Cost > enIyiHucre.Cost)
            {
                enIyiHucre.Uygunluk = aday.Uygunluk;
                enIyiHucre.Cost = aday.Cost;
                enIyiHucre.Konum[0] = aday.Konum[0];
                enIyiHucre.Konum[1] = aday.Konum[1];
                enIyiHucre.Konum[2] = aday.Konum[2];

            }
        }

        public void eliminasyon_dagıtım()
        {
            for (int i = 0; i < populasyonBuyuklugu; i++)
            {

                if (rastgeleSayi(0.0, 1.0) < edIhtimali)
                {
                    for (int j = 0; j < boyutSayisi; j++)
                    {
                        populasyon[i].Konum[j] = rastgeleSayi(parametreAraliklari[j, 0], parametreAraliklari[j, 1]);
                    }
                    populasyon[i].Cost = goruntuIslemleri.ParametreleriDegerlendir(populasyon[i].Konum[0], populasyon[i].Konum[1], populasyon[i].Konum[2]);
                    enIyiyiKontrolEt(populasyon[i]);
                }
            }
        }

        private int hucreKiyasla(Hucre sol, Hucre sag)
        {
            if (sol.Can < sag.Can)
                return -1;
            if (sol.Can > sag.Can)
                return 1;
            else
                return 0;
        }

        public void etkilesim(Hucre h)
        {
            double attract = 0.0, repel = 0.0, diff = 0.0;

            for (int i = 0; i < this.populasyonBuyuklugu; i++)
            {
                for (int j = 0; j < this.boyutSayisi; j++)
                {
                    diff += Math.Pow(h.Konum[j] - this.populasyon[i].Konum[j], 2.0);
                }
                attract += 1.0 * dAttract * Math.Exp(-1.0 * wAtrract * diff);
                repel += hRepel * Math.Exp(-1.0 * wRepel * diff);
            }

            // Sürü etkisini oluşturur.
            h.Uygunluk = h.Cost + attract + repel;
        }

        public void yuvarlanma(Hucre yeniHucre, Hucre eskiHucre)
        {
            double a = -1.0, b = 1.0, temp1 = 0.0, temp2 = 0.0;

            for (int i = 0; i < this.boyutSayisi; i++)
            {
                delta[i] = rastgeleSayi(a, b); // rastgele.NextDouble() * (b - a) + a;
                temp1 += Math.Pow(delta[i], 2.0);
            }
            temp2 = Math.Sqrt(temp1);

            for (int i = 0; i < boyutSayisi; i++)
            {
                yuvarAdım[i] = delta[i] / temp2;
                yeniHucre.Konum[i] = eskiHucre.Konum[i] + eskiHucre.AdımBuyuklugu * yuvarAdım[i];
                // Normalizasyon
                yeniHucre.Konum[i] = Math.Max(Math.Min(yeniHucre.Konum[i], parametreAraliklari[i, 1]), parametreAraliklari[i, 0]);
            }
        }

        public void yuzme(Hucre yeniHucre, Hucre eskiHucre)
        {
            for (int i = 0; i < boyutSayisi; i++)
            {
                yeniHucre.Konum[i] = yeniHucre.Konum[i] + eskiHucre.AdımBuyuklugu * yuvarAdım[i];
                // Normalizasyon
                yeniHucre.Konum[i] = Math.Max(Math.Min(yeniHucre.Konum[i], parametreAraliklari[i, 1]), parametreAraliklari[i, 0]);
            }
        }

        // Shell Sort
        public void populasyonSirala()
        {
            int k = 1;
            Hucre tmpHucre;
            int gap = (int)(populasyonBuyuklugu / Math.Pow(k, 2));
            int i, j;
            while (gap > 0)
            {
                for (j = 0; j < (populasyonBuyuklugu - gap); j++)
                {
                    i = j;
                    while (i >= 0)
                    {
                        if (populasyon[i].Cost < populasyon[i + gap].Cost)
                        {
                            tmpHucre = populasyon[i];
                            populasyon[i] = populasyon[i + gap];
                            populasyon[i + gap] = tmpHucre;
                        }
                        i -= gap;
                    }
                }
                k++;
                gap = (int)(populasyonBuyuklugu / Math.Pow(k, 2));
            }
        }


        public double rastgeleSayi(double min, double max)
        {
            return rastgele.NextDouble() * (max - min) + min;
        }


        public double[,] ParametreAraliklari
        {
            get { return this.parametreAraliklari; }
            set { this.parametreAraliklari = value; }
        }

        public FonksiyonBBA DegerlendirmeFonksiyonu
        {
            get { return this.degerlendirmeFonksiyonu; }
            set { this.degerlendirmeFonksiyonu = value; }
        }

        private double UygunlukFonksiyonu(double[] parametreler)
        {
            return 0;
        }

        public void populasyonuIlkle()
        {
            for (int i = 0; i < populasyon.Length; i++)
            {
                populasyon[i] = new Hucre(boyutSayisi, adimBuyuklugu, parametreAraliklari, goruntuIslemleri);
            }
        }

    }


}
