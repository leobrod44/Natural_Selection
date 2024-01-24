using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
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
    public float speed;
    
    public int baseEyeSight;

    public bool scouting;
    public int fitness;
    public GameObject currentBubble;
    public GameObject plantBubble;
    public GameObject drinkBubble;
    //private int lifeSpan;
    //private int maturity;

    
    private const int maxEnergy = 100;
    private const int maxWater = 100;
    private const int maxFood = 100;

    public float currentWater;
    public float currentFood;

    public int currentEyeSight;
    private bool canMove;
    private bool eating;
    private bool drinking;
    private bool outOfBounds;
    private float birth;
    private float death;
    public int age;
    public int waterCount;
    public int foodCount;
    private Engine engine;
    public Body body;
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

    public List<SensorNeuron> sensors;

    public List<ActionNeuron> actions;

    public Brain brain;

    private Diet diet;

    public float decisionTimer;
    public float drinkTimer;
    public float eatTimer;
    public float scountTimer;     

    public float decisionRate = 1;


    void Awake()
    {
        brain = new Brain(gameObject);
        eating = false;
        drinking = false;
        engine = GameObject.Find("Engine").GetComponent<Engine>();
        currentEyeSight = baseEyeSight;
        //PerformAction = DefaultMovement;
        try
        {
            currentArea = engine.sections[(int)transform.position.x, (int)transform.position.z];
            currentPosition = new Tuple<int, int>((int)transform.position.x, (int)transform.position.z);
        }
        catch
        {
            Kill();
        }
        currentFood = maxFood;
        currentWater = maxWater;
        scountTimer = 0;
        eatTimer = 0;
        drinkTimer = 0;
        fitness = 0;
        outOfBounds = false;
        foodCount = 0;
        waterCount = 0;
        plantBubble.SetActive(false);
        drinkBubble.SetActive(false);

        //interests = new List<Interests>();
        //interests.Add(Interests.WaterF);
        //interests.Add(Interests.WaterS);
        //Debug.Log(brain.Neurons);

    }
    private void Update()
    {
        canMove = true;
        if (drinkTimer>0)
        {
            drinkTimer -= 1;
            canMove=false;
            //drinkBubble.SetActive(true);
            //drinkBubble.transform.LookAt(Camera.main.transform);
            //drinkBubble.transform.rotation = Quaternion.Euler(drinkBubble.transform.rotation.x, 0, 0);    
        }
        else
        {
            drinkBubble.SetActive(false);
        }
        if (eatTimer>0)
        {
            eatTimer -= 1;
            canMove=false;
            //plantBubble.SetActive(true);
            //plantBubble.transform.LookAt(Camera.main.transform);
            //plantBubble.transform.rotation = Quaternion.Euler(plantBubble.transform.rotation.x, 0, 0);
        }
        else
        {
            plantBubble.SetActive(false);
        }
        if (scountTimer>0)
        {
            scountTimer -= 1;
            canMove=false;
        }
        StartCoroutine(Tick());
        
        
    }
    protected void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        gameObject.transform.Find("Name tag").transform.rotation =Quaternion.Euler(48.526f, 0, 0);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, 1.7f, gameObject.transform.position.z);
    }

    IEnumerator Tick()
    {
        if (age >= 150)
        {
            Kill();
        }
        if (canMove)
        {
            
            UpdatePosition();
            ActionNeuron neuronToFire = brain.GetScenarioActionNeuron();
            if (neuronToFire != null)
            {
                neuronToFire.DoAction();
                Engine.actionSupposed++;
            }
        }
  
        decisionTimer = Time.time;
        currentWater -= engine.waterDepletionConstant;
        currentFood -= engine.foodDepletionConstant;
        age += 1;
        CheckDeath();
        yield return null;
        
      
    }
    private void CheckDeath()
    {
        if (currentFood <= 0 || currentWater <= 0)
        {
            Kill();
        }
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    private void Kill()
    {
        if (age > 25)
        {
            Debug.Log("ate and drank");
        }
        gameObject.SetActive(false);
        Generation.currentPopulationSize--;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void UpdatePosition()
    {
        int currentPosx = (int)transform.position.x;
        int currentPosy = (int)transform.position.z;
        if(currentPosx!=currentPosition.Item1 || currentPosy != currentPosition.Item2)
        {
            currentPosition = new Tuple<int, int>(currentPosx, currentPosy);
            area = engine.sections[currentPosx, currentPosy];

            //refreshes tiles once moved
            if (area.Tile != currentArea.Tile)
            {
                currentArea = area;
            }
        }
    }
    public void Eat()
    {
        try
        {
            UpdatePosition();
            if (currentArea.Element.activeInHierarchy)
            {
                currentArea.Element.transform.parent.gameObject.GetComponent<Renderer>().material.color = Color.red; // Set element color to white
                currentArea.Element.SetActive(false);
            }
            currentFood = maxFood;
            Debug.Log("ate");
            foodCount++;
        }
        catch(Exception e)
        {
            Debug.LogError(currentArea);
            Debug.Log(e);
        }

    }

    public void Drink()
    {
        currentWater = maxWater;
        Engine.drinkCountDone++;
        waterCount++;
        Debug.Log("drank");
        //StartCoroutine(StopInPlace(engine.drinkTime));
        //StartCoroutine(ResetWater(5));
    }

    private IEnumerator ResetWater(float seconds)
    {
        currentArea.Type = AreaType.Grass;
        yield return new WaitForSeconds(seconds);
        currentArea.Type = AreaType.Water;

    }


    private void Reproduce(GameObject other)
    {
        GameObject child = Instantiate(gameObject, gameObject.transform);
        GameObject child2 = Instantiate(other.gameObject, other.gameObject.transform);
    }


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
    public bool MoveCloser(Vector3 target)
    {
        var v = new Vector3(target.x - this.currentPosition.Item1, 0, target.z - this.currentPosition.Item2);
        var n = v.normalized;
        int x=0;
        int y=0;

        if (n.x >= 0.5){x = 1;}

        else if(n.x <= -0.5){x = -1;}

        if(n.z >= 0.5){y=1;}

        else if(n.z <= -0.5){ y=-1;}

        this.transform.position = new Vector3(this.transform.position.x+x, 0, this.transform.position.z + y);
        if(transform.position.x == target.x && transform.position.z == target.z)
        {
            //arived
            if (engine.sections[(int)this.brain.nearestFood.x, (int)this.brain.nearestFood.z].Element == null)
            {
                Debug.Log("error finding nearest food");
            }
            return true;
        }
        return false;
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
