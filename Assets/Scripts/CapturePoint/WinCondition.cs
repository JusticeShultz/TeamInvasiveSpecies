using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    public GameObject WinUI;
    public GameObject WaterObject;
    public GameObject OutOfWaterVolume;
    public GameObject InWaterVolume;
    public List<CapturePoint> CapturePoints = new List<CapturePoint>();
    
    bool DoOnce = false;

    void Start ()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if(PlayerController.Player_Controller)
        {
            if(PlayerController.Player_Controller.HMDPos.transform.position.y < WaterObject.transform.position.y - 1.95f)
            {
                OutOfWaterVolume.SetActive(false);
                InWaterVolume.SetActive(true);
            }
            else
            {
                OutOfWaterVolume.SetActive(true);
                InWaterVolume.SetActive(false);
            }
        }

        if(!WinUI) WinUI = GameObject.FindWithTag("Player").GetComponent<Reference>().Pointer;

        if(WaterObject)
        {
            int Mode = 0;

            if (CapturePoints[0].Captured) ++Mode;
            if (CapturePoints[1].Captured) ++Mode;
            if (CapturePoints[2].Captured) ++Mode;
            if (CapturePoints[3].Captured) ++Mode;

            if (Mode == 0)
                WaterObject.transform.position = Vector3.Lerp(WaterObject.transform.position, new Vector3(0, -2, 0), 0.01f);
            else if (Mode == 1)
                WaterObject.transform.position = Vector3.Lerp(WaterObject.transform.position, new Vector3(0, -1.2f, 0), 0.01f);
            else if (Mode == 2)
                WaterObject.transform.position = Vector3.Lerp(WaterObject.transform.position, new Vector3(0, 0.7f, 0), 0.01f);
            else if (Mode == 3)
                WaterObject.transform.position = Vector3.Lerp(WaterObject.transform.position, new Vector3(0, 1.45f, 0), 0.01f);
            else if (Mode == 4)
                WaterObject.transform.position = Vector3.Lerp(WaterObject.transform.position, new Vector3(0, 15, 0), 0.01f);
        }

        foreach (CapturePoint point in CapturePoints)
            if (!point.Captured) return;

        //Win condition here?
        if (!DoOnce)
        {
            DoOnce = true;
            WinUI.SetActive(true);
        }
    }

    public void ReloadLevel()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        SteamVR_Fade.View(Color.black, 0.75f);
        yield return new WaitForSeconds(0.85f);
        Destroy(GameObject.FindWithTag("Player"));
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        yield return new WaitForSeconds(2f);
        SteamVR_Fade.View(Color.clear, 1);
        Destroy(gameObject);
    }
}
