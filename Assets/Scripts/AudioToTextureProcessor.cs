using UnityEngine;

[RequireComponent (typeof(MeshRenderer))]
public class AudioToTextureProcessor : MonoBehaviour {
    private static int AUDIO_SAMPLE_SIZE = 256;

    private Material audioMaterial;
    private float[] audioSampleData;
    private Texture2D audioTexture;


	// Use this for initialization
	void Start () {
        audioSampleData = new float[AUDIO_SAMPLE_SIZE];
        audioTexture = new Texture2D(AUDIO_SAMPLE_SIZE, 1, TextureFormat.RGBA32, false);

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        audioMaterial = meshRenderer.material;
        audioMaterial.SetTexture("_MainTex", audioTexture);
    }
	
	// Update is called once per frame
	void Update () {
        AudioListener.GetSpectrumData(audioSampleData, 0, FFTWindow.Triangle);
        //Debug.Log("Begin Sample Data");

        for (int sampleIndex = 0; sampleIndex < AUDIO_SAMPLE_SIZE; sampleIndex++) {
            //Debug.Log("Sample Data: " + audioSampleData[sampleIndex]);
            Color pixel = new Color((audioSampleData[sampleIndex] * 255.0f), 0.0f, 0.0f, 0.0f);
            audioTexture.SetPixel(sampleIndex, 1, pixel);
        }
        //Debug.Log("End Sample Data");

        audioTexture.Apply();
    }
}
