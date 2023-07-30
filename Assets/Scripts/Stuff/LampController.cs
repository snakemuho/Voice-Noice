using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class LampController : MonoBehaviour
{
    WebcamBrightnessReader webcamBrightness;
    Light myLight;
    LightSensor currentSensor;
    [SerializeField] Renderer lampRend;
    Material lampMat;
    float adjustedIntensity;
    float lightLevel;
    Color baseLanternColor, lightColor;
    [SerializeField, Range(10,100)] float maxRange = 50;

    private void Start()
    {
        webcamBrightness = GetComponent<WebcamBrightnessReader>();
        myLight = GetComponent<Light>();
        lightColor = new Color(1, 0.759434f, 0.759434f);
        lampMat = lampRend.materials[1];
        baseLanternColor = new Color(0.172549f, 0.02352941f, 0.03137255f);
        if (LightSensor.current != null)
        {
            currentSensor = LightSensor.current;
            InputSystem.EnableDevice(currentSensor);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.GamePaused)
            return;

        if (GameManager.Instance.isAlive)
        {
            if (LightSensor.current != null)
            {
                lightLevel = currentSensor.lightLevel.ReadValue();
                SetLightRange(lightLevel);
            }
            else
            {
                SetLightRange(WebcamBrightnessReader.CamBrightess * 200);
            }
        } else
        {
            SetLightRange(myLight.range - Time.fixedDeltaTime);
            //Debug.Log(myLight.range);
        }

        /*if (Input.GetKey(KeyCode.Alpha1))
        {
            if (myLight.range < 9 || myLight.range > 11)
                myLight.range += 0.5f;
            SetLampIntensity(myLight.range);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            if (myLight.range < 29 || myLight.range > 31)
                myLight.range += 1f;
            SetLampIntensity(myLight.range);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            if (myLight.range < 49 || myLight.range > 51)
                myLight.range += 1.5f;
            SetLampIntensity(myLight.range);
        }

        if (Input.GetKey(KeyCode.Z))
        {
            myLight.range -= 0.5f;
            SetLampIntensity(myLight.range);
        }
        if (Input.GetKey(KeyCode.C))
        {
            myLight.range += 0.5f;
            SetLampIntensity(myLight.range);
        }
        if (Input.GetKey(KeyCode.R))
        {
            myLight.range = 0;
            SetLampIntensity(myLight.range);
        }*/
    }

    void SetLightRange(float range)
    {
        if (range < 20)
            myLight.range = 0;
        else myLight.range = Mathf.Clamp(range / 10,2,maxRange);
        SetLampIntensity(myLight.range);
    }

    void SetLampIntensity(float intensity)
    {
        intensity /= 10;
        adjustedIntensity = Mathf.Clamp(intensity - (0.4169f), 0.5f, 5);
        Color color = baseLanternColor * Mathf.Pow(2.0f, adjustedIntensity);
        myLight.color = lightColor * Mathf.Clamp01(intensity);
        lampMat.SetColor("_EmissionColor", color);
    }
}
