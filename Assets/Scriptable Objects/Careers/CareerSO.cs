using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "CareerSO", menuName = "ScriptableObjects/CareerSO", order = 0)]
public class CareerSO : ScriptableObject
{
    public Sprite BackgroundPortrait;
    public Sprite BackgroundLandscape;

    public Sprite TestingTopSprite;
    public Sprite TestingMiddleSprite;
    public Sprite TestingBottomSprite;

    public Sprite PlayerCharacter;
    public Sprite NPC;
    public Sprite PlayerCharacterHappy;
    public Sprite NPCHappy;
    public Sprite PlayerCharacterSad;
    public Sprite NPCSad;

    public string NPCName;
    public string MusicTrackName;

    public Color BackgroundColor;
    public TMP_ColorGradient TextColor;

    public string[] TermList;

    public Minigame1Question[] Minigame1Questions;
    public Minigame2Question[] Minigame2Questions;
    public Minigame3Question[] Minigame3Questions;

    public string Minigame3Scene;

    public string ExplorerWorld;

    public BackgroundItem Shopitem1;

    public string[] Minigame1Script;
    public string[] Minigame2Script;
    public string[] Minigame3Script;

    public DialogueLine[] Minigame1Dialogue;
    public DialogueLine[] Minigame2Dialogue;
    public DialogueLine[] Minigame3Dialogue;

}
