using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Task3;

public static class Operations
{
    #region basic operations
    public static Image<L8> Dilation(ref Image<L8> image, int[,] structuringElement)
    {
        var output = image.Clone();

        var elementHeight = structuringElement.GetLength(0);
        var elementWidth = structuringElement.GetLength(1);

        var elementOffsetX = elementWidth / 2;
        var elementOffsetY = elementHeight / 2;
        
        for (var y = 0; y < image.Height; y++)
        {
            for (var x = 0; x < image.Width; x++)
            {
                if (image[x, y].PackedValue <= 0) continue;
                for (var j = 0; j < elementHeight; j++)
                {
                    for (var i = 0; i < elementWidth; i++)
                    {
                        if (structuringElement[j, i] != 1) continue;
                        var newX = x + i - elementOffsetX;
                        var newY = y + j - elementOffsetY;

                        if (newX >= 0 && newX < image.Width && newY >= 0 && newY < image.Height)
                        {
                            output[newX, newY] = new L8(255);
                        }
                    }
                }
            }
        }
        
        return output;
    }
    public static Image<L8> Erode(ref Image<L8> image, int[,] structuringElement)
    {
        var output = image.Clone();
        
        var elementHeight = structuringElement.GetLength(0);
        var elementWidth = structuringElement.GetLength(1);
        
        var elementOffsetX = elementWidth / 2;
        var elementOffsetY = elementHeight / 2;
        
        for (var y = 0; y < image.Height; y++)
        {
            for (var x = 0; x < image.Width; x++)
            {
                var shouldErode = true;

                for (var j = 0; j < elementHeight; j++)
                {
                    for (var i = 0; i < elementWidth; i++)
                    {
                        if (structuringElement[j, i] != 1) continue;
                        var newX = x + i - elementOffsetX;
                        var newY = y + j - elementOffsetY;

                        if (newX >= 0 && newX < image.Width && newY >= 0 && newY < image.Height &&
                            image[newX, newY].PackedValue != 0) continue;
                        shouldErode = false;
                        break;
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
        var output = Erode(ref image, structuringElement);
        return Dilation(ref output, structuringElement);
    }
    
    public static Image<L8> Close(ref Image<L8> image, int[,] structuringElement)
    {
        var output = Dilation(ref image, structuringElement);
        return Erode(ref output, structuringElement);
    }

    public static Image<L8> HmtTransformation(ref Image<L8> image, int[,] structuringElement)
    {
        var output = new Image<L8>(image.Width, image.Height);

        var elementComplement = structuringElement.ElementComplement();
        
        var eroded = Erode(ref image, structuringElement);
        var complement = image.Complement();
        var erodedComplement = Erode(ref complement, elementComplement);
        
        for (var y = 0; y < image.Height; y++)
        {
            for (var x = 0; x < image.Width; x++)
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
    public static Image<L8> M6(ref Image<L8> input, List<int[,]> structuringElements)
    {
        var output = input.Clone();
        Image<L8> compare;

        do
        {
            compare = output.Clone();
            foreach (var element in structuringElements)
            {
                var temp = HmtTransformation(ref output, element.ElementComplement());
                for (int i = 0; i < output.Height; i++)
                {
                    for (int j = 0; j < output.Height; j++)
                    {
                        if (output[i, j].PackedValue == 255 || temp[i, j].PackedValue == 255)
                        {
                            output[i, j] = new L8(255);
                        }
                        else
                        {
                            output[i, j] = new L8(0);
                        }
                    }
                }
            }
        } while (!compare.IsEqual(output));
        
        return output;
    }
    #endregion
    
    #region region growing (merging)
    public static Image<Rgb24> GrowRegions(Image<Rgb24> image, List<Point> seeds, Func<Rgb24, Rgb24, bool> isSimilarColor)
    {
        var width = image.Width;
        var height = image.Height;
        var visited = new bool[width, height];
        var queue = new Queue<(Point point, Rgb24 color)>();
        var outputImage = image.Clone();
        
        foreach (var seed in seeds)
        {
            var seedColor = image[seed.X, seed.Y];
            queue.Enqueue((seed, seedColor));
            visited[seed.X, seed.Y] = true;
        }

        while (queue.Count > 0)
        {
            var (p, regionColor) = queue.Dequeue();
            outputImage[p.X, p.Y] = regionColor;

            Point[] neighbors =
            [
                new (p.X - 1, p.Y),
                new (p.X + 1, p.Y),
                new (p.X, p.Y - 1),
                new (p.X, p.Y + 1)
            ];

            foreach (var neighbor in neighbors)
            {
                if (neighbor.X < 0 || neighbor.X >= width || neighbor.Y < 0 || neighbor.Y >= height ||
                    visited[neighbor.X, neighbor.Y]) continue;
                var neighborColor = image[neighbor.X, neighbor.Y];
                if (!isSimilarColor(regionColor, neighborColor)) continue;
                queue.Enqueue((neighbor, regionColor));
                visited[neighbor.X, neighbor.Y] = true;
            }
        }

        return outputImage;
    }
    #endregion

    #region other functions
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
    private static bool IsEqual(this Image<L8> image1, Image<L8> image2)
    {
        if (image1.Width != image2.Width || image1.Height != image2.Height)
            return false;

        for (int y = 0; y < image1.Height; y++)
        {
            for (int x = 0; x < image1.Width; x++)
            {
                if (image1[x, y].PackedValue != image2[x, y].PackedValue)
                    return false;
            }
        }

        return true;
    }
    private static int[,] ElementComplement(this int[,] input)
    {
        var output = new int[input.GetLength(0), input.GetLength(1)];
        for (int i = 0; i < input.GetLength(0); i++)
        {
            for (int j = 0; j < input.GetLength(1); j++)
            {
                if (input[i, j] == -1) continue;
                output[i, j] = 1 - input[i, j];
            }
        }
        return output;
    }
    #endregion
}