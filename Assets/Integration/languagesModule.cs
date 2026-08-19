using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class languagesModule : MonoBehaviour {

    // === Public Vars ===
    public KMBombModule BombModule;
    public TextMesh DisplayText;
    public KMSelectable[] Buttons;
    public MeshRenderer[] ButtonsLogos;
    public MeshRenderer[] Leds;

    public string[] Languages;
    
    public Texture DefaultanguageLogo;

    private Dictionary<string, string[]> LanguagesWords = new Dictionary<string, string[]> {
        { "JavaScript", new[] { "function", "console.log", "npm", "const", "callback", "async" } },
        { "PHP", new[] { "echo", "foreach", "array", "laravel", "mysqli", "$this" } },
        { "HTML", new[] { "<div>", "<html>", "href", "<body>", "src", "id" } },
        { "CSS", new[] { "margin", "color", "display", "@media", "padding", "flexbox" } },
        { "Python", new[] { "def", "import", "print", "indentation", "elif", "pip" } },
        { "SQL", new[] { "SELECT", "WHERE", "FROM", "JOIN", "INSERT", "DATABASE" } },
        { "Java", new[] { "public static\n void main", "System.out\n.println", "class", "package", "extends", "JVM" } },
        { "Kotlin", new[] { "val", "var", "fun", "android", "null-safety", "data class" } },
        { "Cpp", new[] { "std::cout", "#include", "pointer", "cin", "virtual", "destructor" } },
        { "Bash", new[] { "echo", "sudo", "chmod", "grep", "$1", "#!/bin/bash" } },
        { "Rust", new[] { "fn", "let mut", "cargo", "borrowing", "match", "unwrap" } }
    };

    private Dictionary<string, Texture> LanguagesLogos = new Dictionary<string, Texture>();

    public Texture2D[] TempLanguagesLogosList; 

    // === Privates Vars ===
    private string currentLanguage;
    private int nbrCorrectReponce = 0;
    private int correctButtonIndex;

    // === Constantes ===
    private const float TIME_PER_WORD = 3f;
    private const int NBR_CORRECT_REPONCE_NEED = 3;
    private Color LedColorGreen = new Color(0, 1f, 0);
    private Color LedColorRed = new Color(1f, 0, 0);

    // Use this for initialization
    void Start () {
        BombModule.OnActivate += OnActivate;
        
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

    // Activation of the module (when the timer start)
    void OnActivate()
    {
        LoadAllImages();

        ChooseRandomReponce();
        StartDisplayWords();
        GenerateButtons();
    }

    // Load all languages' logo
    void LoadAllImages()
    {
        foreach (string iLanguage in Languages)
        {
            Texture2D tex = Resources.Load<Texture2D>("Integration/Assest/Images/" + iLanguage);

            if (tex)
            {
                LanguagesLogos[iLanguage] = tex;
            } else
            {
                Debug.LogWarning("Warn: no language logo found in 'Integration/Assest/Images/' : " + iLanguage);

                // Temp Code
                int id = System.Array.IndexOf(Languages, iLanguage);

                if (TempLanguagesLogosList != null && TempLanguagesLogosList.Length > id)
                {
                    LanguagesLogos[iLanguage] = TempLanguagesLogosList[id];
                } else
                {
                    LanguagesLogos[iLanguage] = DefaultanguageLogo;
                }

            }
        }
    }

    // Select a random langauge how players must find
    void ChooseRandomReponce()
    {
        string oldReponse = currentLanguage;
        do
        {
            currentLanguage = Languages[Random.Range(0, Languages.Length - 1)];
        } while (oldReponse == currentLanguage);
    }

    void StartDisplayWords()
    {
        // On lance la fonction en arrière-plan
        StartCoroutine(UpdateTextEverySecond());
    }

    // La Coroutine qui tourne en boucle sans bloquer le jeu
    IEnumerator UpdateTextEverySecond()
    {
        // Vérification de sécurité pour éviter les erreurs dans la console
        if (DisplayText == null || currentLanguage == null) yield break;

        string lastWordDisplay = "";

        while (nbrCorrectReponce < NBR_CORRECT_REPONCE_NEED)
        {
            // 1. Récupérer le tableau de mots associé
            string[] words = LanguagesWords[currentLanguage];

            // 2. Choisir un mot au hasard dans ce tableau
            string randomWord;
            do
            {
                randomWord = words[Random.Range(0, words.Length)];
            } while (lastWordDisplay == randomWord);

            lastWordDisplay = randomWord;

            // 3. Mettre à jour l'affichage
            DisplayText.text = randomWord;
            DisplayText.color = new Color(1f, 1f, 1f);

            // 4. PAUSE de 1 seconde. C'est ce "yield" qui empêche le jeu de bloquer !
            yield return new WaitForSeconds(TIME_PER_WORD);
        }
    }

    // Update the nbr of lights / leds green or red
    void UpdateLight(int nbrLightOn)
    {
        for (int i = 0; i < Leds.Length; i++)
        {   
            MeshRenderer buttonRenderer = Leds[i].GetComponent<MeshRenderer>();
            if (buttonRenderer)
            {
                if (i < nbrLightOn)
                {
                    buttonRenderer.material.color = LedColorGreen;
                } else
                {
                    buttonRenderer.material.color = LedColorRed;
                }
            }
        }
    }

    // Update the reponces button to correct Language Logo
    void GenerateButtons()
    {
        // Choose a random button to beacome the reponce
        correctButtonIndex = Random.Range(0, Buttons.Length);

        string[] alreadyUsedColors = new string[Buttons.Length];
        
        // Update each button
        for (int i = 0; i < Buttons.Length; i++)
        {
            var rend = ButtonsLogos[i].GetComponent<Renderer>();

            // Correct Button
            if (i == correctButtonIndex)
            {
                // Update the logo image
                rend.material.mainTexture = LanguagesLogos[currentLanguage];
            }
            // Others Buttons
            else
            {
                string randomLanguage;

                // Choose a random language, that is not the same that the correct button
                do
                {
                    randomLanguage = Languages[Random.Range(0, Languages.Length)];
                } while (randomLanguage == currentLanguage || System.Array.IndexOf(alreadyUsedColors, randomLanguage) != -1);

                // Add to always used languages 
                alreadyUsedColors[i] = randomLanguage;

                // Update the logo image
                rend.material.mainTexture = LanguagesLogos[randomLanguage];
            }
        }
    }

    // Call when a button is pressed
    void OnButtonPress(int index)
    {
        // Test if the module always complet
        if (nbrCorrectReponce >= NBR_CORRECT_REPONCE_NEED)
        {
            return;
        }
        
        // Test if it is the correct button
        if (correctButtonIndex == index)
        {
            nbrCorrectReponce++;
            UpdateLight(nbrCorrectReponce);

            DisplayText.text = "Valid";
            DisplayText.color = new Color(0f, 1f, 0f);

            if (nbrCorrectReponce >= NBR_CORRECT_REPONCE_NEED)
            {
                BombModule.HandlePass();
            } else
            {
                ChooseRandomReponce();
                GenerateButtons();
            }
        } else
        {
            DisplayText.text = "Error";
            DisplayText.color = new Color(1f, 0f, 0f);

            Debug.Log("Wrong ");
            BombModule.HandleStrike();
        }
    }
}
