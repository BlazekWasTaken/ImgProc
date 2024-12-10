using System.Xml;
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
    
    public static Image<L8> Open(ref Image<L8> image, int[,] structuringElement)
    {
        Image<L8> output = Erode(ref image, structuringElement);
        return Dilation(ref output, structuringElement);
    }
    
    public static Image<L8> Close(ref Image<L8> image, int[,] structuringElement)
    {
        Image<L8> output = Dilation(ref image, structuringElement);
        return Erode(ref output, structuringElement);
    }

    public static Image<L8> HmtTransformation(ref Image<L8> image, int[,] structuringElement1, int[,] structuringElement2)
    {
        var output = new Image<L8>(image.Width, image.Height);;
        
        var eroded = Erode(ref image, structuringElement1);
        var complement = image.Complement();
        var erodedComplement = Erode(ref complement, structuringElement2);
        
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                byte value = 0;
                if (eroded[x, y].PackedValue == 255 && erodedComplement[x, y].PackedValue == 255) value = 255;
                output[x, y] = new L8(value);
            }
        }
        
        return output;
    }
    #endregion
    
    #region M6
    #endregion
    
    #region region growing (merging)
    public static Image<L8> RegionGrowing(Image<L8> inputImage, List<(int x, int y)> seedPoints, int threshold)
    {
        var outputImage = new Image<L8>(inputImage.Width, inputImage.Height);

        var visited = new bool[inputImage.Height, inputImage.Width];
        
        var neighbors = new List<(int x, int y)>
        {
            ( -1, 0 ), (-1, 1), (1, 0), (1, -1), (1, 1), (-1, -1), (0, -1 ), ( 0, 1 )
        };

        byte currentRegionIntensity = 100;

        foreach (var (seedX, seedY) in seedPoints)
        {
            if (visited[seedY, seedX]) continue;
            
            var queue = new Queue<(int x, int y)>();
            queue.Enqueue((seedX, seedY));
            visited[seedY, seedX] = true;
            outputImage[seedX, seedY] = new L8(currentRegionIntensity);

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();

                foreach (var neighbor in neighbors)
                {
                    var nx = x + neighbor.x;
                    var ny = y + neighbor.y;
                    
                    if (nx < 0 || ny < 0 || nx >= inputImage.Width || ny >= inputImage.Height) continue;
                    
                    if (visited[ny, nx]) continue;
                    
                    int currentIntensity = inputImage[x, y].PackedValue;
                    int neighborIntensity = inputImage[nx, ny].PackedValue;

                    if (Math.Abs(currentIntensity - neighborIntensity) > threshold) continue;
                    outputImage[nx, ny] = new L8(currentRegionIntensity);
                    visited[ny, nx] = true;
                    queue.Enqueue((nx, ny));
                }
            }
            currentRegionIntensity += 50;
        }

        return outputImage;
    }
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
    
    private static Image<L8> Complement(this Image<L8> input)
    {
        var output = input.Clone();
        
        for (int y = 0; y < input.Height; y++)
        {
            for (int x = 0; x < input.Width; x++)
            {
                output[x, y] = new L8((byte)(255 - input[x, y].PackedValue));
            }
        }

        return output;
    }
}