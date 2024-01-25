using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerNeuron : Neuron, Destination, Origin
{
    private List<Tuple<int, float>> m_weights;
    private float m_innerValue;
    private float m_bias;
    private int id;
    public InnerNeuron(GameObject parent, int Id)
    {
        this.parent = parent;
        this.id = Id;
        m_weights = new List<Tuple<int, float>>();
    }
    public override int GetId()
    {
        return id;
    }
    public void ChangeWeight(int sourceId, float val)
    {
        foreach (var weight in m_weights)
        {
            if (weight.Item1 == sourceId)
            {
                m_weights.Remove(weight);
             
                m_weights.Add(new Tuple<int, float>(sourceId, val));
                break;
            }
        }
    }
    public void RemoveWeight(int sourceId)
    {
        foreach (var weight in m_weights)
        {
            if (weight.Item1 == sourceId)
            {
                m_weights.Remove(weight);
                break;
            }
        }

    }
    public float GetOriginValue()
    {
        return m_innerValue;
    }
    public void SetOriginValue(float val)
    {
        m_innerValue = val;
    }
    public void SetBias(float val)
    {
        m_bias = val;
    }
    public float GetBias()
    {
        return m_bias;
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
