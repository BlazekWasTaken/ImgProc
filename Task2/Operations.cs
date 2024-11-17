using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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

                    long sum = 0;
                    for (int i = 0; i <= inputPixel.PackedValue; i++)
                    {
                        sum += values[i];
                    }
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
    // (C2) Standard deviation (--cstdev). Variation coefficient I (--cvarcoi).
    // (C3) Asymmetry coefficient (--casyco).
    // (C4) Flattening coefficient (--casyco).
    // (C5) Variation coefficient II (--cvarcoii).
    // (C6) Information source entropy (--centropy).
    
    #endregion
    
    #region linear filtration in spatial domain
    
    // (S6) Line identification (--slineid).
    
    #endregion
    
    #region non-linear filtration in spatial domain
    
    // (O2) Roberts operator (--orobertsii).
    
    #endregion
}