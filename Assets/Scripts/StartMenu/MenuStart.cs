using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuStart : MonoBehaviour
{
    public GameObject PlayerDisplay;
    public PlayerController PlayerController;
    public Animator animator;
    public GameObject MenuUI;

    public static bool GameStarted = false;

	void Start ()
    {
        GameStarted = false;
        PlayerDisplay.SetActive(false);
        PlayerController.enabled = false;
    }

    public void StartGame()
    {
        MenuUI.SetActive(false);
        animator.SetTrigger("Start");
        StartCoroutine(OpenDropPod());
    }

    public void EndGame()
    {
        Application.Quit();
    }

    IEnumerator OpenDropPod()
    {
        yield return new WaitForSeconds(9f);

        GameStarted = true;
        PlayerDisplay.SetActive(true);
        PlayerController.enabled = true;
        //animator.gameObject.SetActive(false);
    }
}
