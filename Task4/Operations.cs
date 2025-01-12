using System.Numerics;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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
    
    public static (Image<L8> mag, Image<L8> phase) FastFourier(Image<L8> inputImage)
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
        
        var magnitude = new double[height, width];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                magnitude[y, x] = complexData[y, x].Magnitude;
                magnitude[y, x] = Math.Log(1 + magnitude[y, x]);
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
        
        Shift(magnitude);
        Shift(phase);
        
        var minMagnitude = magnitude.Cast<double>().Min();
        var maxMagnitude = magnitude.Cast<double>().Max();
        var outputMag = new Image<L8>(width, height);
        var outputPhase = new Image<L8>(width, height);
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var magnitudeValue = (byte)(255 * (magnitude[y, x] - minMagnitude) / (maxMagnitude - minMagnitude));
                outputMag[x, y] = new L8(magnitudeValue);
                var phaseValue = (byte)(255 * (phase[y, x] / (2 * Math.PI)));
                outputPhase[x, y] = new L8(phaseValue);
            }
        }
        return (outputMag, outputPhase);
    }

    public static Image<L8> InverseFastFourier(Image<L8> magImage, Image<L8> phaseImage)
    {
        if (magImage.Width != phaseImage.Width || magImage.Height != phaseImage.Height) throw new ArgumentException("Images must have the same size.");
        
        var width = magImage.Width;
        var height = magImage.Height;
        
        var complexData = new Complex[height, width];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                // var magnitudeValue = (byte)((17.293893832024828 - 1.1326794916281377) * magImage[y, x].PackedValue / 255);
                // var magnitudeValue = (byte)(Math.Exp(magImage[y, x].PackedValue) - 1);
                var magnitudeValue = magImage[y, x].PackedValue;
                var phaseValue = (byte)(2 * Math.PI * phaseImage[x, y].PackedValue / 255 - Math.PI);
                
                complexData[y, x] = Complex.FromPolarCoordinates(magnitudeValue, phaseValue);
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
                // magnitude[y, x] = Math.Exp(magnitude[y, x]) - 1;
                
                magnitude[y, x] = Math.Log(1 + magnitude[y, x]);
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
        
        var minMagnitude = magnitude.Cast<double>().Min();
        var maxMagnitude = magnitude.Cast<double>().Max();
        var outputImage = new Image<L8>(width, height);
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var magnitudeValue = (byte)(255 * (magnitude[y, x] - minMagnitude) / (maxMagnitude - minMagnitude));
                // var magnitudeValue = (byte)magnitude[y, x];
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