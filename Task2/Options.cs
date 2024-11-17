using CommandLine;

namespace Task2;

public class Options
{
    [Option(longName: "input", Required = true, HelpText = "Input file.")]
    public required string Input { get; set; }
    
    [Option(longName: "histogram", Required = false, HelpText = "Output file.")]
    public string? Histogram { get; set; }
    
    #region image quality improvement
    
    
    [Option(longName: "hhyper", Required = false, HelpText = "Hyperbolic final probability density function. Format: path,min,max.")]
    public string? Hyper { get; set; }
    
    #endregion
    
    #region image characteristics
    
    [Option(longName: "cmean", Required = false, HelpText = "Mean.")]
    public bool Mean { get; set; }
    [Option(longName: "cvariance", Required = false, HelpText = "Variance.")]
    public bool Variance { get; set; }
    
    [Option(longName: "cstdev", Required = false, HelpText = "Standard deviation.")]
    public bool StandardDeviation { get; set; }
    [Option(longName: "cvarcoi", Required = false, HelpText = "Variation coefficient I.")]
    public bool VariationCoefficient1 { get; set; }
    
    [Option(longName: "casyco", Required = false, HelpText = "Asymmetry coefficient.")]
    public bool AsymmetryCoefficient { get; set; }
    [Option(longName: "cflaco", Required = false, HelpText = "Flattening coefficient.")]
    public bool FlatteningCoefficient { get; set; }
    
    [Option(longName: "cvarcoii", Required = false, HelpText = "Variation coefficient II.")]
    public bool VariationCoefficient2 { get; set; }
    [Option(longName: "centropy", Required = false, HelpText = "Information source entropy.")]
    public bool InformationSourceEntropy { get; set; }
    
    #endregion
    
    #region linear filtration in spatial domain
    
    [Option(longName: "slineid", Required = false, HelpText = "Line identification.")]
    public bool LineIdentification { get; set; }
    
    #endregion
    
    #region non-linear filtration in spatial domain
    
    [Option(longName: "orobertsii", Required = false, HelpText = "Roberts operator.")]
    public bool RobertsOperator { get; set; }
    
    #endregion
}