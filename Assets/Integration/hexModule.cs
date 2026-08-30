using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hexModule : MonoBehaviour
{
    // Références à lier dans l'inspecteur Unity
    public KMBombModule BombModule;
    public TextMesh DisplayText;
    public MeshRenderer DisplayBackground;
    public KMSelectable[] Buttons;
    public MeshRenderer[] Leds;
    public Color[] AvailableColors; // Liste des couleurs possibles

    public Color CurrentColor { get; private set; } // Couleur actuelle affichée
    public int correctButtonIndex;

    private int count = 0;

    void Start()
    {
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

    void OnActivate()
    {
        CurrentColor = GenerateRandomHexColor();
        DisplayColor();
        GenerateButtonColors();
    }

    void DisplayColor()
    {
        DisplayText.text = "#" + ColorUtility.ToHtmlStringRGB(CurrentColor);
    }

    Color GenerateRandomHexColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }

    void GenerateButtonColors()
    {
        Color[] alreadyUsedColors = new Color[Buttons.Length];
        correctButtonIndex = Random.Range(0, Buttons.Length);
        for (int i = 0; i < Buttons.Length; i++)
        {
            if (i == correctButtonIndex)
            {
                MeshRenderer buttonRenderer = Buttons[i].GetComponent<MeshRenderer>();
                buttonRenderer.material.color = CurrentColor;
            }
            else
            {
                Color randomColor;
                do
                {
                    randomColor = AvailableColors[Random.Range(0, AvailableColors.Length)];
                } while (randomColor == CurrentColor || System.Array.IndexOf(alreadyUsedColors, randomColor) != -1);

                alreadyUsedColors[i] = randomColor;
                MeshRenderer buttonRenderer = Buttons[i].GetComponent<MeshRenderer>();
                buttonRenderer.material.color = randomColor;
            }
        }
    }

    void UpdateLight(int index)
    {
        MeshRenderer led = Leds[index].GetComponent<MeshRenderer>();
        if (led)
        {
            led.material.color = new Color(0f, 1f, 0f);
        }

    }


    void OnButtonPress(int index)
    {
        Buttons[index].AddInteractionPunch();
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Buttons[index].transform);

        MeshRenderer buttonRenderer = Buttons[index].GetComponent<MeshRenderer>();

        if (index == correctButtonIndex)
        {
            Debug.Log("Correct ");
            count++;
            UpdateLight(count - 1);
            if (count >= 3)
            {
                BombModule.HandlePass();
            }
            else
            {
                CurrentColor = GenerateRandomHexColor();
                DisplayColor();
                GenerateButtonColors();
            }
        }
        else
        {
            Debug.Log("Wrong ");
            BombModule.HandleStrike();
        }

    }
}
