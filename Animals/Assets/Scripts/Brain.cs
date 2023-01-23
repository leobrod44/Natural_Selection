using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Brain
{
    private const int largePrime = 2357;

    private Dictionary<int, Neuron> AllNeurons { get; set; }

    public Dictionary<int, Neuron> Neurons { get; set; }
    public List<string> Connections { get; set; }
    public GameObject Parent { get; set; }
    public Brain(GameObject parent)
    {
        Parent = parent;
        AllNeurons = new Dictionary<int, Neuron>()
        {
            {1, new WaterLevelSensor(parent)},
            {2, new FoodLevelSensor(parent)},
            {3, new WaterDistanceForwardSensor(parent)},
            {4, new WaterDistanceSidesSensor(parent)},
            {5, new FoodDistanceForwardSensor(parent)},
            {6, new FoodDistanceSidesSensor(parent)},
            {7, new RotateSlightRightAction(parent)},
            {8, new RotateSlightLeftAction(parent)},
            {9, new RotateQuarterRightAction(parent)},
            {10, new RotateQuarterLeftAction(parent)},
            {11, new TurnAroundAction(parent)},
            {12, new RotateRandomAction(parent)},
        };

        Neurons = new Dictionary<int, Neuron>();
        Connections = new List<string>();
        InitializeBrain(6, 6, 3);
    }
    // create net

    // create connection

    public void InitializeBrain(int numSensorNeurons, int numActorNeurons, int numInternalNeurons)
    {
        HashSet<int> usedSensors = new HashSet<int>();
        HashSet<int> usedActors = new HashSet<int>();
        HashSet<int> usedInternals = new HashSet<int>();
        System.Random rand;

        //generate sensors
        rand = new System.Random();
        usedSensors =new HashSet<int>(Enumerable.Range(1, Neuron.lastInputNeuron).OrderBy(x => rand.Next()).Take(numSensorNeurons));

        //generate actors TODO rand range going to like 18
        rand = new System.Random();
        usedActors = new HashSet<int>(Enumerable.Range(Neuron.lastInputNeuron+1, Neuron.totalNeuronsAvailable-Neuron.lastInputNeuron).OrderBy(x => rand.Next()).Take(numActorNeurons));

        //generate inner neurons
        for (int i = 0; i < numInternalNeurons; i++)
        {
            Neuron innerNeuron = new InnerNeuron(Parent);
            AllNeurons.Add(innerNeuron.Id, innerNeuron);
            usedInternals.Add(innerNeuron.Id);
        }

        foreach (var Id in usedSensors.Union(usedActors.Union(usedInternals)))
        {
            Neurons.Add(Id, AllNeurons[Id]);
        }
        //generate connections

        var unionedSensorInner = new HashSet<int>(usedSensors.Union(usedInternals));
        var unionedActorInner = new HashSet<int>(usedActors.Union(usedInternals));

        foreach (int id in unionedSensorInner)
        {
            int randNeuron = UnityEngine.Random.Range(0, unionedActorInner.Count);
            var destination = unionedActorInner.ElementAt(randNeuron);
            var weight = UnityEngine.Random.Range(0f, 4f);
            string connection = CreateConnection(id, destination, weight);
            Connections.Add(connection);
        }
    }

    public string CreateConnection(int sourceId, int destinationId, float weight)
    {
        string sourceBin = IdToBinary(sourceId);
        string destBin = IdToBinary(destinationId);
        string sourceBit = Neurons[sourceId] is SensorNeuron ? "1" : "0";
        string destBit = Neurons[destinationId] is ActionNeuron ? "1" : "0";
        string weightBin = WeightToBinary(4.274f);
        string connection32Bit = sourceBit + sourceBin + destBit + destBin + weightBin;
        string connectionHex = Convert.ToInt64(connection32Bit, 2).ToString("X"); ;
        return connectionHex;
    }

    private string IdToBinary(int num)
    {
        string fmt = "0000000.##";
        string bin = "";
        ValidateBinaryFormat(num);
        if(num>0)
            bin = Convert.ToInt32(Convert.ToString(num, 2)).ToString(fmt);
        else
        {
            bin = Convert.ToString(num, 2);
            bin = bin.Substring(bin.Length - 7);
        }
        return bin;
    }

    private int BinaryToId(string bin) 
    {
        int value;
        if (bin[0]=='0')
            value = Convert.ToInt32(bin,2);
        else
        {
            string newBin = bin.Replace('1', 'a').Replace('0', 'b').Replace('a', '0').Replace('b', '1');
            value = -Convert.ToInt32(newBin,2) -1;
        }
        return value;
    }


    private string WeightToBinary(float num)
    {
        int value = (int)(num * largePrime);
        string fmt = "0000000000000000.##";
        string bin = Convert.ToInt64(Convert.ToString(value, 2)).ToString(fmt);
        return bin;
    }

    private float BinaryToWeight(string bin)
    {
        long x = Convert.ToInt64(bin);
        float y = (float) x / largePrime;
        return y;
    }
    private static void ValidateBinaryFormat(int num)
    {
        if (Math.Abs(num)>64)
            throw new InvalidBinaryFormatException(num);
    }


}

class InvalidBinaryFormatException : Exception
{
    public InvalidBinaryFormatException() { }

    public InvalidBinaryFormatException(int num)
        : base(String.Format("Invalid Binary Format, Exceeding maxium of 256 for 8 bit binary: {0}", num))
    {

    }
}
