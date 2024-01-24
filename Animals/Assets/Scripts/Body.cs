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

    public float bodySize;
    public float eyeSize;
    public float legSize;
    private Vector3 defaultTorso= new Vector3(0.714f, 0.396f, 1.151f)*3;
    private Vector3 defaultHead = new Vector3(0.4719f, 0.729f, 0.2379f)*1;
    private Vector3 defaultLegs = new Vector3(0.3f, 1.25f, 0.159f)*1;


    #region Physical Components
    public GameObject skeleton;
    public GameObject Torso => skeleton.gameObject;
    public GameObject Head => Torso.transform.Find("head").gameObject;
    public GameObject Eyes => Head.transform.Find("eyes").gameObject;
    public GameObject RightEye => Eyes.transform.Find("right eye").gameObject;
    public GameObject LeftEye => Eyes.transform.Find("left eye").gameObject;
    #endregion

    public Body(GameObject skeleton, float bodySize, float eyeSize, float legSize, Color primary, Color secondary, string name)
    {
        this.skeleton = skeleton;
        this.bodySize = bodySize;
        this.eyeSize = eyeSize;
        this.legSize = legSize;
        skeleton.name=name;
        name = skeleton.name;
        
        primaryColor = primary;
        secondaryColor = secondary;
        SetBodySize(bodySize);
        SetHeadAndEyeSize(eyeSize);
        SetLegSize(legSize);
    }

    public Body() { }

    private void SetColor(GameObject comp)
    {
        comp.GetComponent<Renderer>().material.color = primaryColor;
    }

    public void SetColor(GameObject comp, Color color)
    {
        comp.GetComponent<Renderer>().material.color = color;
    }
    private void SetBodySize(float width)
    {
        Torso.transform.localScale = new Vector3(defaultTorso.x+ width/2, defaultTorso.y +width /2, defaultTorso.z +width);
        SetColor(Torso, primaryColor);
    }
    private void SetHeadAndEyeSize(float width)
    {

        SetColor(Head, secondaryColor);
        Head.transform.localScale = new Vector3( defaultHead.x + width/2,defaultHead.y +  width/2, defaultHead.z+ width/2);
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
                child.transform.localScale = new Vector3(defaultLegs.x+ width/3, defaultLegs.y+ width / 2,defaultLegs.z+ width/3);
                SetColor(child.gameObject, secondaryColor);
            }
        }
    }



    public void DisplayName(GameObject parent)
    {
        GameObject nameDisplay = new GameObject("Name tag");
        nameDisplay.transform.parent = parent.transform;
        nameDisplay.transform.rotation = Quaternion.Euler(48.526f, 0, 0);
        //nameDisplay.transform.LookAt(nameDisplay.transform.position + Camera.main.transform.rotation * Vector3.forward,
        //Camera.main.transform.rotation* Vector3.up);
        TextMesh tm = nameDisplay.AddComponent<TextMesh>();
        tm.text = parent.name;
        nameDisplay.transform.position = new Vector3(parent.transform.position.x, parent.transform.position.y+6, parent.transform.position.z);
        tm.color = Color.white;
        tm.alignment = TextAlignment.Center;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.characterSize = 0.065f;
        tm.fontSize = 200;
    }

}
