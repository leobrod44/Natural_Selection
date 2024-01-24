using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using static System.Collections.Specialized.BitVector32;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

public class Generation : MonoBehaviour
{
    // Start is called before the first frame update

    
    public static int generation = 1;
    public static int highScore;
    public int deadline;
    public int mutationPercentage;
    private Engine engine;

    [SerializeField]
    public List<GameObject> currentPopulation;
    public List<GameObject> originalPopulation;
    public List<GameObject> generationPopulation;
    public List<GameObject> survivors;
    public static int currentPopulationSize;
    private float time;
    private bool timeLimitFlag=true;
    private bool survivorListSet;
    private int itteration;
    // brain constructors to give neural net
    // brain function to breen characters
    // fitness
    // selection process
    // speed up and slow timescale

    void Start()
    {
        engine = GameObject.Find("Engine").GetComponent<Engine>();
        survivors = new List<GameObject>();
        originalPopulation = new List<GameObject>();
        generationPopulation = new List<GameObject>();
        itteration = 1;
        GenerateFirstPopulation();
        generationPopulation = currentPopulation;
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.time > time + deadline || currentPopulationSize<=0)
        {
            StartCoroutine(WaitForAllInactiveCoroutine(currentPopulation));
            //var t = AverageLifespan(currentPopulation);
            //Debug.Log("Action supposed: " + Engine.actionSupposed + " done: " + Engine.actionDone);
            //((int)((float)percentage / 100 * (float)engine.animalCount));
            //TODO select top from all isntead of each batch
            
            if (itteration == engine.samples)
            {
                var avg = AverageLifespan(generationPopulation);
                survivors = new List<GameObject>();
                var top = SelectTop(engine.selectionCount, generationPopulation);
                survivors.AddRange(top); 
                currentPopulation= new List<GameObject>();
                generationPopulation = new List<GameObject>();
                GenerateNewPopulation(survivors);
                generationPopulation.AddRange(currentPopulation);
                var x = survivors.Concat(currentPopulation);
                Clean(x.ToList());
                generation++;             
                highScore = avg;
                Debug.Log("Generation: " + generation + " average lifespan: " + avg);

                //Debug.Log("EAT supposed: " + Engine.eatCountsupposed + " done: " + Engine.eatCountDone + " DRINK supposed: " + Engine.drinkCountsupposed + " done: " + Engine.drinkCountDone);
                
                originalPopulation = currentPopulation;
                itteration = 1;
                
            }
            else
            {
                itteration++;
                generationPopulation.AddRange(currentPopulation);

                if (originalPopulation.Count() == 0)
                {
                    GenerateFirstPopulation();
                    ResetMap();
                }
                else
                {
                    //original?
                    GenerateNewPopulation(survivors);
                }

            }
        }
       
        //TODO 
        //fix animals getting out of bounds, i think the error of currenttile messes them em try catch
        //make names face camera: probably rotation*-1
        //add layer, optimize network
        //add UI, make name breeding by type, 
        //add mutations, 
        //fix selection function to chose top
        //balance time to survive,
        //add breeding, 
        //make spawn in general location but not same spot,
        //make it such that survivors not complete is filled with random, 
        //make food spawn back
        //cinemachine
        //add varying colors say fall, winter, spring 

    }
    private int AverageLifespan(List<GameObject> population)
    {
        try
        {
            var total = 0;
            foreach (var animal in population)
            {
                total += animal.GetComponent<Animal>().age;
            }
            return total / population.Count;
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            Application.Quit();
            return 0;
        }
    
    }

    private List<GameObject> SelectTop(int number, List<GameObject> population)
    {
        //TODO change selection according to fitness
        int numAnimals = number;

        foreach(var animal in population)
        {
            var a = animal.GetComponent<Animal>();
            animal.GetComponent<Animal>().fitness = a.age* engine.ageFactor+a.foodCount*engine.foodFactor+a.waterCount* engine.waterFactor;
        }
        //int maxNum = percentage <= population.Count ? percentage: population.Count ;
        List<GameObject> ageOrdered = population.OrderBy(x => x.GetComponent<Animal>().fitness).Reverse().ToList();
        List<int> ages = ageOrdered.Select(x => x.GetComponent<Animal>().fitness).ToList();
        //Debug.Log("Selected average lifespan: " + (ages.Take(numAnimals).ToList().Sum() / ages.Take(numAnimals).ToList().Count));
        return ageOrdered.Take(numAnimals).ToList();
    }
    private List<GameObject> BreedAnimals(List<GameObject> survivorObjs)
    {

        System.Random rand = new System.Random();
        List<GameObject> newPopulation = new List<GameObject>();
        int count = 0;
        while (newPopulation.Count < engine.sampleSize)
        {
            if (count > 2000)
            {
                Application.Quit();
                break;
            }
            List<GameObject> survivorPool = new List<GameObject>(survivorObjs);
            if (survivorObjs.Count > 1)
            {

            }
            foreach (var survivor in survivorObjs)
            {
                if (newPopulation.Count >= engine.sampleSize)
                    break;

                GameObject other = survivor;
                while (other == survivor)
                     other = survivorPool[(rand.Next(survivorPool.Count()))];

                GameObject child = CreateOffSprings(survivor, other);
                newPopulation.Add(child);
                rand = new System.Random();
            }
            count++;
        }
        return newPopulation;
        

        
    }
    private GameObject CreateOffSprings(GameObject animal1, GameObject animal2)
    {
        Brain x;

        GameObject o = BreedBodies(animal1, animal2);
        x = o.GetComponent<Animal>().brain;
        Brain brains2 = animal2.GetComponent<Animal>().brain;
        Brain brains1 = animal1.GetComponent<Animal>().brain;
        o.GetComponent<Animal>().brain = Crossover(brains1, brains2);
        if (o.GetComponent<Animal>().brain.mutated)
        {
            o.GetComponent<Animal>().body.SetColor(o.GetComponent<Animal>().body.Torso, UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
            o.GetComponent<Animal>().body.SetColor(o.GetComponent<Animal>().body.Head, UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
            o.GetComponent<Animal>().brain.mutated = false;
        }
        o.GetComponent<Animal>().age = 0;
        o.GetComponent<Animal>().brain.nearestFood = Vector3.zero;
        o.GetComponent<Animal>().brain.nearestWater = Vector3.zero;
        o.GetComponent<Animal>().brain.lastFood = Vector3.zero;
        o.GetComponent<Animal>().brain.lastWater = Vector3.zero;

        var z2 =o.GetComponent<Animal>().body;

        return o;

    }
        private Brain Crossover(Brain brains1, Brain brains2)
        {
        System.Random rand = new System.Random();

        var InnersActors = brains1.AllNeurons.Where(x => x.Value is Destination).ToList();

        foreach (var n in InnersActors)
        {
            //try mutation
            var mutate = UnityEngine.Random.Range(0f, 100f);
            if (mutate <= mutationPercentage)
            {
                var newWeight = UnityEngine.Random.Range(-4f, 4f);
                var destAsNeuron = (Neuron)n.Value;
                ((Destination)destAsNeuron).SetBias(UnityEngine.Random.Range(-1f,1f));
                Neuron t;
                int index;
                try
                {
                    if (n.Value is ActionNeuron)
                    {
                        index = brains1.Inners[UnityEngine.Random.Range(0, brains1.Inners.Length - 1)];
                        
                        brains1.Neurons.TryGetValue(index, out t);
                        brains1.CreateConnection(t.Id, destAsNeuron.Id, newWeight);
                    }
                    else if (n.Value is InnerNeuron)
                    {
                        index = brains1.Sensors[UnityEngine.Random.Range(0, brains1.Sensors.Length - 1)];
                        brains1.Neurons.TryGetValue(index, out t);
                        brains1.CreateConnection(t.Id, destAsNeuron.Id, newWeight);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    Application.Quit();
                }
               
                brains1.mutated = true;
                continue;
            }
            Destination n1 = null;
            Destination n2 = null;
            if (brains1.Neurons.ContainsKey(n.Key))
            {
                Neuron t;
                brains1.Neurons.TryGetValue(n.Key, out t);
                n1 = (Destination)t;  
            }
            if (brains2.Neurons.ContainsKey(n.Key))
            {
                Neuron t;
                brains1.Neurons.TryGetValue(n.Key, out t);
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
                        brains1.CreateConnection(key, destAsNeuron.Id, newWeight);
                    }
                }
                foreach (var key in dif1from2)
                {
                    if (rand.Next(2) == 0)
                    {
                        brains1.RemoveConnection(key, destAsNeuron.Id);
                    }
                }
                foreach (var key in inBoth)
                {
                    if (rand.Next(2) == 0)
                    {
                        var w1 = kv1.Where(x => x.Item1 == key).Select(x => x.Item2).First();
                        var w2 = kv2.Where(x => x.Item1 == key).Select(x => x.Item2).First();
                        var newWeight = (w1 + w2) / 2f;
                        brains1.RemoveConnection(key, destAsNeuron.Id);
                        brains1.CreateConnection(key, destAsNeuron.Id, newWeight);
                    }
                }

            }
            else if (n1 != null && n2 == null)
            {
                //maybe take other brain's neuron
                if (rand.Next(2) == 0)
                {
                    brains1.AddNeuron((Neuron)n2);
                }
            }
            else
            {
                //maybe remove
                if (rand.Next(2) == 0)
                {
                    Neuron destAsNeuron = (Neuron)n1;
                    brains1.RemoveNeuron(destAsNeuron.Id);
                }
            }
        }

        return brains1;
    }
 
    private GameObject BreedBodies(GameObject animal1, GameObject animal2)
    {

        Body body1 = animal1.GetComponent<Animal>().body;
        Body body2 = animal2.GetComponent<Animal>().body;
        Color primary1 = body1.primaryColor;
        Color primary2 = body2.primaryColor;
        Color secondary1 = body1.secondaryColor;
        Color secondary2 = body2.secondaryColor;
        float scale = Random.Range(0f, 1f);
        //var newPrimary1 = (primary1 * scale + primary2 * (1 - scale)) / 2f;
        //var newSecondary1 = (secondary1 * scale + secondary2 * (1 - scale)) / 2f;
        var newPrimary1 = new Color(Random.Range(primary1.r, primary2.r), Random.Range(primary1.g, primary2.g), Random.Range(primary1.b, primary2.b), Random.Range(primary1.a, primary2.a));
        var newSecondary1 = new Color(Random.Range(secondary1.r, secondary2.r), Random.Range(secondary1.g, secondary2.g), Random.Range(secondary1.b, secondary2.b), Random.Range(secondary1.a, secondary2.a));
        string name = body1.skeleton.name.Split(' ')[0] + " " + body2.skeleton.name.Split(' ')[1];
        GameObject skeleton = Instantiate(body1.skeleton);
        GameObject newAnimal = CreateAnimal(
            skeleton,
            (body1.bodySize + body2.bodySize) / 2f,
            (body1.eyeSize + body2.eyeSize) / 2f,
            (body1.legSize + body2.legSize) / 2f,
            newPrimary1,
            newSecondary1,
            name,
            false
        );
            
        return newAnimal;
       
    }

    public void GenerateFirstPopulation()
    {
        //Clean();

        engine = GameObject.Find("Engine").GetComponent<Engine>();
        currentPopulation = new List<GameObject>();
        //Clean();
        for (int i = 0; i < engine.sampleSize; i++)
        {
            var bodySize = Random.Range(0.2f, 1);
            var eyeSize = Random.Range(0.1f, 0.5f);
            var legSize = Random.Range(0.2f, 0.8f) ;
            var primaryColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            var secondaryColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            var newAnimal = CreateAnimal(engine.availableSkeletons[0], bodySize, eyeSize, legSize, primaryColor, secondaryColor, GeneEncoding.GenerateLatinName(),true);
            
            currentPopulation.Add(newAnimal);
        }
        currentPopulationSize = currentPopulation.Count;

        time = Time.time;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Clean(List<GameObject> except)
    {
        List<Animal> activeAndInactive = GameObject.FindObjectsOfType<Animal>(true).ToList();
        foreach (var animal in activeAndInactive)
        {

            if (except != null)
            {
                if (except.Contains(animal.gameObject)) //!originalPopulation.Contains(previous) && 
                {
                    continue;
                }
            }
            animal.gameObject.SetActive(true);
            DestroyImmediate(animal.gameObject);
        }
        List<Animal> activeAndInactive2 = GameObject.FindObjectsOfType<Animal>(true).ToList();
    }
    public void ResetMap()
    {
        foreach (var area in engine.foodAreas)
        {
            area.Element.SetActive(true);
            area.Element.transform.parent.gameObject.GetComponent<Renderer>().material.color = Color.green;
        }
        //Clean();
        
    }

    public void GenerateNewPopulation(List<GameObject> pop)
    {
        //Clean();
        ResetMap();

        currentPopulation = new List<GameObject>();
        
        List<GameObject> newPopulation = BreedAnimals(pop);
        
        foreach (var animal in newPopulation)
        {
            animal.SetActive(true);
            currentPopulation.Add(animal);
        }
        currentPopulationSize = currentPopulation.Count;
        
        time = Time.time;
    }


    public GameObject CreateAnimal(GameObject skeleton, float bodySize, float eyeSize, float legSize, Color primary, Color secondary, string name, bool display)
    {
        GameObject bodySkeleton = Instantiate(skeleton);
        Body newAnimalBody = new Body(bodySkeleton, bodySize, eyeSize, legSize, primary, secondary,name);
        GameObject newAnimal = SpawnAnimal(bodySkeleton);
        newAnimal.GetComponent<Animal>().body = newAnimalBody;
        if (display)
            bodySkeleton.GetComponent<Animal>().body.DisplayName(bodySkeleton);
        return newAnimal;
    }
    public GameObject SpawnAnimal(GameObject skeleton)
    {
        //var rand = (int)Random.Range(0, engine.grassAreas.Count);
        //var area = engine.grassAreas.ElementAt(rand);
        //skeleton.transform.position = area.Tile.transform.position;
        var x = (int)Random.Range(engine.mapSize/2-80, engine.mapSize/2+80);
        var y = (int)Random.Range(engine.mapSize/2 - 80, engine.mapSize/2 + 80);
        skeleton.transform.position = new Vector3(x, 0, y);
        return skeleton;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    IEnumerator  WaitForAllInactiveCoroutine(List<GameObject> elementsToCheck)
    {
        // Wait until all elements are inactive
        while (!AreAllElementsInactive(elementsToCheck))
        {
            yield return null; // Wait for the next frame
        }

        // All elements are inactive, continue with your logic here
        Debug.Log("All elements are inactive. Continuing...");


        // Add your code to execute after all elements are inactive
    }

    bool AreAllElementsInactive(List<GameObject> elementsToCheck)
    {
        // Check if all elements are inactive
        foreach (GameObject element in elementsToCheck)
        {
            if(element==null)
            {
                continue;
            }
            if (element.activeSelf)
            {
                return false; // At least one element is still active
            }
        }

        return true; // All elements are inactive
    }
}

//private Tuple<Brain, Brain> BreedBrainDep(GameObject animal1, GameObject animal2)
//{
//    Brain brain1 = animal1.GetComponent<Animal>().brain;
//    Brain brain2 = animal2.GetComponent<Animal>().brain;
//    List<string> sensorConnections1 = brain1.SensorConnections;
//    List<string> sensorConnections2 = brain2.SensorConnections;
//    List<string> innerConnections1 = brain1.InnerConnections;
//    List<string> innerConnections2 = brain2.InnerConnections;

//    var bredSensorSections = BreedSections(sensorConnections1, sensorConnections2);
//    brain1.SensorConnections = bredSensorSections.Item1;
//    brain2.SensorConnections = bredSensorSections.Item2;

//    var bredInnerSections = BreedSections(innerConnections1, innerConnections2);
//    brain1.InnerConnections = bredInnerSections.Item1;
//    brain2.InnerConnections = bredInnerSections.Item2;


//    return new Tuple<Brain, Brain>(brain1, brain2);
//}


//private Tuple<List<string>,List<string>> BreedSections(List<string> section1, List<string> section2)
//{
//    var length = Math.Max(section1.Count, section2.Count);
//    List<string> newConnections1 = new List<string>();
//    List<string> newConnections2 = new List<string>();
//    for (int i = 0; i < length; i++)
//    {
//        var bredGenes = BreedGenes(section1[i], section2[i]);
//        newConnections1.Add(bredGenes.Item1);
//        newConnections2.Add(bredGenes.Item2);
//    }
//    return new Tuple<List<string>, List<string>>(newConnections1, newConnections2);
//}
//private Tuple<string,string> BreedGenes(string connection1, string connection2)
//{
//    System.Random rand = new System.Random();

//    string binary1 = GeneEncoding.HexToBin(connection1);
//    string binary2 = GeneEncoding.HexToBin(connection2);

//    var val = rand.Next(0, 3);
//    if (val == 0)
//    {
//        var temp = binary1.Substring(0, 8);
//        var temp2 = binary2.Substring(0, 8);
//        binary1 = temp2 + binary1.Substring(8, binary1.Length-8);
//        binary2 = temp + binary2.Substring(8, binary2.Length-8);
//    }

//    rand = new System.Random();
//    val = rand.Next(0, 3);
//    if (val == 0)
//    {
//        var temp = binary1.Substring(8, 8);
//        var temp2 = binary2.Substring(8, 8);
//        var x = binary1.Length;
//        binary1 = binary1.Substring(0, 8) + temp2 + binary1.Substring(16, 16);
//        binary2 = binary2.Substring(0, 8) + temp + binary2.Substring(16, 16);
//    }

//    rand = new System.Random();
//    val = rand.Next(0, 3);
//    if (val == 0)
//    {
//        var x = binary1.Length;
//        float val1 = GeneEncoding.BinaryToFloat(binary1.Substring(16, 16));
//        float val2 = GeneEncoding.BinaryToFloat(binary2.Substring(16, 16));
//        float mean = (val1 + val2) / 2f;

//        binary1 = binary1.Substring(0, 17) + GeneEncoding.FloatToBinary(mean);
//        binary2 = binary2.Substring(0, 17) + GeneEncoding.FloatToBinary(mean);
//    }
//    //1/3 chance of swapping source, destination, meaning weight

//    return new Tuple<string, string>(GeneEncoding.BinToHex(binary1), GeneEncoding.BinToHex(binary2));
//}

//private Tuple<string, string> Mutate(string connection1, string connection2)
//{
//    string binary1 = GeneEncoding.HexToBin(connection1);
//    string binary2 = GeneEncoding.HexToBin(connection2);
//    var x = binary1.Length;
//    var r = new System.Random();

//    binary1 = binary1.Substring(0, 17) + GeneEncoding.FloatToBinary(UnityEngine.Random.Range(-4f, 4f));
//    binary2 = binary2.Substring(0, 17) + GeneEncoding.FloatToBinary(UnityEngine.Random.Range(-4f, 4f));
//    return new Tuple<string, string>(GeneEncoding.BinToHex(binary1), GeneEncoding.BinToHex(binary2));
//}
//private Tuple<string, string> MeanWeights(string connection1, string connection2)
//{
//    string binary1 = GeneEncoding.HexToBin(connection1);
//    string binary2 = GeneEncoding.HexToBin(connection2);
//    var x = binary1.Length;

//    float val1 = GeneEncoding.BinaryToFloat(binary1.Substring(16, 16));
//    float val2 = GeneEncoding.BinaryToFloat(binary2.Substring(16, 16));

//    float mean = (val1 + val2) / 2f;
//    binary1 = binary1.Substring(0, 17) + GeneEncoding.FloatToBinary(mean);

//    binary2 = binary2.Substring(0, 17) + GeneEncoding.FloatToBinary(mean);

//    return new Tuple<string, string>(GeneEncoding.BinToHex(binary1), GeneEncoding.BinToHex(binary2));

//}