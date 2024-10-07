using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImgProcTask1;

public class Operations
{
    public static Image<Rgb24> Brightness(Image<Rgb24> input, int value)
    {
        var output = new Image<Rgb24>(input.Width, input.Height);

        for (int i = 0; i < input.Height; i++)
        {
            for (int j = 0; j < input.Width; j++)
            {
                output[i, j] = new Rgb24(AddBytes(input[i, j].R, value), AddBytes(input[i, j].G, value), AddBytes(input[i, j].B, value));
            }
        }

        return output;
    }

    public static Image<Rgb24> Negative(Image<Rgb24> input)
    {
        var output = new Image<Rgb24>(input.Width, input.Height);

        for (int i = 0; i < input.Height; i++)
        {
            for (int j = 0; j < input.Width; j++)
            {
                output[i, j] = new Rgb24(FlipByte(input[i, j].R), FlipByte(input[i, j].G), FlipByte(input[i, j].B));
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
    private static byte FlipByte(byte input)
    {
        return (byte)(255 - input);
    }

    #endregion
    
}