using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImgProcTask1;

public class Operations
{
    public static Image<Rgb24> Brightness(Image<Rgb24> input, int value)
    {
        var output = new Image<Rgb24>(input.Width, input.Height);

        for (int i = 0; i < input.Width; i++)
        {
            for (int j = 0; j < input.Height; j++)
            {
                output[i, j] = new Rgb24(AddBytes(input[i, j].R, value), AddBytes(input[i, j].G, value), AddBytes(input[i, j].B, value));
            }
        }

        return output;
    }
    
    public static Image<Rgb24> Contrast(Image<Rgb24> input, double value)
    {
        var output = new Image<Rgb24>(input.Width, input.Height);
        double a = (1 - value) * 128;

        for (int i = 0; i < input.Width; i++)
        {
            for (int j = 0; j < input.Height; j++)
            {
                output[i, j] = new Rgb24(AddBytes(MultiplyByte(input[i, j].R, value), a), AddBytes(MultiplyByte(input[i, j].G, value), a), AddBytes(MultiplyByte(input[i, j].B, value), a));
            }
        }

        return output;
    }

    public static Image<Rgb24> Negative(Image<Rgb24> input)
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

    public static Image<Rgb24> HorizontalFlip(Image<Rgb24> input)
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
    
    public static Image<Rgb24> VerticalFlip(Image<Rgb24> input)
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
    
    public static Image<Rgb24> DiagonalFlip(Image<Rgb24> input)
    {
        return VerticalFlip(HorizontalFlip(input));
    }

    public static Image<Rgb24> Shrink(Image<Rgb24> input, int factor)
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
    
    public static Image<Rgb24> Enlarge(Image<Rgb24> input, int factor)
    {
        var newWidth = input.Width * factor;
        var newHeight = input.Height * factor;
        
        var output1 = new Image<Rgb24>(newWidth, newHeight);
        
        for (int i = 0; i < input.Width; i++)
        {
            for (int j = 0; j < input.Height; j++)
            {
                for (int k = 0; k < factor; k++)
                {
                    for (int l = 0; l < factor; l++)
                    {
                        output1[i * factor + k, j * factor + l] = new Rgb24(input[i, j].R, input[i, j].G, input[i, j].B);
                    }
                }
            }
        }

        return output1;
    }

    public static Image<Rgb24> MidpointFilter(Image<Rgb24> input)
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
    private static byte AddBytes(byte input, double value)
    {
        var result = input + value;
        return result switch
        {
            > 255 => 255,
            < 0 => 0,
            _ => (byte)result
        };
    }
    private static byte MultiplyByte(byte input, double value)
    {
        var result = input * value;
        return result switch
        {
            > 255 => 255,
            < 0 => 0,
            _ => (byte)result
        };
    }
    private static byte FlipByte(byte input)
    {
        return (byte)(255 - input);
    }

    private static Rgb24 GetMidpoint(List<Rgb24> values)
    {
        var pixel = new Rgb24();

        pixel.R = (byte)((values.Max(x => x.R) + values.Min(x => x.R)) / 2);
        pixel.G = (byte)((values.Max(x => x.G) + values.Min(x => x.G)) / 2);
        pixel.B = (byte)((values.Max(x => x.B) + values.Min(x => x.B)) / 2);

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