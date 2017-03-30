﻿using UnityEngine;

//
// Summary:
//     ///
//     Provides functionality for converting a playing AudioSource spectrum into texture data for display via a renderer
//     ///
[RequireComponent (typeof(MeshRenderer), typeof(AudioSource))]
public class AudioToTextureProcessor : MonoBehaviour {
    [Tooltip("The hieght of the texture to show a histogram of the audio specturm data. 1 means no histogram data.")]
    [Range(1, 100)]
    public int audioTextureHeight = 50;
    private int audioTextureWidth;

    private AudioSpectrumAnalyser audioSpectrumAnalyser;
    private Material audioMaterial;
    private Texture2D audioTexture;


	// Use this for initialization
	void Start () {
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSpectrumAnalyser = new AudioSpectrumAnalyser(audioSource, AudioSpectrumAnalyser.AudioSampleSize.Medium, FFTWindow.Rectangular);

        audioTextureWidth = (int)AudioSpectrumAnalyser.AudioSampleSize.Medium;
        audioTexture = new Texture2D(audioTextureWidth, audioTextureHeight, TextureFormat.RGBA32, false);

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        audioMaterial = meshRenderer.material;
        audioMaterial.SetTexture("_MainTex", audioTexture);
    }
	
	// Update is called once per frame
	void Update () {
        ShiftAudioHistogramTextureData();

        audioSpectrumAnalyser.UpdateSpectrumData();

        float[] audioSpectrumData = audioSpectrumAnalyser.audioSpectrumData;

        //Debug.Log("Begin Sample Data");
        for (int index = 0; index < audioTextureWidth; index++) {
            //Debug.Log("Sample Data: " + audioSpectrumData[index]);

            float red = 0.0f, blue = 0.0f, green = 0.0f;
            if(index == audioSpectrumAnalyser.dominantFrequencyBandIndex) {
                green = (audioSpectrumData[index] * 255.0f);
            }
            else {
                red = (audioSpectrumData[index] * 255.0f);
            }

            Color pixel = new Color(red, green, blue, 0.0f);
            audioTexture.SetPixel(index, 0, pixel);
        }
        //Debug.Log("End Sample Data");

        audioTexture.Apply();
    }

    //
    // Summary:
    //     ///
    //     Shifts the pixel data within the audio texture up by one row.
    //     ///
    void ShiftAudioHistogramTextureData() {
        if (audioTexture.height > 1) {
            Color[] pixelData = audioTexture.GetPixels(0, 0, audioTexture.width, audioTexture.height - 1);
            audioTexture.SetPixels(0, 1, audioTexture.width, audioTexture.height - 1, pixelData);
        }
    }

}
