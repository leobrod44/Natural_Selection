using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

public class Body
{
    public Color primaryColor;

    public Color secondaryColor;

    public int AnimalIdentifier;

    public String name;

    #region Physical Components
    public GameObject skeleton;
    public GameObject Torso => skeleton.gameObject;
    public GameObject Head => Torso.transform.Find("head").gameObject;
    public GameObject Eyes => Head.transform.Find("eyes").gameObject;
    public GameObject RightEye => Eyes.transform.Find("right eye").gameObject;
    public GameObject LeftEye => Eyes.transform.Find("left eye").gameObject;
    #endregion

    public Body(GameObject skeleton, float bodySize, float eyeSize, float legSize, Color primary, Color secondary)
    {
        this.skeleton = skeleton;
        skeleton.name = GeneEncoding.GenerateLatinName();
        name = skeleton.name;
        DisplayName(skeleton);
        primaryColor = primary;
        secondaryColor = secondary;
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
                child.transform.localScale += new Vector3(width/3, width / 2, width/3);
                SetColor(child.gameObject, secondaryColor);
            }
        }
    }



    private void DisplayName(GameObject parent)
    {
        GameObject nameDisplay = new GameObject("Name tag");
        nameDisplay.transform.rotation = Camera.main.transform.rotation;
        nameDisplay.transform.LookAt(nameDisplay.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        TextMesh tm = nameDisplay.AddComponent<TextMesh>();
        tm.text = parent.name;
        nameDisplay.transform.parent = parent.transform;
        nameDisplay.transform.position = new Vector3(parent.transform.position.x, parent.transform.position.y+1, parent.transform.position.z);
        
        tm.color = Color.magenta;
        tm.fontStyle = FontStyle.Bold;
        tm.alignment = TextAlignment.Center;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.characterSize = 0.065f;
        tm.fontSize = 80;
    }

}
