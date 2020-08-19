using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KontrastIyilestirmeProgrami
{

    public delegate double FonksiyonBTA(double[] parametreler);

    class BenzetimliTavlamaAlgoritmasi
    {
        private int parametreSayisi;
        private double[,] parametreAraliklari;
        private FonksiyonBTA degerlendirmeFonksiyonu;
        private Random rastgele;
        private double sicaklik; // this.sicaklik degeri
        private double sicaklikAzaltmaSabiti;  // sicakligi degerini azaltmak icin degeri carpacagimiz sabit
        private double minSicaklik; // this.sicaklik degerinin ulasinca duracagi en kücük deger
        private double[] cozumYeni;
        private double[] cozum;
        private double uygunlukYeni;
        private double uygunluk;
        private double alfaGamaDegisimMiktari;
        private double betaDegisimMiktari;
        private int iterasyonSayisi;
        private double iterasyonSayisiAzaltmaSabiti;
       
        public BenzetimliTavlamaAlgoritmasi()
        {
            this.parametreAraliklari = new double[3, 2] { { 0, 10 }, { -50, 50 }, { 0, 10 } };
            this.degerlendirmeFonksiyonu = this.UygunlukFonksiyonu;
            this.rastgele = new Random();
            this.iterasyonSayisi = 500;
            this.iterasyonSayisiAzaltmaSabiti = 0.97;
            this.parametreSayisi = 3;
            this.sicaklik = 1000.0;
            this.sicaklikAzaltmaSabiti = 0.98;
            this.minSicaklik = 1.0;
            this.alfaGamaDegisimMiktari = 0.3;  // verilen miktarin 0.1 fazlasina kadar cikabilir
            this.betaDegisimMiktari = 4;
            this.cozum = new double[this.parametreSayisi];
            this.cozumYeni = new double[this.parametreSayisi];
        }

        /* Baslangicta rastgele bir cozum uretilir */
        public void BaslangicCozumUret()
        {
            int i;
            for (i = 0; i < this.parametreSayisi; i++)
                this.cozum[i] = this.rastgele.NextDouble() * (this.parametreAraliklari[i, 1] - this.parametreAraliklari[i, 0]) + this.parametreAraliklari[i, 0];
        }

        /* Uzerinde bulunulan cozume komsu cozum uretir */
        public void KomsuCozumUret()
        {
            int i;
            int ileriMiGeriMi;  // komsu deger secilirken ileriden mi yoksa geriden mi komsu degerin secilecegini belirtir
            double rastgeleDegisimMiktari;

            for (i = 0; i < this.parametreSayisi; i++)
            {
                ileriMiGeriMi = this.rastgele.Next(2);  // 0: geri, 1: ileri

                if (i == 0 || i == 2) // alfa ya da gama ise
                {
                    rastgeleDegisimMiktari = Math.Round((this.rastgele.NextDouble() * alfaGamaDegisimMiktari), 1) + 0.1;
                    if (ileriMiGeriMi == 0)
                    {
                        this.cozumYeni[i] = cozum[i] - rastgeleDegisimMiktari; // gerideki komsusunu secer
                        if (this.cozumYeni[i] < parametreAraliklari[0, 0]) // alfa veya gama icin yazilabilir (parametreAraliklari[2, 0])
                            this.cozumYeni[i] = parametreAraliklari[0, 0];
                    }
                    else
                    {
                        this.cozumYeni[i] = cozum[i] + rastgeleDegisimMiktari;  // ilerideki komsusunu secer
                        if (this.cozumYeni[i] > parametreAraliklari[0, 1])
                            this.cozumYeni[i] = parametreAraliklari[0, 1];
                    }
                }
                else  // beta ise 
                {
                    rastgeleDegisimMiktari = (double)this.rastgele.Next(1, (int)(betaDegisimMiktari + 1));
                    if (ileriMiGeriMi == 0)
                    {
                        this.cozumYeni[i] = cozum[i] - rastgeleDegisimMiktari;
                        if (this.cozumYeni[i] < parametreAraliklari[1, 0])
                            this.cozumYeni[i] = parametreAraliklari[1, 0];
                    }
                    else
                    {
                        this.cozumYeni[i] = cozum[i] + rastgeleDegisimMiktari;
                        if (this.cozumYeni[i] > parametreAraliklari[1, 1])
                            this.cozumYeni[i] = parametreAraliklari[1, 1];
                    }
                }
            }
            this.uygunlukYeni = this.degerlendirmeFonksiyonu(this.cozumYeni);  // yeni cozumun uygunluk degeri hesaplanir
        }
       
        /* Bulunan cozum ile ona komsu olan cozumu karsilastirir. Komsu cozum daha iyi sonuc verirse bunu yeni cozum olarak secer, eger daha kotu sonuc verirse belli bir olasiliga gore bunu yeni cozum olarak secer */
        public void CozumleriKarsilastirVeYeniCozumuSec()
        {
            int i;
            double kotuCozumSecilmeEsikDegeri;
            double rastgeleOlasilik;  // kotu cozumun secilip secilmeyecegini bulmak icin uretilen rastgele olasilik degeri

            if (this.uygunlukYeni >= this.uygunluk)  // komsu cozumun uygunlugu buyukse komsu cozum yeni cozum olarak secilir
            {
                for (i = 0; i < parametreSayisi; i++)
                    this.cozum[i] = this.cozumYeni[i];
            }
            else  // komsu cozum eski cozumden kotuyse belli bir olasilikla kotu cozum secilir veya secilmez
            {
                kotuCozumSecilmeEsikDegeri = 1 / Math.Exp(Math.Abs(uygunlukYeni - uygunluk) / this.sicaklik);  // Sicaklik azaldikca kotu cozum secme olasiligi azaliyor
                rastgeleOlasilik = this.rastgele.NextDouble();
                if (rastgeleOlasilik < kotuCozumSecilmeEsikDegeri)  // kotu secim secilir
                {
                    for (i = 0; i < parametreSayisi; i++)
                        this.cozum[i] = this.cozumYeni[i];
                }  // else'e gerek yok cunku eski cozum uzerinden devam edilecek
            }
        }

        /* Belli bir sabite gore sicakligi dusurur */
        public void SicaklikAzalt()
        {
            this.sicaklik = this.sicaklik * this.sicaklikAzaltmaSabiti;  // sicaklik azaltma fonksiyonu
        }

        /* Belli bir sabite gore iterasyon sayisini dusurur */
        public void IterasyonSayisiAzalt()
        {
            if ((int)(this.iterasyonSayisiAzaltmaSabiti * this.iterasyonSayisi) > 50)
                this.iterasyonSayisi = (int)(this.iterasyonSayisiAzaltmaSabiti * this.iterasyonSayisi);  // iterasyon sayisi azaltma fonksiyonu
        }

        public Double[] AlgoritmaCalistir()
        {
            int i;
           
            BaslangicCozumUret();

            // Sicaklik minimum degere ulasincaya kadar dongu doner
            while (this.sicaklik > this.minSicaklik)
            {
                for (i = 0; i < this.iterasyonSayisi; i++)
                {
                    this.uygunluk = this.degerlendirmeFonksiyonu(cozum);
                    KomsuCozumUret();
                    CozumleriKarsilastirVeYeniCozumuSec();

                    //Console.WriteLine(i + ". iterasyon sonucu: " + this.uygunluk);
                }
                SicaklikAzalt();
                IterasyonSayisiAzalt();
            }
            return cozum;
        }
        
        public FonksiyonBTA DegerlendirmeFonksiyonu
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
