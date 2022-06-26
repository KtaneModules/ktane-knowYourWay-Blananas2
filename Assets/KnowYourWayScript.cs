using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class KnowYourWayScript : MonoBehaviour {

    public KMBombModule Module;
    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable[] Buttons;
    public TextMesh[] Labels;
    public GameObject[] Bars;
    public Material[] LEDs;
    public TextMesh Center;

    string Directions = "ULDR";
    private List<string> DirectionNames = new List<string> { "Up", "Left", "Down", "Right" };
    string Arrows = "↑←↓→";
    string Input = "";
    string Answer = "";
    int LEDLoc = -1;        int LEDInd = -1;        int LEDOri = -1;
    int ArrowLoc = -1;      int ArrowInd = -1;      int ArrowOri = -1;
    int UpperLoc = -1;      int UpperInd = -1;      int UpperOri = -1;
    int UButtonLoc = -1;    int UButtonInd = -1;    int UButtonOri = -1;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;
    private bool tpStrikeCheck = false;

    void Awake () {
        moduleId = moduleIdCounter++;

        Module.OnActivate += delegate () { ModuleStart(); };

        foreach (KMSelectable button in Buttons) {
            button.OnInteract += delegate () { ButtonPress(button); return false; };
        }

    }

    // Use this for initialization
    void ModuleStart () {
        Center.transform.localPosition = new Vector3(0.0001f, 0.0158f, 0.0052f);

        //Locations
        Debug.LogFormat("[Know Your Way #{0}] LOCATIONS:", moduleId);

        LEDLoc = UnityEngine.Random.Range(0,4);
        Bars[LEDLoc].GetComponent<MeshRenderer>().material = LEDs[1];
        Debug.LogFormat("[Know Your Way #{0}] The LED is located at {1}.", moduleId, DirectionNames[LEDLoc]);

        ArrowLoc = UnityEngine.Random.Range(0,4);
        Center.text = Arrows[ArrowLoc].ToString();
        Debug.LogFormat("[Know Your Way #{0}] The arrow is pointing at {1}.", moduleId, DirectionNames[ArrowLoc]);

        UpperLoc = UnityEngine.Random.Range(0,4);
        for (int l = 0; l < 4; l++) {
            Labels[l].text = Directions[(UpperLoc + l) % 4].ToString();
            if (Directions[(UpperLoc + l) % 4].ToString() == "U") {
                UButtonLoc = l;
            }
        }
        Debug.LogFormat("[Know Your Way #{0}] The upper button is labeled '{1}'.", moduleId, Directions[UpperLoc]);
        Debug.LogFormat("[Know Your Way #{0}] The 'U' button is located at {1}.", moduleId, DirectionNames[UButtonLoc]);

        //Indications
        Debug.LogFormat("[Know Your Way #{0}] INDICATIONS:", moduleId);

        if (UButtonLoc == 1) { LEDInd = 2; }
        else if (ArrowLoc == 3) { LEDInd = 0; }
        else if (LEDLoc != ArrowLoc) { LEDInd = 1; }
        else { LEDInd = 3; }
        Debug.LogFormat("[Know Your Way #{0}] The LED indicates where {1} is.", moduleId, DirectionNames[LEDInd]);

        if (ArrowLoc == (LEDLoc + 2) % 4) { ArrowInd = 2; }
        else if (LEDLoc == (UButtonLoc + 3) % 4) { ArrowInd = 0; }
        else if (LEDLoc != 3) { ArrowInd = 1; }
        else { ArrowInd = 3; }
        Debug.LogFormat("[Know Your Way #{0}] The arrow indicates where {1} is.", moduleId, DirectionNames[ArrowInd]);

        if (LEDLoc == 2) { UpperInd = 2; }
        else if (ArrowLoc != (UButtonLoc + 1) % 4 && ArrowLoc != (UButtonLoc + 3) % 4) { UpperInd = 0; }
        else if (UButtonLoc != 0) { UpperInd = 1; }
        else { UpperInd = 3; }
        Debug.LogFormat("[Know Your Way #{0}] The upper button indicates where {1} is.", moduleId, DirectionNames[UpperInd]);

        if (ArrowLoc == UButtonLoc) { UButtonInd = 2; }
        else if (LEDLoc != (UButtonLoc + 2) % 4 && LEDLoc != UButtonLoc) { UButtonInd = 0; }
        else if (ArrowLoc != 2) { UButtonInd = 1; }
        else { UButtonInd = 3; }
        Debug.LogFormat("[Know Your Way #{0}] The 'U' button indicates where {1} is.", moduleId, DirectionNames[UButtonInd]);

        //Orientations
        Debug.LogFormat("[Know Your Way #{0}] ORIENTATIONS:", moduleId);

        if (ArrowInd == LEDInd) { LEDOri = 0; }
        else if (UpperInd == LEDInd) { LEDOri = 3; }
        else if (UButtonInd == LEDInd) { LEDOri = 2; }
        else { LEDOri = 1; }
        Debug.LogFormat("[Know Your Way #{0}] The orientation of the LED is {1}.", moduleId, DirectionNames[LEDOri]);

        if (UpperInd == ArrowInd) { ArrowOri = 3; }
        else if (UButtonInd == ArrowInd) { ArrowOri = 2; }
        else if (LEDInd == ArrowInd) { ArrowOri = 1; }
        else { ArrowOri = 0; }
        Debug.LogFormat("[Know Your Way #{0}] The orientation of the arrow is {1}.", moduleId, DirectionNames[ArrowOri]);

        if (UButtonInd == UpperInd) { UpperOri = 2; }
        else if (LEDInd == UpperInd) { UpperOri = 1; }
        else if (ArrowInd == UpperInd) { UpperOri = 0; }
        else { UpperOri = 3; }
        Debug.LogFormat("[Know Your Way #{0}] The orientation of the upper button is {1}.", moduleId, DirectionNames[UpperOri]);

        if (LEDInd == UButtonInd) { UButtonOri = 1; }
        else if (ArrowInd == UButtonInd) { UButtonOri = 0; }
        else if (UpperInd == UButtonInd) { UButtonOri = 3; }
        else { UButtonOri = 2; }
        Debug.LogFormat("[Know Your Way #{0}] The orientation of the 'U' button is {1}.", moduleId, DirectionNames[UButtonOri]);

        GenerateAnswer(LEDLoc, LEDInd, LEDOri);
        GenerateAnswer(ArrowLoc, ArrowInd, ArrowOri);
        GenerateAnswer(UpperLoc, UpperInd, UpperOri + (4 - UpperLoc));
        GenerateAnswer(UButtonLoc, UButtonInd, UButtonOri);
        Debug.LogFormat("[Know Your Way #{0}] The final answer is the buttons labeled {1}.", moduleId, Answer);
	}

    void GenerateAnswer (int a, int b, int c) {
        switch (((a - b + c - UButtonLoc) + 8) % 4) {
            case 0: Answer += "U"; break;
            case 1: Answer += "L"; break;
            case 2: Answer += "D"; break;
            case 3: Answer += "R"; break;
            default: Debug.LogFormat("[Know Your Way #{0}] There was a problem generating the part of the answer after \"{1}\" (Number ended up being {2}), please let Blan know immediately.", moduleId, Answer, ((a - b + c - UButtonLoc) + 8) % 4); break;
        }
    }

    void ButtonPress(KMSelectable button)
    {
        if (moduleSolved)
            return;
        button.AddInteractionPunch();
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

        for (int i = 0; i < 4; i++)
        {
            if (button == Buttons[i])
            {
                Input += Directions[(UpperLoc + i) % 4].ToString();
            }
        }

        for (int i = 0; i < Input.Length; i++)
        {
            if (Input[i] != Answer[i])
            {
                Debug.LogFormat("[Know Your Way #{0}] Incorrect answer (Buttons labeled {1}) submitted, module striked.", moduleId, Input);
                Module.HandleStrike();
                tpStrikeCheck = !tpStrikeCheck;
                Input = "";
                return;
            }
        }

        if (Input.Length == 4)
        {
            Debug.LogFormat("[Know Your Way #{0}] Correct answer (Buttons labeled {1}) submitted, module solved.", moduleId, Input);
            Module.HandlePass();
            moduleSolved = true;
            Center.transform.localPosition = new Vector3(0.0020f, 0.0158f, 0f);
            Center.text = "✓";
        }

        /*
        if (Input.Length == 4) {
            if (Input == Answer) {
                Module.HandlePass();
                moduleSolved = true;
                Debug.LogFormat("[Know Your Way #{0}] Correct answer (Buttons labeled {1}) submitted, module solved.", moduleId, Input);
                Center.transform.localPosition = new Vector3(0.0020f, 0.0158f, 0f);
                Center.text = "✓";
            } else {
                Module.HandleStrike();
                Debug.LogFormat("[Know Your Way #{0}] Incorrect answer (Buttons labeled {1}) submitted, module striked.", moduleId, Input);
                Input = "";
            }
        }
        */
    }

    //twitch plays
#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Press the buttons labeled UDLR with !{0} press UDLR.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.Trim().ToLowerInvariant();
        if (!command.StartsWith("press ")) yield break;

        string iterator = command.Substring(6);
        IEnumerable<char> invalid = iterator.Where(x => !x.EqualsAny('u', 'd', 'l', 'r'));
        if(invalid.Any()) yield break;
        bool startingStrike = tpStrikeCheck;

        foreach (char character in iterator)
        {
            yield return null;
            for (int i = 0; i < 4; i++)
            {
                if (Labels[i].text.ToLower()[0] == character)
                {
                    Buttons[i].OnInteract();
                    if (startingStrike != tpStrikeCheck)
                        yield break;
                    yield return new WaitForSeconds(0.1f);
                    break;
                }
            }
        }
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        for (int i = Input.Length; i < Answer.Length; i++)
        {
            Buttons[(Directions.IndexOf(Answer[i]) + UButtonLoc) % 4].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        yield break;
    }
}
