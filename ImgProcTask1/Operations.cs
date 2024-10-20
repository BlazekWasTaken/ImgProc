using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImgProcTask1;

public static class Operations
{
    #region Elementary operations (B)
    public static Image<Rgb24> Brightness(ref Image<Rgb24> input, int value)
    {
        var output = new Image<Rgb24>(input.Width, input.Height);

        for (int i = 0; i < input.Width; i++)
        {
            for (int j = 0; j < input.Height; j++)
            {
                output[i, j] = new Rgb24(AddBytes(input[i, j].R, value), AddBytes(input[i, j].G, value),
                    AddBytes(input[i, j].B, value));
            }
        }

        return output;
    }
    public static Image<Rgb24> Contrast(ref Image<Rgb24> input, double a)
    {
        var output = new Image<Rgb24>(input.Width, input.Height);
        double b = (1 - a) * 128;

        
            for (int i = 0; i < input.Width; i++)
            {
                for (int j = 0; j < input.Height; j++)
                {
                    output[i, j] = new Rgb24(AddBytes(MultiplyByte(input[i, j].R, a), b), AddBytes(MultiplyByte(input[i, j].G, a), b), AddBytes(MultiplyByte(input[i, j].B, a), b));
                }
            }

        return output;
    }
    public static Image<Rgb24> Negative(ref Image<Rgb24> input)
    {
        var output = new Image<Rgb24>(input.Width, input.Height);

        for (int i = 0; i < input.Width; i++)
        {
            for (int j = 0; j < input.Height; j++)
            {
                output[i, j] = new Rgb24(FlipByte(input[i, j].R), FlipByte(input[i, j].G), FlipByte(input[i, j].B));
            }
        }
        return output;
    }
    #endregion
    
    #region Geometric operations (G)
    public static Image<Rgb24> HorizontalFlip(ref Image<Rgb24> input)
    {
        var output = new Image<Rgb24>(input.Width, input.Height);

        for (int i = 0; i < input.Width; i++)
        {
            for (int j = 0; j < input.Height; j++)
            {
                output[i, j] = input[input.Width - i - 1, j];
            }
        }
        return output;
    }
    public static Image<Rgb24> VerticalFlip(ref Image<Rgb24> input)
    {
        var output = new Image<Rgb24>(input.Width, input.Height);

        for (int i = 0; i < input.Width; i++)
        {
            for (int j = 0; j < input.Height; j++)
            {
                output[i, j] = input[i, input.Height - j - 1];
            }
        }
        return output;
    }
    public static Image<Rgb24> DiagonalFlip(ref Image<Rgb24> input)
    {
        var hflip = HorizontalFlip(ref input);
        return VerticalFlip(ref hflip);
    }
    public static Image<Rgb24> Shrink(ref Image<Rgb24> input, int factor)
    {
        var newWidth = input.Width / factor;
        var newHeight = input.Height / factor;
        
        var output = new Image<Rgb24>(newWidth, newHeight);
        
        for (int i = 0; i < newWidth; i++)
        {
            for (int j = 0; j < newHeight; j++)
            {
                output[i, j] = new Rgb24(input[i * factor, j * factor].R, input[i * factor, j * factor].G,
                    input[i * factor, j * factor].B);
            }
        }
        return output;
    }
    public static Image<Rgb24> Enlarge(ref Image<Rgb24> input, int factor)
    {
        var newWidth = input.Width * factor;
        var newHeight = input.Height * factor;
        
        var output = new Image<Rgb24>(newWidth, newHeight);
        
        for (int i = 0; i < input.Width; i++)
        {
            for (int j = 0; j < input.Height; j++)
            {
                for (int k = 0; k < factor; k++)
                {
                    for (int l = 0; l < factor; l++)
                    {
                        output[i * factor + k, j * factor + l] = new Rgb24(input[i, j].R, input[i, j].G, input[i, j].B);
                    }
                }
            }
        }
        return output;
    }
    #endregion

    #region Noise removal (N)
    public static Image<Rgb24> MidpointFilter(ref Image<Rgb24> input)
    {
        var output = new Image<Rgb24>(input.Width, input.Height);

        for (int i = 1; i < input.Width - 1; i++)
        {
            for (int j = 1; j < input.Height - 1; j++)
            {
                output[i, j] = GetMidpoint(GetSurrounding(input, i, j));
            }
        }
        return output;
    }
    public static Image<Rgb24> ArithmeticMeanFilter(ref Image<Rgb24> input)
    {
        var output = new Image<Rgb24>(input.Width, input.Height);

        for (int i = 1; i < input.Width - 1; i++)
        {
            for (int j = 1; j < input.Height - 1; j++)
            {
                output[i, j] = GetArithmeticMean(GetSurrounding(input, i, j));
            }
        }
        return output;
    }

    #endregion
    
    #region Analysis (E)
    public static double MeanSquaredError(ref Image<Rgb24> input, ref Image<Rgb24> output)
    {
        if (input.Height != output.Height || input.Width != output.Width) throw new ArgumentException("Images are of different sizes");
        
        ulong sum = 0;
        
        for (int i = 0; i < input.Width; i++)
        {
            for (int j = 0; j < input.Height; j++)
            {
                sum += (ulong)Math.Pow(SubtractBytes(input[i, j].R, output[i, j].R), 2);
                sum += (ulong)Math.Pow(SubtractBytes(input[i, j].G, output[i, j].G), 2);
                sum += (ulong)Math.Pow(SubtractBytes(input[i, j].B, output[i, j].B), 2);
            }
        }
        sum /= (ulong)(input.Width * input.Height * 3);
        var square = Math.Sqrt(sum);

        return square;
    }
    public static double PeakMeanSquaredError(ref Image<Rgb24> input, ref Image<Rgb24> output)
    {
        if (input.Height != output.Height || input.Width != output.Width) throw new ArgumentException("Images are of different sizes");
        
        var p = (int)(Math.Pow(2, input.PixelType.BitsPerPixel / 3) - 1);
        return MeanSquaredError(ref input, ref output)/(Math.Pow(p, 2));
    }
    public static double SignalToNoiseRatio(ref Image<Rgb24> input, ref Image<Rgb24> output)
    {
        if (input.Height != output.Height || input.Width != output.Width) throw new ArgumentException("Images are of different sizes");

        ulong summ = 0;

        for (int i = 0; i < input.Width; i++)
        {
            for (int j = 0; j < input.Height; j++)
            {
                summ += (ulong)Math.Pow(output[i, j].R, 2) + (ulong)Math.Pow(output[i, j].G, 2) + (ulong)Math.Pow(output[i, j].B, 2);
            }
        }

        ulong sum = 0;
        
        for (int i = 0; i < input.Width; i++)
        {
            for (int j = 0; j < input.Height; j++)
            {
                sum += (ulong)Math.Pow(SubtractBytes(input[i, j].R, output[i, j].R), 2);
                sum += (ulong)Math.Pow(SubtractBytes(input[i, j].G, output[i, j].G), 2);
                sum += (ulong)Math.Pow(SubtractBytes(input[i, j].B, output[i, j].B), 2);
            }
        }
        
        if (sum == 0) throw new DivideByZeroException();
        return summ / sum;
    }
    public static double PeakSignalToNoiseRatio(ref Image<Rgb24> input, ref Image<Rgb24> output)
    {
        if (input.Height != output.Height || input.Width != output.Width) throw new ArgumentException("Images are of different sizes");
        
        ulong p = (ulong)(Math.Pow(2, input.PixelType.BitsPerPixel / 3) - 1);

        ulong sum = 0;
        
        for (int i = 0; i < input.Width; i++)
        {
            for (int j = 0; j < input.Height; j++)
            {
                sum += (ulong)Math.Pow(SubtractBytes(input[i, j].R, output[i, j].R), 2);
                sum += (ulong)Math.Pow(SubtractBytes(input[i, j].G, output[i, j].G), 2);
                sum += (ulong)Math.Pow(SubtractBytes(input[i, j].B, output[i, j].B), 2);
            }
        }
        
        if (sum == 0) throw new DivideByZeroException();

        return 10 * Math.Log10((Math.Pow(p, 2) * input.Height * input.Width) / sum);
    }
    public static int MaximumDifference(ref Image<Rgb24> input, ref Image<Rgb24> output)
    {
        if (input.Height != output.Height || input.Width != output.Width) throw new ArgumentException("Images are of different sizes");
        
        List<int> difference = []; 

        for (int i = 0; i < input.Height; i++)
        {
            for (int j = 0; j < input.Width; j++)
            {
                var inp = ((input[i,j].R + input[i,j].G + input[i,j].B) / 3);
                var outp = ((output[i,j].R + output[i,j].G + output[i,j].B) / 3);
                difference.Add(inp - outp);
            }
        }

        return difference.Max();
    }
    #endregion

    #region Utility functions
    private static byte AddBytes(byte input, int value)
    {
        var result = input + value;
        return result switch
        {
            > 255 => 255,
            < 0 => 0,
            _ => (byte)result
        };
    }
    private static byte AddBytes(double input, double value)
    {
        var result = input + value;
        return result switch
        {
            > 255 => 255,
            < 0 => 0,
            _ => (byte)result
        };
    }
    private static byte SubtractBytes(byte input1, byte input2)
    {
        return (byte)Math.Abs(input1 - input2);
    }
    private static double MultiplyByte(byte input, double value)
    {
        var result = input * value;
        return result;
    }
    private static byte FlipByte(byte input)
    {
        return (byte)~input;
    }
    private static Rgb24 GetMidpoint(List<Rgb24> values)
    {
        var pixel = new Rgb24
        {
            R = (byte)((values.Max(x => x.R) + values.Min(x => x.R)) / 2),
            G = (byte)((values.Max(x => x.G) + values.Min(x => x.G)) / 2),
            B = (byte)((values.Max(x => x.B) + values.Min(x => x.B)) / 2)
        };
        return pixel;
    }
    private static Rgb24 GetArithmeticMean(List<Rgb24> values)
    {
        var pixel = new Rgb24
        {
            R = (byte)(values.Sum(x => x.R) / values.Count),
            G = (byte)(values.Sum(x => x.G) / values.Count),
            B = (byte)(values.Sum(x => x.B) / values.Count)
        };
        return pixel;
    }
    private static List<Rgb24> GetSurrounding(Image<Rgb24> input, int i, int j)
    {
        var surrounding = new List<Rgb24>();

        for (int k = i - 1; k <= i + 1; k++)
        {
            for (int l = j - 1; l <= j + 1; l++)
            {
                surrounding.Add(input[k, l]);
            }
        }
        return surrounding;
    }
    #endregion
    
}