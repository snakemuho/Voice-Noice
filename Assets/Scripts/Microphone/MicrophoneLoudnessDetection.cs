using UnityEngine;

namespace Anathema.Microphone
{
    public class MicrophoneLoudnessDetection : MonoBehaviour
    {
        public int sampleWindow = 512;
        public AudioSource source;
        AudioClip micClip;
        string micName;
        // Start is called before the first frame update
        void Start()
        {
            source = GetComponent<AudioSource>();
            //foreach (var mic in Microphone.devices)
            //    Debug.Log(mic.ToString());
            micName = UnityEngine.Microphone.devices[0];
            MicToAudioClip();
        }

        // Update is called once per frame
        void Update()
        {
            //micClip = Microphone.Start(micName, true, 2, AudioSettings.outputSampleRate);
            //cubeMovement.SetMicClip(micClip);
        }

        public void MicToAudioClip()
        {
            micClip = UnityEngine.Microphone.Start(null, true, 4, AudioSettings.outputSampleRate);
            source.clip = micClip;
            //cubeMovement.SetMicClip(micClip);
        }

        public float GetLoudnessFromMic()
        {
            return GetLoudnessFromAudioClip(UnityEngine.Microphone.GetPosition(UnityEngine.Microphone.devices[0]), micClip);
        }

        public float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip)
        {
            int startPosition = clipPosition - sampleWindow;

            if (startPosition < 0)
                return 0;
            float[] waveData = new float[sampleWindow];
            clip.GetData(waveData, startPosition);

            float totalLoudness = 0;

            for (int i = 0; i < sampleWindow; i++)
            {
                totalLoudness += Mathf.Abs(waveData[i]);
            }
            return totalLoudness / sampleWindow;
        }

        public float[] GetSpectrumFromAudioclip(float[] spectrum, AudioSource aSource)
        {
            spectrum = new float[256];
            aSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
            for (int i = 1; i < spectrum.Length - 1; i++)
            {
                Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
                Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
                Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
                Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
            }
            return spectrum;
        }
    }
}
