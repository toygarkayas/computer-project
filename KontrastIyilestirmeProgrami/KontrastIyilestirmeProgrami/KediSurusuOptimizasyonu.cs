using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KontrastIyilestirmeProgrami
{
    class Kedi
    {
        Random r;
        private double[] konum;
        private bool mod;
        private double cost;
        private double uygunluk;
        private double secilmeIhtimali;
        private double[] hiz; // Hız.
        private double c1;
        private int boyut_sayisi;
        private double[,] parametreAraliklari;
        private GoruntuIslemleri goruntuIslemleri;

        public Kedi(int boyut_sayisi, double[,] parametreAraliklari, GoruntuIslemleri goruntuIslemleri)
        {
            this.boyut_sayisi = boyut_sayisi;
            this.parametreAraliklari = parametreAraliklari;
            this.goruntuIslemleri = goruntuIslemleri;
            r = new Random();
            c1 = 0.01; // Arama modundaki hızın ivmesi.
            this.konum = new double[boyut_sayisi];
            secilmeIhtimali = 0;
            hiz = new double[boyut_sayisi];

            double aralik;
            double minimum;

            for (int i = 0; i < boyut_sayisi; i++)
            {
                aralik = parametreAraliklari[i, 1] - parametreAraliklari[i, 0];
                minimum = parametreAraliklari[i, 0];
                this.konum[i] = r.NextDouble() * (aralik) + minimum;
                this.hiz[i] = 0;
            }
            this.uygunluk = goruntuIslemleri.ParametreleriDegerlendir(this.konum[0], this.konum[1], this.konum[2]);
        }

        public void hizGuncelle(Kedi enIyi)
        {
            double r1 = r.NextDouble();
            for (int i = 0; i < konum.Length; i++)
            {
                this.hiz[i] += r1 * c1 * (enIyi.Konum[i] - this.konum[i]);

            }
        }

        public double[] getKonum()
        {
            return this.konum;
        }

        public void setKonum(double[] konum)
        {
            this.konum = konum;
        }

        public double getUygunluk()
        {
            return this.uygunluk;
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

        public double[] Hiz
        {
            get { return this.hiz; }
            set { this.hiz = value; }
        }

        public double Cost
        {
            get { return this.cost; }
            set { this.cost = value; }
        }

        public bool Mod
        {
            get { return this.mod; }
            set { this.mod = value; }
        }

        public double SecilmeIhtimali
        {
            get { return this.secilmeIhtimali; }
            set { this.secilmeIhtimali = value; }
        }
    }

    public delegate double FonksiyonKSO(double[] parametreler);

    class KediSurusuOptimizasyonu
    {
        private Random r;
        private double aramaOranı;
        private int populasyonBuyuklugu;
        private int boyutSayisi;
        private Kedi[] populasyon;
        private Kedi enIyiKedi;
        private double[] enIyiKonumlar;
        private GoruntuIslemleri goruntuIslemleri;
        private FonksiyonKSO degerlendirmeFonksiyonu;
        private double[,] parametreAraliklari;

        private int AHH;         // Arama Hafızası Havuzu
        private double[,] SBA;   // Seçilen Boyutun Arama Aralığı
        private int DBS;         // Değişen Boyutların Sayısı
        private bool KPD;        // Kendi Pozisyonunu Değerlendirme


        public KediSurusuOptimizasyonu(GoruntuIslemleri goruntuIsl)
        {
            r = new Random();
            aramaOranı = 10;
            populasyonBuyuklugu = 100;
            boyutSayisi = 3;
            enIyiKonumlar = new double[boyutSayisi];


            AHH = 20;
            KPD = false;
            DBS = boyutSayisi;
            SBA = new double[3, 2] { { -5, 5 }, { -10, 10 }, { -5, 5 } };

            this.degerlendirmeFonksiyonu = this.UygunlukFonksiyonu;
            this.goruntuIslemleri = goruntuIsl;
            //this.parametreAraliklari = parametreAra;
            this.parametreAraliklari = new double[3, 2] { { 0, 10 }, { -50, 50 }, { 0, 10 } };



        }

        public double[] aramaBaslat(int sayac)
        {
            int sayacIlkhali = sayac;
            populasyonOlustur();
            enIyiKedi = new Kedi(boyutSayisi, this.parametreAraliklari, this.goruntuIslemleri);
            double oncekiUygunluk = this.enIyiKedi.Uygunluk;

            while (sayac > 0)
            {
                if (oncekiUygunluk == this.enIyiKedi.Uygunluk)
                {
                    sayac--;
                }
                else
                {
                    oncekiUygunluk = this.enIyiKedi.Uygunluk;
                    sayac = sayacIlkhali;
                }

                modlariAta();
                hareketEt();
                uygunluklariHesapla();
                //Console.WriteLine("En iyi değer kso: " + enIyiKedi.Uygunluk + " Params: " + enIyiKedi.Konum[0] + ":" + enIyiKedi.Konum[1] + ":" + enIyiKedi.Konum[2]);

            }

            return this.enIyiKedi.Konum;
        }

        public bool hedefeUlasildi()
        {
            if (enIyiKedi.Uygunluk > 300)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void hareketEt()
        {
            for (int i = 0; i < populasyon.Length; i++)
            {
                if (populasyon[i].Mod)
                {
                    izlemeModu(populasyon[i]);
                }
                else
                {
                    aramaModu(populasyon[i]);
                }
            }

        }

        public void izlemeModu(Kedi kedicik)
        {
            kedicik.hizGuncelle(enIyiKedi);

            for (int i = 0; i < boyutSayisi; i++)
            {
                kedicik.Konum[i] += kedicik.Hiz[i];
            }
            kedicik.Konum = konumlariNormalizeEt(kedicik.Konum);
        }

        public void aramaModu(Kedi kedicik)
        {
            Kedi[] adaylar = new Kedi[AHH];

            for (int i = 0; i < adaylar.Length; i++)
            {
                //adaylar[i] = (Kedi) kedicik.Clone();
                adaylar[i] = new Kedi(boyutSayisi, parametreAraliklari, goruntuIslemleri);
                adaylar[i].Konum[0] = kedicik.Konum[0];
                adaylar[i].Konum[1] = kedicik.Konum[1];
                adaylar[i].Konum[2] = kedicik.Konum[2];

                adaylar[i].Uygunluk = kedicik.Uygunluk;
            }
            double[,] olasiKonumlar = new double[AHH, boyutSayisi];

            Kedi enIyi = new Kedi(boyutSayisi, parametreAraliklari, goruntuIslemleri);
            //Kedi enIyi = (Kedi)kedicik.Clone();
            enIyi.Konum[0] = adaylar[0].Konum[0];
            enIyi.Konum[1] = adaylar[0].Konum[1];
            enIyi.Konum[2] = adaylar[0].Konum[2];

            enIyi.Uygunluk = adaylar[0].Uygunluk;

            // Olası konumları üret.
            for (int i = 0; i < AHH; i++)
            {
                for (int j = 0; j < boyutSayisi; j++)
                {
                    olasiKonumlar[i, j] = rastgeleSayi(SBA[j, 0], SBA[j, 1]);
                    //Olası konumları yeni üretilen kedilere ata.
                    adaylar[i].Konum[j] += olasiKonumlar[i, j];
                }

                adaylar[i].Konum = konumlariNormalizeEt(adaylar[i].Konum);
                adaylar[i].Uygunluk = goruntuIslemleri.ParametreleriDegerlendir(adaylar[i].Konum[0], adaylar[i].Konum[1], adaylar[i].Konum[2]);
                //secilmeIhtimaliHesapla(adaylar[i]);

                if (enIyi.Uygunluk < adaylar[i].Uygunluk)
                {
                    enIyi.Konum = adaylar[i].getKonum();
                    enIyi.Uygunluk = adaylar[i].getUygunluk();
                }
            }
            kedicik.Konum = enIyi.getKonum();
            kedicik.Uygunluk = enIyi.getUygunluk();
        }

        public void uygunluklariHesapla()
        {
            for (int i = 0; i < populasyon.Length; i++)
            {
                populasyon[i].Uygunluk = goruntuIslemleri.ParametreleriDegerlendir(populasyon[i].Konum[0], populasyon[i].Konum[1], populasyon[i].Konum[2]);

                if (enIyiKedi.Uygunluk < populasyon[i].Uygunluk)
                {
                    enIyiKedi.Konum = populasyon[i].getKonum();
                    enIyiKedi.Uygunluk = populasyon[i].getUygunluk();
                }
            }
        }

        public void modlariAta()
        {
            int aramaIhtimali;
            for (int i = 0; i < populasyon.Length; i++)
            {
                aramaIhtimali = Convert.ToInt32(rastgeleSayi(0, 100));
                if (aramaIhtimali < aramaOranı)
                {
                    populasyon[i].Mod = true;
                }
                else
                {
                    populasyon[i].Mod = false;
                }
            }
        }

        public void populasyonOlustur()
        {
            populasyon = new Kedi[populasyonBuyuklugu];

            for (int i = 0; i < populasyonBuyuklugu; i++)
            {
                populasyon[i] = new Kedi(boyutSayisi, parametreAraliklari, goruntuIslemleri);
            }

        }

        public double[] konumlariNormalizeEt(double[] konumlar)
        {
            double[] min = new double[] { this.parametreAraliklari[0, 0], this.parametreAraliklari[1, 0], this.parametreAraliklari[2, 0] };
            double[] max = new double[] { this.parametreAraliklari[0, 1], this.parametreAraliklari[1, 1], this.parametreAraliklari[2, 1] };


            for (int i = 0; i < boyutSayisi; i++)
            {
                //konumlar[i] = (konumlar[i] - min[i])/(max[i] - min[i])
                konumlar[i] = Math.Min(Math.Max(this.parametreAraliklari[i, 0], konumlar[i]), this.parametreAraliklari[i, 1]);
            }

            return konumlar;
        }


        public void secilmeIhtimaliHesapla(Kedi kedicik)
        {
            kedicik.SecilmeIhtimali = kedicik.Uygunluk;
        }

        public double rastgeleSayi(double min, double max)
        {
            return (r.NextDouble() * (max - min) + min + 0.0000001);
        }

        private double UygunlukFonksiyonu(double[] parametreler)
        {
            return 0;
        }

        public double[,] ParametreAraliklari
        {
            get { return this.parametreAraliklari; }
            set { this.parametreAraliklari = value; }
        }

        public FonksiyonKSO DegerlendirmeFonksiyonu
        {
            get { return this.degerlendirmeFonksiyonu; }
            set { this.degerlendirmeFonksiyonu = value; }
        }
    }
}
