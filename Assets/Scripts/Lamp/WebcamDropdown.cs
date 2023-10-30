using UnityEngine;
using UnityEngine.UI;

namespace Anathema.Lamp.Webcam
{
    public class WebcamDropdown : MonoBehaviour
    {
        Dropdown dropdown;
        void Start()
        {
            dropdown = GetComponent<Dropdown>();
            dropdown.options.Clear();
            foreach (var webcam in WebCamTexture.devices)
            {
                dropdown.options.Add(new Dropdown.OptionData() { text = webcam.name } );
            }
            dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });
        }

        void DropdownItemSelected(Dropdown dropdown)
        {
            WebcamBrightnessReader.SetWebcam(dropdown.options[dropdown.value].text);
        }
    }
}
