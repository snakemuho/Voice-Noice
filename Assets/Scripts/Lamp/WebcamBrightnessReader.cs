using UnityEngine;

namespace Anathema.Lamp.Webcam
{
    public class WebcamBrightnessReader : MonoBehaviour
    {
        static WebCamTexture webCamTexture;
        static string webcamName;
        [SerializeField] Material webcamMaterial;
        [SerializeField] RenderTexture rendTexture;
        Texture2D webcamPixelated;
        public static float CamBrightess { get; private set; }
        private void Start()
        {
            webcamPixelated = new Texture2D(32, 18, TextureFormat.RGBA32, false);
            if (webCamTexture == null)
                webCamTexture = new WebCamTexture(webcamName, requestedWidth: 32, requestedHeight: 18);
            if (!webCamTexture.isPlaying)
                webCamTexture.Play();

            //Mathf.Sqrt(0.299 * R ^ 2 + 0.587 * G ^ 2 + 0.114 * B ^ 2)

            Graphics.Blit(webCamTexture, rendTexture);
            webcamPixelated.ReadPixels(new Rect(0, 0, rendTexture.width, rendTexture.height), 0, 0, true);
            webcamPixelated.Apply();
        
            webcamMaterial.SetTexture("_WebcamTex", webcamPixelated);

  
        }

        public static void SetWebcam(string name)
        {
            webcamName = name;
            if (webCamTexture != null && webCamTexture.isPlaying )
                webCamTexture.Stop();
            webCamTexture = new WebCamTexture(webcamName, requestedWidth: 32, requestedHeight: 18);
            if (!webCamTexture.isPlaying)
                webCamTexture.Play();

            print(name);
        }
        private void FixedUpdate()
        {
            PixelateWebcam();
        }

        void PixelateWebcam()
        {
            Graphics.Blit(webCamTexture, rendTexture);
            webcamPixelated.ReadPixels(new Rect(0, 0, rendTexture.width, rendTexture.height), 0, 0, true);
            webcamPixelated.Apply();

            Color[] pixels = webcamPixelated.GetPixels();

            float tempBrightness = default;
            foreach (Color pixel in pixels)
            {
                tempBrightness += Mathf.Sqrt(0.299f * pixel.r * pixel.r + 0.587f * pixel.g * pixel.g + 0.114f * pixel.b * pixel.b);
            }
            CamBrightess = tempBrightness / pixels.Length;
            //print(CamBrightess);
        }
    }
}
