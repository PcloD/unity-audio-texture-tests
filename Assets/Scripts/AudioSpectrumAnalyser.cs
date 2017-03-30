using UnityEngine;

//
// Summary:
//     ///
//     An AudioSource spectrum anlyser that tracks key data about the AudioSource during playback 
//
//     References:
//     http://answers.unity3d.com/questions/157940/getoutputdata-and-getspectrumdata-they-represent-t.html
//     ///
public class AudioSpectrumAnalyser  {
    private const float MINIMUM_AMPLITUDE_FOR_PITCH = 0.02f;

    // Constant values for sample sizes that must be a power of 2.
    public enum AudioSampleSize : uint {
        VerySmall = 64,
        Small = 128,
        Medium = 256,
        Large = 512,
        VeryLarge = 1024
    }

    private readonly AudioSource audioSource;
    private readonly AudioSampleSize sampleSize;
    private readonly FFTWindow fftWindow;

    private float[] spectrumData;
    public float[] audioSpectrumData { get { return spectrumData; } }

    private float pitchValue = 0.0f;
    public float pitch { get { return pitchValue; } }

    private uint maxAmplitudeBandIndex = 0;
    public uint dominantFrequencyBandIndex { get { return maxAmplitudeBandIndex; }  }

    //
    // Summary:
    //     ///
    //     Create a new audio spectrum analyser
    //     ///
    //
    // Parameters:
    //   audioSource:
    //     AudioSource to analyse.
    //   audioSampleSize:
    //     One of the sample size constants that represent different powers of 2 to ensure valid input.  
    //   audioFFTWindow:
    //     The FFT to apply when accessing the spectrum data from the AudioSource.
    public AudioSpectrumAnalyser(AudioSource audioSource, AudioSampleSize audioSampleSize, FFTWindow audioFFTWindow) {
        this.audioSource = audioSource;
        fftWindow = audioFFTWindow;

        sampleSize = audioSampleSize;
        spectrumData = new float[(uint)sampleSize];
    }

    //
    // Summary:
    //     ///
    //     Updates the audio source spectrum data and any additional processing
    //     ///
    public void UpdateSpectrumData() {
        if(audioSource == null) {
            Debug.Log("Invalid audio source specified");
            return;
        }

        audioSource.GetSpectrumData(spectrumData, 0, fftWindow);

        pitchValue = DeterminePitch();
    }

    //
    // Summary:
    //     ///
    //     Determines the dominant frequency band and pitch value from the sampled audio spectrum
    //     ///
    private float DeterminePitch() {
        float maxAmplitude = 0.0f;
        for (uint index = 0; index < (uint)sampleSize; index++) {
            if (spectrumData[index] > maxAmplitude && spectrumData[index] > MINIMUM_AMPLITUDE_FOR_PITCH) {
                maxAmplitude = spectrumData[index];
                maxAmplitudeBandIndex = index;
            }
        }

        float amplitudeBandIndex = maxAmplitudeBandIndex;
        if (maxAmplitudeBandIndex > 0 && (maxAmplitudeBandIndex < (uint)sampleSize - 1)) { 
            float previousBandIndexAmplitude = spectrumData[maxAmplitudeBandIndex - 1] / maxAmplitude;
            float nextBandIndexAmplitude = spectrumData[maxAmplitudeBandIndex + 1] / maxAmplitude;
            amplitudeBandIndex += 0.5f * (previousBandIndexAmplitude * previousBandIndexAmplitude - nextBandIndexAmplitude * nextBandIndexAmplitude);
        }

        return amplitudeBandIndex * ((AudioSampleRate() * 0.5f) / (uint)sampleSize);
    }

    //
    // Summary:
    //     ///
    //     Wrapper method for the system sample rate
    //     ///
    private float AudioSampleRate() {
        return AudioSettings.outputSampleRate;
    }
}
