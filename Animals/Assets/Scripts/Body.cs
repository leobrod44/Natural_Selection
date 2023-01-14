using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

public class Body
{
    public string name;

    public Color primaryColor;

    public Color secondaryColor;

    public int AnimalIdentifier;

    #region Physical Components
    public GameObject skeleton;
    public GameObject Torso => skeleton.gameObject;
    public GameObject Head => Torso.transform.Find("head").gameObject;
    public GameObject Eyes => Head.transform.Find("eyes").gameObject;
    public GameObject RightEye => Eyes.transform.Find("right eye").gameObject;
    public GameObject LeftEye => Eyes.transform.Find("left eye").gameObject;
    #endregion

    public Body(GameObject skeleton, float bodySize, float eyeSize, float legSize, int x, int y)
    {
        this.skeleton = skeleton;
        name = GenerateCoolName();
        skeleton.name = name;
        primaryColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        secondaryColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        SetBodySize(bodySize);
        SetHeadAndEyeSize(eyeSize);
        SetLegSize(legSize);
    }

    private void SetColor(GameObject comp)
    {
        comp.GetComponent<Renderer>().material.color = primaryColor;
    }

    private void SetColor(GameObject comp, Color color)
    {
        comp.GetComponent<Renderer>().material.color = color;
    }
    private void SetBodySize(float width)
    {
        Torso.transform.localScale += new Vector3(width/2, width/2, width);
        SetColor(Torso, primaryColor);
    }
    private void SetHeadAndEyeSize(float width)
    {

        SetColor(Head, secondaryColor);
        Head.transform.localScale += new Vector3(width/2, width/2, width/2);
        //LeftEye.transform.localScale += new Vector3(width/2, width/2, width/2);
        //RightEye.transform.localScale += new Vector3(width / 2, width / 2, width / 2);
        SetColor(LeftEye, primaryColor);
        SetColor(RightEye, primaryColor);
    }
    private void SetLegSize(float width)
    {
        foreach (Transform child in Torso.transform)
        {
            if (child.name.Contains("leg"))
            {
                child.transform.localScale += new Vector3(width, width / 2, width);
                SetColor(child.gameObject, secondaryColor);
            }
        }
    }

    private string GenerateCoolName()
    {
        System.Random r = new System.Random();
        int lenFirst = r.Next(2, 6);
        int lenLast = r.Next(2, 6);
        string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "t" };
        string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
        string[] ends = {"o", "is", "us","ien","a"};
        string firstName = "";
        firstName += consonants[r.Next(consonants.Length)].ToUpper();
        firstName += vowels[r.Next(vowels.Length)];
        int c = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
        while (c < lenFirst)
        {
            firstName += consonants[r.Next(consonants.Length)];
            c++;
            firstName += vowels[r.Next(vowels.Length)];
            c++;
        }

        firstName += consonants[r.Next(consonants.Length)];
        c++;
        firstName += ends[r.Next(ends.Length)];
        c++;

        string secondName = "";
        secondName += consonants[r.Next(consonants.Length)].ToUpper();
        secondName += vowels[r.Next(vowels.Length)];
        c = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
        while (c < lenLast)
        {
            secondName += consonants[r.Next(consonants.Length)];
            c++;
            secondName += vowels[r.Next(vowels.Length)];
            c++;
        }
        secondName += consonants[r.Next(consonants.Length)];
        c++;
        secondName += ends[r.Next(ends.Length)];
        c++;

        return firstName + " " + secondName;
    }

}
