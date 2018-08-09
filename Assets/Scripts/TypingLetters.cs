using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypingLetters : MonoBehaviour {

    public float delay = 0.1f;
    public string fullText;
    private Text displayTextBox;
    private string currentText = "";
    [HideInInspector] public bool finished = false;

	// Use this for initialization
	void Start ()
    {
        fullText = GetComponent<Text>().text;
        displayTextBox = GetComponent<Text>();
        displayTextBox.text = "";
        StartCoroutine(ShowText());
	}
	
	IEnumerator ShowText ()
    {
        yield return new WaitForSeconds(1.5f);
        for(int i = 0; i < fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            displayTextBox.text = currentText;
            yield return new WaitForSeconds(delay);
        }
        finished = true;
    }
}
