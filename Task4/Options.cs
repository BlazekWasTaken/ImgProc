using CommandLine;

namespace Task4;

public class Options
{
    //1. Standard discrete fourier transform
    //2. Implement the fast fourier transform
    //Both should yield the same result
    //One dimensional DFT and FFT
    //3. Inverse DFT and FFT to get back the original sequence (result identical to input)
    //Present the spectrum to the user with the circle in the middle after filtering - phase and/or magnitude (??)
    [Option(longName: "fourier", Required = false, HelpText = "Direct and inverse fast Fourier transform with decimation in frequency domain")]
    public string? Fourier { get; set; }
    
    [Option(longName: "low-pass", Required = false, HelpText = "Low-pass filter (high-cut filter)")]
    public string? LowPassFilter { get; set; }
    
    [Option(longName: "high-pass", Required = false, HelpText = "High-pass filter (low-cut filter)")]
    public string? HighPassFilter { get; set; }
    
    [Option(longName: "band-pass", Required = false, HelpText = "Band-pass filter")]
    public string? BandPassFilter { get; set; }
    
    [Option(longName: "band-cut", Required = false, HelpText = "Band-cut filter")]
    public string? BandCutFilter { get; set; }
    
    [Option(longName: "high-pass-edge", Required = false, HelpText = "High-pass filter with detection of edge direction")]
    public string? HighPassEdgeFilter { get; set; }
    
    [Option(longName: "phase-modifying", Required = false, HelpText = "Phase modifying filter")]
    public string? PhaseModifyingFilter { get; set; }
}