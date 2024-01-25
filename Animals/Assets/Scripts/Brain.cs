using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
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
        Neuron.totalNeuronsAvailable = 0;
        Neuron.lastInputNeuron = 0;
        ActionNeuron.actionNeuronCount = 0;
        SensorNeuron.sensorNeuronCount = 0;
        var neuronsToAdd = new List<Neuron>()
        {
            new WaterLevelSensor(parent),
            new FoodLevelSensor(parent),
            new WaterDistanceX(parent),
            new WaterDistanceZ(parent),
            new FoodDistanceX(parent),
            new FoodDistanceZ(parent),
            new MoveUp(parent),
            new MoveDown(parent),
            new MoveLeft(parent),
            new MoveRight(parent),
        };
        AllNeurons = new Dictionary<int, Neuron>();
        foreach (var n in neuronsToAdd)
        {
            AllNeurons.Add(n.GetId(), n);
        }

        Neurons = new Dictionary<int, Neuron>();
        SensorConnections = new List<string>();
        InnerConnections = new List<string>();
        usedActors = new HashSet<int>();
        
    }



    #region Building Brain
    public void CreateNewConnections(int numSensorNeurons, int numInternalNeurons)
    {
        HashSet<int> usedInternals = new HashSet<int>();
        System.Random rand;

        //generate sensors
        HashSet<int>  usedSensors = new HashSet<int>(Enumerable.Range(1, Neuron.lastInputNeuron));

        //generate actors TODO rand range going to like 18
        HashSet<int>  usedActors = new HashSet<int>(Enumerable.Range(Neuron.lastInputNeuron + 1, Neuron.totalNeuronsAvailable - Neuron.lastInputNeuron));
        Sensors = usedSensors.ToArray();
        Actors = usedActors.ToArray();
        //generate inner neurons
        int innerNeuronCount = -1;
        for (int j  = 0; j< engine.numberOfInnerLayers; j++)
        {
            for (int i = 0; i < numInternalNeurons; i++)
            {
                Neuron innerNeuron = new InnerNeuron(Parent, innerNeuronCount);
                AllNeurons.Add(innerNeuron.GetId(), innerNeuron);
                usedInternals.Add(innerNeuron.GetId());
                innerNeuronCount--;
            }
        }
        Inners = usedInternals.ToArray();
        //store all used neurons
        foreach (var Id in usedSensors.Union(usedActors.Union(usedInternals)))
        {
            Neuron n = null;
            AllNeurons.TryGetValue(Id, out n);
            Neurons.Add(Id, n);
        }
        var innerActors = usedInternals.Union(usedActors);

        foreach (var Id in innerActors)
        {
            Neuron n = null;
            AllNeurons.TryGetValue(Id, out n);
            try
            {
                ((Destination)n).SetBias(UnityEngine.Random.Range(-1f, 1f));
            }
            catch (Exception e)
            {
                Debug.Log("Neuron " + Id + " is not a destination neuron");
            }
            
        }

        //create sensor connections
        foreach (int id in usedSensors)
        {
            // Select a random amount of elements from the usedInternals set
            int numElements = UnityEngine.Random.Range(Neuron.lastInputNeuron, innerActors.Count()+1);
            HashSet<int> selectedElements = new HashSet<int>(innerActors.OrderBy(x => UnityEngine.Random.value).Take(numElements));
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
                var weight = UnityEngine.Random.Range(-engine.weightLimit, engine.weightLimit);
                weight = weight == 0f ? 0.01f : weight;
                string connection = CreateConnection(id, id2, weight);
                InnerConnections.Add(connection);
            }
        }

        //store neuron iDs
        
        Inners = usedInternals.ToArray();

    }
    public void MergeConnections(Brain brain1, Brain brain2, int mutationPercentage)
    {
        System.Random rand = new System.Random();
        //generate sensors
        HashSet<int> usedSensors = new HashSet<int>(Enumerable.Range(1, Neuron.lastInputNeuron));

        //generate actors TODO rand range going to like 18
        HashSet<int> usedActors = new HashSet<int>(Enumerable.Range(Neuron.lastInputNeuron + 1, Neuron.totalNeuronsAvailable - Neuron.lastInputNeuron));
        Sensors = usedSensors.ToArray();
        Actors = usedActors.ToArray();
        Inners = new int[0];
        var InAct = brain1.AllNeurons.Where(x => x.Value is Destination).ToList();
        var InnersActorsKeys = InAct.Select(x => x.Key).ToList();
        foreach (var Id in usedSensors.Union(usedActors))
        {
            Neuron n = null;
            AllNeurons.TryGetValue(Id, out n);
            Neurons.Add(Id, n);
        }
        foreach (var n in InnersActorsKeys)
        {
            //try mutation
            var mutate = UnityEngine.Random.Range(0f, 100f);
            if (mutate <= mutationPercentage)
            {
                var newWeight = UnityEngine.Random.Range(-4f, 4f);
                var bias = UnityEngine.Random.Range(-1f, 1f);
                try
                {
                    if (Actors.Contains(n))
                    {
                        var index = brain1.Inners[UnityEngine.Random.Range(0, Inners.Length - 1)];
                        AddToMerge(index, n, newWeight, brain1);
                    }
                    else if (Inners.Contains(n))
                    {
                        var index = Sensors[UnityEngine.Random.Range(0, Inners.Length - 1)];
                        AddToMerge(index, n, newWeight, brain1);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    Application.Quit();
                }

                this.mutated = true;
                continue;
            }
            Destination n1 = null;
            Destination n2 = null;
            if (brain1.Neurons.ContainsKey(n))
            {
                Neuron t;
                brain1.Neurons.TryGetValue(n, out t);
                n1 = (Destination)t;
            }
            if (brain2.Neurons.ContainsKey(n))
            {
                Neuron t;
                brain2.Neurons.TryGetValue(n, out t);
                n2 = (Destination)t;

            }
            if (n1 == null && n2 == null)
            {
                continue;
            }
            //TODO mutate
            if (n1 != null && n2 != null)
            {
                //swap biases
                if (rand.Next(2) == 0)
                {
                    var b2 = n2.GetBias();
                    n1.SetBias((n1.GetBias() + b2) / 2f);
                }

                var kv1 = n1.GetWeights();
                var kv2 = n2.GetWeights();

                var keys1 = kv1.Select(x => x.Item1).ToList();
                var keys2 = kv2.Select(x => x.Item1).ToList();

                //in 2 not in 1
                var dif2from1 = keys1.Except(keys2).ToList();
                //in 1 not in 2
                var dif1from2 = keys2.Except(keys1).ToList();
                //in both
                var inBoth = keys1.Intersect(keys2).ToList();

                var destAsNeuron = (Neuron)n1;

                foreach (var key in dif2from1)
                {
                    if (rand.Next(2) == 0)
                    {
                        var newWeight = kv1.Where(x => x.Item1 == key).Select(x => x.Item2).First();

                        AddToMerge(key, destAsNeuron.GetId(), newWeight, brain1);

                    }
                }
                foreach (var key in dif1from2)
                {
                    if (rand.Next(2) == 0)
                    {
                        var newWeight = kv2.Where(x => x.Item1 == key).Select(x => x.Item2).First();
                        AddToMerge(key, destAsNeuron.GetId(), newWeight, brain1);
                    }
                }
                foreach (var key in inBoth)
                {
                    if (rand.Next(2) == 0)
                    {
                        var w1 = kv1.Where(x => x.Item1 == key).Select(x => x.Item2).First();
                        var w2 = kv2.Where(x => x.Item1 == key).Select(x => x.Item2).First();
                        var newWeight = (w1 + w2) / 2f;
                        AddToMerge(key, destAsNeuron.GetId(), newWeight, brain1);
                    }
                }
            }
            else if (n1 != null && n2 == null)
            {
                //maybe take other brain's neuron
                if (rand.Next(2) == 0)
                {
                    this.AddNeuron((Neuron)n1);
                }
            }
            else
            {
                if (rand.Next(2) == 0)
                {
                    this.AddNeuron((Neuron)n2);
                }
            }
        }
        
    }
    private void AddToMerge(int s,int d, float w, Brain sourceBrain)
    {
        Neuron source = null;
        AllNeurons.TryGetValue(s, out source);
        Neuron dest = null;
        AllNeurons.TryGetValue(d, out dest);
        if (source == null)
        {
            var newInner = new InnerNeuron(Parent, s);
            AllNeurons.Add(s, newInner);
            Inners = Inners.Append(s).ToArray();
            Neurons.Add(s, newInner);
        }
        else if (dest == null)
        {
            var newInner = new InnerNeuron(Parent, d);
            AllNeurons.Add(d, newInner);
            Inners = Inners.Append(d).ToArray();
            Neurons.Add(d, newInner);
        }
        string newConn = this.CreateConnection(s, d, w);
        if (Sensors.Contains(s))
        {
            SensorConnections.Add(newConn);
        }
        else
        {
            InnerConnections.Add(newConn);
        }
    }


    public string CreateConnection(int sourceId, int destinationId, float weight)
    {
        Neuron source;
        Neurons.TryGetValue(sourceId, out source);
        Neuron destination;
        Neurons.TryGetValue(destinationId, out destination);

        if (Actors.Contains(destinationId))
        {
            ((ActionNeuron)destination).AddWeight(sourceId, weight);
        }
        else if (Inners.Contains(destinationId))
        {
            ((InnerNeuron)destination).AddWeight(sourceId, weight);
        }
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
        if (n == null)
        {
            Debug.Log("hi");
        }
        if (Neurons.ContainsKey(n.GetId()))
        {
            Debug.Log("Brain alread contains neuron: " + n.GetId());
            return false;
        }
        if (n is (InnerNeuron))
        {
            foreach(var c in ((InnerNeuron)n).GetWeights())
            {
                this.CreateConnection(c.Item1, n.GetId(), c.Item2);
            }
            Inners.Append(n.GetId());
        }
        else
        {
            foreach (var c in ((ActionNeuron)n).GetWeights())
            {
                this.CreateConnection(c.Item1, n.GetId(), c.Item2);
            }
            Actors.Append(n.GetId());
        }
        Neurons.Add(n.GetId(), n);
        
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
        SetSensorValues();
        Propagate();

        //get action neurons
        List<Neuron> actionNeurons = new List<Neuron>();
        List<float> vals = new List<float>();
        foreach (var a in Actors)
        {
            actionNeurons.Add(Neurons[a]);
            var val = ((ActionNeuron)Neurons[a]).GetActivatedValue();
            vals.Add(val);
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
