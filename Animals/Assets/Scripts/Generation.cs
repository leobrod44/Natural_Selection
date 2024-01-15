using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using static System.Collections.Specialized.BitVector32;

public class Generation : MonoBehaviour
{
    // Start is called before the first frame update

    public int selectionPercentage;
    public int deadline;
    public int mutationPercentage;
    private Engine engine;
    [SerializeField]
    public List<GameObject> currentPopulation;
    public static int currentPopulationSize;
    private List<GameObject> survivors;
    private float time;
    private bool timeLimitFlag=true;
    private bool survivorListSet;
    // brain constructors to give neural net
    // brain function to breen characters
    // fitness
    // selection process
    // speed up and slow timescale

    void Start()
    {
        engine = GameObject.Find("Engine").GetComponent<Engine>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > time + deadline || currentPopulationSize<=0)
        {
            try
            {
                //survivors = currentPopulation.Where(x => x.activeInHierarchy).ToList();
                float t = Time.time - time;
                GenerateNewPopulation(currentPopulation);
                time = Time.time;
                Debug.Log(" age: " + t);
            }
            catch (Exception e)
            {
                Debug.Log(e);
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

    private List<GameObject> SelectTop(List<GameObject> population)
    {
        //TODO change selection according to fitness
        int numAnimals = ((int)((float)selectionPercentage / 100 * (float)engine.animalCount));
        //int maxNum = percentage <= population.Count ? percentage: population.Count ;
        List<GameObject> ageOrdered = population.OrderBy(x => x.GetComponent<Animal>().age).ToList();
        return ageOrdered.Take(numAnimals).ToList();
    }
    //TODO how to breed? how to return, bred all genes between both? also fill rest of animals
    //maybe while !full keep foreaching
    private List<GameObject> BreedAnimals(List<GameObject> survivorObjs)
    {
        System.Random rand = new System.Random();
        List<GameObject> newPopulation = new List<GameObject>();

        while (newPopulation.Count < engine.animalCount)
        {
            List<GameObject> survivorPool = new List<GameObject>(survivorObjs);
            //if(newPopulation.Count+survivorObjs.Count > engine.animalCount)
            //{
            //    survivorObjs = survivorObjs.Take(engine.animalCount - newPopulation.Count).ToList();
            //}
            if (survivorObjs.Count > 1)
            {
                foreach (var survivor in survivorObjs)
                {
                    if (survivorPool.Count <= 1 || newPopulation.Count >= engine.animalCount)
                        break;
                    GameObject other = survivor;
                    while (other == survivor)
                        other = survivorPool[(rand.Next(survivorPool.Count()))];
                    CreateOffSprings(survivor, other);
                    newPopulation.Add(survivor);
                    newPopulation.Add(other);
                    rand = new System.Random();
                }
            }
            else
            {
                GameObject survivor = survivorObjs[0];
                GameObject other = survivorObjs[0];
                CreateOffSprings(survivor, other);
                newPopulation.Add(survivor);
                newPopulation.Add(other);
            }
            
        }
        return newPopulation;
    }
    private void CreateOffSprings(GameObject animal1, GameObject animal2)
    {
        BreedBrain(animal1, animal2);
        BreedBodies(animal1, animal2);
    }
    private Tuple<Brain,Brain> BreedBrain(GameObject animal1, GameObject animal2)
    {
        System.Random rand = new System.Random();

        Brain brain1 = animal1.GetComponent<Animal>().brain;
        Brain brain2 = animal2.GetComponent<Animal>().brain;

        List<string> sensorConnections1 = brain1.SensorConnections;
        List<string> sensorConnections2 = brain2.SensorConnections;
        

        for (int i = 0; i < sensorConnections1.Count; i++)
        {
            int r = rand.Next(3);
            if (r == 0)
            {
                string temp = sensorConnections1[i];
                sensorConnections1[i] = sensorConnections2[i];
                sensorConnections2[i] = temp;
            }
            else if (r == 1)
            {
                var meanedWeights = MeanWeights(sensorConnections1[i], sensorConnections2[i]);
                sensorConnections1[i] = meanedWeights.Item1;
                sensorConnections2[i] = meanedWeights.Item2;
            }
        }

        List<string> innerConnections1 = brain1.InnerConnections;
        List<string> innerConnections2 = brain2.InnerConnections;

        for (int i = 0; i < innerConnections1.Count; i++)
        {
            int r = rand.Next(3);
            if (r == 0)
            {
                string temp = innerConnections1[i];
                innerConnections1[i] = innerConnections2[i];
                innerConnections2[i] = temp;
            }
            else if (r == 1)
            {

            }
        }

        return new Tuple<Brain, Brain>(brain1, brain2);
    }
    private Tuple<Brain, Brain> BreedBrainDep(GameObject animal1, GameObject animal2)
    {
        Brain brain1 = animal1.GetComponent<Animal>().brain;
        Brain brain2 = animal2.GetComponent<Animal>().brain;
        List<string> sensorConnections1 = brain1.SensorConnections;
        List<string> sensorConnections2 = brain2.SensorConnections;
        List<string> innerConnections1 = brain1.InnerConnections;
        List<string> innerConnections2 = brain2.InnerConnections;

        var bredSensorSections = BreedSections(sensorConnections1, sensorConnections2);
        brain1.SensorConnections = bredSensorSections.Item1;
        brain2.SensorConnections = bredSensorSections.Item2;

        var bredInnerSections = BreedSections(innerConnections1, innerConnections2);
        brain1.InnerConnections = bredInnerSections.Item1;
        brain2.InnerConnections = bredInnerSections.Item2;


        return new Tuple<Brain, Brain>(brain1, brain2);
    }

    private void BreedBodies(GameObject animal1, GameObject animal2)
    {
        Body body1 = animal1.GetComponent<Animal>().body;
        Body body2 = animal2.GetComponent<Animal>().body;
        Color primary1 = body1.primaryColor;
        Color primary2 = body2.primaryColor;
        Color secondary1 = body1.secondaryColor;
        Color secondary2 = body2.secondaryColor;
        float scale = Random.Range(0f, 1f);
        var newPrimary1 = (primary1 * scale + primary2 *(1-scale)) /2f;
        var newSecondary1 = (secondary1 * scale + secondary2 * (1 - scale)) / 2f;
        scale = Random.Range(0f, 1f);
        var newPrimary2 = (primary1 * scale + primary2 * (1 - scale)) / 2f;
        var newSecondary2 = (secondary1 * scale + secondary2 * (1 - scale)) / 2f;
        body1.primaryColor = newPrimary1;
        body1.secondaryColor = newSecondary1;
        body2.primaryColor = newPrimary2;
        body2.secondaryColor = newSecondary2;
        animal1.name = GeneEncoding.GenerateLatinName();

        //TODO breed size;
    }

    private Tuple<List<string>,List<string>> BreedSections(List<string> section1, List<string> section2)
    {
        var length = Math.Max(section1.Count, section2.Count);
        List<string> newConnections1 = new List<string>();
        List<string> newConnections2 = new List<string>();
        for (int i = 0; i < length; i++)
        {
            var bredGenes = BreedGenes(section1[i], section2[i]);
            newConnections1.Add(bredGenes.Item1);
            newConnections2.Add(bredGenes.Item2);
        }
        return new Tuple<List<string>, List<string>>(newConnections1, newConnections2);
    }
    private Tuple<string,string> BreedGenes(string connection1, string connection2)
    {
        System.Random rand = new System.Random();

        string binary1 = GeneEncoding.HexToBin(connection1);
        string binary2 = GeneEncoding.HexToBin(connection2);

        var val = rand.Next(0, 3);
        if (val == 0)
        {
            var temp = binary1.Substring(0, 8);
            var temp2 = binary2.Substring(0, 8);
            binary1 = temp2 + binary1.Substring(8, binary1.Length-8);
            binary2 = temp + binary2.Substring(8, binary2.Length-8);
        }

        rand = new System.Random();
        val = rand.Next(0, 3);
        if (val == 0)
        {
            var temp = binary1.Substring(8, 8);
            var temp2 = binary2.Substring(8, 8);
            var x = binary1.Length;
            binary1 = binary1.Substring(0, 8) + temp2 + binary1.Substring(16, 16);
            binary2 = binary2.Substring(0, 8) + temp + binary2.Substring(16, 16);
        }

        rand = new System.Random();
        val = rand.Next(0, 3);
        if (val == 0)
        {
            var x = binary1.Length;
            float val1 = GeneEncoding.BinaryToWeight(binary1.Substring(16, 16));
            float val2 = GeneEncoding.BinaryToWeight(binary2.Substring(16, 16));
            float mean = (val1 + val2) / 2f;
            binary1 = binary1.Substring(0, 17) + GeneEncoding.WeightToBinary(mean);
            binary2 = binary2.Substring(0, 17) + GeneEncoding.WeightToBinary(mean);
        }
        //1/3 chance of swapping source, destination, meaning weight

        return new Tuple<string, string>(GeneEncoding.BinToHex(binary1), GeneEncoding.BinToHex(binary2));
    }
    private Tuple<string, string> MeanWeights(string connection1, string connection2)
    {
        string binary1 = GeneEncoding.HexToBin(connection1);
        string binary2 = GeneEncoding.HexToBin(connection2);
        var x = binary1.Length;

        float val1 = GeneEncoding.BinaryToWeight(binary1.Substring(16, 16));
        float val2 = GeneEncoding.BinaryToWeight(binary2.Substring(16, 16));

        float mean = (val1 + val2) / 2f;

        binary1 = binary1.Substring(0, 17) + GeneEncoding.WeightToBinary(mean);

        binary2 = binary2.Substring(0, 17) + GeneEncoding.WeightToBinary(mean);

        return new Tuple<string, string>(GeneEncoding.BinToHex(binary1), GeneEncoding.BinToHex(binary2));

    }
    public void GenerateInitialPopulation()
    {
        //Clean();
        engine = GameObject.Find("Engine").GetComponent<Engine>();
        currentPopulation = new List<GameObject>();
        for (int i = 0; i < engine.animalCount; i++)
        {
            var bodySize = Random.Range(0.2f, 1);
            var eyeSize = Random.Range(0.1f, 0.5f);
            var legSize = Random.Range(0.2f, 0.8f);
            var primaryColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            var secondaryColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            var newAnimal = CreateAnimal(bodySize, eyeSize, legSize, primaryColor, secondaryColor);
            currentPopulation.Add(newAnimal);
        }
        currentPopulationSize = currentPopulation.Count;
    }
    public void Clean()
    {
        foreach (var animal in currentPopulation)
        {
            Destroy(animal);
        }
       // currentPopulation.Clear();
    }
    public void ResetMap()
    {
        foreach (var area in engine.foodAreas)
        {
            area.Element.SetActive(true);
            area.Element.transform.parent.gameObject.GetComponent<Renderer>().material.color = Color.green;
        }
        Clean();
        
    }

    public void GenerateNewPopulation(List<GameObject> pop)
    {
        //Clean();
        ResetMap();
        List<GameObject> topSurvivors = SelectTop(pop);
        List<GameObject> newPopulation = BreedAnimals(topSurvivors);
        
        List<GameObject> previousPop = new List<GameObject>(currentPopulation);
        currentPopulation = new List<GameObject>();
        foreach (var animal in newPopulation)
        {
            var b = animal.GetComponent<Animal>().body;
        }
        foreach (var previous in previousPop)
        {
            previous.SetActive(true);
            Destroy(previous);
        }
        foreach (var animal in newPopulation)
        {
            var b = animal.GetComponent<Animal>().body;
            CreateAnimal(animal);
        }
        currentPopulationSize = currentPopulation.Count;
        time = Time.time;
        survivorListSet = false;
    }

    public GameObject CreateAnimal(float bodySize, float eyeSize, float legSize, Color primary, Color secondary)
    {
        var x = Random.Range(0, Engine.MAPSIZE - 1);
        var y = Random.Range(0, Engine.MAPSIZE - 1);
        GameObject newAnimal = Instantiate(engine.availableSkeletons[0]);
        newAnimal.transform.position = new Vector3(x, 0.9f, y);
        Body newAnimalBody = new Body(newAnimal, bodySize, eyeSize, legSize, primary, secondary);
        newAnimal.GetComponent<Animal>().body = newAnimalBody;
        return newAnimal;
    }
    public void CreateAnimal(GameObject animal)
    {

        GameObject newAnimal = Instantiate(animal);
        var x = Random.Range(0, Engine.MAPSIZE - 1);
        var y = Random.Range(0, Engine.MAPSIZE - 1);
        newAnimal.transform.position = new Vector3(x, 0.5f, y);
        var angle = UnityEngine.Random.Range(0, 360f);
        var dir = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0, Mathf.Cos(Mathf.Deg2Rad * angle));
        transform.rotation = new Quaternion(dir.x, dir.y, dir.z, 180f);
        newAnimal.name = newAnimal.name.Replace("(Clone)", "");
        currentPopulation.Add(newAnimal);
        newAnimal.GetComponent<Animal>().enabled = true;
        newAnimal.GetComponent<Animal>().body = animal.GetComponent<Animal>().body;
        var b=  newAnimal.GetComponent<Animal>().body;

    }
}
