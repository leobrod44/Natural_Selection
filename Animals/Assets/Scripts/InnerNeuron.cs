using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerNeuron : Neuron
{
    public static int NumInnerLayers;
    public static int Count = -1;
    public InnerNeuron(GameObject parent)
    {
        this.parent = parent;
        Id = Count;
        Count--;
    }
}
