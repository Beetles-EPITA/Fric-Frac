using System;
using System.Collections;
using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    // References :
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset preset;
    // Variables :
    [SerializeField, Range(0, 24)] private float TimeOfDay;

    private float startLight = 6f;

    private void Update()
    {
        if (preset == null) 
            return;

        if (Application.isPlaying && RoomManager.Instance.start)
        {
            if (startLight < 16f || startLight > 16.1f)
            {
                startLight -= 0.001f;
                if (startLight < 0) startLight = 24f;
                UpdateLighting(startLight / 24f);
            }
            else
            {
                Debug.LogError("END");
            }
        }
        else
        {
            UpdateLighting(TimeOfDay/24f);
        }
    }
    
    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = preset.ambientColor.Evaluate(timePercent);
        RenderSettings.fogColor = preset.fogColor.Evaluate(timePercent);

        if (directionalLight != null)
        {
            directionalLight.color = preset.directionalColor.Evaluate(timePercent);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f)-90f, 170, 0));
        }
    }
    
    private void OnValidate()
    {
        if (directionalLight != null)
            return;
        if (RenderSettings.sun != null)
        {
            directionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    directionalLight = light;
                    return;
                }
            }
        }
    }
}