using System.Diagnostics;
using CommandLine;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Task3;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(RunOptions);

return;

// static List<Point> GenerateSeedPoints(int width, int height, int count)
// {
//     var seedPoints = new List<Point>();
//     
//     var random = new Random();
//     for (int i = 0; i < count; i++)
//     {
//         seedPoints.Add(new Point(random.Next(0, width), random.Next(0, height)));
//     }
//
//     return seedPoints;
// }

static void RunOptions(Options opts)
{
    try
    {
        var s = new Stopwatch();
        s.Start();
        
        if (!string.IsNullOrEmpty(opts.RegionGrowing))
        {
            const int threshold = 50;

            var image = Image.Load<Rgb24>(opts.Input);

            List<Point> seedPoints =
            [
                new (100, 100), new (100, 200), new (100, 300), new (100, 400), new (100, 500),
                new (200, 100), new (200, 200), new (200, 300), new (200, 400), new (200, 500),
                new (300, 100), new (300, 200), new (300, 300), new (300, 400), new (300, 500),
                new (400, 100), new (400, 200), new (400, 300), new (400, 400), new (400, 500),
                new (500, 100), new (500, 200), new (500, 300), new (500, 400), new (500, 500)
            ];
            
            Operations.GrowRegions(image, seedPoints, EuclideanDistance).SaveAsBmp(opts.RegionGrowing);
            return;

            // bool SeparateColors(Rgb24 color1, Rgb24 color2)
            // {
            //     return Math.Abs(color1.R - color2.R) <= threshold &&
            //            Math.Abs(color1.G - color2.G) <= threshold &&
            //            Math.Abs(color1.B - color2.B) <= threshold;
            // }
            // bool MeanColor(Rgb24 color1, Rgb24 color2)
            // {
            //     return (Math.Abs(color1.R - color2.R) + Math.Abs(color1.G - color2.G) +
            //            Math.Abs(color1.B - color2.B)) / 3 <= threshold;
            // }
            bool EuclideanDistance(Rgb24 color1, Rgb24 color2)
            {
                return Math.Sqrt(Math.Pow(color1.R - color2.R, 2) + Math.Pow(color1.G - color2.G, 2) +
                                 Math.Pow(color1.B - color2.B, 2)) <= threshold;
            }
        }
        
        Image<L8> output;
        var input = Image.Load<L8>(opts.Input);

        if (!string.IsNullOrEmpty(opts.Dilation))
        {
            var structuringElement = new[,]
            {
                { 1, 1, 1 },
                { 1, 0, 1 },
                { 1, 1, 1 }
            };
            output = Operations.Dilation(ref input, structuringElement);
            output.SaveAsBmp(opts.Dilation);
        }
        if (!string.IsNullOrEmpty(opts.Erosion))
        {
            var structuringElement = new[,]
            {
                { 1, 1, 1 },
                { 1, 0, 1 },
                { 1, 1, 1 }
            };
            output = Operations.Erode(ref input, structuringElement);
            output.SaveAsBmp(opts.Erosion);
        }

        if (!string.IsNullOrEmpty(opts.Opening))
        {
            var structuringElement = new[,]
            {
                { 1, 1, 1 },
                { 1, 0, 1 },
                { 1, 1, 1 }
            };
            output = Operations.Open(ref input, structuringElement);
            output.SaveAsBmp(opts.Opening);
        }
        
        if (!string.IsNullOrEmpty(opts.Closing))
        {
            var structuringElement = new[,]
            {
                { 1, 1, 1 },
                { 1, 0, 1 },
                { 1, 1, 1 }
            };
            output = Operations.Close(ref input, structuringElement);
            output.SaveAsBmp(opts.Closing);
        }

        if (!string.IsNullOrEmpty(opts.HmtTransformation))
        {
            var structuringElement = new[,]
            {
                { 0, 0, 0 },
                { -1, 1, -1 },
                { 1, 1, 1 }
            };
            output = Operations.HmtTransformation(ref input, structuringElement);
            output.SaveAsBmp(opts.HmtTransformation);
        }
        if (!string.IsNullOrEmpty(opts.M6))
        {
            var elements = new List<int[,]>
            {
                new[,]
                {
                    { 0, 0, 0 },
                    { -1, 1, -1 },
                    { 1, 1, 1 }
                },
                new[,]
                {
                    { -1, 0, 0 },
                    { 1, 1, 0 },
                    { 1, 1, -1 }
                },
                new[,]
                {
                    { 1, -1, 0 },
                    { 1, 1, 0 },
                    { 1, -1, 0 }
                },
                new[,]
                {
                    { 1, 1, -1 },
                    { 1, 1, 0 },
                    { -1, 0, 0 }
                },
                new[,]
                {
                    { 1, 1, 1 },
                    { -1, 1, -1 },
                    { 0, 0, 0 }
                },
                new[,]
                {
                    { -1, 1, 1 },
                    { 0, 1, 1 },
                    { 0, 0, -1 }
                },
                new[,]
                {
                    { 0, -1, 1 },
                    { 0, 1, 1 },
                    { 0, -1, 1 }
                },
                new[,]
                {
                    { 0, 0, -1 },
                    { 0, 1, 1 },
                    { -1, 1, 1 }
                }
            };
            
            output = Operations.M6(ref input, elements);
            output.SaveAsBmp(opts.M6);
        }
        
        s.Stop();
        Console.WriteLine("Time: " + s.ElapsedMilliseconds + "ms");
    }
    catch (Exception e)
    {
        Console.WriteLine("Something went wrong: " + e.Message);
    }
}