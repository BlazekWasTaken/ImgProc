using System.Diagnostics;
using CommandLine;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Task3;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(RunOptions);

return;

static void RunOptions(Options opts)
{
    try
    {
        var s = new Stopwatch();
        s.Start();
        
        var input = Image.Load<L8>(opts.Input);
        Image<L8> output;

        if (!string.IsNullOrEmpty(opts.Dilation))
        {
            var structuringElement = new int[3, 3]
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
            var structuringElement = new int[3, 3]
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
            var structuringElement = new int[3, 3]
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
            var structuringElement = new int[3, 3]
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
            var structuringElement1 = new int[3, 3]
            {
                { 1, -1, -1 },
                { 1, 0, -1 },
                { 1, -1, -1 }
            };
            var structuringElement2 = new int[3, 3]
            {
                { 0, -1, -1 },
                { 0, 1, -1 },
                { 0, -1, -1 }
            };
            output = Operations.HmtTransformation(ref input, structuringElement1, structuringElement2);
            output.SaveAsBmp(opts.HmtTransformation);
        }
        
        s.Stop();
        Console.WriteLine("Time: " + s.ElapsedMilliseconds + "ms");
    }
    catch (Exception e)
    {
        Console.WriteLine("Something went wrong: " + e.Message);
    }
}