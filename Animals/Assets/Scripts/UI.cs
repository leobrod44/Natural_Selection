using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class UI : MonoBehaviour
{

    // Reference to the Text component on your Canvas
    public TMP_Text lifepsan;
    public TMP_Text lifepsanVariation;
    public TMP_Text generation;
    // The variable you want to display

    private int lastValueGen;
    private int lastScore;
    void Start()
    {
       

        // Update the text property of the Text component with the value of your variable

    }
    private void Update()
    {
        // Check if the Text component is assigned
        if (Generation.generation > lastValueGen)
        {
            generation.text = Generation.generation.ToString();
            if (Generation.generation > 0)
            {
                if (Generation.highScore > lastScore)
                {
                    lifepsan.text = Generation.highScore.ToString();
                    lifepsanVariation.text = "+" + (Generation.highScore - lastScore).ToString();
                    lifepsanVariation.color = Color.green;
                }
                if (Generation.highScore < lastScore)
                {
                    lifepsan.text = Generation.highScore.ToString();
                    lifepsanVariation.text = "" + (Generation.highScore - lastScore).ToString();
                    lifepsanVariation.color = Color.red;
                    
                }
                if( Generation.highScore == lastScore)
                {
                    lifepsan.text = Generation.highScore.ToString();
                    lifepsanVariation.text = "" + (Generation.highScore - lastScore).ToString();
                    lifepsanVariation.color = Color.white;
                }
            }
            lastScore = Generation.highScore;
            lastValueGen = Generation.generation;
        }
       
     


    }
}



