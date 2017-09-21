using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;

public class DialogueManager : MonoBehaviour 
{
    private Audio currentAudio;

    private string[] filelines;
    private string currentSubtitle;

    private int nextSubtitle = 0;
    private float rate = 44100;

    private GUIStyle subtitleStyle;
    private List<float> timestamps;
    private List<string>subtitleText;

    private AudioManager audioManager;

    private void Start()
    {
        subtitleStyle = new GUIStyle();
        timestamps = new List<float>();
        subtitleText = new List<string>();

        audioManager = this.GetComponent<AudioManager>();
        audioManager.PlayAudio += play;

        setSubtitleStyle();
    }

    private void setSubtitleStyle()
    {
        subtitleStyle.wordWrap = true;
        subtitleStyle.alignment = TextAnchor.MiddleCenter;
        subtitleStyle.normal.textColor = Color.white;
        subtitleStyle.fontSize = Mathf.FloorToInt(Screen.height * 0.05f);
    }

    private void play(Audio audio)
    {
        currentAudio = audio;
        setupSubtitles(audio.Subtitles);
    }

    private void setupSubtitles(TextAsset dialogueText)
    {
        timestamps.Clear();
        subtitleText.Clear();
        nextSubtitle = 0;


        if (dialogueText == null) return;
        filelines = dialogueText.text.Split('\n');
        for (var i = 0; i < filelines.Length; i++)
        {
            if (filelines[i] == string.Empty)
                continue;
            
            string[] splitTemp = filelines[i].Split('|');
            timestamps.Add(float.Parse(splitTemp[0]));
            subtitleText.Add(splitTemp[1]);
        }

        if (subtitleText[0] != null)
            currentSubtitle = subtitleText[0];
    }

    private void OnGUI()
    {
        if (currentAudio == null) 
            return;
        
        if (nextSubtitle <= 0 || subtitleText[nextSubtitle - 1].Contains("<break>"))
        {
            GUI.Label(new Rect(0, 0, 0, 0), "");
            waitForNexLine();
            return;
        }

        GUI.depth = -1001;
        subtitleStyle.fixedWidth = Screen.width / 1.5f;
        Vector2 size = subtitleStyle.CalcSize(new GUIContent())*1.2f;
        GUI.contentColor = Color.black;
        GUI.Label(new Rect(Screen.width / 2 - size.x / 2 + 1, Screen.height / 1.25f - size.y + 1, size.x, size.y), currentSubtitle);
        GUI.contentColor = Color.white;
        GUI.Label(new Rect(Screen.width / 2 - size.x / 2, Screen.height / 1.25f - size.y, size.x, size.y), currentSubtitle);
        waitForNexLine();
    }

    private void waitForNexLine()
    {
        if (nextSubtitle >= subtitleText.Count) 
            return;
        if (currentAudio.Source.timeSamples / rate <= timestamps[nextSubtitle]) 
            return;
        currentSubtitle = subtitleText[nextSubtitle];
        nextSubtitle++;
    }
}
