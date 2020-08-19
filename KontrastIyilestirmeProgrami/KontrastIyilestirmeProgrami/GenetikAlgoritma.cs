using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KontrastIyilestirmeProgrami
{
    public delegate double FonksiyonGA(double[] parametreler);

    public class GenetikAlgoritma
    {
        private int populasyonSayisi;
        private int kromozomBoyutu;
        private double[,] parametreAraliklari;
        private FonksiyonGA degerlendirmeFonksiyonu;
        private int mutasyonOlasiligi;
        private double[,] populasyon;
        private double[,] secilenPopulasyon;
        private int[] yeniPopulasyonIndisler;
        private double[] populasyonUygunluk;
        private Random rastgele;
        private double toplamUygunlukDegeri;
        private double[] enUygunBirey;
        private double[] iterasyonSonuclari;
        private int iterasyonSonuclariIndisi;
        private int iterasyonSayisiKosulu;

        public GenetikAlgoritma()
        {
            this.populasyonSayisi = 300;
            this.kromozomBoyutu = 3;    // [alfa, beta, gama]
            this.parametreAraliklari = new double[3, 2] { { 0, 10 }, { -50, 50 }, { 0, 10 } };
            this.degerlendirmeFonksiyonu = this.UygunlukFonksiyonu;
            this.populasyonUygunluk = new double[this.populasyonSayisi];
            this.rastgele = new Random();
            this.mutasyonOlasiligi = 3;
            this.iterasyonSayisiKosulu = 100;
            this.iterasyonSonuclari = new double[this.iterasyonSayisiKosulu];
            this.iterasyonSonuclariIndisi = 0;
        }

        public double ToplamUygunlukDegeri
        {
            get { return this.toplamUygunlukDegeri; }
            set { this.toplamUygunlukDegeri = value; }
        }

        public int KromozomBoyutu
        {
            get { return this.kromozomBoyutu; }
            set { this.kromozomBoyutu = value; }
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

        public FonksiyonGA DegerlendirmeFonksiyonu
        {
            get { return this.degerlendirmeFonksiyonu; }
            set { this.degerlendirmeFonksiyonu = value; }
        }
        
        public int MutasyonOlasiligi
        {
            get { return this.mutasyonOlasiligi; }
            set { this.mutasyonOlasiligi = value; }
        }

        public int PopulasyonSayisi
        {
            get { return this.populasyonSayisi; }
            set { this.populasyonSayisi = value; }
        }

        private double UygunlukFonksiyonu(double[] parametreler)
        {
            return 0;
        }

        public void PopulasyonOlustur()
        {
            this.populasyon = new double[this.populasyonSayisi, this.kromozomBoyutu];

            // ilk populasyon rastgele olusturulur
            for (int i = 0; i < this.populasyonSayisi; i++)
            {
                for (int j = 0; j < this.kromozomBoyutu; j++)
                {
                    this.populasyon[i, j] = this.rastgele.NextDouble() * (this.parametreAraliklari[j, 1] - this.parametreAraliklari[j, 0]) + this.parametreAraliklari[j, 0];
                }
            }
        }
        
        public void PopulasyonDegerlendir()
        {
            int i, j, enUygunBireyIndis = 0;
            this.toplamUygunlukDegeri = 0.0;
            double[] parametreler = new double[this.kromozomBoyutu];
            this.enUygunBirey = new double[this.kromozomBoyutu];
            double max = 0.0;

            for (i = 0; i < this.populasyonSayisi; i++)
            {
                for (j = 0; j < this.kromozomBoyutu; j++)
                {
                    parametreler[j] = this.populasyon[i, j];
                }
                this.populasyonUygunluk[i] = this.degerlendirmeFonksiyonu(parametreler);

                // enUygunDegeri bulur ve indisini saklar
                if (this.populasyonUygunluk[i] > max)
                { 
                    max = this.populasyonUygunluk[i];
                    enUygunBireyIndis = i;
                }

                this.toplamUygunlukDegeri += this.populasyonUygunluk[i];
            }

            this.iterasyonSonuclari[(this.iterasyonSonuclariIndisi % this.iterasyonSayisiKosulu)] = max;   // en uygun bireyin uygunluk degerini saklar)
            //Console.WriteLine(max);
            
 
            // en uygun bireyin degerlerini saklar
            for (j = 0; j < this.kromozomBoyutu; j++)
            {
                this.enUygunBirey[j] = this.populasyon[enUygunBireyIndis, j];
            }
        }

        public void YeniPopulasyonOlustur()
        {
            int i, j;
            double rastgeleSayi;
            double toplam;
            this.yeniPopulasyonIndisler = new int[this.populasyonSayisi];
            this.secilenPopulasyon = new double[this.populasyonSayisi, this.kromozomBoyutu];

            for (i = 0; i < this.populasyonSayisi; i++)
            {
                rastgeleSayi = rastgele.NextDouble() * this.toplamUygunlukDegeri;
                toplam = 0.0;
                for(j = 0; j < this.populasyonSayisi; j++)
                {
                    toplam += this.populasyonUygunluk[j];
                    if (rastgeleSayi <= toplam)
                    {
                        this.yeniPopulasyonIndisler[i] = j;
                        break;
                    }
                }
            }
            /* Secilen indislerdeki bireyler yeni bir diziye aktarılır (secilenPopulasyon) */
            for (i = 0; i < this.populasyonSayisi; i++)
            {
                for (j = 0; j < this.kromozomBoyutu; j++)
                {
                    this.secilenPopulasyon[i, j] = this.populasyon[this.yeniPopulasyonIndisler[i], j];
                }
            }

            /* Secilen populasyonu sabit olan populasyon dizisine aktarır */
            PopulasyonAktar();
        }

        public void PopulasyonAktar()
        {
            int i, j;
            for (i = 0; i < this.populasyonSayisi; i++)
            {
                for (j = 0; j < this.kromozomBoyutu; j++)
                {
                    this.populasyon[i, j] = this.secilenPopulasyon[i, j];
                }
            }
        }

        public void PopulasyonCaprazla()
        {
            int i, j, caprazlamaYeri;
            double temp;

            for (i = 0; i < this.populasyonSayisi; i += 2)
            {
                caprazlamaYeri = rastgele.Next(1, 3);   // [1, 2] aralığında bir sayı üretir
                for (j = caprazlamaYeri; j < this.kromozomBoyutu; j++)
                {
                    // swapping
                    temp = this.populasyon[i, j];
                    this.populasyon[i, j] = this.populasyon[i + 1, j];
                    this.populasyon[i + 1, j] = temp;
                }
            }
        }

        public void PopulasyonMutasyonaUgrat()
        {
            int i, j, k, mutasyonSayisi;
            mutasyonSayisi = this.populasyonSayisi * this.kromozomBoyutu * this.mutasyonOlasiligi / 100;    // kac adet parametrenin mutasyona ugrayacagini belirtir
            
            for (k = 0; k < mutasyonSayisi; k++)
            {
                i = rastgele.Next(0, this.populasyonSayisi); // i: degistirilecek bireyin indisini gosterir
                j = rastgele.Next(0, 3);    // j: degistirilecek parametrenin indisini gosterir
                this.populasyon[i, j] = this.rastgele.NextDouble() * (this.parametreAraliklari[j, 1] - this.parametreAraliklari[j, 0]) + this.parametreAraliklari[j, 0];
            }
        }
        
        public bool sonIterasyonSonuclariEsitMi(double[] iterasyonSonuclari)
        {
            int i = 1;
            bool sonuc = true;
            double deger = iterasyonSonuclari[0];
            while (sonuc == true && i < iterasyonSonuclari.Length)
            {
                if ((int)iterasyonSonuclari[i] != (int)deger)
                {
                    sonuc = false;
                }
                i++;
            }
            return sonuc;
        }

        public Double[] IterasyonBaslat()
        {
            int iterasyonSayaci = 1;
            PopulasyonOlustur();
            bool esitMi = false;

            while (esitMi == false) {

                PopulasyonDegerlendir();
                YeniPopulasyonOlustur();
                PopulasyonCaprazla();
                PopulasyonMutasyonaUgrat();
                if (iterasyonSayaci >= this.iterasyonSayisiKosulu)
                {
                    esitMi = sonIterasyonSonuclariEsitMi(this.iterasyonSonuclari);
                }

                this.iterasyonSonuclariIndisi++;
                iterasyonSayaci++;
            }

            return enUygunBirey;
        }
        
    }
}
