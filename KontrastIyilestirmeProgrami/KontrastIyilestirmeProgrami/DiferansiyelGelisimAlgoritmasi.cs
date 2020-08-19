using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KontrastIyilestirmeProgrami
{
    public delegate double FonksiyonDGA(double[] parametreler);

    class DiferansiyelGelisimAlgoritmasi
    {
        private int populasyonSayisi;   // N
        private int parametreSayisi;    // D
        private double[,] parametreAraliklari;
        private FonksiyonDGA degerlendirmeFonksiyonu;
        private double mutasyonFaktoru;     // F
        private double kombinasyonOlasiligi;    // CR
        private double[,] populasyon;
        private double[,] yeniPopulasyon;
        private Random rastgele;
        private int iterasyonSayisi;
        private int enUygunBireyIndis;
        private double[] enUygunBirey;
        private double maxUygunluk;
        private double[] iterasyonSonuclari;
        private int iterasyonSonuclariIndisi;
        private int iterasyonSayisiKosulu;
        private int maxIterasyon;

        public DiferansiyelGelisimAlgoritmasi()
        {
            this.populasyonSayisi = 300;
            this.parametreSayisi = 3;    // [alfa, beta, gama]
            this.parametreAraliklari = new double[3, 2] { { 0, 10 }, { -50, 50 }, { 0, 10 } };
            this.mutasyonFaktoru = 1.8;   // [0, 2] araliginda uniform
            this.kombinasyonOlasiligi = 0.8;    // [0, 1] araliginda uniform
            this.iterasyonSayisi = 1;
            this.degerlendirmeFonksiyonu = this.UygunlukFonksiyonu;
            this.rastgele = new Random();
            this.populasyon = new double[this.populasyonSayisi, this.parametreSayisi];
            this.yeniPopulasyon = new double[this.populasyonSayisi, this.parametreSayisi];
            this.enUygunBireyIndis = -1;
            this.maxUygunluk = -1;
            this.enUygunBirey = new double[this.parametreSayisi];
            this.iterasyonSayisiKosulu = 200;
            this.iterasyonSonuclari = new double[this.iterasyonSayisiKosulu];
            this.iterasyonSonuclariIndisi = 0;
            this.maxIterasyon = 2000;
        }

        public Double[] AlgoritmaCalistir()
        {
            int i, j;
            double[] donorBirey = new double[this.parametreSayisi];
            double[] denemeBirey = new double[this.parametreSayisi];
            double[] secilenBirey = new double[this.parametreSayisi];
            bool esitMi = false;

            Ilklendirme();
            while (esitMi == false && this.iterasyonSayisi <= this.maxIterasyon)
            {
                this.maxUygunluk = 0;
                // Her hedef birey icin, birbirinden ve bu hedef bireyden farklı 3 birey secilir
                for (i = 0; i < this.populasyonSayisi; i++)
                {
                    donorBirey = Mutasyon(i);
                    denemeBirey = Rekombinasyon(i, donorBirey);
                    secilenBirey = Seleksiyon(i, denemeBirey);
                    YeniPopulasyonOlustur(i, secilenBirey);
                }

                if (this.iterasyonSayisi >= this.iterasyonSayisiKosulu)
                {
                    esitMi = sonIterasyonSonuclariEsitMi();
                }


                //Console.WriteLine(this.iterasyonSayisi + ". iterasyon sonucu:" + "\t" + this.maxUygunluk);
                
                this.iterasyonSonuclari[(this.iterasyonSonuclariIndisi % this.iterasyonSayisiKosulu)] = this.maxUygunluk;
                this.iterasyonSonuclariIndisi++;
                this.iterasyonSayisi++;

                PopulasyonAktar();
            }

            // En uygun birey
            for (j = 0; j < parametreSayisi; j++)
            {
                this.enUygunBirey[j] = this.populasyon[this.enUygunBireyIndis, j];
            }
            
            return enUygunBirey;
        }

        public void Ilklendirme()
        {
            // ilk populasyon rastgele olusturulur
            for (int i = 0; i < this.populasyonSayisi; i++)
            {
                for (int j = 0; j < this.parametreSayisi; j++)
                {
                    this.populasyon[i, j] = this.rastgele.NextDouble() * (this.parametreAraliklari[j, 1] - this.parametreAraliklari[j, 0]) + this.parametreAraliklari[j, 0];
                }
            }
        }
        public double[] Mutasyon(int i)      // i: mevcutBireyIndisi
        {
            int j;
            double[] donorBirey = new double[this.parametreSayisi];
            int rastgeleBirey1Indis = -1;
            int rastgeleBirey2Indis = -1;
            int rastgeleBirey3Indis = -1;

            // Hedef bireyden ve birbirlerinden farklı rastgele 3 birey secilir
            do
            {
                rastgeleBirey1Indis = rastgele.Next(0, this.populasyonSayisi);
                rastgeleBirey2Indis = rastgele.Next(0, this.populasyonSayisi);
                rastgeleBirey3Indis = rastgele.Next(0, this.populasyonSayisi);

            } while (rastgeleBirey1Indis == i || rastgeleBirey2Indis == i || rastgeleBirey3Indis == i || rastgeleBirey1Indis == rastgeleBirey2Indis || rastgeleBirey1Indis == rastgeleBirey3Indis || rastgeleBirey2Indis == rastgeleBirey3Indis);

            // Donor birey olusturulur
            for (j = 0; j < this.parametreSayisi; j++)
            {
                donorBirey[j] = populasyon[rastgeleBirey1Indis, j] + mutasyonFaktoru * Math.Abs(populasyon[rastgeleBirey2Indis, j] - populasyon[rastgeleBirey3Indis, j]);
                if((donorBirey[j] > parametreAraliklari[j, 1]) || (donorBirey[j] < parametreAraliklari[j, 0]))
                {
                    donorBirey[j] = populasyon[rastgeleBirey1Indis, j];
                }
            }
            return donorBirey;
        }
        public double[] Rekombinasyon(int i, double[] donorBirey)
        {
            int j;
            double[] denemeBirey = new double[this.parametreSayisi];
            int rastgeleInt = rastgele.Next(0, this.parametreSayisi);
            double rastgeleDouble = rastgele.NextDouble();

            for (j = 0; j < this.parametreSayisi; j++)
            {
                if (rastgeleDouble <= this.kombinasyonOlasiligi || j == rastgeleInt)
                    denemeBirey[j] = donorBirey[j];
                else
                    denemeBirey[j] = this.populasyon[i, j];
            }
            return denemeBirey;
        }
        public double[] Seleksiyon(int i, double[] denemeBirey)
        {
            int j;
            double[] secilenBirey = new double[this.parametreSayisi];
            double[] mevcutBirey = new double[this.parametreSayisi];

            // Mevcut birey
            for (j = 0; j < parametreSayisi; j++)
                mevcutBirey[j] = this.populasyon[i, j];

            //Console.WriteLine("Deneme: alfa:" + denemeBirey[0] + " beta:" + denemeBirey[1] + " gama:" + denemeBirey[2]);
            double denemeBireyUygunluk = this.degerlendirmeFonksiyonu(denemeBirey);
            //Console.WriteLine("Mevcut: alfa:" + mevcutBirey[0] + " beta:" + mevcutBirey[1] + " gama:" + mevcutBirey[2]);
            double mevcutBireyUygunluk = this.degerlendirmeFonksiyonu(mevcutBirey);

            if (denemeBireyUygunluk >= mevcutBireyUygunluk)
            {
                for (j = 0; j < parametreSayisi; j++)
                    secilenBirey[j] = denemeBirey[j];
                if (denemeBireyUygunluk > this.maxUygunluk)
                {
                    this.maxUygunluk = denemeBireyUygunluk;
                    this.enUygunBireyIndis = i;
                }
            }
            else
            {
                for (j = 0; j < parametreSayisi; j++)
                    secilenBirey[j] = mevcutBirey[j];
                if (mevcutBireyUygunluk > this.maxUygunluk)
                {
                    this.maxUygunluk = mevcutBireyUygunluk;
                    this.enUygunBireyIndis = i;
                }
            }
            return secilenBirey;
        }
        public void YeniPopulasyonOlustur(int i, double[] secilenBirey)
        {
            int j;
            for (j = 0; j < this.parametreSayisi; j++)
                this.yeniPopulasyon[i, j] = secilenBirey[j];
        }
        public void PopulasyonAktar()
        {
            int i, j;
            for (i = 0; i < this.populasyonSayisi; i++)
            {
                for (j = 0; j < this.parametreSayisi; j++)
                {
                    this.populasyon[i, j] = this.yeniPopulasyon[i, j];
                }
            }
        }

        public bool sonIterasyonSonuclariEsitMi()
        {
            int i = 1;
            bool sonuc = true;
            double deger = this.iterasyonSonuclari[0];
            while (sonuc == true && i < this.iterasyonSonuclari.Length)
            {
                if ((int)this.iterasyonSonuclari[i] != (int)deger)
                    sonuc = false;
                i++;
            }
            return sonuc;
        }
        public FonksiyonDGA DegerlendirmeFonksiyonu
        {
            get { return this.degerlendirmeFonksiyonu; }
            set { this.degerlendirmeFonksiyonu = value; }
        }
        private double UygunlukFonksiyonu(double[] parametreler)
        {
            return 0;
        }
        public double[] EnUygunBirey
        {
            get { return this.enUygunBirey; }
            set { this.enUygunBirey = value; }
        }
        public double[,] ParametreAraliklari
        {
            get { return this.parametreAraliklari; }
            set { this.parametreAraliklari = value; }
        }
    }
}
