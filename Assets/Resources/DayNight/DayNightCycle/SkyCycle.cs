using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SkyCycle : MonoBehaviour
{
    public float secondsPerMinute = 0.625f;
    public float startTime = 12f;

    public float latitudeAngle = 45f;
    public Transform sunTilt;
    public LightingManager Manager;
    public static bool losed = false;
    
    private float day;
    private float min;
    private float smoothMin;

    private float texOffset;
    private Material skyMat;
    private Transform sunOrbit;
    private float time;
    
    // Start is called before the first frame update
    void Start()
    {
        skyMat = GetComponent<Renderer>().sharedMaterial;
        sunOrbit = sunTilt.GetChild(0);

        sunTilt.eulerAngles = new Vector3(Mathf.Clamp(latitudeAngle, 0, 90), sunOrbit.eulerAngles.y, sunOrbit.eulerAngles.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying && RoomManager.Instance.start)
        {
            if(RoomManager.Instance.FinalScreen.gameObject.activeSelf)
                return;
            if (sunOrbit.rotation.y > 0.85f && PhotonNetwork.IsMasterClient)
            {
                losed = true;
                RoomManager.Instance.photonView.RPC("CheckWin", RpcTarget.All, (int) Laucher.Team.Resident);
            }
            if (time == 0f) time = Time.time;
            smoothMin = ((Time.time-time) / secondsPerMinute) + (startTime * 60);
            day = Mathf.Floor(smoothMin / 1440) + 1;

            smoothMin = smoothMin - (Mathf.Floor(smoothMin / 1440) * 1440); //clamp smoothMin between 0-1440
            min = Mathf.Round(smoothMin);
            sunOrbit.localEulerAngles =
                new Vector3(sunOrbit.localEulerAngles.x, smoothMin / 4, sunOrbit.localEulerAngles.z);
            float f = ((smoothMin / 1440) * 2) * Mathf.PI;
            texOffset = Mathf.Cos(f) * 0.25f + 0.25f;
            skyMat.mainTextureOffset =
                new Vector2(Mathf.Round((texOffset - (Mathf.Floor(texOffset / 360) * 360)) * 1000) / 1000, 0);
        }
    }
}
