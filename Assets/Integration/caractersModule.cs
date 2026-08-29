using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class caractersModule : MonoBehaviour {

    // === Public Vars ===
    public KMBombModule BombModule;
    public KMSelectable[] Buttons;
    public MeshRenderer[] ButtonsLeds;
    public TextMesh[] ButtonsTexts;

    // === Private Vars ===
    private int[] correctButtonIndex;
    private int correctColumnIndex;
    private int[] correctSequence;

    private int nbrCaracterCorrect = 0;

    private string[][] columns = new string[][] {
        new string[] { "_",  "{",  "£",  "§",  "!",  "}",  ">",  "(",  "."  },
        new string[] { "/*", "]",  "||", "[",  "{",  "?.", ")",  "_",  "$"  },
        new string[] { "{",  "~",  "&&", "!",  "++", "*",  "\\", "!=", "||" },
        new string[] { "#",  "/",  "||", ".",  "£",  "}",  ")",  "==", "/*" },
        new string[] { ")",  "]",  "==", "*/",  "<", "||", "^",  ":",  "\\" },
        new string[] { "[",  "--", "_",  "==", "]",  "£",  "|",  "&&", ","  },
        new string[] { ">",  "~",  "}",  "[",  "!",  "==", "|",  "?.", "||" },
        new string[] { ">",  "<",  "&",  "£",  "}",  ":",  ",",  "]",  "?." },
        new string[] { "§",  "/",  "--", "==", "£",  "::", ",",  "<",  "->" },
        new string[] { "!",  ",",  "(",  "*/", "[",  "!=", "/",  "~",  "\\" },
        new string[] { "}",  "&",  ",",  "/*", "->", "~",  "++", "::", "!"  },

        // new string[] { "_", "/*", "{", "#", ")", "[", "&gt;", "&gt;", "§", "!", "}" },
        // new string[] { "{", "]", "~", "/", "]", "--", "~", "&lt;", "/", ",", "&" },
        // new string[] { "£", "||", "&&", "||", "==", "_", "}", "&", "--", "(", "," },
        // new string[] { "§", "[", "!", ".", "*/", "==", "[", "£", "==", "*/", "/*" },
        // new string[] { "!", "{", "++", "£", "&lt;", "]", "!", "}", "£", "[", "->" },
        // new string[] { "}", "?.", "*", "}", "||", "£", "==", ";", "::", "!=", "~" },
        // new string[] { "&gt;", ")", "\\", ")", "^", "|", "|", ",", ",", "/", "++" }, 
        // new string[] { "(", "_", "!=", "==", ";", "&&", "?.", "]", "&lt;", "~", "::" },
        // new string[] { ".", "$", "||", "/*", "\\", ",", "||", "?.", "->", "\\", "!" }, 
    };

    void Start () {
        BombModule.OnActivate += OnActivate;
        ButtonsInitialisation();
    }

    void ButtonsInitialisation()
    {
        for (int i = 0; i < Buttons.Length; i++)
        {
            int buttonIndex = i;

            Buttons[buttonIndex].OnInteract += delegate ()
            {
                OnButtonPress(buttonIndex);
                return false;
            };
        }
    }
    
    void OnActivate()
    {
        nbrCaracterCorrect = 0;
        correctColumnIndex = SelecteRandomColumn();
        GenerateRanomSequence(correctColumnIndex);
        UpdateButtons();
    }

    // Return a random column index
    int SelecteRandomColumn()
    {
        return Random.Range(0, columns.Length - 1);
    }

    private static readonly Random _random = new Random();

    public static int [] GetRandomIndices(int n, int l)
    {
        // Validation des arguments
        if (l > n)
            Debug.LogError("ArgumentException : l ne peut pas être supérieur à n");
        if (n <= 0 || l < 0)
            Debug.LogError("ArgumentOutOfRangeException : Les valeurs doivent être positives.");

        // 1. Initialiser une liste avec tous les index de 0 à n-1
        List<int> pool = new List<int>(n);
        for (int i = 0; i < n; i++)
        {
            pool.Add(i);
        }

        //List<int> result = new List<int>(l);
        int[] result = new int[l];

        // 2. Fisher-Yates partiel : on pioche l' éléments au hasard
        for (int i = 0; i < l; i++)
        {
            // Choisir un index entre i et n-1
            int randomIndex = Random.Range(i, n); // _random.Next(i, n);

            // Échanger l'élément choisi avec l'élément à la position actuelle 'i'
            int temp = pool[i];
            pool[i] = pool[randomIndex];
            pool[randomIndex] = temp;

            // Ajouter l'élément mélangé au résultat
            result[i] = pool[i];
            //result.Add(pool[i]);
        }

        return result;
    }

    void GenerateRanomSequence (int columnIndex)
    {
        correctButtonIndex = GetRandomIndices(columns[columnIndex].Length, Buttons.Length);
        correctSequence = new int[Buttons.Length];
        System.Array.Copy(correctButtonIndex, correctSequence, correctButtonIndex.Length);
        System.Array.Sort(correctSequence);
    }

    void UpdateButtons ()
    {
        for (int i = 0; i < Buttons.Length; i++)
        {
            // Update Led
            MeshRenderer buttonLed = ButtonsLeds[i].GetComponent<MeshRenderer>();
            if (nbrCaracterCorrect >= Buttons.Length || correctButtonIndex[i] < correctSequence[nbrCaracterCorrect])
            {
                buttonLed.material.color = new Color(0.1f, 1f, 0.1f);
            } else
            {
                buttonLed.material.color = new Color(0.1f, 0.1f, 0.1f);
            }

            // Update Text
            TextMesh buttonText = ButtonsTexts[i].GetComponent<TextMesh>();
            buttonText.text = columns[correctColumnIndex][correctButtonIndex[i]];
        }
    }

    void OnButtonPress(int index)
    {
        Debug.Log("Button Press :" + index.ToString());
        Debug.Log(" -> :" + columns[correctColumnIndex][correctButtonIndex[index]]);

        if (nbrCaracterCorrect >= Buttons.Length) {
            return;
        }

        MeshRenderer buttonLed = ButtonsLeds[index].GetComponent<MeshRenderer>();

        Debug.Log("A :" + correctSequence[nbrCaracterCorrect].ToString());
        Debug.Log("B :" + correctButtonIndex[index].ToString());

        if (correctSequence[nbrCaracterCorrect] == correctButtonIndex[index])
        {
            // Correct
            nbrCaracterCorrect++;
            UpdateButtons();
            // buttonLed.material.color = new Color(0.1f, 1f, 0.1f);

            if (nbrCaracterCorrect == Buttons.Length)
            {
                BombModule.HandlePass();
            }
        } else
        {
            // Incorrect
            buttonLed.material.color = new Color(1f, 0.1f, 0.1f);
            BombModule.HandleStrike();
        }
    }

}
