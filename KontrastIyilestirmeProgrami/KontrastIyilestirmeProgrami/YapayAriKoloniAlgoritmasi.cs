using System;
public delegate double FonksiyonYAKA(double[] parametreler);
public class YapayAriKoloniAlgoritmasi
{
    private int besinSayisi;
    private int isciAriSayisi;
    private int gozcuAriSayisi;
    private int parametreBoyutu; 
    private double[,] parametreAraliklari;
    private double[][] besinMatrisi;
    private FonksiyonYAKA evaluationFonksiyonu;
    private Random rastgele;
    private int[] failureSayac;
    private double[] uygunlukDegerleri;
    private double[] olasilikDegerleri;
    private double[] enYuksekKaynak;

    public YapayAriKoloniAlgoritmasi()
    {
        this.besinSayisi = 40;
        this.isciAriSayisi = this.besinSayisi;
        this.gozcuAriSayisi = this.besinSayisi;
        this.parametreBoyutu = 3; // [alfa, beta, gama];
        this.parametreAraliklari = new double[3, 2] { { 0, 10 }, { -50, 50 }, { 0, 10 } }; // alfa 0-10, beta -50,50 , gama 0-10 arasında değer alabilir.
        this.besinMatrisi = new double[this.besinSayisi][];
        this.rastgele = new Random();
        this.evaluationFonksiyonu = this.UygunlukFonksiyonu;
        this.failureSayac = new int[this.besinSayisi];
        this.uygunlukDegerleri = new double[this.besinSayisi];
        this.olasilikDegerleri = new double[this.besinSayisi];
    }

    public FonksiyonYAKA DegerlendirmeFonksiyonu
    {
        get { return this.evaluationFonksiyonu; }
        set { this.evaluationFonksiyonu = value; }
    }

    public int IsciAriSayisi
    {
        get { return this.isciAriSayisi; }
        set { this.isciAriSayisi = value; }
    }

    public int GozcuAriSayisi
    {
        get { return this.gozcuAriSayisi; }
        set { this.gozcuAriSayisi = value; }
    }

    public int ParametreBoyutu
    {
        get { return this.parametreBoyutu; }
        set { this.parametreBoyutu = value; }
    }

    public double[,] ParametreAraliklari
    {
        get { return this.parametreAraliklari; }
        set { this.parametreAraliklari = value; }
    }

    public FonksiyonYAKA EvaluationFonksiyonu
    {
        get { return this.evaluationFonksiyonu; }
        set { this.evaluationFonksiyonu = value; }
    }

    public int[] CozumGelistirememeSayac
    {
        get { return this.failureSayac; }
        set { this.failureSayac = value; }
    }

    public double[] UygunlukDegerleri
    {
        get { return this.uygunlukDegerleri; }
        set { this.uygunlukDegerleri = value; }
    }

    public double[] OlasilikDegerleri
    {
        get { return this.olasilikDegerleri; }
        set { this.olasilikDegerleri = value; }
    }

    public double[][] BesinMatrisi
    {
        get { return this.besinMatrisi; }
        set { this.besinMatrisi = value; }
    }

    public double[] RastgeleBesinKaynagiUret()
    {
        double[] besinKaynagi = new double[ParametreBoyutu];
        for (int i = 0; i < ParametreBoyutu; i++)
        {
            double min = ParametreAraliklari[i, 0];
            double max = ParametreAraliklari[i, 1];
            double number = (min + (rastgele.NextDouble() * (max - min)));

            besinKaynagi[i] = number;
        }

        return besinKaynagi;
    }

    public void BesinMatrisiOlustur()
    {
        
        for (int i = 0; i < IsciAriSayisi; i++)
        {
            double[] besinKaynagi = RastgeleBesinKaynagiUret();
            BesinMatrisi[i] = besinKaynagi;
            failureSayac[i] = 0;

        }

    }

    public void IlkUygunlukDegeriHesapla()
    {
        for (int i = 0; i < this.besinSayisi; i++)
        {
            double[] parametreler = this.BesinMatrisi[i];
            double fi = this.evaluationFonksiyonu(parametreler);
            UygunlukDegerleri[i] = fi;
            
            if (fi >= 0)
            {
                this.UygunlukDegerleri[i] = 1 / (1 + fi);
            } else
            {
                this.UygunlukDegerleri[i] = 1 + Math.Abs(fi);
            }
        }
    }

    public double UygunlukDegeriHesapla(double[] kaynak)
    {
       
        double fi = this.evaluationFonksiyonu(kaynak);
      
        
        if (fi >= 0)
        {
            return 1 / (1 + fi);
        }
        else
        {
           return 1 + Math.Abs(fi);
        }

    }

    public void IsciAriFazi()
    {
        for (int i = 0; i < this.IsciAriSayisi; i++)
        {
            double[] yeniKaynak = YeniKaynakUret(BesinMatrisi[i], i);
            double yeniKaynakUygunlukDegeri = UygunlukDegeriHesapla(yeniKaynak);
            double xi_UygunlukDegeri = UygunlukDegerleri[i];
            if (yeniKaynakUygunlukDegeri > xi_UygunlukDegeri)
            {
                BesinMatrisi[i] = yeniKaynak;
                UygunlukDegerleri[i] = yeniKaynakUygunlukDegeri;
                CozumGelistirememeSayac[i] = 0;
            } else
            {
                CozumGelistirememeSayac[i] = CozumGelistirememeSayac[i] + 1;
            }
        }

    }

    public void GozcuAriFazi()
    {
        for( int i = 0; i< this.besinSayisi; i ++)
        {
            double rastgeleDeger = rastgele.NextDouble();
            if(rastgeleDeger < this.olasilikDegerleri[i])
            {
                double[] yeniKaynak = YeniKaynakUret(BesinMatrisi[i], i);
                double yeniKaynakUygunlukDegeri = UygunlukDegeriHesapla(yeniKaynak);
                double xi_UygunlukDegeri = UygunlukDegerleri[i];
                if (yeniKaynakUygunlukDegeri > xi_UygunlukDegeri)
                {
                    BesinMatrisi[i] = yeniKaynak;
                    UygunlukDegerleri[i] = yeniKaynakUygunlukDegeri;
                    CozumGelistirememeSayac[i] = 0;
                }
                else
                {
                    CozumGelistirememeSayac[i] = CozumGelistirememeSayac[i] + 1;
                }

            }
        }
    }

    private double[] YeniKaynakUret(double[] xi, int i)
    {
        double[] vi = new double[ParametreBoyutu];
        double minVal = -1;
        double maxVal = 1;

        int k = rastgele.Next(IsciAriSayisi); // 0 20 arası rastgele deger. 20 dahil değil;
        while (k == i)
        {
            k = rastgele.Next(IsciAriSayisi);
        }
        double[] xk = BesinMatrisi[k];
   
        for (int j = 0; j < ParametreBoyutu; j++)
        {
            double rastgeleDeger = rastgele.NextDouble() * (maxVal - minVal) + minVal;
            double tmp = xi[j] + (rastgeleDeger * (xi[j] - xk[j]));

            if (tmp < ParametreAraliklari[j , 0])
            {
                vi[j] = ParametreAraliklari[j, 0];
            } else if (tmp >= ParametreAraliklari[j , 0] && tmp <= ParametreAraliklari[j, 1])
            {
                vi[j] = tmp;
            } else if (tmp > ParametreAraliklari[j, 1])
            {
                vi[j] = ParametreAraliklari[j, 1];
            }
        }

        return vi;
    }

    public void OlasilikDegeriHesapla()
    {
        double totalUygunlukDegeri = 0.0;
        for (int i = 0; i < IsciAriSayisi; i++)
        {
            totalUygunlukDegeri = totalUygunlukDegeri + UygunlukDegerleri[i];
        }

        for (int i = 0; i < IsciAriSayisi; i++)
        {
            OlasilikDegerleri[i] = UygunlukDegerleri[i] / totalUygunlukDegeri;
        }
    }

    private double UygunlukFonksiyonu(double[] parametreler)
    {
        return 0;
    }

    public void EnIyiCozumuHafizadaTut()
    {
        double enYuksekDeger = this.evaluationFonksiyonu(this.enYuksekKaynak);
         
        for(int i = 0 ; i<this.isciAriSayisi; i++)
        {
            double deger = this.evaluationFonksiyonu(this.besinMatrisi[i]);
            if (deger > this.evaluationFonksiyonu(enYuksekKaynak))
            {
                this.enYuksekKaynak = this.besinMatrisi[i];
            }
        }
    }

    public Double[] IterasyonBaslat(int iterasyon)
    {
        BesinMatrisiOlustur();
        this.enYuksekKaynak = this.besinMatrisi[0];
        for (int i = 0; i< 200; i++)
        {
           IsciAriFazi();
           OlasilikDegeriHesapla();
           GozcuAriFazi();
           EnIyiCozumuHafizadaTut();
           FailureKaynakTemizle();
        }
        return this.enYuksekKaynak;
    }

    public void FailureKaynakTemizle()
    {
        for( int i = 0; i< this.besinSayisi; i++)
        {
            if(this.failureSayac[i] > 5)
            {
                Double[] yeniKaynak = this.RastgeleBesinKaynagiUret();
                this.besinMatrisi[i] = yeniKaynak;
            }
        }
    }
}