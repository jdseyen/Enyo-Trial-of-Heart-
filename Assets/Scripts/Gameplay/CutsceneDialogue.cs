using UnityEngine;

public class CutsceneDialogue : MonoBehaviour
{
    public AudioClip[] voiceClips;
    public AudioSource narrationAudio;

    int currentLine = 0;

    void Start()
    {
        PlayCurrentVoice();
    }

    public void NextVoice()
    {
        currentLine++;

        if (currentLine < voiceClips.Length)
        {
            PlayCurrentVoice();
        }
    }

    void PlayCurrentVoice()
    {
        narrationAudio.Stop();
        narrationAudio.clip = voiceClips[currentLine];
        narrationAudio.Play();
    }
}