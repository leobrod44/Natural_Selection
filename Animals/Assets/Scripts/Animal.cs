using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Animal: MonoBehaviour
{
    delegate void Act();
    private Act PerformAction;
    delegate void Sens();
    private Act sens;

    //temp
    private float nextMovementTime;
    private float time;

    private GameObject Target;

    //states
    [SerializeField]
    private float speed;
    
    public int eyeSight;
    
    //private int lifeSpan;
    //private int maturity;

    
    private const int maxEnergy = 100;
    private const int maxWater = 100;
    private const int maxFood = 100;

    public float currentWater;
    public float currentFood;
    private bool canMove;
    private bool eating;
    private bool drinking;
    private bool outOfBounds;
    private Engine engine;

    private List<Interests> interests;

    [SerializeField]
    private Area currentArea;
    [SerializeField]
    private Area area;
    private float areaX;
    [SerializeField]
    private float areaY;
    [SerializeField]
    public Tuple<int, int> currentPosition;
    private int memoryCapacity; //List<GameObject>

    //stomachSize -> - speed same distance, 2x speed will use half time but 2x energy
    //lifeSpan -> - maturity
    //energy -> - stomachSize
    //eyesight -> memoryCapacity

    //maybe think in terms of attribut points, spending them on everything
    public List<SensorNeuron> sensors;

    public List<ActionNeuron> actions;

    public Brain brain;

    private Diet diet;

    private float decisionTimer;

    public float decisionRate = 1;


    void Awake()
    {
        brain = new Brain(gameObject);
        eating = false;
        drinking = false;
        engine = GameObject.Find("Engine").GetComponent<Engine>();
        PerformAction = DefaultMovement;
        canMove = true;
        currentArea = engine.sections[(int)transform.position.x, (int)transform.position.z];
        currentPosition = new Tuple<int, int>((int)transform.position.x, (int)transform.position.z);
        
        currentFood = maxFood;
        currentWater = maxWater;

        speed = 2;
        eyeSight = 4;
        //interests = new List<Interests>();
        //interests.Add(Interests.WaterF);
        //interests.Add(Interests.WaterS);
        //sensors = new List<SensorNeuron>();

        //sensors.Add(new WaterLevelSensor(gameObject));
        //sensors.Add(new WaterDistanceForwardSensor(gameObject));
        //sensors.Add(new WaterDistanceSidesSensor(gameObject));
        //sensors.Add(new FoodLevelSensor(gameObject));
        //sensors.Add(new FoodDistanceForwardSensor(gameObject));
        //sensors.Add(new FoodDistanceSidesSensor(gameObject));

        //actions = new List<ActionNeuron>();

        //actions.Add(new TurnAroundAction(gameObject));
        //actions.Add(new RotateQuarterLeftAction(gameObject));
        //actions.Add(new RotateQuarterRightAction(gameObject));
        //actions.Add(new RotateSlightLeftAction(gameObject));
        //actions.Add(new RotateSlightRightAction(gameObject));
        //actions.Add(new RotateRandomAction(gameObject));
        Debug.Log(brain.Neurons);
    }
    void Update()
    {
        //verify sensors, 
        CheckDeath();
        CheckTile();
        Scan();
        if (canMove)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            if (Time.time > decisionTimer + engine.animalDecisionRate && !outOfBounds)
            {
                ActionNeuron neuronToFire = brain.GetScenarioActionNeuron();
                neuronToFire.DoAction();
                decisionTimer = Time.time;
            }
        }
        
        Tick();
            
    }

    private void CheckDeath()
    {
        if (currentFood <= 0 || currentWater <= 0)
        {
            Destroy(gameObject);
        }
    }


    private void Scan()
    {
        int currentPosx = (int)transform.position.x;
        int currentPosy = (int)transform.position.z;
        if(currentPosx!=currentPosition.Item1 || currentPosy != currentPosition.Item2)
        {
            currentPosition = new Tuple<int, int>(currentPosx, currentPosy);
            TriggerScans(currentPosx, currentPosy);
        }
        
    }
    private void TriggerScans(int currentPosx,int currentPosy) //List<Interests> interests
    {
        //TODO need to generate bounds  for map
        //Use Physics.OverlapSphere (someposition, someradius);
        try
        {
            area = engine.sections[currentPosx, currentPosy];
        }
        catch
        {
            Debug.Log(currentPosx);
            Debug.Log(currentPosy);
        }
        //refreshes tiles once moved
        if (area.Tile != currentArea.Tile)
        {
            currentArea.currentAnimals.Remove(gameObject);
            currentArea = area;
            currentArea.currentAnimals.Add(gameObject);
        }

    }

    private void CheckTile()
    {
        int currentPosx = (int)transform.position.x;
        int currentPosy = (int)transform.position.z;
        if (currentPosx != currentPosition.Item1 || currentPosy != currentPosition.Item2)
        {
            currentPosition = new Tuple<int, int>(currentPosx, currentPosy);
            try
            {
                area = engine.sections[currentPosx, currentPosy];
            }
            catch
            {
                Debug.Log(currentPosx);
                Debug.Log(currentPosy);
            }
        }
        //refreshes tiles once moved
        if (area.Tile != currentArea.Tile)
        {
            currentArea.currentAnimals.Remove(gameObject);
            currentArea = area;
            currentArea.currentAnimals.Add(gameObject);
        }
        if (currentArea.Type == AreaType.Food)
        {
            Eat();
        }
        else if(currentArea.Type == AreaType.Water)
        {
            Drink();
        }
        if (transform.position.x > Engine.MAPSIZE
                    || transform.position.x < 0
                    || transform.position.y > Engine.MAPSIZE
                    || transform.position.y < 0)
        {
            ActionNeuron tempTurnAroundNeuron = new TurnAroundAction(gameObject);
            tempTurnAroundNeuron.DoAction();
            outOfBounds = true;
        }
        else
        {
            outOfBounds = false;
                
        }
    }
    private void Eat()
    {
        if (!eating)
        {
            currentFood = maxFood;
            DestroyImmediate(currentArea.Element);
            currentArea.Element = null;
            currentArea.Type = AreaType.Grass;
            StartCoroutine(Eating(2));
            StartCoroutine(StopInPlace(2));
        }
        
    }

    private void Drink()
    {
        currentWater = maxWater;
        StartCoroutine(StopInPlace(engine.drinkTime));
        StartCoroutine(ResetWater(5));

    }

    private IEnumerator ResetWater(float seconds)
    {
        currentArea.Type = AreaType.Grass;
        yield return new WaitForSeconds(seconds);
        currentArea.Type = AreaType.Water;
    }

    private IEnumerator Eating(float seconds) 
    {
        eating = true;
        yield return new WaitForSeconds(seconds);
        eating = false;
    }

    private void Reproduce(GameObject other)
    {
        GameObject child = Instantiate(gameObject, gameObject.transform);
        GameObject child2 = Instantiate(other.gameObject, other.gameObject.transform);
        StartCoroutine(StopInPlace(5));
    }

    private IEnumerator StopInPlace(float seconds)
    {
        canMove = false;
        yield return new WaitForSeconds(seconds);
        canMove = true;
    }

    private void Tick()
    {
        currentWater -= Time.deltaTime * (speed + eyeSight);
        currentFood -= Time.deltaTime * (speed + eyeSight);
    }

    //public Brain()   !!! change this one for smthing like Connections, which creates the template to use, Brain will do the actions
    //{

    //}

    //public Brain(int numSensors, int numActions)
    //{

    //}

    //public Brain(List<Sensor> sensors, List<Action> actions)
    //{

    //}



    // input of 0 to 1 -> connection weight -4.0f to 4.0f, neutral neuron tanh(sum(inputs)) -1 to 1, action neuron tanh(sum(inputs)) -1, 1

    // select random action and sensory, then connect randomly

    // fitness function (maybe pop size), selection function, crossover function, mutation function

    // [0][1101010][1][0101010][1010001010101010]
    // [source type][source id][destination type][destination id][weight]

    public void DefaultMovement()
    {
        time += Time.deltaTime;

        if (time >= nextMovementTime)
        {
            var angle = UnityEngine.Random.Range(0, 360f);
            var dir = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0, Mathf.Cos(Mathf.Deg2Rad * angle));
            transform.rotation= new Quaternion( dir.x,dir.y,dir.z,180f);
            nextMovementTime = UnityEngine.Random.Range(0, 5);
            transform.Rotate(dir);
            time = 0.0f;
        }

        transform.position += transform.forward  * Time.deltaTime;

    }
    public enum Diet
    {
        Carnivore,
        Herbivore,
        Omnvirore,
    }

    public enum Interests
    {
        AllyF,
        AllyS,
        EnemyF,
        EnemyS,
        WaterF,
        WaterS,
        FoodF,
        FoodS
    }
}
