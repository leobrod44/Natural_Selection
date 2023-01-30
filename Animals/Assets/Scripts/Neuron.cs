using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Neuron
{
    //when adding a new neuron, add the class, add it to the brain dict, change total num
    public static readonly int totalNeuronsAvailable = 13;
    public static readonly int lastInputNeuron = 6;
    public Animal animal;
    public GameObject parent { get; set; }
    public int Id;
    public Brain brain => animal.brain;

}

public interface Destination
{
    public List<Tuple<int, float>> GetWeights();
    public void AddWeight(int sourceId, float val);
    public float GetActivatedValue();
    public void SetActivatedValue(float val);
}

public interface Origin
{
    public float GetOriginValue();
    public void SetOriginValue(float val);
}
