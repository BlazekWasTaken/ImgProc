using System.Numerics;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Png.Chunks;
using SixLabors.ImageSharp.PixelFormats;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Task4;

//t2 all f
public static class Operations
{
    #region fourier

    public static Image<L8> StandardDiscreteFourier(Image<L8> input)
    {
        var output = new Image<L8>(input.Width, input.Height);
        var width = input.Width;
        var height = input.Height;
        var magnitude = new double[width, height];
        
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
                magnitude[u, v] = Math.Sqrt(sumReal * sumReal + sumImag * sumImag);
                magnitude[u, v] = Math.Log(1 + magnitude[u, v]);
                var maxMagnitude = magnitude.Cast<double>().Max();
                var normalizedValue = (byte)(255 * (magnitude[u, v] / maxMagnitude));
                output[u, v] = new L8(normalizedValue);
            }
        }
        return output;
    }
    
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
        
        var magnitudeForImage = new double[height, width];
        var magnitude = new double[height, width];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                magnitude[y, x] = complexData[y, x].Magnitude;
                magnitudeForImage[y, x] = Math.Log(1 + magnitude[y, x]);
            }
        }
        
        var phase = new double[height, width];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                phase[y, x] = complexData[y, x].Phase;
            }
        }
        
        Shift(magnitudeForImage);
        
        var minMagnitude = magnitudeForImage.Cast<double>().Min();
        var maxMagnitude = magnitudeForImage.Cast<double>().Max();
        var outputMag = new Image<L8>(width, height);
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var magnitudeValue = (byte)(255 * (magnitudeForImage[y, x] - minMagnitude) / (maxMagnitude - minMagnitude));
                outputMag[x, y] = new L8(magnitudeValue);
            }
        }
        outputMag.Metadata.GetPngMetadata().TextData.Add(new PngTextData("magnitude", JsonConvert.SerializeObject(magnitude), "", ""));
        outputMag.Metadata.GetPngMetadata().TextData.Add(new PngTextData("phase", JsonConvert.SerializeObject(phase), "", ""));
        return outputMag;
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
        
        var magnitude = new double[height, width];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                magnitude[y, x] = complexData[y, x].Magnitude;
            }
        }
        
        var minMagnitude = magnitude.Cast<double>().Min();
        var maxMagnitude = magnitude.Cast<double>().Max();
        var outputImage = new Image<L8>(width, height);
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var magnitudeValue = (byte)(255 * (magnitude[y, x] - minMagnitude) / (maxMagnitude - minMagnitude));
                outputImage[x, y] = new L8(magnitudeValue);
            }
        }
        return outputImage;
    }

    //this is the fast part and I don't completely understand it but well
    private static void FastFourier1D(Complex[] data, bool inverse)
    {
        //self-explanatory
        var n = data.Length;
        if ((n & (n - 1)) != 0)
        {
            throw new ArgumentException("Input size must be a power of 2.");
        }

        //wikipedia has a great article on that one
        BitReversal(data);

        //this is the iterative part (decimation in frequency)
        double sign = inverse ? 1 : -1; //direction of the transform
        for (var length = 2; length <= n; length *= 2)
        {
            //dark magic:
            var halfLength = length / 2;
            var wLength = Complex.Exp(new Complex(0, sign * 2 * Math.PI / length)); // Twiddle factor

            for (var i = 0; i < n; i += length)
            {
                var w = Complex.One; // Start with W^0
                for (var j = 0; j < halfLength; j++)
                {
                    var even = data[i + j];
                    var odd = w * data[i + j + halfLength];
                    data[i + j] = even + odd;              // Butterfly operation
                    data[i + j + halfLength] = even - odd; // Butterfly operation
                    w *= wLength; // Update twiddle factor
                }
            }
            //end of dark magic
        }

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
    
    //Swap the quadrants so that it has a spark in the middle
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
    
    #endregion
}