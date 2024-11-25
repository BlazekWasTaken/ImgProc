using System.Diagnostics;
using CommandLine;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Task2;

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

        if (!string.IsNullOrEmpty(opts.Histogram))
        {
            output = Operations.Histogram(ref input);
            output.SaveAsBmp(opts.Histogram);
        }
        if (!string.IsNullOrEmpty(opts.Hyper))
        {
            var values = opts.Hyper.Split(',');
            if (values.Length != 3) throw new Exception("Invalid parameter format.");
            var min = byte.Parse(values[1]);
            var max = byte.Parse(values[2]);
            output = Operations.Hyperbolic(ref input, min, max);
            var path = values[0].Split('.');
            output.SaveAsBmp($"{path[0]}_{min}_{max}.{path[1]}");
        }
        if (!string.IsNullOrEmpty(opts.LineIdentification))
        {
            var values = opts.LineIdentification.Split(',');
            if (values.Length != 2) throw new Exception("Invalid parameter format.");
            var variantString = values[1];
            if (variantString.Equals("improved"))
            {
                output = Operations.LineIdentificationImproved(ref input);
            }
            else
            {
                var variant = int.Parse(values[1]);
                if (variant is < 1 or > 4) throw new Exception("Invalid variant.");
                output = Operations.LineIdentification(ref input, variant);
                
            }
            var path = values[0].Split('.');
            output.SaveAsBmp($"{path[0]}_{variantString}.{path[1]}");
        }

        if (!string.IsNullOrEmpty(opts.RobertsOperator))
        {
            output = Operations.RobertsOperatorIi(input);
            output.SaveAsBmp(opts.RobertsOperator);
        }
        if (opts.Mean)
        {
            Console.WriteLine("Mean: " + Operations.Mean(ref input));
        }
        if (opts.Variance)
        {
            Console.WriteLine("Variance: " + Operations.Variance(ref input));
        }
        if (opts.FlatteningCoefficient)
        {
            Console.WriteLine("Flattening coefficient: " + Operations.FlatteningCoefficient(ref input));
        }

        if (opts.AsymmetryCoefficient)
        {
            Console.WriteLine("Asymmetry coefficient: " + Operations.AsymmetryCoefficient(ref input));
        }

        if (opts.StandardDeviation)
        {
            Console.WriteLine("Standard deviation: " + Operations.StandardDeviation(ref input));
        }

        if (opts.InformationSourceEntropy)
        {
            Console.WriteLine("Information source entropy: " + Operations.InformationSourceEntropy(ref input));
        }

        if (opts.VariationCoefficient2)
        {
            Console.WriteLine("Variation coefficient 2: " + Operations.VariationCoefficient2(ref input));
        }

        if (opts.VariationCoefficient1)
        {
            Console.WriteLine("Variation coefficient 1: " + Operations.VariationCoefficient(ref input));
        }
        
        s.Stop();
        Console.WriteLine("Time: " + s.ElapsedMilliseconds + "ms");
    }
    catch (Exception e)
    {
        Console.WriteLine("Something went wrong: " + e.Message);
    }
}