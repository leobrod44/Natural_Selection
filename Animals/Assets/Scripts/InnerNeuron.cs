using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerNeuron : Neuron, Destination, Origin
{
    private List<Tuple<int, float>> m_weights;
    private float m_innerValue;
    public InnerNeuron(GameObject parent, int Id)
    {
        this.parent = parent;
        this.Id = Id;
        m_weights = new List<Tuple<int, float>>();
    }
    public float GetOriginValue()
    {
        return m_innerValue;
    }
    public void SetOriginValue(float val)
    {
        m_innerValue = val;
    }

    public List<Tuple<int, float>> GetWeights()
    {
        return m_weights;
    }
    public void AddWeight(int sourceId, float val)
    {
        m_weights.Add(new Tuple<int, float>(sourceId, val));
    }
    public float GetActivatedValue()
    {
        return m_innerValue;
    }
    public void SetActivatedValue(float val)
    {
        m_innerValue = val;
    }
}
