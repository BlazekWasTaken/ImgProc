using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Task2;

public static class Operations
{
    #region histogram

    private static Dictionary<int, int> HistogramData(Image<L8> input)
    {
        var values = Enumerable.Range(0, 256).ToDictionary(i => i, i => 0);
        
        if (System.Diagnostics.Debugger.IsAttached)
        {
            for (int i = 0; i < input.Width; i++)
            {
                for (int j = 0; j < input.Height; j++)
                {
                    values[input[i, j].PackedValue]++;
                }
            }
        }
        else
        {
            Parallel.For(0, input.Width, i =>
            {
                Parallel.For(0, input.Height, j =>
                {
                    values[input[i, j].PackedValue]++;
                });
            });
        }
        return values;
    }

    public static Image<L8> Histogram(ref Image<L8> input)
    {
        var output = new Image<L8>(256, 256, new L8(255));
        var values = HistogramData(input);
        var max = values.Values.Max();
        var factor = max / output.Height + 1;
        for (int i = 0; i < output.Width; i++)
        {
            var height = Convert.ToByte(values[i] / factor);
            output[i, 255 - height] = new L8(0);
        }
        return output;
    }

    #endregion
    
    #region image quality improvement
    
    // (H5) Hyperbolic final probability density function (--hhyper).
    
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