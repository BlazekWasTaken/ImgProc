using System.Numerics;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png.Chunks;
using SixLabors.ImageSharp.PixelFormats;

namespace Task4;

public static class Operations
{
    #region fourier

    public static Image<L8> StandardDiscreteFourier(Image<L8> input)
    {
        var width = input.Width;
        var height = input.Height;
        var magnitude = new Complex[width, height];
        
        for (var u = 0; u < width; u++)
        {
            for (var v = 0; v < height; v++)
            {
                var sumReal = 0.0;
                var sumImag = 0.0;
                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        var pixelValue = input[x, y].PackedValue;
                        var angle = -2 * Math.PI * ((u * x / (double)width) + (v * y / (double)height));
                        sumReal += pixelValue * Math.Cos(angle);
                        sumImag += pixelValue * Math.Sin(angle);
                    }
                }
                magnitude[u, v] = new Complex(sumReal, sumImag);
            }
        }
        Shift(magnitude);
        return magnitude.ToImage(height, width, false);
    }

    public static Image<L8> StandardInverseFourier(Image<L8> input)
    {
        var width = input.Width;
        var height = input.Height;
        
      var magnitudeString = input.Metadata.GetPngMetadata().TextData
            .FirstOrDefault(x => x.Keyword == "magnitude").Value;
        var phaseString = input.Metadata.GetPngMetadata().TextData
            .FirstOrDefault(x => x.Keyword == "phase").Value;
        
        var magnitudeData = JsonConvert.DeserializeObject<double[,]>(magnitudeString);
        var phaseData = JsonConvert.DeserializeObject<double[,]>(phaseString);
        
        if (magnitudeData is null || phaseData is null) throw new ArgumentException("No metadata.");
        
        var complexData = new Complex[height, width];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                complexData[y, x] = Complex.FromPolarCoordinates(magnitudeData[y, x], phaseData[y, x]);
            }
        }
        var spatialData = new Complex[input.Height, input.Width];

        for (var y = 0; y < height; y ++)
        {
            for (var x = 0; x < width; x ++)
            {
                var sum = Complex.Zero;

                for(var v = 0; v < height; v++)
                {
                    for(var u = 0; u < width; u++)
                    {
                        var angle = 2 * Math.PI * ((u * x / (double)width) + (v * y / (double)height));
                        var exp = new Complex(Math.Cos(angle), Math.Sin(angle));
                        sum += complexData[v, u] * exp;
                    }
                }
                spatialData[x, y] = 1/(double)width*(1/(double)height)*sum;
            }
        }
        
        return spatialData.ToImage(height, width, true);
    }
    
    #endregion fourier
    
    #region fast fourier
    
    public static Image<L8> FastFourier(Image<L8> inputImage)
    {
        var width = inputImage.Width;
        var height = inputImage.Height;
        
        var complexData = new Complex[height, width];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                complexData[y, x] = new Complex(inputImage[x, y].PackedValue, 0);
            }
        }

        for (var y = 0; y < height; y++)
        {
            var row = new Complex[width];
            for (var x = 0; x < width; x++)
            {
                row[x] = complexData[y, x];
            }
            FastFourier1D(row, false);
            for (var x = 0; x < width; x++)
            {
                complexData[y, x] = row[x];
            }
        }
        
        for (var x = 0; x < width; x++)
        {
            var column = new Complex[height];
            for (var y = 0; y < height; y++)
            {
                column[y] = complexData[y, x];
            }
            FastFourier1D(column, false);
            for (var y = 0; y < height; y++)
            {
                complexData[y, x] = column[y];
            }
        }
        
        Shift(complexData);
        var image = complexData.ToImage(width, height, false);
        return image;
    }

    public static Image<L8> InverseFastFourier(Image<L8> magImage)
    {
        var width = magImage.Width;
        var height = magImage.Height;
        
        var magnitudeString = magImage.Metadata.GetPngMetadata().TextData
            .FirstOrDefault(x => x.Keyword == "magnitude").Value;
        var phaseString = magImage.Metadata.GetPngMetadata().TextData
            .FirstOrDefault(x => x.Keyword == "phase").Value;

        var magnitudeData = JsonConvert.DeserializeObject<double[,]>(magnitudeString);
        var phaseData = JsonConvert.DeserializeObject<double[,]>(phaseString);
        
        if (magnitudeData is null || phaseData is null) throw new ArgumentException("No metadata.");
        
        var complexData = new Complex[height, width];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                complexData[y, x] = Complex.FromPolarCoordinates(magnitudeData[y, x], phaseData[y, x]);
            }
        }

        for (var y = 0; y < height; y++)
        {
            var row = new Complex[width];
            for (var x = 0; x < width; x++)
            {
                row[x] = complexData[y, x];
            }
            FastFourier1D(row, true);
            for (var x = 0; x < width; x++)
            {
                complexData[y, x] = row[x];
            }
        }
        
        for (var x = 0; x < width; x++)
        {
            var column = new Complex[height];
            for (var y = 0; y < height; y++)
            {
                column[y] = complexData[y, x];
            }
            FastFourier1D(column, true);
            for (var y = 0; y < height; y++)
            {
                complexData[y, x] = column[y];
            }
        }

        var image = complexData.ToImage(height, width, true);
        return image;
    }
    
    public static (Image<L8> magImage, Image<L8> filter, Image<L8> result) Filter(Image<L8> image, Func<double, bool> comparison)
    {
        var width = image.Width;
        var height = image.Height;

        var r1 = FastFourier(image);
        
        var magnitudeString = r1.Metadata.GetPngMetadata().TextData
            .FirstOrDefault(x => x.Keyword == "magnitude").Value;
        var phaseString = r1.Metadata.GetPngMetadata().TextData
            .FirstOrDefault(x => x.Keyword == "phase").Value;

        var magnitudeData = JsonConvert.DeserializeObject<double[,]>(magnitudeString);
        var phaseData = JsonConvert.DeserializeObject<double[,]>(phaseString);
        var complexData = new Complex[width, height];

        var dc = Complex.FromPolarCoordinates(magnitudeData![height / 2, width / 2], phaseData![height / 2, width / 2]);
        var min = magnitudeData.Cast<double>().Min();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var distanceFromCenter = Math.Sqrt(Math.Pow((height - 1.0) / 2 - i, 2) + Math.Pow((width - 1.0) / 2 - j, 2));
                complexData[i, j] = comparison(distanceFromCenter)
                    ? Complex.FromPolarCoordinates(magnitudeData[i, j], phaseData[i, j])
                    : min;
            }
        }
        complexData[height / 2, width / 2] = dc;
        
        var r2 = complexData.ToImage(height, width, false);
        var r3 = InverseFastFourier(r2);
        
        return (r1, r2, r3);
    }

    public static (Image<L8> magImage, Image<L8> filter, Image<L8> result) HighPassEdgeFilter(
        Image<L8> image,
        Image<L8> mask)
    {
        if (image.Height != mask.Height || image.Width != mask.Width) 
            throw new ArgumentException("Image and mask have to be the same size");
        
        var width = image.Width;
        var height = image.Height;

        var r1 = FastFourier(image);
        
        var magnitudeString = r1.Metadata.GetPngMetadata().TextData
            .FirstOrDefault(x => x.Keyword == "magnitude").Value;
        var phaseString = r1.Metadata.GetPngMetadata().TextData
            .FirstOrDefault(x => x.Keyword == "phase").Value;

        var magnitudeData = JsonConvert.DeserializeObject<double[,]>(magnitudeString);
        var phaseData = JsonConvert.DeserializeObject<double[,]>(phaseString);
        var complexData = new Complex[width, height];

        var dc = Complex.FromPolarCoordinates(magnitudeData![height / 2, width / 2], phaseData![height / 2, width / 2]);
        var min = magnitudeData.Cast<double>().Min();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var compare = mask[j, i].PackedValue == 255;
                complexData[i, j] = compare
                    ? Complex.FromPolarCoordinates(magnitudeData[i, j], phaseData[i, j])
                    : min;
            }
        }
        complexData[height / 2, width / 2] = dc;
        
        var r2 = complexData.ToImage(height, width, false);
        var r3 = InverseFastFourier(r2);
        
        return (r1, r2, r3);
    }

    public static (Image<L8> magImage, Image<L8> filter, Image<L8> result) PhaseFilter(Image<L8> image, int k, int l)
    {
        var width = image.Width;
        var height = image.Height;

        var r1 = FastFourier(image);
        
        var magnitudeString = r1.Metadata.GetPngMetadata().TextData
            .FirstOrDefault(x => x.Keyword == "magnitude").Value;
        var phaseString = r1.Metadata.GetPngMetadata().TextData
            .FirstOrDefault(x => x.Keyword == "phase").Value;

        var magnitudeData = JsonConvert.DeserializeObject<double[,]>(magnitudeString);
        var phaseData = JsonConvert.DeserializeObject<double[,]>(phaseString);
        var complexData = new Complex[width, height];
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var maskValue = Complex.Exp(
                    new Complex(
                        0,
                        -2 * j * k * Math.PI / height + -2 * i * l * Math.PI / width + (k + l) * Math.PI
                    ));
                
                complexData[i, j] = Complex.FromPolarCoordinates(
                    magnitudeData![i, j] * maskValue.Magnitude, 
                    phaseData![i, j] * maskValue.Phase
                    );
            }
        }
        
        var r2 = complexData.ToImage(height, width, false);
        var r3 = InverseFastFourier(r2);
        
        return (r1, r2, r3);
    }
    
    private static void FastFourier1D(Complex[] data, bool inverse)
    {
        var n = data.Length;
        if ((n & (n - 1)) != 0)
        {
            throw new ArgumentException("Input size must be a power of 2.");
        }
        
        double sign = inverse ? 1 : -1;
        for (var length = n; length >= 2; length /= 2)
        {
            var halfLength = length / 2;
            var wLength = Complex.Exp(new Complex(0, sign * 2 * Math.PI / length));

            for (var i = 0; i < n; i += length)
            {
                var w = Complex.One;
                for (var j = 0; j < halfLength; j++)
                {
                    var even = data[i + j];
                    var odd = data[i + j + halfLength];
                    data[i + j] = even + odd;
                    data[i + j + halfLength] = (even - odd) * w;
                    w *= wLength;
                }
            }
        }
        
        BitReversal(data);

        if (!inverse) return;
        {
            for (var i = 0; i < n; i++)
            {
                data[i] /= n;
            }
        }
    }

    private static void BitReversal(Complex[] data)
    {
        var n = data.Length;
        var bits = (int)Math.Log2(n);

        for (var i = 0; i < n; i++)
        {
            var reversed = ReverseBits(i, bits);
            if (reversed > i)
            {
                (data[i], data[reversed]) = (data[reversed], data[i]);
            }
        }
    }

    private static int ReverseBits(int num, int bits)
    {
        var reversed = 0;
        for (var i = 0; i < bits; i++)
        {
            reversed = (reversed << 1) | (num & 1);
            num >>= 1;
        }
        return reversed;
    }
    
    private static void Shift<T>(T[,] data)
    {
        var height = data.GetLength(0);
        var width = data.GetLength(1);

        var halfHeight = height / 2;
        var halfWidth = width / 2;

        for (var y = 0; y < halfHeight; y++)
        {
            for (var x = 0; x < halfWidth; x++)
            {
                Swap(ref data[y, x], ref data[y + halfHeight, x + halfWidth]);
                Swap(ref data[y, x + halfWidth], ref data[y + halfHeight, x]);
            }
        }
    }

    private static void Swap<T>(ref T a, ref T b)
    {
        (a, b) = (b, a);
    }

    private static Image<L8> ToImage(this Complex[,] fourier, int height, int width, bool isStandardImage)
    {
        var magnitudeForImage = new double[height, width];
        var magnitude = new double[height, width];
        var phase = new double[height, width];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                magnitude[y, x] = fourier[y, x].Magnitude; 
                magnitudeForImage[y, x] = !isStandardImage ? Math.Log(1 + magnitude[y, x]) : magnitude[y, x];
            }
        }
        
        if (!isStandardImage)
        {
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    phase[y, x] = fourier[y, x].Phase;
                }
            }
        }
        
        var minMagnitude = magnitudeForImage.Cast<double>().Min();
        var maxMagnitude = magnitudeForImage.Cast<double>().Max();
        var image = new Image<L8>(width, height);
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var magnitudeValue = (byte)(255 * (magnitudeForImage[y, x] - minMagnitude) / (maxMagnitude - minMagnitude));
                image[x, y] = new L8(magnitudeValue);
            }
        }

        if (isStandardImage) return image;
        image.Metadata.GetPngMetadata().TextData.Add(
            new PngTextData(
                "magnitude",
                JsonConvert.SerializeObject(magnitude),
                "",
                ""));
        image.Metadata.GetPngMetadata().TextData.Add(
            new PngTextData(
                "phase",
                JsonConvert.SerializeObject(phase),
                "",
                ""));

        return image;
    }
    
    #endregion fast fourier
}