using UnityEngine;

[RequireComponent(typeof(ParticleSystem), typeof(AudioSource))]
public class AudioParticles : MonoBehaviour {
    private AudioSpectrumAnalyser audioSpectrumAnalyser;
    private ParticleSystem particleSystem;

    // Use this for initialization
    void Start () {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSpectrumAnalyser = new AudioSpectrumAnalyser(audioSource, AudioSpectrumAnalyser.AudioSampleSize.Medium, FFTWindow.Rectangular);

        particleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update () {
        audioSpectrumAnalyser.UpdateSpectrumData();

        float[] audioSpectrumData = audioSpectrumAnalyser.audioSpectrumData;

        ParticleSystem.MainModule main = particleSystem.main;
        main.startSizeMultiplier = audioSpectrumData[audioSpectrumAnalyser.dominantFrequencyBandIndex]*2.0f; 
    }
}
