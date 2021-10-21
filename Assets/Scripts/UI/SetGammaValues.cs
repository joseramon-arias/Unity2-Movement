using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SetGammaValues : MonoBehaviour
{
    [SerializeField] private VolumeProfile _profile;

    private void OnEnable()
    {
        if(_profile.TryGet(out LiftGammaGain liftGammaGain) && TryGetComponent(out Slider slider))
        {
            float currentGamma = liftGammaGain.gamma.value.x;
            slider.SetValueWithoutNotify(currentGamma);
        }
    }

    public void SetGamma(float value)
    {
        if (_profile.TryGet(out LiftGammaGain liftGammaGain))
        {
            liftGammaGain.gamma.value = Vector4.one * value;
        }
    }
}
