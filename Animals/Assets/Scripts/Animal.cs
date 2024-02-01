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
    public int baseEyesight;

    private GameObject Target;

    //states
    [SerializeField]
    public float speed;

    public bool scouting;
    public float fitness;
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
    public int currentEyesight;

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
    private Area lastFood;
    private Area lastWater;



    public void InitializeAnimal()
    {
        eating = false;
        drinking = false;
        engine = GameObject.Find("Engine").GetComponent<Engine>();
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
        baseEyesight = Engine.baseEyesight;
        scountTimer = 0;
        eatTimer = 0;
        drinkTimer = 0;
        fitness = 0;
        outOfBounds = false;
        foodCount = 0;
        waterCount = 0;
        plantBubble.SetActive(false);
        drinkBubble.SetActive(false);
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
        if (age >= 200)
        {
            Kill();
        }
        if (canMove)
        {
            //updates current position and area
            UpdatePosition();
            //CheckActions();
            //gives nearest food an nearest water
            UpdateScan();
            ActionNeuron neuronToFire = brain.GetScenarioActionNeuron();
            if (neuronToFire != null)
            {
                neuronToFire.DoAction();
                //UpdatePosition();
            }
        }
        currentWater -= engine.waterDepletionConstant;
        currentFood -= engine.foodDepletionConstant;
        if (eatTimer >= 0)
        {
            eatTimer -= 1;
        }
        if (drinkTimer >= 0)
        {
            drinkTimer -= 1;
        }

        age += 1;
        CheckDeath();
        yield return null;
        
      
    }
    public void UpdateScan()
    {
        //n^2 loop for nearby objects
        var initialPosX = (int)transform.position.x;
        var initialPosY = (int)transform.position.z;
        int range= 1;
        bool foodFound = false;
        bool waterFound = false;
        //top row
        while (true)
        {

            for (int i = 1; i <= range; i++)
            {
                var a1 = initialPosX + i; //    -1,1   0,1    1 1
                var a2 = initialPosX - i; //    -1,1   0,1    1 1
                var b1 = initialPosY + range;// -1,0   0,0    1,0  
                var b2 = initialPosY - range;//     -1 -1  0,-1   1,-1

                var c1 = initialPosY + i - 1;//-1 0 1
                var c2 = initialPosY - i - 1;//-1 0 1
                var d1 = initialPosX + range;//1
                var d2 = initialPosX - range;//-1

                if (!foodFound)
                {
                    foodFound = TestFoodArea(a1, b1);
                }
                if(!waterFound)
                {
                    waterFound = TestWaterArea(a1, b1);
                }
                
                if (!foodFound)
                {
                    foodFound = TestFoodArea(a2, b1);
                }
                if(!waterFound)
                {
                    waterFound = TestWaterArea(a2, b1);
                }
                if (!foodFound)
                {
                    foodFound = TestFoodArea(a1, b2);
                }
                if (!waterFound)
                {
                    waterFound = TestWaterArea(a1, b2);
                }
                if (!foodFound)
                {
                    foodFound = TestFoodArea(a2, b2);
                }
                if (!waterFound)
                {
                    waterFound = TestWaterArea(a2, b2);
                }

                if (!foodFound)
                {
                    foodFound = TestFoodArea(d1, c1);
                }
                if(!waterFound)
                {
                    waterFound = TestWaterArea(d1, c1);
                }
                if (!foodFound)
                {
                    foodFound = TestFoodArea(d2, c1);
                }
                if (!waterFound)
                {
                    waterFound = TestWaterArea(d2, c1);
                }
                if (!foodFound)
                {
                    foodFound = TestFoodArea(d1, c2);
                }
                if (!waterFound)
                {
                    waterFound = TestWaterArea(d1, c2);
                }
                if (!foodFound)
                {
                    foodFound = TestFoodArea(d2, c2);
                }
                if (!waterFound)
                {
                    waterFound = TestWaterArea(d2, c2);
                }
                if (foodFound && waterFound)
                {
                    break;
                }
            }
            if (foodFound && waterFound || range > 200)
            {
                break;
            }
            range += 1;
        }

    }
    private bool TestWaterArea(int x, int z)
    {
        if (x < 0 || x >= engine.mapSize || z < 0 || z >= engine.mapSize)
        {
            return false;
        }
        if (engine.sections[x, z].Type == AreaType.Drinkable)
        {
            brain.nearestWater = new Vector3(x, 0, z);
            return true;

        }
        return false;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    private bool TestFoodArea(int x, int z)
    {
        if (x< 0 || x >= engine.mapSize || z < 0 || z >= engine.mapSize || engine.targeted[x,z]==1)
        {
            return false;
        }   
        if (engine.sections[x, z].Type == AreaType.Food && engine.sections[x,z].Element.activeInHierarchy && brain.lastFood.x!=x &&brain.lastFood.z!=z)
        {
            engine.targeted[(int)brain.nearestFood.x, (int)brain.nearestFood.z] = 0;
            brain.nearestFood = new Vector3(x, 0, z);
            engine.targeted[x, z] = 1;
            return true;

        }
        return false;
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
        gameObject.SetActive(false);
        Generation.currentPopulationSize--;
    }

    
    public void UpdatePosition()
    {
        try 
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
        catch
        {
            Kill();
        }
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void CheckActions()
    {
        if (area.Tile == currentArea.Tile)
        {
            if (eatTimer<=0 && currentArea.Type == AreaType.Food && currentArea != lastFood)
            {
                Eat();
            }
            if (drinkTimer<=0 && currentArea.Type == AreaType.Drinkable && currentArea != lastWater)
            {
                Drink();
            }
        }
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Eat()
    {
        try
        {
            //UpdatePosition();
            if (currentArea.Element.activeInHierarchy)
            {
                currentArea.Element.transform.parent.gameObject.GetComponent<Renderer>().material.color = Color.red; // Set element color to white
                engine.deactivatedFood.Add(currentArea);
                engine.deactivatedTimer.Add(engine.foodRespawn);
                currentArea.Element.SetActive(false);
                currentFood = maxFood;
                eatTimer = Engine.numEatTicks;
                Debug.Log("ate");
                foodCount++;
            }
            else
            {
                currentFood = maxFood;
                foodCount++;
                eatTimer = Engine.numEatTicks;
                Debug.Log("pas supposer");
            }
            lastFood = currentArea;
            
        }
        catch(Exception e)
        {
            Debug.LogError(currentArea);
            Debug.Log(e);
        }

    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Drink()
    {
        currentWater = maxWater;
        Engine.drinkCountDone++;
        drinkTimer = Engine.numDrinkTicks;
        waterCount++;
        Debug.Log("drank");
        lastWater = currentArea;
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
