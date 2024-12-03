using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Task2;

public static class Operations
{
    #region histogram
    private static Dictionary<int, int> HistogramData(ref Image<L8> input)
    {
        var values = Enumerable.Range(0, 256).ToDictionary(i => i, i => 0);
        input.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                var row = accessor.GetRowSpan(y);
                for (int x = 0; x < accessor.Width; x++)
                {
                    ref L8 pixel = ref row[x];
                    values[pixel.PackedValue]++;
                }
            }
        });
        return values;
    }
    public static Image<L8> Histogram(ref Image<L8> input)
    {
        var output = new Image<L8>(256, 256, new L8(255));
        var values = HistogramData(ref input);
        var factor = values.Values.Max() / output.Height + 1;
        for (int i = 0; i < output.Width; i++)
        {   
            var height = Convert.ToByte(values[i] / factor);
            for (int j = 255 - height; j < 256; j++)
            {
                output[i, j] = new L8(0);
            }
        }
        return output;
    }
    #endregion
    
    #region image quality improvement
    // (H5) Hyperbolic final probability density function (--hhyper).
    public static Image<L8> Hyperbolic(ref Image<L8> input, byte min, byte max)
    {
        if (min >= max) throw new Exception("Min has to be smaller than max.");
        if (min == 0) throw new Exception("Min has to be greater than 0.");
        
        var output = new Image<L8>(input.Width, input.Height);
        var values = HistogramData(ref input);
        
        input.ProcessPixelRows(output, (inputAccessor, outputAccessor) =>
        {
            for (int y = 0; y < inputAccessor.Height; y++)
            {
                var inputRow = inputAccessor.GetRowSpan(y);
                var outputRow = outputAccessor.GetRowSpan(y);

                for (int x = 0; x < inputRow.Length; x++)
                {
                    ref var inputPixel = ref inputRow[x];
                    ref var outputPixel = ref outputRow[x];
                    
                    var n = inputAccessor.Height * inputAccessor.Width;
                    long sum = values.Take(inputPixel.PackedValue + 1).Sum(kv => kv.Value);
                    var newValue = Convert.ToByte(min * Math.Pow((float)max / min, 1.0 / n * sum));
                    outputPixel = new L8(newValue);
                }
            }
        });
        return output;
    }
    #endregion
    
    #region image characteristics
    // (C1) Mean (--cmean). Variance (--cvariance).
    public static double Mean(ref Image<L8> input)
    {
        var result = 0.0;
        var values = HistogramData(ref input);
        for (int i = 0; i < values.Keys.Count; i++)
        {
            result += i * values[i];
        }
        return result / (input.Width * input.Height);
    }
    public static double Variance(ref Image<L8> input)
    {
        var mean = Mean(ref input);
        var result = 0.0;
        var values = HistogramData(ref input);
        for (int i = 0; i < values.Keys.Count; i++)
        {
            result += Math.Pow(i - mean, 2) * values[i];
        }
        return result / (input.Width * input.Height);
    }
    // (C2) Standard deviation (--cstdev). Variation coefficient I (--cvarcoi).
    public static double StandardDeviation(ref Image<L8> input)
    {
        return Math.Sqrt(Variance(ref input));
    }

    public static double VariationCoefficient(ref Image<L8> input)
    {
        return StandardDeviation(ref input)/Mean(ref input);
    }

    // (C3) Asymmetry coefficient (--casyco).
    public static double AsymmetryCoefficient(ref Image<L8> input)
    {
        var a = (Math.Pow(StandardDeviation(ref input), 3));
        var mean = Mean(ref input);
        var result = 0.0;
        var values = HistogramData(ref input);
        for (int i = 0; i < values.Keys.Count; i++)
        {
            result += Math.Pow(i - mean, 2) * values[i];
        }
        return result / (input.Width * input.Height * a);
    }

    // (C4) Flattening coefficient (--cflaco).
    public static double FlatteningCoefficient(ref Image<L8> input)
    {
        var mean = Mean(ref input);
        var variance = Variance(ref input);
        var values = HistogramData(ref input);
        var result = 0.0;
        for (int i = 0; i < values.Keys.Count; i++)
        {
            result += Math.Pow(i - mean, 4) * values[i] - 3;
        }
        return result / (input.Width * input.Height) / Math.Pow(variance, 2);
    }
    // (C5) Variation coefficient II (--cvarcoii).
    public static double VariationCoefficient2(ref Image<L8> input)
    {
        var a = 0.0;
        var values = HistogramData(ref input);
        for (int i = 0; i < values.Keys.Count; i++)
        {
            a += Math.Pow(values[i], 2);
        }
        return Math.Pow((1.0 / (input.Width * input.Height)),2) * a;
    }
    
    // (C6) Information source entropy (--centropy).
    public static double InformationSourceEntropy(ref Image<L8> input)
    {
        var a = 0.0;
        var n = 1.0 * input.Width * input.Height;
        var values = HistogramData(ref input);
        for (int i = 0; i < values.Keys.Count; i++)
        {
            if (values[i] == 0) continue;
            var temp = values[i] * Math.Log2(values[i]/n);
            a += temp;
        }
        return (-1.0 / n) * a;
    }
    
    #endregion
    
    #region linear filtration in spatial domain
    // (S6) Line identification (--slineid).
    public static Image<L8> LineIdentification(ref Image<L8> input, int variant)
    {
        var output = new Image<L8>(input.Width, input.Height);

        for (int i = 0; i < input.Width; i++)
        {
            for (int j = 0; j < input.Height; j++)
            {
                if (i == 0 || j == 0 || i == input.Width - 1 || j == input.Height - 1) output[i, j] = input[i, j];
                else
                {
                    var value = variant switch
                    {
                        1 => // vertical
                            input[i - 1, j - 1].PackedValue * -1 + input[i, j - 1].PackedValue * 2 +
                            input[i + 1, j - 1].PackedValue * -1 +
                            input[i - 1, j].PackedValue * -1 + input[i, j].PackedValue * 2 +
                            input[i + 1, j].PackedValue * -1 +
                            input[i - 1, j + 1].PackedValue * -1 + input[i, j + 1].PackedValue * 2 +
                            input[i + 1, j + 1].PackedValue * -1,
                        2 => // horizontal
                            input[i - 1, j - 1].PackedValue * -1 + input[i, j - 1].PackedValue * -1 +
                            input[i + 1, j - 1].PackedValue * -1 +
                            input[i - 1, j].PackedValue * 2 + input[i, j].PackedValue * 2 +
                            input[i + 1, j].PackedValue * 2 +
                            input[i - 1, j + 1].PackedValue * -1 + input[i, j + 1].PackedValue * -1 +
                            input[i + 1, j + 1].PackedValue * -1,
                        3 => // diagonal 45
                            input[i - 1, j - 1].PackedValue * -1 + input[i, j - 1].PackedValue * -1 +
                            input[i + 1, j - 1].PackedValue * 2 +
                            input[i - 1, j].PackedValue * -1 + input[i, j].PackedValue * 2 +
                            input[i + 1, j].PackedValue * -1 +
                            input[i - 1, j + 1].PackedValue * 2 + input[i, j + 1].PackedValue * -1 +
                            input[i + 1, j + 1].PackedValue * -1,
                        4 => // diagonal 135
                            input[i - 1, j - 1].PackedValue * 2 + input[i, j - 1].PackedValue * -1 +
                            input[i + 1, j - 1].PackedValue * -1 +
                            input[i - 1, j].PackedValue * -1 + input[i, j].PackedValue * 2 +
                            input[i + 1, j].PackedValue * -1 +
                            input[i - 1, j + 1].PackedValue * -1 + input[i, j + 1].PackedValue * -1 +
                            input[i + 1, j + 1].PackedValue * 2,
                        _ => 0
                    };

                    output[i, j] = new L8(value.ToByte());
                }
            }
        }
        return output;
    }
    
    public static Image<L8> LineIdentificationImproved(ref Image<L8> input)
    {
        var output = new Image<L8>(input.Width, input.Height, new L8(0));
        
        input.ProcessPixelRows(output, (inputAccessor, outputAccessor) =>
        {
            for (int y = 1; y < inputAccessor.Height - 1; y++)
            {
                var inputRowHigh = inputAccessor.GetRowSpan(y - 1);
                var inputRow = inputAccessor.GetRowSpan(y);
                var inputRowLow = inputAccessor.GetRowSpan(y + 1);
                var outputRow = outputAccessor.GetRowSpan(y);

                for (int x = 1; x < inputRow.Length - 1; x++)
                {
                    var value = inputRowHigh[x - 1].PackedValue * -1 + inputRowHigh[x].PackedValue * -1 + inputRowHigh[x + 1].PackedValue * 2 +
                                inputRow[x - 1].PackedValue * -1 + inputRow[x].PackedValue * 2 + inputRow[x + 1].PackedValue * -1 +
                                inputRowLow[x - 1].PackedValue * 2 + inputRowLow[x].PackedValue * -1 + inputRowLow[x + 1].PackedValue * -1;
                    outputRow[x].PackedValue = value.ToByte();
                }
            }
        });
        return output;
    }
    #endregion
    
    #region non-linear filtration in spatial domain
    
    // (O2) Roberts operator II (--orobertsii) initial implementation.
    public static Image<L8> RobertsOperatorIiInitial(ref Image<L8> input)
    {
        var output = new Image<L8>(input.Width, input.Height, new L8(0));
        for (int i = 0; i < input.Width - 1; i++)
        {
            for (int j = 0; j < input.Height - 1; j++)
            {
                output[i, j] = new L8( (Math.Abs(input[i, j].PackedValue - input[i + 1, j].PackedValue)+
                                        Math.Abs(input[i, j + 1].PackedValue - input[i + 1, j].PackedValue)).ToByte());
            }
        }
        return output;
    }
    // (O2) Roberts operator II (--orobertsii) optimized implementation.
    public static Image<L8> RobertsOperatorIi(Image<L8> input)
    {
        var output = new Image<L8>(input.Width, input.Height, new L8(0));
        Parallel.For(0, input.Width - 1, i =>
        {
            Parallel.For(0, input.Height - 1, j =>
            { 
                output[i, j] = new L8((Math.Abs(input[i, j].PackedValue - input[i + 1, j].PackedValue) + 
                                       Math.Abs(input[i, j + 1].PackedValue - input[i + 1, j].PackedValue)).ToByte());
            });
        });
        return output;
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
}