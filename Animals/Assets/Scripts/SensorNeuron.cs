using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SensorNeuron
{
    public GameObject parent;
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
    public Animal brain;
    public WaterLevelSensor(GameObject parent) : base(parent)
    {
        brain = parent.GetComponent<Animal>();
    }
    public override float GetSensorValue()
    {
        return brain.currentWater/100f;
    }
}

public class FoodLevelSensor : SensorNeuron
{
    public Animal brain;
    public FoodLevelSensor(GameObject parent) : base(parent)
    {
        brain = parent.GetComponent<Animal>();
    }
    public override float GetSensorValue()
    {
        return brain.currentFood / 100f;
    }
}

public class WaterDistanceForwardSensor : SensorNeuron
{
    public Animal brain;
    const int WATERLAYER = 4;
    public WaterDistanceForwardSensor(GameObject parent) : base(parent)
    {
        brain = parent.GetComponent<Animal>();
    }
    public override float GetSensorValue()
    {
        
        Collider[] inRadiusWater = Physics.OverlapSphere(new Vector3(parent.transform.position.x, 0, parent.transform.position.z), brain.eyeSight, 1 << WATERLAYER);
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
    public Animal brain;
    const int WATERLAYER = 4;
    public WaterDistanceSidesSensor(GameObject parent) : base(parent)
    {
        brain = parent.GetComponent<Animal>();
    }
    public override float GetSensorValue()
    {

        Collider[] inRadiusWater = Physics.OverlapSphere(new Vector3(parent.transform.position.x, 0, parent.transform.position.z), brain.eyeSight, 1 << WATERLAYER);
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
    public Animal brain;
    const int FOODLAYER = 7;
    public FoodDistanceForwardSensor(GameObject parent) : base(parent)
    {
        brain = parent.GetComponent<Animal>();
    }
    public override float GetSensorValue()
    {

        Collider[] inRadiusFood = Physics.OverlapSphere(new Vector3(parent.transform.position.x, 0, parent.transform.position.z), brain.eyeSight, 1 << FOODLAYER);
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
    public Animal brain;
    const int FOODLAYER = 7;
    public FoodDistanceSidesSensor(GameObject parent) : base(parent)
    {
        brain = parent.GetComponent<Animal>();
    }
    public override float GetSensorValue()
    {

        Collider[] inRadiusFood = Physics.OverlapSphere(new Vector3(parent.transform.position.x, 0, parent.transform.position.z), brain.eyeSight, 1 << FOODLAYER);
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
