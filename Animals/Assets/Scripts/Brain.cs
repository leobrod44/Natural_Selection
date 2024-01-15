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

    public Vector3 nearestWater;

    public Vector3 nearestFood;

    public Vector3 nearestAlly;

    private Vector3 nearestEnemy;

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
            //{Ally distance
            //{Enemy distance
            {7, new TargetWater(parent)},
            {8, new TargetFood(parent)},
            {9, new RotateRandomAction(parent)},
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
        HashSet<int> usedSensors = new HashSet<int>();
        HashSet<int> availableActors = new HashSet<int>();
        HashSet<int> usedInternals = new HashSet<int>();
        System.Random rand;

        //generate sensors
        rand = new System.Random();
        usedSensors = new HashSet<int>(Enumerable.Range(1, Neuron.lastInputNeuron));

        //generate actors TODO rand range going to like 18
        rand = new System.Random();
        availableActors = new HashSet<int>(Enumerable.Range(Neuron.lastInputNeuron + 1, Neuron.totalNeuronsAvailable - Neuron.lastInputNeuron));

        //generate inner neurons
        int innerNeuronCount = -1;
        for (int i = 0; i < numInternalNeurons; i++)
        {
            Neuron innerNeuron = new InnerNeuron(Parent, innerNeuronCount);
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
            foreach(int id2 in usedInternals)
            {
                var weight = UnityEngine.Random.Range(-4f, 4f);
                //weird edge case where weight is 0 although float
                weight = weight == 0f ? 0.01f : weight;
                string connection = CreateConnection(id, id2, weight);
                SensorConnections.Add(connection);
            }
        }
        //create inner neuron connections
        foreach (int id in usedInternals)
        {
            foreach(int id2 in availableActors)
            {
                var weight = UnityEngine.Random.Range(-4f, 4f);
                //weird edge case where weight is 0 although float
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
        public void InitializeBrainDep(int numSensorNeurons, int numInternalNeurons)
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
            //weird edge case where weight is 0 although float
            weight = weight == 0f ? 0.01f: weight;
            string connection = CreateConnection(id, destination, weight);
            SensorConnections.Add(connection);
        }
        //create inner neuron connections
        foreach (int id in usedInternals)
        {
            int randNeuron = UnityEngine.Random.Range(0, unionedActorInner.Count);
            var destination = unionedActorInner.ElementAt(randNeuron);
            var weight = UnityEngine.Random.Range(-4f, 4f);
            //weird edge case where weight is 0 although float
            weight = weight == 0f ? 0.01f : weight;
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
        string sourceBin = GeneEncoding.IdToBinary(sourceId);
        string destBin = GeneEncoding.IdToBinary(destinationId);
        string sourceBit = Neurons[sourceId] is SensorNeuron ? "1" : "0";
        string destBit = Neurons[destinationId] is ActionNeuron ? "1" : "0";
        string weightBin = GeneEncoding.WeightToBinary(weight);
        string connection32Bit = sourceBit + sourceBin + destBit + destBin + weightBin;
        var c = weightBin.Length;
        var x = connection32Bit.Length;
        string connectionHex = GeneEncoding.BinToHex(connection32Bit);
        return connectionHex;
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
