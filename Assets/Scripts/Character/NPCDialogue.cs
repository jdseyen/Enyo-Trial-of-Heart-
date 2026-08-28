using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    [Header("NPC Info")]
    public string npcName;
    public Sprite npcPortrait;

    [Header("Dialogue")]
    [TextArea(2, 5)]
    public string[] dialogueLines;

    [Header("Auto Progress Lines")]
    public bool[] autoProgressLines;

    [Header("Dialogue Settings")]
    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.04f;

    [Header("Voice")]
    public AudioClip voiceSound;

    [Range(0f, 1f)]
    public float voiceVolume = 0.15f;

    [Range(0.5f, 2f)]
    public float voicePitch = 1f;
}