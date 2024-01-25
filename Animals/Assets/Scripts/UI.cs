using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System.Linq;
using System;

public class UI : MonoBehaviour
{

    // Reference to the Text component on your Canvas
    public TMP_Text lifepsan;
    public TMP_Text lifepsanVariation;
    public TMP_Text generation;
    // The variable you want to display

    private int lastValueGen;
    private float lastScore;
    public List<float> scores;
    //0,11.425,12.48583,14.40917,12.46917,13.265,13.27583,15.00083,16.45667,17.9175,16.97833,15.56,17.33417,18.50417,17.07083,17.57166,16.64417,17.86667,16.89167,17.15833,18.90583
    //40 pop, 3 samples

    //0,9.49133,9.734666,11.81266,11.938,11.066,10.86267,10.564,12.09267,11.67066,11.84267,12.23267,12.02933,12.106,13.312,12.966,14.31666,12.61266,12.39,13.22066,13.05866,13.60866,13.73333,15.18733,13.22933,13.22333,13.236,13.51266,12.29133,13.30867,14.696,13.80733,13.144



    void Start()
    {
       scores= new List<float>();

        // Update the text property of the Text component with the value of your variable

    }
    private void Update()
    {
        // Check if the Text component is assigned
        if (Generation.generation > lastValueGen)
        {
            generation.text = Generation.generation.ToString();
            scores.Add(Generation.highScore);
            Debug.Log(String.Join(",", scores));
            if (Generation.generation > 1)
            {
                if (Generation.highScore > lastScore)
                {
                    lifepsan.text = Generation.highScore.ToString().ToString() + "000";
                    if (lifepsan.text.Contains("."))
                        lifepsan.text = lifepsan.text.Substring(0, lifepsan.text.IndexOf(".") + 3);
                    lifepsanVariation.text = "+" + (Generation.highScore - lastScore).ToString()+"000";
                    if (lifepsanVariation.text.Contains("."))
                        lifepsanVariation.text = lifepsanVariation.text.Substring(0, lifepsanVariation.text.IndexOf(".") + 3);

                    lifepsanVariation.color = Color.green;
                }
                if (Generation.highScore < lastScore)
                {
                    lifepsan.text = Generation.highScore.ToString().ToString() + "000";
                    if (lifepsan.text.Contains("."))
                        lifepsan.text = lifepsan.text.Substring(0, lifepsan.text.IndexOf(".") + 3);
                    lifepsanVariation.text = "" + (Generation.highScore - lastScore).ToString() + "000"; ;
                    if (lifepsanVariation.text.Contains("."))
                        lifepsanVariation.text = lifepsanVariation.text.Substring(0, lifepsanVariation.text.IndexOf(".") + 3);
                    lifepsanVariation.color = Color.red;
                    
                }
                if( Generation.highScore == lastScore)
                {
                    lifepsan.text = Generation.highScore.ToString().ToString() + "000";
                    if (lifepsan.text.Contains("."))
                        lifepsan.text = lifepsan.text.Substring(0, lifepsan.text.IndexOf(".") + 3);
                    lifepsanVariation.text = "" + (Generation.highScore - lastScore).ToString() + "000"; ;

                    if (lifepsanVariation.text.Contains("."))   
                        lifepsanVariation.text = lifepsanVariation.text.Substring(0, lifepsanVariation.text.IndexOf(".") + 3);
                    lifepsanVariation.color = Color.white;
                }
            }
            lastScore = Generation.highScore;
            lastValueGen = Generation.generation;
        }
       
     


    }
}



