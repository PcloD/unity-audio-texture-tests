using UnityEngine;

[RequireComponent(typeof(ParticleSystem), typeof(AudioSource))]
public class AudioParticles : MonoBehaviour {
    private AudioSpectrumAnalyser audioSpectrumAnalyser;
    private ParticleSystem theParticleSystem;

    // Use this for initialization
    void Start () {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSpectrumAnalyser = new AudioSpectrumAnalyser(audioSource, AudioSpectrumAnalyser.AudioSampleSize.Medium, FFTWindow.Rectangular);

        theParticleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update () {
        audioSpectrumAnalyser.UpdateSpectrumData();

        float[] audioSpectrumData = audioSpectrumAnalyser.audioSpectrumData;

        ParticleSystem.MainModule main = theParticleSystem.main;

        // Alter the size
        main.startSizeMultiplier = audioSpectrumAnalyser.dominantFrequencyBandIndex >= 0 ? audioSpectrumData[audioSpectrumAnalyser.dominantFrequencyBandIndex]*2.0f : 0.0f;

        // Alter the colour
        float bandIndex = audioSpectrumAnalyser.dominantFrequencyBandIndex >= 0 ? audioSpectrumAnalyser.dominantFrequencyBandIndex : 0;
        // NOTE: Arbitrary scale here due to the dominant frequency band often being at the lower end of the spectrum.
        float normalisedBandIndex = bandIndex * 32.0f / (float)((int)AudioSpectrumAnalyser.AudioSampleSize.Medium-1);
        float red = 1.0f - normalisedBandIndex;
        Debug.Log("bandIndex = " + bandIndex + " Normalised = " + normalisedBandIndex+ " red = "+red);
        Color particleColour = new Color(red, 0.0f, 0.0f, 1.0f);
        main.startColor = particleColour;
    }
}
