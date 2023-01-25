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
    public int[] Sensors { get; set; }
    public int[] Inners { get; set; }
    public int[] Actors { get; set; }
    //public List<string> Connections => SensorConnections.ToHashSet().Union(InnerConnections.ToHashSet());
    public List<string> SensorConnections { get; set; }
    public List<string> InnerConnections { get; set; }

    private HashSet<int> usedActors;
    public GameObject Parent { get; set; }

    private Engine engine;
    public Brain(GameObject parent)
    {
        engine = GameObject.Find("Engine").GetComponent<Engine>();
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
        SensorConnections = new List<string>();
        InnerConnections = new List<string>();
        usedActors = new HashSet<int>();
        InitializeBrain(engine.numberOfSensorNeurons,engine.numberOfInnerNeurons);
    }
    


    #region Building Brain
    public void InitializeBrain(int numSensorNeurons, int numInternalNeurons)
    {
        HashSet<int> usedSensors = new HashSet<int>();
        HashSet<int> availableActors = new HashSet<int>();
        HashSet<int> usedInternals = new HashSet<int>();
        System.Random rand;

        //generate sensors
        rand = new System.Random();
        usedSensors =new HashSet<int>(Enumerable.Range(1, Neuron.lastInputNeuron).OrderBy(x => rand.Next()).Take(numSensorNeurons));

        //generate actors TODO rand range going to like 18
        rand = new System.Random();
        availableActors = new HashSet<int>(Enumerable.Range(Neuron.lastInputNeuron+1, Neuron.totalNeuronsAvailable-Neuron.lastInputNeuron).OrderBy(x => rand.Next()).Take(Neuron.totalNeuronsAvailable - Neuron.lastInputNeuron));

        //generate inner neurons
        int innerNeuronCount = -1;
        for (int i = 0; i < numInternalNeurons; i++)
        {
            Neuron innerNeuron = new InnerNeuron(Parent,innerNeuronCount);
            AllNeurons.Add(innerNeuron.Id, innerNeuron);
            usedInternals.Add(innerNeuron.Id);
            innerNeuronCount--;
        }

        //store all used neurons
        foreach (var Id in usedSensors.Union(availableActors.Union(usedInternals)))
        {
            Neurons.Add(Id, AllNeurons[Id]);
        }
        //generate connections

        var unionedSensorInner = new HashSet<int>(usedSensors.Union(usedInternals));
        var unionedActorInner = new HashSet<int>(availableActors.Union(usedInternals));

        //create sensor connections
        foreach (int id in usedSensors)
        {
            int randNeuron = UnityEngine.Random.Range(0, unionedActorInner.Count);
            var destination = unionedActorInner.ElementAt(randNeuron);
            var weight = UnityEngine.Random.Range(-4f, 4f);
            string connection = CreateConnection(id, destination, weight);
            SensorConnections.Add(connection);
        }
        //create inner neuron connections
        foreach (int id in usedInternals)
        {
            int randNeuron = UnityEngine.Random.Range(0, unionedActorInner.Count);
            var destination = unionedActorInner.ElementAt(randNeuron);
            var weight = UnityEngine.Random.Range(-4f, 4f);
            string connection = CreateConnection(id, destination, weight);
            InnerConnections.Add(connection);
        }

        //store neuron iDs
        Sensors = usedSensors.ToArray();
        Inners = usedInternals.ToArray();
        Actors = usedActors.ToArray();
    }

    public string CreateConnection(int sourceId, int destinationId, float weight)
    {
        if (Neurons[destinationId] is ActionNeuron)
        {
            ((ActionNeuron)Neurons[destinationId]).AddWeight(sourceId, weight);
            usedActors.Add(destinationId);
        }
        else if (Neurons[destinationId] is InnerNeuron)
        {
            ((InnerNeuron)Neurons[destinationId]).AddWeight(sourceId, weight);
        }
        string sourceBin = IdToBinary(sourceId);
        string destBin = IdToBinary(destinationId);
        string sourceBit = Neurons[sourceId] is SensorNeuron ? "1" : "0";
        string destBit = Neurons[destinationId] is ActionNeuron ? "1" : "0";
        string weightBin = WeightToBinary(weight);
        string connection32Bit = sourceBit + sourceBin + destBit + destBin + weightBin;
        string connectionHex = Convert.ToInt64(connection32Bit, 2).ToString("X"); ;
        return connectionHex;
    }
    #endregion

    #region Propagation and Decision Making
    public ActionNeuron GetScenarioActionNeuron()
    {
        //set values through network
        Propagate();

        //get action neurons
        var actionNeurons = Actors.Select(x => (ActionNeuron)Neurons[x]);

        //get the highest value acion neuron
        var highestValueNeuron = actionNeurons.Aggregate(actionNeurons.First(), (highest, next) 
            => next.GetActivatedValue() > highest.GetActivatedValue() ? next: highest);

        //perform the action on that neuron
        return highestValueNeuron;
    }

    /// <summary>
    /// Propagates sensor inputs and sets values through network
    /// </summary>
    private void Propagate()
    {
        SetSensorValues();
        //propagate inner neurons first, since actor neurons can depend on inner neurons propagation output
        foreach(int id in Inners)
        {
            FeedForwardInternal<InnerNeuron>(Neurons[id]);
        }
        foreach(int id in Actors)
        {
            FeedForwardInternal<ActionNeuron>(Neurons[id]);
        }
    }

    private void SetSensorValues()
    {
        for (int i = 0; i < Sensors.Length; i++)
        {
            try
            {
                var Id = Sensors[i];
                ((SensorNeuron)Neurons[Id]).SetSensorValue();
            }
            catch
            {
                Debug.Log("Wrong type of neuron stored in sensor neuron layer");
            }
        }
    }
    /// <summary>
    /// Takes a neuron as parameter and computes the activation function of the sum of it's inputs
    /// </summary>
    /// <typeparam name="T">Type of destination neuron</typeparam>
    /// <param name="n">Destination neuron to update propagated value</param>
    /// 
    private void FeedForwardInternal<T>(Neuron n) where T : Neuron, Destination
    {
        T destinationNeuron = (T)n;
        float sum = 0;

        //compute sensor values first
        List<Tuple<InnerNeuron, InnerNeuron>> postInnerConnections = new List<Tuple<InnerNeuron, InnerNeuron>>();
        foreach (Tuple<int, float> weight in destinationNeuron.GetWeights())
        {
            var originNeuron = Neurons[weight.Item1];
            if (originNeuron is InnerNeuron && destinationNeuron is InnerNeuron)
            {
                InnerNeuron origin = (InnerNeuron)originNeuron;
                sum += ComputeActivation<InnerNeuron>(origin, weight.Item2);
            }
            else if (originNeuron is SensorNeuron)
            {
                SensorNeuron origin = (SensorNeuron)originNeuron;
                sum += ComputeActivation<SensorNeuron>(origin, weight.Item2);
            }
        }
        //return Tuple<float, List<Tuple<InnerNeuron, InnerNeuron>>>(sum, innerNeurons);
        var activatedValue = (float)Math.Tanh(sum);
        destinationNeuron.SetActivatedValue(activatedValue);

        //if (originneuron is innerneuron)
        //{
        //}
        //TODO case where inner neuron reads a further inner neuron activation value as input but nto assigned yet
        // postInnerConnections.Add(new Tuple<InnerNeuron, InnerNeuron> ( (InnerNeuron)originNeuron, (InnerNeuron)originNeuron));
    }
    /// <summary>
    /// Computes weight * sensor value;
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <param name="originNeuron"></param>
    /// <param name="weight"></param>
    private float ComputeActivation<U>(U originNeuron, float weight) where U : Neuron, Origin
    {
            return originNeuron.GetOriginValue() * weight;
    }
    #endregion

    #region Helpers
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
        string bin = "";
        if (num > 0)
            bin = Convert.ToInt64(Convert.ToString(value, 2)).ToString(fmt);
        else
        {
            bin = Convert.ToString(value, 2);
            //bin = bin.Substring(bin.Length - 7);
        }
        float val = BinaryToWeight(bin);
        return bin;
    }

    private float BinaryToWeight(string bin)
    {
        int value;
        if (bin[0] == '0')
            value = Convert.ToInt32(bin, 2);
        else
        {
            string newBin = bin.Replace('1', 'a').Replace('0', 'b').Replace('a', '0').Replace('b', '1');
            value = -Convert.ToInt32(newBin, 2) - 1;
        }
        float dividedValue = value!=0? (float)value / (float)largePrime : 0;
        return dividedValue;
    }
    private static void ValidateBinaryFormat(int num)
    {
        if (Math.Abs(num)>64)
            throw new InvalidBinaryFormatException(num);
    }

    #endregion

}

class InvalidBinaryFormatException : Exception
{
    public InvalidBinaryFormatException() { }

    public InvalidBinaryFormatException(int num)
        : base(String.Format("Invalid Binary Format, Exceeding maxium of 256 for 8 bit binary: {0}", num))
    {

    }
}
