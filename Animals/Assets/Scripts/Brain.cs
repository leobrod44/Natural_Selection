using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Brain
{
    private const int largePrime = 2357;

    public Dictionary<int, Neuron> AllNeurons { get; set; }
    public Dictionary<int, Neuron> Neurons { get; set; }
    public int[] Sensors { get; set; }
    public int[] Inners { get; set; }
    public int[] Actors { get; set; }
    //public List<string> Connections => SensorConnections.ToHashSet().Union(InnerConnections.ToHashSet());
    public List<string> SensorConnections { get; set; }
    public List<string> InnerConnections { get; set; }

    private HashSet<int> usedActors;
    public GameObject Parent { get; set; }

    public Vector3 nearestWater;

    public Vector3 nearestFood;

    public Vector3 lastFood;

    public Vector3 lastWater;

    public Vector3 nearestAlly;

    private Vector3 nearestEnemy;

    private Engine engine;
    public bool mutated = false;



    public Brain(GameObject parent)
    {
        engine = GameObject.Find("Engine").GetComponent<Engine>();
        Parent = parent;
        AllNeurons = new Dictionary<int, Neuron>()
        {
            {1, new WaterLevelSensor(parent)},
            {2, new FoodLevelSensor(parent)},
            {3, new WaterDistanceSensor(parent)},
            {4, new FoodDistanceSensor(parent)},
            {5, new TargetWater(parent)},
            {6, new TargetFood(parent)},
            {7, new Scout(parent)},
            //{Ally distance
            //{Enemy distance
            //{7, new RotateSlightRightAction(parent)},
            //{8, new RotateSlightLeftAction(parent)},
            //{9, new RotateQuarterRightAction(parent)},
            //{10, new RotateQuarterLeftAction(parent)},
            //{11, new TurnAroundAction(parent)},
            //{12, new RotateRandomAction(parent)},
            //{13, new DoNothingAction(parent)},
            

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
        HashSet<int> usedInternals = new HashSet<int>();
        System.Random rand;

        //generate sensors
        HashSet<int>  usedSensors = new HashSet<int>(Enumerable.Range(1, Neuron.lastInputNeuron));

        //generate actors TODO rand range going to like 18
        HashSet<int>  usedActors = new HashSet<int>(Enumerable.Range(Neuron.lastInputNeuron + 1, Neuron.totalNeuronsAvailable - Neuron.lastInputNeuron));

        //generate inner neurons
        int innerNeuronCount = -1;
        for (int j  = 0; j< engine.numberOfInnerLayers; j++)
        {
            for (int i = 0; i < numInternalNeurons; i++)
            {
                Neuron innerNeuron = new InnerNeuron(Parent, innerNeuronCount);
                AllNeurons.Add(innerNeuron.Id, innerNeuron);
                usedInternals.Add(innerNeuron.Id);
                innerNeuronCount--;
            }

        }
        //store all used neurons
        foreach (var Id in usedSensors.Union(usedActors.Union(usedInternals)))
        {
            Neurons.Add(Id, AllNeurons[Id]);
        }
        foreach (var n in usedActors.Union(usedInternals))
        {
            ((Destination)Neurons[n]).SetBias(UnityEngine.Random.Range(-1f, 1f));
        }

        //create sensor connections
        foreach (int id in usedSensors)
        {
            // Select a random amount of elements from the usedInternals set
            var innerActors = usedInternals.Union(usedActors);
            int numElements = UnityEngine.Random.Range(0, innerActors.Count()+1);
            HashSet<int> selectedElements = new HashSet<int>(usedInternals.OrderBy(x => UnityEngine.Random.value).Take(numElements));
            foreach(int id2 in selectedElements)
            {
                var weight = UnityEngine.Random.Range(-engine.weightLimit, engine.weightLimit);
                weight = weight == 0f ? 0.01f : weight;
                string connection = CreateConnection(id, id2, weight);
                SensorConnections.Add(connection);
            }
        }
        //create inner neuron connections
        foreach (int id in usedInternals)
        {
            int numElements = UnityEngine.Random.Range(0, usedActors.Count+1);
            HashSet<int> selectedElements = new HashSet<int>(usedActors.OrderBy(x => UnityEngine.Random.value).Take(numElements));
            foreach (int id2 in selectedElements)
            {
                var weight = UnityEngine.Random.Range(engine.weightLimit, engine.weightLimit);
                weight = weight == 0f ? 0.01f : weight;
                string connection = CreateConnection(id, id2, weight);
                InnerConnections.Add(connection);
            }
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
        }
        else if (Neurons[destinationId] is InnerNeuron)
        {
            ((InnerNeuron)Neurons[destinationId]).AddWeight(sourceId, weight);
        }
        Neuron source;
        Neurons.TryGetValue(sourceId, out source);
        Neuron destination;
        Neurons.TryGetValue(destinationId, out destination);

        string sourceBin = GeneEncoding.IdToBinary(sourceId);
        string destBin = GeneEncoding.IdToBinary(destinationId);
        string sourceBit = source is SensorNeuron ? "1" : "0";
        string destBit = destination is ActionNeuron ? "1" : "0";
        string weightBin = GeneEncoding.FloatToBinary(weight);
        string connection32Bit = sourceBit + sourceBin + destBit + destBin + weightBin;
        var c = weightBin.Length;
        var x = connection32Bit.Length;
        string connectionHex = GeneEncoding.BinToHex(connection32Bit);
        return connectionHex;

    }
    public void RemoveConnection(int sourceId, int destinationId)
    {
        Neuron source;
        Neurons.TryGetValue(sourceId, out source);

        Neuron dest;
        Neurons.TryGetValue(destinationId, out dest);

        if (source is SensorNeuron)
        {
            if(dest is ActionNeuron)
            {
                ((ActionNeuron)dest).RemoveWeight(sourceId);
            }
            else if (dest is InnerNeuron)
            {
                ((InnerNeuron)dest).RemoveWeight(sourceId);
            }
            else
            {
                Debug.Log("type " + dest.GetType() + " not supported");
            }
        }
        else if (source is InnerNeuron)
        {
            ((ActionNeuron)dest).RemoveWeight(sourceId);
        }
        else
        {
            Debug.Log("type "  + " not supported");
        }
    }
    public bool AddNeuron(Neuron n)
    {
        if (Neurons.ContainsKey(n.Id))
        {
            Debug.Log("Brain alread contains neuron: " + n.Id);
            return false;
        }

        if (n is (SensorNeuron))
        {
            Sensors.Append(n.Id);
        }
        else if (n is (InnerNeuron))
        {
            Inners.Append(n.Id);
        }
        else
        {
            Actors.Append(n.Id);
        }
        Neurons.Add(n.Id, n);
        
        return true;
        //TODO change all proper string connections

    }
    public bool RemoveNeuron(int id)
    {
        if (!Neurons.ContainsKey(id))
        {
            Debug.Log("Brain dosent contain neuron: " + id);
            return false;
        }
        
        if (Neurons[id] is (SensorNeuron))
        {
            Sensors = Sensors.Where(x => x != id).ToArray();
        }
        else if (Neurons[id] is (InnerNeuron))
        {
            Inners = Inners.Where(x => x != id).ToArray();
        }
        else
        {
            Actors = Actors.Where(x => x != id).ToArray();
        }
        Neurons.Remove(id);
        return true;
    }
    #endregion
    #region Change Generation

    //fitness for when breed integrated
    //selection

    //mutate
    #endregion
    #region Propagation and Decision Making
    public ActionNeuron GetScenarioActionNeuron()
    {
        if(Actors.Length == 0)
        {
            Debug.Log("No action neurons in brain");
            return null;
        }
        //set values through network
        Propagate();

        //get action neurons
        List<Neuron> actionNeurons = new List<Neuron>();
        foreach (var a in Actors)
        {
            actionNeurons.Add(Neurons[a]);
            var val = ((ActionNeuron)Neurons[a]).GetActivatedValue();
            //Debug.Log("Action Neuron " + a + " value: " + val);
        }
        var aNeurons = Actors.Select(x => (ActionNeuron)Neurons[x]);
        var highestValueNeuron = aNeurons.Aggregate(aNeurons.First(), (highest, next) => next.GetActivatedValue() > highest.GetActivatedValue() ? next : highest);



        //get the highest value acion neuron

        //var highestValueNeuron = actionNeurons.Aggregate(actionNeurons.First(), (highest, next) 
        //    => next.GetActivatedValue() > highest.GetActivatedValue() ? next: highest);

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
        foreach (int id in Inners)
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
        //Scan nearby values
        for (int i = 0; i < Sensors.Length; i++)
        {
            Neuron neuron = null;
            Type t = null;

            neuron = (Neurons[Sensors[i]]);
            t = neuron.GetType();
            ((SensorNeuron)Neurons[Sensors[i]]).SetSensorValue();

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
        var weights = destinationNeuron.GetWeights();
        //compute sensor values first
        List<Tuple<InnerNeuron, InnerNeuron>> postInnerConnections = new List<Tuple<InnerNeuron, InnerNeuron>>();
        foreach (Tuple<int, float> weight in weights)
        {
            Neuron originNeuron;
            Neurons.TryGetValue(weight.Item1, out originNeuron);
 
            if (originNeuron is InnerNeuron)
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
        var activatedValue = (float)Math.Tanh(sum+((Destination)n).GetBias());
        destinationNeuron.SetActivatedValue(activatedValue);
        //Debug.Log("Weights : " + destinationNeuron.GetWeights().ToString());

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
    
    #endregion

}
