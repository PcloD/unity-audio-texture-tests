using UnityEngine;

[RequireComponent (typeof(MeshRenderer), typeof(AudioSource))]
public class AudioToTextureProcessor : MonoBehaviour {
    private static int AUDIO_SAMPLE_SIZE = 256;

    [Tooltip("The hieght of the texture to show a histogram of the audio specturm data. 1 means no histogram data.")]
    [Range(1, 100)]
    public int audioTextureHeight = 50;

    private AudioSource audioSource;
    private Material audioMaterial;
    private float[] audioSampleData;
    private Texture2D audioTexture;


	// Use this for initialization
	void Start () {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSampleData = new float[AUDIO_SAMPLE_SIZE];
        audioTexture = new Texture2D(AUDIO_SAMPLE_SIZE, audioTextureHeight, TextureFormat.RGBA32, false);

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        audioMaterial = meshRenderer.material;
        audioMaterial.SetTexture("_MainTex", audioTexture);
    }
	
	// Update is called once per frame
	void Update () {
        audioSource.GetSpectrumData(audioSampleData, 0, FFTWindow.Triangle);
        //Debug.Log("Begin Sample Data");

        ShiftAudioHistogramTextureData();

        for (int sampleIndex = 0; sampleIndex < AUDIO_SAMPLE_SIZE; sampleIndex++) {
            //Debug.Log("Sample Data: " + audioSampleData[sampleIndex]);
            Color pixel = new Color((audioSampleData[sampleIndex] * 255.0f), 0.0f, 0.0f, 0.0f);
            audioTexture.SetPixel(sampleIndex, 0, pixel);
        }
        //Debug.Log("End Sample Data");

        audioTexture.Apply();
    }

    // Shift the pixel data leaving from the first row onwards.
    void ShiftAudioHistogramTextureData() {
        if (audioTexture.height > 1) {
            Color[] pixelData = audioTexture.GetPixels(0, 0, audioTexture.width, audioTexture.height - 1);
            audioTexture.SetPixels(0, 1, audioTexture.width, audioTexture.height - 1, pixelData);
        }
    }

}
