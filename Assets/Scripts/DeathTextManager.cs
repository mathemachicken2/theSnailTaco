using System.Collections;
using TMPro;
using UnityEngine;

public class DeathTextManager : MonoBehaviour
{
    public static DeathTextManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TMP_Text firedText;
    [SerializeField] private TMP_Text arrestedText;
    [SerializeField] private TMP_Text deathText;
    [SerializeField] private TMP_Text eatingText;
    [SerializeField] private TMP_Text checkReviewText;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // -------------------------------
    // General Arrested Messages
    // -------------------------------
    public void ShowArrestedMessages()
    {
        StartCoroutine(ArrestedMessageSequence());
    }

    private IEnumerator ArrestedMessageSequence()
    {
        firedText.text = "You're fired";
        firedText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        arrestedText.text = "You're going to prison, so you can't keep your job...........";
        arrestedText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        firedText.gameObject.SetActive(false);
        arrestedText.gameObject.SetActive(false);
    }
    public void ShowEatingMessage(string message)
    {
        StartCoroutine(EatingMessageSequence(message));
    }
    //Eating
    private IEnumerator EatingMessageSequence(string message)
    {
        eatingText.text = message;
        eatingText.gameObject.SetActive(true);

        yield return new WaitForSeconds(5f);

        checkReviewText.text = "Check the pickle taqueria's google reviews";
        checkReviewText.gameObject.SetActive(true);

        yield return new WaitForSeconds(5f);

        eatingText.gameObject.SetActive(false);
        checkReviewText.gameObject.SetActive(false);
    }

    // -------------------------------
    // Death By Blender
    // -------------------------------
    public void ShowDeathByBlenderMessage()
    {
        StartCoroutine(DeathByBlender());
    }

    private IEnumerator DeathByBlender()
    {
        firedText.text = "You're fired";
        firedText.gameObject.SetActive(true);

        deathText.text = "You jumped into a blender........";
        deathText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        deathText.gameObject.SetActive(false);
        firedText.gameObject.SetActive(false);
    }
}