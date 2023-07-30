using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFromMic : MonoBehaviour
{
    //public AudioSource source;
    public AudioLoudnessDetection detector;
    public float loudnessSensitivity = 2, threshold = .1f;

    public float[] spectrum;
    float horizontalMovement, verticalMovement;
    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;
    Vector3 velocity, desiredVelocity;
    Rigidbody body;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        //source = GetComponent<AudioSource>();
    }


    // Update is called once per frame
    void Update()
    {
        velocity = body.velocity;
        spectrum = new float[256];
        float loudness = detector.GetLoudnessFromMic() * loudnessSensitivity;
        //source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        spectrum = detector.GetSpectrumFromAudioclip(spectrum, detector.source);
        if (loudness < threshold)
        {
            loudness = 0;
        }
        if (loudness > 0 && loudness <= 0.5f)
            horizontalMovement = -loudness * 2;
        else if (loudness > 0.5f) horizontalMovement = loudness / 2;
        else horizontalMovement = 0;
        desiredVelocity = new Vector3(horizontalMovement, 0f) * maxSpeed;

        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);

        body.velocity = velocity;
    }

    //public void SetMicClip(AudioClip clip)
    //{
    //    source.clip = clip;
    //}
}
