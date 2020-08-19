using System;
using System.Drawing;
public class GoruntuIslemleri
{
    private int[, ,] resimX;
    private int pikselSayisi;
    private int[,] ortResimX;
    private int[,] ortResimY;
    private int[,] ortResimZ;
    private int[] histogramX;
    private int[] histogramY;
    private int[] histogramZ;
    private double meanX;
    private double meanY;
    private double meanZ;
    private int medianX;
    private int medianY;
    private int medianZ;
    private double stdSapmaX;
    private double stdSapmaY;
    private double stdSapmaZ;
    private double mse;
    private double psnr;
    private double ssim;
    private double msez;
    private double psnrz;
    private double ssimz;

    public double MeanX
    {
        get { return this.meanX; }
    }
    public double MeanY
    {
        get { return this.meanY; }
    }
    public double MeanZ
    {
        get { return this.meanZ; }
    }
    public int MedianX
    {
        get { return this.medianX; }
    }
    public int MedianY
    {
        get { return this.medianY; }
    }
    public int MedianZ
    {
        get { return this.medianZ; }
    }
    public double StdSapmaX
    {
        get { return this.stdSapmaX; }
    }
    public double StdSapmaY
    {
        get { return this.stdSapmaY; }
    }
    public double StdSapmaZ
    {
        get { return this.stdSapmaZ; }
    }
    public double MSE
    {
        get { return this.mse; }
    }
    public double PSNR
    {
        get { return this.psnr; }
    }
    public double SSIM
    {
        get { return this.ssim; }
    }
    public double MSEZ
    {
        get { return this.msez; }
    }
    public double PSNRZ
    {
        get { return this.psnrz; }
    }
    public double SSIMZ
    {
        get { return this.ssimz; }
    }
    public Bitmap ResimX
    {
        set
        {
            this.resimX = new int[value.Height, value.Width, 3];
            this.pikselSayisi = value.Width * value.Height;
            this.ortResimX = new int[value.Height, value.Width];
            this.histogramX = new int[256];
            for (int y = 0; y < value.Height; y++)
                for (int x = 0; x < value.Width; x++)
                {
                    Color renkX = value.GetPixel(x, y);
                    this.resimX[y, x, 0] = renkX.R;
                    this.resimX[y, x, 1] = renkX.G;
                    this.resimX[y, x, 2] = renkX.B;
                    int ortRenkX = Convert.ToInt32(0.299 * renkX.R + 0.587 * renkX.G + 0.114 * renkX.B);
                    this.ortResimX[y, x] = ortRenkX;
                    this.histogramX[ortRenkX]++;
                }
            this.meanX = 0;
            this.medianX = -1;
            int ortaDeger = this.pikselSayisi / 2;
            this.stdSapmaX = 0;
            for (int i = 0; i < 256; i++)
            {
                this.meanX += i * this.histogramX[i];
                ortaDeger -= this.histogramX[i];
                this.medianX = ((this.medianX == -1) && (ortaDeger <= 0)) ? i : this.medianX;
                this.stdSapmaX += (i * i) * (double)this.histogramX[i];
            }
            this.meanX /= this.pikselSayisi;
            this.stdSapmaX /= this.pikselSayisi;
            this.stdSapmaX -= this.meanX * this.meanX;
            this.stdSapmaX = Math.Sqrt(this.stdSapmaX);
        }
    }
    public Bitmap ResimY
    {
        set
        {
            this.pikselSayisi = value.Width * value.Height;
            this.ortResimY = new int[value.Height, value.Width];
            this.histogramY = new int[256];
            for (int y = 0; y < value.Height; y++)
                for (int x = 0; x < value.Width; x++)
                {
                    Color renkY = value.GetPixel(x, y);
                    int ortRenkY = Convert.ToInt32(0.299 * renkY.R + 0.587 * renkY.G + 0.114 * renkY.B);
                    this.ortResimY[y, x] = ortRenkY;
                    this.histogramY[ortRenkY]++;
                }
            this.meanY = 0;
            this.medianY = -1;
            int ortaDeger = this.pikselSayisi / 2;
            this.stdSapmaY = 0;
            for (int i = 0; i < 256; i++)
            {
                this.meanY += i * this.histogramY[i];
                ortaDeger -= this.histogramY[i];
                this.medianY = ((this.medianY == -1) && (ortaDeger <= 0)) ? i : this.medianY;
                this.stdSapmaY += (i * i) * (double)this.histogramY[i];
            }
            this.meanY /= this.pikselSayisi;
            this.stdSapmaY /= this.pikselSayisi;
            this.stdSapmaY -= this.meanY * this.meanY;
            this.stdSapmaY = Math.Sqrt(this.stdSapmaY);
        }
    }
    public Bitmap ResimZ
    {
        set
        {
            this.pikselSayisi = value.Width * value.Height;
            this.ortResimZ = new int[value.Height, value.Width];
            this.histogramZ = new int[256];
            for (int y = 0; y < value.Height; y++)
                for (int x = 0; x < value.Width; x++)
                {
                    Color renkZ = value.GetPixel(x, y);
                    int ortRenkZ = Convert.ToInt32(0.299 * renkZ.R + 0.587 * renkZ.G + 0.114 * renkZ.B);
                    this.ortResimZ[y, x] = ortRenkZ;
                    this.histogramZ[ortRenkZ]++;
                }
            this.meanZ = 0;
            this.medianZ = -1;
            int ortaDeger = this.pikselSayisi / 2;
            this.stdSapmaZ = 0;
            for (int i = 0; i < 256; i++)
            {
                this.meanZ += i * this.histogramZ[i];
                ortaDeger -= this.histogramZ[i];
                this.medianZ = ((this.medianZ == -1) && (ortaDeger <= 0)) ? i : this.medianZ;
                this.stdSapmaZ += (i * i) * (double)this.histogramZ[i];
            }
            this.meanZ /= this.pikselSayisi;
            this.stdSapmaZ /= this.pikselSayisi;
            this.stdSapmaZ -= this.meanZ * this.meanZ;
            this.stdSapmaZ = Math.Sqrt(this.stdSapmaZ);
        }
    }
    public void ResimleriKarsilastir()
    {
        if ((this.histogramX == null) || (this.histogramY == null))
            return;
        int genislik = this.ortResimX.GetLength(1);
        int yukseklik = this.ortResimX.GetLength(0);
        int cerceveBoyutu = 8;
        int cerceveAlani = 64;
        double c1 = 6.5025;
        double c2 = 58.5225;
        this.mse = 0;
        this.psnr = 1;
        this.ssim = 0;
        for (int rY = 0; rY <= yukseklik - cerceveBoyutu; rY++)
            for (int rX = 0; rX <= genislik - cerceveBoyutu; rX++)
            {
                this.mse += (this.ortResimX[rY, rX] - this.ortResimY[rY, rX]) * (this.ortResimX[rY, rX] - this.ortResimY[rY, rX]);
                int ortX = 0;
                int ortY = 0;
                for (int cY = rY; cY < rY + cerceveBoyutu; cY++)
                    for (int cX = rX; cX < rX + cerceveBoyutu; cX++)
                    {
                        ortX += this.ortResimX[cY, cX];
                        ortY += this.ortResimY[cY, cX];
                    }
                ortX /= cerceveAlani;
                ortY /= cerceveAlani;
                double varX = 0.0;
                double varY = 0.0;
                double cvXY = 0.0;
                for (int cY = rY; cY < rY + cerceveBoyutu; cY++)
                    for (int cX = rX; cX < rX + cerceveBoyutu; cX++)
                    {
                        varX += (this.ortResimX[cY, cX] - ortX) * (this.ortResimX[cY, cX] - ortX);
                        varY += (this.ortResimY[cY, cX] - ortY) * (this.ortResimY[cY, cX] - ortY);
                        cvXY += (this.ortResimX[cY, cX] - ortX) * (this.ortResimY[cY, cX] - ortY);
                    }
                varX /= (cerceveAlani - 1);
                varY /= (cerceveAlani - 1);
                cvXY /= (cerceveAlani - 1);
                //double q = ((4 * ortX * ortY * cvXY) / ((ortX * ortX + ortY * ortY) * (varX + varY)));
                double ssim = ((2 * ortX * ortY + c1) * (2 * cvXY + c2)) / ((ortX * ortX + ortY * ortY + c1) * (varX + varY + c2));
                this.ssim += (!double.IsNaN(ssim) ? ssim : 0);
            }
        this.mse /= this.pikselSayisi;
        this.psnr = (this.mse != 0) ? (10 * Math.Log10(65025 / this.mse)) : 1;
        this.ssim /= ((genislik - (cerceveBoyutu - 1)) * (yukseklik - (cerceveBoyutu - 1)));
    }
    public Bitmap ParametreleriUygula(double alfa, double beta, double gama)
    {
        gama = 1 / gama;
        int[] donusumTablosu = new int[256];
        for (int i = 0; i < 256; i++)
        {
            donusumTablosu[i] = Convert.ToInt32(alfa * i + beta);
            donusumTablosu[i] = Math.Min(Math.Max(0, donusumTablosu[i]), 255);
            donusumTablosu[i] = Convert.ToInt32(Math.Pow(donusumTablosu[i] / 255.0, gama) * 255);
        }
        Bitmap yeniResim = new Bitmap(this.ortResimX.GetLength(1), this.ortResimX.GetLength(0));
        for (int y = 0; y < yeniResim.Height; y++)
            for (int x = 0; x < yeniResim.Width; x++)
                yeniResim.SetPixel(x, y, Color.FromArgb(donusumTablosu[this.resimX[y, x, 0]], donusumTablosu[this.resimX[y, x, 1]], donusumTablosu[this.resimX[y, x, 2]]));
        return yeniResim;
    }
    public double ParametreleriDegerlendir(double alfa, double beta, double gama)
    {
        gama = 1 / gama;
        int[] histogram = new int[256];
        for (int i = 0; i < 256; i++)
        {
            int renk = Convert.ToInt32(alfa * i + beta);
            renk = Math.Min(Math.Max(0, renk), 255);
            renk = Convert.ToInt32(Math.Pow(renk / 255.0, gama) * 255);
            histogram[renk] += this.histogramX[i];
        }
        int minRenk = 255;
        int maxRenk = 0;
        int renkSayisi = 0;
        double mean = 0;
        for (int i = 0; i < 256; i++)
        {
            minRenk = Math.Min(minRenk, (histogram[i] > 0) ? i : 255);
            maxRenk = Math.Max(maxRenk, (histogram[i] > 0) ? i : 0);
            mean += i * histogram[i];
            renkSayisi += Math.Sign(histogram[i]);
        }
        mean /= this.pikselSayisi;
        double varyans = 0;
        for (int i = 0; i < 256; i++)
            varyans += (i - mean) * (i - mean);
        varyans /= 256;
        double stdSapma = Math.Sqrt(varyans);
        return Math.Sqrt((maxRenk - minRenk + 1) * renkSayisi);  
        //return Math.Abs(256 - (mean + stdSapma) / 2) / Math.Log(varyans); ;
    }
    public void ResimleriKarsilastir2()
    {
        if ((this.histogramZ == null) || (this.histogramY == null))
            return;
        int genislik = this.ortResimZ.GetLength(1);
        int yukseklik = this.ortResimZ.GetLength(0);
        int cerceveBoyutu = 8;
        int cerceveAlani = 64;
        double c1 = 6.5025;
        double c2 = 58.5225;
        this.msez = 0;
        this.psnrz = 1;
        this.ssimz = 0;
        for (int rY = 0; rY <= yukseklik - cerceveBoyutu; rY++)
            for (int rZ = 0; rZ <= genislik - cerceveBoyutu; rZ++)
            {
                this.msez += (this.ortResimZ[rY, rZ] - this.ortResimY[rY, rZ]) * (this.ortResimZ[rY, rZ] - this.ortResimY[rY, rZ]);
                int ortZ = 0;
                int ortY = 0;
                for (int cY = rY; cY < rY + cerceveBoyutu; cY++)
                    for (int cZ = rZ; cZ < rZ + cerceveBoyutu; cZ++)
                    {
                        ortZ += this.ortResimZ[cY, cZ];
                        ortY += this.ortResimY[cY, cZ];
                    }
                ortZ /= cerceveAlani;
                ortY /= cerceveAlani;
                double varZ = 0.0;
                double varY = 0.0;
                double cvZY = 0.0;
                for (int cY = rY; cY < rY + cerceveBoyutu; cY++)
                    for (int cZ = rZ; cZ < rZ + cerceveBoyutu; cZ++)
                    {
                        varZ += (this.ortResimZ[cY, cZ] - ortZ) * (this.ortResimZ[cY, cZ] - ortZ);
                        varY += (this.ortResimY[cY, cZ] - ortY) * (this.ortResimY[cY, cZ] - ortY);
                        cvZY += (this.ortResimZ[cY, cZ] - ortZ) * (this.ortResimY[cY, cZ] - ortY);
                    }
                varZ /= (cerceveAlani - 1);
                varY /= (cerceveAlani - 1);
                cvZY /= (cerceveAlani - 1);
                //double q = ((4 * ortX * ortY * cvXY) / ((ortX * ortX + ortY * ortY) * (varX + varY)));
                double ssim = ((2 * ortZ * ortY + c1) * (2 * cvZY + c2)) / ((ortZ * ortZ + ortY * ortY + c1) * (varZ + varY + c2));
                this.ssimz += (!double.IsNaN(ssim) ? ssim : 0);
            }
        this.msez /= this.pikselSayisi;
        this.psnrz = (this.msez != 0) ? (10 * Math.Log10(65025 / this.msez)) : 1;
        this.ssimz /= ((genislik - (cerceveBoyutu - 1)) * (yukseklik - (cerceveBoyutu - 1)));
    }
}   
