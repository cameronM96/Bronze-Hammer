﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypingLetters : MonoBehaviour {

    public float delay = 0.1f;
    public string fullText;
    private Text displayTextBox;
    private string currentText = "";

	// Use this for initialization
	void Start ()
    {
        fullText = transform.GetChild(0).GetComponent<Text>().text;
        displayTextBox = GetComponent<Text>();
        StartCoroutine(ShowText());
	}
	
	IEnumerator ShowText ()
    {
        for(int i = 0; i < fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            displayTextBox.text = currentText;
            yield return new WaitForSeconds(delay);
        }
    }
}
