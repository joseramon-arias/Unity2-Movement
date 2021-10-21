using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetMixerParameter : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private string _parameterName = "Missing Param";

    private void OnEnable()
    {
        if(_mixer.GetFloat(_parameterName, out float value) && TryGetComponent(out Slider slider))
        {
            slider.SetValueWithoutNotify(value);
        }
    }

    public void SetValue(float value)
    {
        _mixer.SetFloat(_parameterName, value);
    }
}
