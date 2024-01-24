using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class SensorNeuron : Neuron, Origin
{
    private float m_sensorValue;

    public SensorNeuron(GameObject parent)
    {
        this.parent = parent;
    }
    public abstract void SetSensorValue();

    protected Vector3 GetDistance(Vector3 tile)
    {
        var distanceVector = tile - parent.transform.position;
        return distanceVector;
    }
    protected Vector3 GetClosestTile(Collider[] collisions, Vector3 exception) {

        if (collisions.Count() == 0){
            return new Vector3(parent.transform.position.x, 0, parent.transform.position.z);
        }
        float closest = float.MaxValue;
        GameObject closestTile = collisions[0].gameObject;
        foreach (var tile in collisions)
        {
            var p = parent.transform.position;
            var distance = (tile.transform.position - parent.transform.position).sqrMagnitude;
            var animalX = (int)parent.transform.position.x;
            var animalZ = (int)parent.transform.position.z;
            var tileX = (int)tile.transform.position.x;
            var tileZ = (int)tile.transform.position.z;
            if (distance > closest ||
                (animalX == tileX && animalZ == tileZ) || (animalX == exception.x && animalZ == exception.z))
            {
                continue;
            }
            else
            {
                closest = distance;
                closestTile = tile.gameObject;
            }
        }
        return closestTile.transform.position;
    }

    public float GetOriginValue()
    {
        return m_sensorValue;
    }
    public void SetOriginValue(float val)
    {
        m_sensorValue = val;
    }
}

public class WaterLevelSensor : SensorNeuron
{
    private const int id = 1;
    public WaterLevelSensor(GameObject parent) : base(parent)
    {
        animal = parent.GetComponent<Animal>();
        Id = id;
    }
    public override void SetSensorValue()
    {
        SetOriginValue(animal.currentWater / 100f);
       // Debug.Log("Water: " + animal.currentWater / 100f);
    }
}

public class FoodLevelSensor : SensorNeuron
{
    private const int id = 2;
    public FoodLevelSensor(GameObject parent) : base(parent)
    {
        animal = parent.GetComponent<Animal>();
        Id = id;
    }
    public override void SetSensorValue()
    {
        SetOriginValue(animal.currentFood / 100f);
       // Debug.Log("Food: " + animal.currentFood / 100f);
    }
}

public class WaterDistanceSensor : SensorNeuron
{
    const int WATERLAYER = 4;
    private const int id = 3;
    public WaterDistanceSensor(GameObject parent) : base(parent)
    {
        animal = parent.GetComponent<Animal>();
        Id = id;
    }
    public override void SetSensorValue()
    {

        Collider[] inRadiusWater = Physics.OverlapSphere(new Vector3(parent.transform.position.x, 0, parent.transform.position.z), animal.currentEyeSight, 1 << WATERLAYER);
        float closest = 0;
        if (inRadiusWater.Length > 0)
        {
            var closestWater = GetClosestTile(inRadiusWater, Vector3.zero);
            brain.nearestWater = closestWater;
            var distanceVector = GetDistance(closestWater);
            closest = distanceVector.magnitude * 1 / animal.currentEyeSight;
        }

        SetOriginValue(closest);
    }
}

public class FoodDistanceSensor : SensorNeuron
{

    const int FOODLAYER = 7;
    private const int id = 4;
    public FoodDistanceSensor(GameObject parent) : base(parent)
    {
        animal = parent.GetComponent<Animal>();
        Id = id;
    }
    public override void SetSensorValue()
    {

        Collider[] inRadiusFood = Physics.OverlapSphere(new Vector3(parent.transform.position.x, 0, parent.transform.position.z), animal.baseEyeSight, 1 << FOODLAYER);
        float closest = 0;
        if (inRadiusFood.Length > 0)
        {
            var closestFood = GetClosestTile(inRadiusFood, brain.lastFood);
            brain.nearestFood = closestFood;
            brain.lastFood = closestFood;
          
            var distanceVector = GetDistance(closestFood);
            closest =distanceVector.magnitude * 1 / animal.baseEyeSight;
        }
        //Debug.Log("Food: " + closest);
        SetOriginValue(closest);
    }
}

//public class WaterDistanceForwardSensor : SensorNeuron
//{
//    const int WATERLAYER = 4;
//    private const int id = 5;
//    public WaterDistanceForwardSensor(GameObject parent) : base(parent)
//    {
//        animal = parent.GetComponent<Animal>();
//        Id = id;
//    }
//    public override void SetSensorValue()
//    {

//        Collider[] inRadiusWater = Physics.OverlapSphere(new Vector3(parent.transform.position.x, 0, parent.transform.position.z), animal.eyeSight, 1 << WATERLAYER);
//        float closest = 0;
//        if (inRadiusWater.Length > 0)
//        {
//            var closestWater = GetClosestTile(inRadiusWater);
//            brain.nearestWater = closestWater;
//            var distanceVector = GetDistance(closestWater);
//            var pos = parent.transform.position;
//            var angle = Mathf.Deg2Rad * Vector3.Angle(distanceVector, parent.transform.forward);
//            closest = Mathf.Cos(angle) * distanceVector.magnitude * 1 / animal.eyeSight;
//        }
//        //Debug.Log("Water: " + closest);
//        SetOriginValue(closest);
//    }
//}

//public class WaterDistanceSidesSensor : SensorNeuron
//{
//    const int WATERLAYER = 4;
//    private const int id = 6;
//    public WaterDistanceSidesSensor(GameObject parent) : base(parent)
//    {
//        animal = parent.GetComponent<Animal>();
//        Id = id;
//    }
//    public override void SetSensorValue()
//    {

//        Collider[] inRadiusWater = Physics.OverlapSphere(new Vector3(parent.transform.position.x, 0, parent.transform.position.z), animal.eyeSight, 1 << WATERLAYER);
//        float closest = 0;
//        if (inRadiusWater.Length > 0)
//        {
//            var closestWater = GetClosestTile(inRadiusWater);
//            brain.nearestWater = closestWater;
//            var distanceVector = GetDistance(closestWater);
//            var angle = Mathf.Deg2Rad * Vector3.Angle(distanceVector, parent.transform.forward);
//            closest = Mathf.Sin(angle) * distanceVector.magnitude * 1 / animal.eyeSight;
//        }
//        //Debug.Log("Water: " + closest);
//        SetOriginValue(closest);
//    }
//}
//public class FoodDistanceForwardSensor : SensorNeuron
//{
//    const int FOODLAYER = 7;
//    private const int id = 5;
//    public FoodDistanceForwardSensor(GameObject parent) : base(parent)
//    {
//        animal = parent.GetComponent<Animal>();
//        Id = id;
//    }
//    public override void SetSensorValue()
//    {

//        Collider[] inRadiusFood = Physics.OverlapSphere(new Vector3(parent.transform.position.x, 0, parent.transform.position.z), animal.eyeSight, 1 << FOODLAYER);
//        float closest = 0;
//        if (inRadiusFood.Length > 0)
//        {
//            var closestFood = GetClosestTile(inRadiusFood);
//            brain.nearestFood = closestFood;
//            var distanceVector = GetDistance(closestFood);
//            var angle = Mathf.Deg2Rad * Vector3.Angle(distanceVector, parent.transform.forward);
//            closest = Mathf.Cos(angle) * distanceVector.magnitude *1/animal.eyeSight;
//        }

//        //Debug.Log("Food: " + closest);
//        SetOriginValue(closest);
//    }
//}

//public class FoodDistanceSidesSensor : SensorNeuron
//{
//    const int FOODLAYER = 7;
//    private const int id = 6;
//    public FoodDistanceSidesSensor(GameObject parent) : base(parent)
//    {
//        animal = parent.GetComponent<Animal>();
//        Id = id;
//    }
//    public override void SetSensorValue()
//    {

//        Collider[] inRadiusFood = Physics.OverlapSphere(new Vector3(parent.transform.position.x, 0, parent.transform.position.z), animal.eyeSight, 1 << FOODLAYER);
//        float closest = 0;
//        if (inRadiusFood.Length > 0)
//        {
//            var closestFood = GetClosestTile(inRadiusFood);
//            brain.nearestFood = closestFood;
//            var distanceVector = GetDistance(closestFood);
//            var angle = Mathf.Deg2Rad * Vector3.Angle(distanceVector, parent.transform.forward);
//            closest = Mathf.Sin(angle) * distanceVector.magnitude * 1 / animal.eyeSight;
//        }
//        //Debug.Log("Food: " + closest);
//        SetOriginValue(closest);
//    }
//}
