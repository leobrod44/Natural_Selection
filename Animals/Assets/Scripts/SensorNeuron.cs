using System.Collections;
using System.Collections.Generic;
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
    protected Vector3 GetClosestTile(Collider[] collisions) {

        float closest = float.MaxValue;
        GameObject closestTile = collisions[0].gameObject;
        foreach (var tile in collisions)
        {
            var distance = (tile.transform.position - parent.transform.position).sqrMagnitude;
            if (distance < closest)
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

public class WaterDistanceForwardSensor : SensorNeuron
{
    const int WATERLAYER = 4;
    private const int id = 3;
    public WaterDistanceForwardSensor(GameObject parent) : base(parent)
    {
        animal = parent.GetComponent<Animal>();
        Id = id;
    }
    public override void SetSensorValue()
    {

        Collider[] inRadiusWater = Physics.OverlapSphere(new Vector3(parent.transform.position.x, 0, parent.transform.position.z), animal.eyeSight, 1 << WATERLAYER);
        float closest = 0;
        if (inRadiusWater.Length > 0)
        {
            var closestWater = GetClosestTile(inRadiusWater);
            brain.nearestWater = closestWater;
            var distanceVector = GetDistance(closestWater);
            var pos = parent.transform.position;
            var angle = Mathf.Deg2Rad * Vector3.Angle(distanceVector, parent.transform.forward);
            closest = Mathf.Cos(angle) * distanceVector.magnitude;
        }
        //Debug.Log("Water: " + closest);
        SetOriginValue(closest);
    }
}

public class WaterDistanceSidesSensor : SensorNeuron
{
    const int WATERLAYER = 4;
    private const int id = 4;
    public WaterDistanceSidesSensor(GameObject parent) : base(parent)
    {
        animal = parent.GetComponent<Animal>();
        Id = id;
    }
    public override void SetSensorValue()
    {

        Collider[] inRadiusWater = Physics.OverlapSphere(new Vector3(parent.transform.position.x, 0, parent.transform.position.z), animal.eyeSight, 1 << WATERLAYER);
        float closest = 0;
        if (inRadiusWater.Length > 0)
        {
            var closestWater = GetClosestTile(inRadiusWater);
            brain.nearestWater = closestWater;
            var distanceVector = GetDistance(closestWater);
            var angle = Mathf.Deg2Rad * Vector3.Angle(distanceVector, parent.transform.forward);
            closest = Mathf.Sin(angle) * distanceVector.magnitude;
        }
        //Debug.Log("Water: " + closest);
        SetOriginValue(closest);
    }
}
public class FoodDistanceForwardSensor : SensorNeuron
{
    const int FOODLAYER = 7;
    private const int id = 5;
    public FoodDistanceForwardSensor(GameObject parent) : base(parent)
    {
        animal = parent.GetComponent<Animal>();
        Id = id;
    }
    public override void SetSensorValue()
    {

        Collider[] inRadiusFood = Physics.OverlapSphere(new Vector3(parent.transform.position.x, 0, parent.transform.position.z), animal.eyeSight, 1 << FOODLAYER);
        float closest = 0;
        if (inRadiusFood.Length > 0)
        {
            var closestFood = GetClosestTile(inRadiusFood);
            brain.nearestFood = closestFood;
            var distanceVector = GetDistance(closestFood);
            var angle = Mathf.Deg2Rad * Vector3.Angle(distanceVector, parent.transform.forward);
            closest = Mathf.Cos(angle) * distanceVector.magnitude;
        }
        //Debug.Log("Food: " + closest);
        SetOriginValue(closest);
    }
}

public class FoodDistanceSidesSensor : SensorNeuron
{
    const int FOODLAYER = 7;
    private const int id = 6;
    public FoodDistanceSidesSensor(GameObject parent) : base(parent)
    {
        animal = parent.GetComponent<Animal>();
        Id = id;
    }
    public override void SetSensorValue()
    {

        Collider[] inRadiusFood = Physics.OverlapSphere(new Vector3(parent.transform.position.x, 0, parent.transform.position.z), animal.eyeSight, 1 << FOODLAYER);
        float closest = 0;
        if (inRadiusFood.Length > 0)
        {
            var closestFood = GetClosestTile(inRadiusFood);
            brain.nearestFood = closestFood;
            var distanceVector = GetDistance(closestFood);
            var angle = Mathf.Deg2Rad * Vector3.Angle(distanceVector, parent.transform.forward);
            closest = Mathf.Sin(angle) * distanceVector.magnitude;
        }
        //Debug.Log("Food: " + closest);
        SetOriginValue(closest);
    }
}
