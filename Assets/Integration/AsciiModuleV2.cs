
// Concept
// Le module affiche un mot(en majuscules).
// Les experts disposent d’un tableau de règles(prioritaires) pour convertir chaque lettre en score.
// Le désamorceur doit entrer la somme totale des scores des lettres.
// Répéter l’opération 3 mots pour valider le module.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AsciiModuleV2 : MonoBehaviour
{

    public KMBombModule BombModule;
    public TextMesh DisplayText;
    public TextMesh[] DisplayOutput;
    public KMSelectable[] Buttons;
    public KMSelectable SubmitButton;

    private string[] AvailableWords = new string[] {
        "python",
        "script",
        "java",
        "ordi",
        "algo",
        "cia",
        "cours",
        "club"
    }; // Liste des mots possibles

    private Dictionary<char, int> ScoresLetters = new Dictionary<char, int> {
        { 'a', 15 },
        { 'b', 7   },
        { 'c', 5   },
        { 'd', 7   },
        { 'e', 15  },
        { 'f', 20  },
        { 'g', 103 },
        { 'h', 5   },
        { 'i', 11  },
        { 'j', 106 },
        { 'k', 107 },
        { 'l', 5   },
        { 'm', -10 },
        { 'n', -10 },
        { 'o', 11  },
        { 'p', 5   },
        { 'q', 20  },
        { 'r', 20  },
        { 's', 115 },
        { 't', 5   },
        { 'u', 11  },
        { 'v', 118 },
        { 'w', 119 },
        { 'x', -10 },
        { 'y', 11  },
        { 'z', 122 }
    }; // Liste des couleurs possibles

    private string CurrentWord;
    private int CurrentOutput = 0;
    private bool Solved = false;
    private int CurrentLetter = 0;
    private bool WrongAnimation = false;

    // Initialization of the module
    void Start()
    {
        BombModule.OnActivate += OnActivate;
    }
    
    IEnumerator StartWrongAnimation()
    {
        // Don't start another animation, if another one is running
        if (WrongAnimation)
        {
            yield break;
        }
        WrongAnimation = true;

        // Update Display
        DisplayWord("Wrong !");
        SetDisplayColor(new Color(1f, 0f, 0f));

        // suspend execution for 1 seconds
        yield return new WaitForSeconds(1);

        // Update State
        WrongAnimation = false;

        // Update Display
        UpdateDisplay();
        SetDisplayColor(new Color(1f, 1f, 1f));
    }

    void WrontAnswer()
    {
        // Log
        Debug.Log("Wrong !");
        // KTANE Update
        BombModule.HandleStrike();
        // Reset the module
        CurrentLetter = 0;
        // Start Animation
        StartCoroutine("StartWrongAnimation");
    }

    // Function call when you press the button "Submit". Test the output.
    void OnSubmit(string word, int output)
    {
        // Test if the module is already solved
        if (Solved || WrongAnimation)
        {
            return;
        }
        
        if (CurrentLetter >= 0 && CurrentLetter < word.Length)
        {
            char letter = word[CurrentLetter];
            if ((ScoresLetters[letter] + 200) % 100 == (output + 200) % 100)
            {
                CurrentLetter++;
            }
            else
            {
                WrontAnswer();
            }
        }
        else
        {
            if (CalcScoreWord(word) % 100 == output % 100)
            {
                BombModule.HandlePass();
                Solved = true;
            }
            else
            {
                WrontAnswer();
            }
        }

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (WrongAnimation)
        {
            Debug.LogWarning("Shouldn't happen");
            return;
        }

        // Update Display
        if (Solved)
        {
            DisplayWord("Valid");
            SetDisplayColor(new Color(0f, 1f, 0f));
        }
        else if (CurrentLetter >= 0 && CurrentLetter < CurrentWord.Length)
        {
            DisplayWord(CurrentWord[CurrentLetter].ToString());
            SetDisplayColor(new Color(1f, 1f, 1f));
        }
        else
        {
            DisplayWord("Score ?");
            SetDisplayColor(new Color(1f, 1f, 0f));
        }
    }

    // This function return the score of the word gived
    int CalcScoreWord(string word)
    {
        int score = 0;

        string explication = "";

        //===// Add the score of each letter //===//

        foreach (char letter in word)
        {
            if (ScoresLetters.ContainsKey(letter))
            {
                score += ScoresLetters[letter];
                explication += "+ " + ScoresLetters[letter].ToString() + " (" + letter.ToString() + ") ";
            }
        }

        //===// Add Bonus/Malus //===//

        // Rule 1 : Si le mot contient au moins 3 voyelles
        int nbrVoyelle = 0;
        foreach (char letter in "aeiouy")
        {
            nbrVoyelle += word.Split(letter).Length - 1;
        }
        if (nbrVoyelle >= 3)
        {
            score += 8;
            explication += "+ 8 (voyelles) ";
        }

        // Rule 2 : Si le mot contient au moins 2 fois la même lettre
        string str = word;
        while (str.Length > 0)
        {
            int count = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[0] == str[i])
                {
                    count++;
                }
            }

            if (count >= 2)
            {
                score -= 5;
                explication += "- 5 (same letter) ";
                break;
            }

            str = str.Replace(str[0].ToString(), string.Empty);
        }

        // Rule 3 : Si la première et la dernière lettre sont identiques
        if (word[0] == word[word.Length - 1])
        {
            score += 12;
            explication += "+ 12 (first and last) ";
        }

        // Rule 4 : Si le mot a une longueur paire
        if (word.Length % 2 == 0)
        {
            score += 4;
            explication += "+ 4 (paire) ";
        }
        else
        {
            score -= 3;
            explication += "- 3 (impaire) ";
        }

        print(explication + "= " + score.ToString());
        return score;
    }

    // Call be KTANE to initialize the module.
    void OnActivate()
    {
        // Choose a random word
        CurrentWord = GenerateRandomWord();

        // Init the Submit button
        SubmitButton.OnInteract += delegate ()
        {
            OnSubmit(CurrentWord, CurrentOutput);
            return false;
        };

        // Init all digit button (+/-) use to enter the output
        for (int i = 0; i < Buttons.Length; i++)
        {
            int buttonIndex = i;

            Buttons[buttonIndex].OnInteract += delegate ()
            {
                int value = 1;
                if (buttonIndex % 2 == 1)
                {
                    value = -1;
                }
                value *= Mathf.FloorToInt(Mathf.Pow(10, buttonIndex / 2));

                OnButtonPress(Mathf.FloorToInt(buttonIndex / 2), value);
                return false;
            };
        }

        // Initialisation des Text Mesh (+/-) de l'Output
        for (int i = 0; i < DisplayOutput.Length; i++)
        {
            DisplayOutput[i].text = "0";
        }

        // Update the Display Screen
        UpdateDisplay();
    }

    // Update the Word Display to the current word
    void DisplayWord(string word)
    {
        DisplayText.text = word;
    }

    void SetDisplayColor(Color color)
    {
        DisplayText.color = color;
    }

    // Update the digit displayed on the module
    void UpdateDigit(int value)
    {
        string valueInString = value.ToString();

        for (int i = 0; i < DisplayOutput.Length; i++)
        {
            if (i <= valueInString.Length - 1)
            {
                DisplayOutput[i].text = valueInString[valueInString.Length - i - 1].ToString();
            }
            else
            {
                DisplayOutput[i].text = "0";
            }
        }
    }

    // Return a random word between available words
    string GenerateRandomWord()
    {
        return AvailableWords[Random.Range(0, AvailableWords.Length)];
    }

    // Function call by digit button, to change the output
    void OnButtonPress(int index, int value)
    {
        if (Solved || WrongAnimation)
        {
            return;
        }

        // The output value must be between 0 and 99
        CurrentOutput = (CurrentOutput + value + 200) % 100;

        // Update Digit Screen to the new value
        UpdateDigit(CurrentOutput);
    }
}
