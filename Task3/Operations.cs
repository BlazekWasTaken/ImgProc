using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Task3;

public static class Operations
{
    #region basic operations
    public static Image<L8> Dilation(ref Image<L8> image, int[,] structuringElement)
    {
        var output = image.Clone();

        int elementHeight = structuringElement.GetLength(0);
        int elementWidth = structuringElement.GetLength(1);

        int elementOffsetX = elementWidth / 2;
        int elementOffsetY = elementHeight / 2;
        
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                if (image[x, y].PackedValue > 0)
                {
                    for (int j = 0; j < elementHeight; j++)
                    {
                        for (int i = 0; i < elementWidth; i++)
                        {
                            if (structuringElement[j, i] == 1)
                            {
                                int newX = x + i - elementOffsetX;
                                int newY = y + j - elementOffsetY;

                                if (newX >= 0 && newX < image.Width && newY >= 0 && newY < image.Height)
                                {
                                    output[newX, newY] = new L8(255);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        return output;
    }
    public static Image<L8> Erode(ref Image<L8> image, int[,] structuringElement)
    {
        Image<L8> output = image.Clone();
        
        int elementHeight = structuringElement.GetLength(0);
        int elementWidth = structuringElement.GetLength(1);
        
        int elementOffsetX = elementWidth / 2;
        int elementOffsetY = elementHeight / 2;
        
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                bool shouldErode = true;

                for (int j = 0; j < elementHeight; j++)
                {
                    for (int i = 0; i < elementWidth; i++)
                    {
                        if (structuringElement[j, i] == 1)
                        {
                            int newX = x + i - elementOffsetX;
                            int newY = y + j - elementOffsetY;

                            if (newX < 0 || newX >= image.Width || newY < 0 || newY >= image.Height || image[newX, newY].PackedValue == 0)
                            {
                                shouldErode = false;
                                break;
                            }
                        }
                    }

                    if (!shouldErode)
                        break;
                }

                output[x, y] = shouldErode ? new L8(255) : new L8(0);
            }
        }

        return output;
    }
    #endregion
    
    #region M6
    #endregion
    
    #region region growing (merging)
    #endregion
    
    private static byte ToByte(this int value)
    {
        return value switch
        {
            < byte.MinValue => 0,
            > byte.MaxValue => byte.MaxValue,
            _ => (byte)value
        };
    }
}