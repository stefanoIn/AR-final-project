using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterText : MonoBehaviour
{
    [Tooltip("Seconds between characters (e.g. 0.02 fast, 0.05 normal, 0.1 slow)")]
    public float typingSpeed = 0.03f;

    [Tooltip("Start typing automatically when enabled")]
    public bool playOnEnable = true;

    TMP_Text tmp;
    string fullText;
    Coroutine routine;

    void Awake()
    {
        tmp = GetComponent<TMP_Text>();
        fullText = tmp.text;
    }

    void OnEnable()
    {
        if (playOnEnable)
            Play();
    }

    // Call this to restart the effect any time.
    public void Play()
    {
        if (tmp == null) tmp = GetComponent<TMP_Text>();
        if (string.IsNullOrEmpty(fullText)) fullText = tmp.text;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Type());
    }

    IEnumerator Type()
    {
        tmp.text = "";

        foreach (char c in fullText)
        {
            tmp.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    // Optional: if you change the text in Inspector and want to re-cache it.
    public void RefreshTextFromCurrent()
    {
        fullText = tmp.text;
    }
}
