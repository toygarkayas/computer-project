using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KontrastIyilestirmeProgrami
{
    public delegate double FonksiyonPSOA(double[] parametreler);

    class ParcacikSuruOptAlgoritmasi
    {
        private FonksiyonPSOA degerlendirmeFonksiyonu;
        private Random rastgele;
        private double[,] parametreAraliklari;
        private int parametreSayisi;
        private int parcacikSayisi;
        private double maxHizAlfaGama;
        private double minHizAlfaGama;
        private double maxHizBeta;
        private double minHizBeta;
        private double c1;  // Ogrenme faktoru 1: Parcacigin yerel en iyisinin carpilacagi deger
        private double c2;  // OGrenme faktoru 2: Parcacigin kuresel en iyisinin carpilacagi deger
        private double[,] konumlar;
        private double[,] hizlar;
        private double[,] yerelEnIyiDegerlerVeKonumlar;
        private double[] globalEnIyiDegerVeKonum;  // [Uyguluk degeri, alfa, beta, gama]
        private double[] enIyiKonum;
        private double sinirKontrolSabiti;
        private double hizSinirSabiti;
        private double[] uygunluklar;
        private double[] iterasyonSonuclari;
        private int iterasyonSonuclariIndisi;
        private int iterasyonSayisiKosulu;

        public ParcacikSuruOptAlgoritmasi()
        {
            this.parametreAraliklari = new double[3, 2] { { 0, 10 }, { -50, 50 }, { 0, 10 } };
            this.degerlendirmeFonksiyonu = this.UygunlukFonksiyonu;
            this.rastgele = new Random();
            this.parametreSayisi = 3;
            this.parcacikSayisi = 40;
            this.c1 = 2;  // [0,2] arasinda
            this.c2 = 2;  // c1 + c2 = 4 olmasi yapilan deneylere gore daha iyi bir sonuc verdigi gorulmustur
            this.sinirKontrolSabiti = 0.3;  // [0,1] arasinda
            this.hizSinirSabiti = 0.2;
            this.konumlar = new double[this.parcacikSayisi, this.parametreSayisi];
            this.hizlar = new double[this.parcacikSayisi, this.parametreSayisi];
            this.uygunluklar = new double[this.parcacikSayisi];
            this.yerelEnIyiDegerlerVeKonumlar = new double[this.parcacikSayisi, this.parametreSayisi + 1]; // 1 degeri uygunluk degeri icindir
            this.globalEnIyiDegerVeKonum = new double[this.parametreSayisi + 1];  // 1 degeri uygunluk degeri icindir
            this.globalEnIyiDegerVeKonum[0] = 0.0;
            this.enIyiKonum = new double[this.parametreSayisi]; 
            this.iterasyonSayisiKosulu = 250; //Son 250 iterasyonun sonucu eşitse sonuç döndürülecek.
            this.iterasyonSonuclari = new double[this.iterasyonSayisiKosulu];
            this.iterasyonSonuclariIndisi = 0;
        }

        public Double[] AlgoritmaCalistir()
        {
            ParcaciklariOlustur();
            HizSinirlariniBelirle();
            bool esitMi = false;

            int iterasyon = 0;

            while (esitMi == false)
            {
                UygunluklariHesapla();
                YerelEnIyileriGuncelle();
                GlobalEnIyiGuncelle();
                //Console.WriteLine(iterasyon + ". iterasyon sonucu: " + globalEnIyiDegerVeKonum[0]);
                ParcacikHizlariniGuncelle();
                ParcacikKonumlariniGuncelle();

                
                if (iterasyon >= this.iterasyonSayisiKosulu)
                {
                    esitMi = sonIterasyonSonuclariEsitMi();
                }

                this.iterasyonSonuclari[(this.iterasyonSonuclariIndisi % this.iterasyonSayisiKosulu)] = this.globalEnIyiDegerVeKonum[0];
                this.iterasyonSonuclariIndisi++;
                iterasyon++;
            }
            
            for (int j = 0; j < this.parametreSayisi; j++)
                this.enIyiKonum[j] = this.globalEnIyiDegerVeKonum[j+1];
            return enIyiKonum;
        }

        public void ParcaciklariOlustur()
        {
            /* Parcaciklarin konumlari this.rastgele atanir ve hizlari sıfırlanır. */
            for (int i = 0; i < this.parcacikSayisi; i++)
            {
                this.yerelEnIyiDegerlerVeKonumlar[i, 0] = 0.0;  // Baslangic yerel en iyi uygunluk degeri
                for (int j = 0; j < this.parametreSayisi; j++)
                { 
                    this.konumlar[i, j] = this.rastgele.NextDouble() * (this.parametreAraliklari[j, 1] - this.parametreAraliklari[j, 0]) + this.parametreAraliklari[j, 0];
                    this.yerelEnIyiDegerlerVeKonumlar[i, j + 1] = this.konumlar[i, j];
                    this.hizlar[i, j] = 0.0;  // Baslangic hizlari 0 olarak ilklendirilir
                }
            }
        }

        public void ParcacikHizlariniGuncelle()
        {
            double hizYeni = 0.0;
            double random1, random2;

            /*  */
            for (int i = 0; i < this.parcacikSayisi; i++)
            {
                for (int j = 0; j < this.parametreSayisi; j++)
                {
                    random1 = this.rastgele.NextDouble();
                    random2 = this.rastgele.NextDouble();
                    hizYeni = this.hizlar[i, j] + this.c1 * random1 * (this.yerelEnIyiDegerlerVeKonumlar[i, j+1] - this.konumlar[i, j]) + this.c2 * random2 * (this.globalEnIyiDegerVeKonum[j+1] - this.konumlar[i, j]);                    
                    if (j == 1)
                    {
                        if (hizYeni > this.maxHizBeta)
                            hizYeni = this.maxHizBeta;
                        else if (hizYeni < this.minHizBeta)
                            hizYeni = this.minHizBeta;
                    }
                    else
                    {
                        if (hizYeni > this.maxHizAlfaGama)
                            hizYeni = this.maxHizAlfaGama;
                        else if (hizYeni < this.minHizAlfaGama)
                            hizYeni = this.minHizAlfaGama;
                    }

                    this.hizlar[i, j] = hizYeni;                   
                    hizYeni = 0.0;
                }
            }
        }
        
        public void ParcacikKonumlariniGuncelle()
        {
            double konumYeni = 0.0;
            double random; 

            /*  */
            for (int i = 0; i < this.parcacikSayisi; i++)
            {
                for (int j = 0; j < this.parametreSayisi; j++)
                {
                    konumYeni = this.konumlar[i, j] + this.hizlar[i, j];
                    if(konumYeni > this.parametreAraliklari[j, 1])
                    {
                        random = this.rastgele.NextDouble();
                        konumYeni = konumYeni - this.sinirKontrolSabiti * random * (this.parametreAraliklari[j, 1] - this.parametreAraliklari[j, 0]);
                        if (konumYeni > this.parametreAraliklari[j, 1])
                        {
                            konumYeni = this.parametreAraliklari[j, 1];
                        }
                    }
                    else if (konumYeni < parametreAraliklari[j, 0])
                    {
                        random = this.rastgele.NextDouble();
                        konumYeni = konumYeni + this.sinirKontrolSabiti * random * (this.parametreAraliklari[j, 1] - this.parametreAraliklari[j, 0]);
                        if (konumYeni < this.parametreAraliklari[j, 0])
                        {
                            konumYeni = this.parametreAraliklari[j, 0];
                        }
                    }

                    this.konumlar[i, j] = konumYeni;
                    konumYeni = 0.0; 
                }
            }
        }

        public void GlobalEnIyiGuncelle()
        {
            for(int i = 0; i < this.parcacikSayisi; i++)
            {
                if(this.yerelEnIyiDegerlerVeKonumlar[i, 0] > this.globalEnIyiDegerVeKonum[0])
                {
                    this.globalEnIyiDegerVeKonum[0] = this.yerelEnIyiDegerlerVeKonumlar[i, 0];
                    for(int j = 0; j < parametreSayisi; j++)                   
                        this.globalEnIyiDegerVeKonum[j + 1] = this.konumlar[i, j];                    
                }
            }
        }

        public void YerelEnIyileriGuncelle()
        {
            for (int i = 0; i < this.parcacikSayisi; i++)
            {
                if (this.uygunluklar[i] > this.yerelEnIyiDegerlerVeKonumlar[i, 0])
                    this.yerelEnIyiDegerlerVeKonumlar[i, 0] = this.uygunluklar[i];
            }
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

            for (i = 0; i < this.parcacikSayisi; i++)
            {
                for (j = 0; j < this.parametreSayisi; j++)
                    parametreler[j] = this.konumlar[i, j];
                this.uygunluklar[i] = this.degerlendirmeFonksiyonu(parametreler);
            }
        }

        public bool sonIterasyonSonuclariEsitMi()
        {
            int i = 1;
            bool sonuc = true;
            double deger = this.iterasyonSonuclari[0];
            while (sonuc == true && i < this.iterasyonSonuclari.Length)
            {
                if (this.iterasyonSonuclari[i] != deger)
                    sonuc = false;
                i++;
            }
            return sonuc;
        }
        private double UygunlukFonksiyonu(double[] parametreler)
        {
            return 0;
        }
        public FonksiyonPSOA DegerlendirmeFonksiyonu
        {
            get { return this.degerlendirmeFonksiyonu; }
            set { this.degerlendirmeFonksiyonu = value; }
        }
        public double[,] ParametreAraliklari
        {
            get { return this.parametreAraliklari; }
            set { this.parametreAraliklari = value; }
        }
    }
}
