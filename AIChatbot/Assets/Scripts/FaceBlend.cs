using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FaceBlend : MonoBehaviour
{
    enum MouthShapes {SIL, AE, RR, UH, EH, SS, OO, MM, CH, NN, TH, FF};
    public SentisTTS tts;
    public Animator animator;
    // TODO: Create a map of all phenomes to appropriate mouth shapes
    // Use SentisTTS phenome generator to get all the phenomes in the given text
    // assume each phenome takes an equal part of the audio to play and have it go along with the elevenlabs generated audio files

    readonly Dictionary<string, MouthShapes> phonemeToMouthShape = new Dictionary<string, MouthShapes>
    {
        // Silence
        ["<blank>"] = MouthShapes.SIL,
        ["<unk>"] = MouthShapes.SIL,
        ["<sos/eos>"] = MouthShapes.SIL,
        ["."] = MouthShapes.SIL,
        [".."] = MouthShapes.SIL,
        ["?"] = MouthShapes.SIL,
        ["!"] = MouthShapes.SIL,
        [","] = MouthShapes.SIL,
        ["\""] = MouthShapes.SIL,

        // Closed lips (M, B, P)
        ["M"] = MouthShapes.MM,
        ["B"] = MouthShapes.MM,
        ["P"] = MouthShapes.MM,

        // Top teeth on bottom lip (F, V)
        ["F"] = MouthShapes.FF,
        ["V"] = MouthShapes.FF,

        // Tongue between teeth (TH, DH)
        ["TH"] = MouthShapes.TH,
        ["DH"] = MouthShapes.TH,

        // Alveolar stop (T, D, N)
        ["T"] = MouthShapes.NN,
        ["D"] = MouthShapes.NN,
        ["N"] = MouthShapes.NN,

        // Back of tongue against soft palate (K, G, NG)
        ["K"] = MouthShapes.NN,
        ["G"] = MouthShapes.NN,
        ["NG"] = MouthShapes.NN,

        // Affricates (CH, JH)
        ["CH"] = MouthShapes.CH,
        ["JH"] = MouthShapes.CH,

        // Sibilants (S, Z, SH, ZH)
        ["S"] = MouthShapes.SS,
        ["Z"] = MouthShapes.SS,
        ["SH"] = MouthShapes.SS,
        ["ZH"] = MouthShapes.SS,

        // Approximants (L, R, Y, W)
        ["L"] = MouthShapes.TH,
        ["R"] = MouthShapes.RR,
        ["Y"] = MouthShapes.SS,
        ["W"] = MouthShapes.OO,

        // Vowels
        ["AA1"] = MouthShapes.AE,
        ["AA2"] = MouthShapes.AE,
        ["AA0"] = MouthShapes.AE,

        ["AE1"] = MouthShapes.AE,
        ["AE2"] = MouthShapes.AE,
        ["AE0"] = MouthShapes.AE,

        ["AH0"] = MouthShapes.AE,
        ["AH1"] = MouthShapes.AE,
        ["AH2"] = MouthShapes.AE,

        ["AO1"] = MouthShapes.OO,
        ["AO2"] = MouthShapes.OO,
        ["AO0"] = MouthShapes.OO,

        ["AW1"] = MouthShapes.UH,
        ["AW2"] = MouthShapes.UH,
        ["AW0"] = MouthShapes.UH,

        ["AY1"] = MouthShapes.AE,
        ["AY2"] = MouthShapes.AE,
        ["AY0"] = MouthShapes.AE,

        ["EH1"] = MouthShapes.EH,
        ["EH2"] = MouthShapes.EH,
        ["EH0"] = MouthShapes.EH,

        ["ER0"] = MouthShapes.RR,
        ["ER1"] = MouthShapes.RR,
        ["ER2"] = MouthShapes.RR,

        ["EY1"] = MouthShapes.AE,
        ["EY2"] = MouthShapes.AE,
        ["EY0"] = MouthShapes.AE,

        ["IH1"] = MouthShapes.EH,
        ["IH0"] = MouthShapes.EH,
        ["IH2"] = MouthShapes.EH,

        ["IY1"] = MouthShapes.EH,
        ["IY0"] = MouthShapes.EH,
        ["IY2"] = MouthShapes.EH,

        ["OW1"] = MouthShapes.OO,
        ["OW2"] = MouthShapes.OO,
        ["OW0"] = MouthShapes.OO,

        ["OY1"] = MouthShapes.OO,
        ["OY2"] = MouthShapes.OO,
        ["OY0"] = MouthShapes.OO,

        ["UH1"] = MouthShapes.UH,
        ["UH2"] = MouthShapes.UH,
        ["UH0"] = MouthShapes.UH,

        ["UW1"] = MouthShapes.UH,
        ["UW2"] = MouthShapes.UH,
        ["UW0"] = MouthShapes.UH,

        ["HH"] = MouthShapes.SIL
    };

    // Returns the string of the name of the bool variable for the given mouthshape
    string BoolForShape(MouthShapes shape)
    {
        switch (shape)
        {
            case MouthShapes.SIL:
                return "SIL";
            case MouthShapes.AE:
                return "AE";
            case MouthShapes.RR:
                return "RR";
            case MouthShapes.UH:
                return "UH";
            case MouthShapes.EH:
                return "EH";
            case MouthShapes.SS:
                return "SS";
            case MouthShapes.OO:
                return "OO";
            case MouthShapes.MM:
                return "MM";
            case MouthShapes.CH:
                return "CH";
            case MouthShapes.NN:
                return "NN";
            case MouthShapes.TH:
                return "TH";
            case MouthShapes.FF:
                return "FF";
            default:
                Debug.LogError("Invalid phenome given");
                return "";

        }
    }

    public void StartMouthAnimation(float seconds, string text)
    {
        string ptext = tts.TextToPhonemes(text);
        string[] phenomes = ptext.Split();
        StartCoroutine(_MouthMove(seconds, phenomes));
    }

    private IEnumerator _MouthMove(float seconds, string[] phenomes)
    {
        var initial_shape = phonemeToMouthShape[phenomes[0]];
        animator.SetBool(BoolForShape(initial_shape), true);

        float sec_per_shape = seconds / phenomes.Length;
        float time = 0.0f;
        int i = 0;
        while (i < phenomes.Length)
        {
            // Phenome to animation
            time += Time.deltaTime;
            if (time > sec_per_shape)
            {
                // Disable current mouth shape
                var shape = phonemeToMouthShape[phenomes[i]];
                animator.SetBool(BoolForShape(shape), false);

                time = 0.0f;
                i++;
                animator.SetTrigger("Default");
                if (i < phenomes.Length)
                {
                    // Set next mouth shape
                    shape = phonemeToMouthShape[phenomes[i]];
                    animator.SetBool(BoolForShape(shape), true);
                }
            }
            yield return null;
        }
        Debug.Log("DONE!");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string message = "Hello World";
            float length = tts.TextToSpeech(message);
            StartMouthAnimation(length, message);
        }
    }
}
