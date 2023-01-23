using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SensorNeuron: Neuron
{
    public SensorNeuron(GameObject parent)
    {
        this.parent = parent;
    }
    public abstract float GetSensorValue();

    internal Vector3 GetDistanceClosest(Collider[] collisions)
    {
        float closest = 0;
        GameObject closestTile = collisions[0].gameObject;
        foreach (var waterTile in collisions)
        {
            var distance = (waterTile.transform.position - parent.transform.position).sqrMagnitude;
            if (distance < closest)
            {
                closest = distance;
                closestTile = waterTile.gameObject;
            }
        }

        var distanceVector = closestTile.transform.position - parent.transform.position;
        return distanceVector;
    }
}

public class WaterLevelSensor: SensorNeuron
{
    private const int id = 1;
    public WaterLevelSensor(GameObject parent) : base(parent)
    {
        animal = parent.GetComponent<Animal>();
        Id = id;
    }
    public override float GetSensorValue()
    {
        return animal.currentWater/100f;
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
    public override float GetSensorValue()
    {
        return animal.currentFood / 100f;
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
    public override float GetSensorValue()
    {
        
        Collider[] inRadiusWater = Physics.OverlapSphere(new Vector3(parent.transform.position.x, 0, parent.transform.position.z), animal.eyeSight, 1 << WATERLAYER);
        float closest = 0;
        if (inRadiusWater.Length > 0)
        {
            var distanceVector = GetDistanceClosest(inRadiusWater);
            var angle = Mathf.Deg2Rad*Vector3.Angle(distanceVector, parent.transform.forward);
            closest = Mathf.Cos(angle) * distanceVector.magnitude;
        }

        return closest;
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
    public override float GetSensorValue()
    {

        Collider[] inRadiusWater = Physics.OverlapSphere(new Vector3(parent.transform.position.x, 0, parent.transform.position.z), animal.eyeSight, 1 << WATERLAYER);
        float closest = 0;
        if (inRadiusWater.Length > 0)
        {
            var distanceVector = GetDistanceClosest(inRadiusWater);
            var angle = Mathf.Deg2Rad * Vector3.Angle(distanceVector, parent.transform.forward);
            closest = Mathf.Sin(angle) * distanceVector.magnitude;
        }
        return closest;
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
    public override float GetSensorValue()
    {

        Collider[] inRadiusFood = Physics.OverlapSphere(new Vector3(parent.transform.position.x, 0, parent.transform.position.z), animal.eyeSight, 1 << FOODLAYER);
        float closest = 0;
        if (inRadiusFood.Length > 0)
        {
            var distanceVector = GetDistanceClosest(inRadiusFood);
            var angle = Mathf.Deg2Rad * Vector3.Angle(distanceVector, parent.transform.forward);
            closest = Mathf.Cos(angle) * distanceVector.magnitude;
        }
        return closest;
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
    public override float GetSensorValue()
    {

        Collider[] inRadiusFood = Physics.OverlapSphere(new Vector3(parent.transform.position.x, 0, parent.transform.position.z), animal.eyeSight, 1 << FOODLAYER);
        float closest = 0;
        if (inRadiusFood.Length > 0)
        {
            var distanceVector = GetDistanceClosest(inRadiusFood);
            var angle = Mathf.Deg2Rad * Vector3.Angle(distanceVector, parent.transform.forward);
            closest = Mathf.Sin(angle) * distanceVector.magnitude;
        }
        return closest;
    }
}
