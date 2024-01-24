using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ActionNeuron: Neuron, Destination
{
    private List<Tuple<int, float>> m_weights;

    private float result;

    private float bias;
    public ActionNeuron(GameObject parent)
    {
        this.parent = parent;
        m_weights = new List<Tuple<int, float>>();
    }
    public abstract void DoAction();
    public void ChangeWeight(int sourceId, float val)
    {
        foreach (var weight in m_weights)
        {
            if (weight.Item1 == sourceId)
            {
                m_weights.Remove(weight);
                m_weights.Add(new Tuple<int, float>(sourceId, val));
                break;
            }
        }
    }
    public void RemoveWeight(int sourceId)
    {
        foreach (var weight in m_weights)
        {
            if (weight.Item1 == sourceId)
            {
                m_weights.Remove(weight);
                break;
            }
        }

    }
    public List<Tuple<int, float>> GetWeights()
    {
        return m_weights;
    }
    public void AddWeight(int sourceId, float val)
    {
        m_weights.Add(new Tuple<int, float>(sourceId, val));
    }
    public void SetBias(float val)
    {
        bias = val;
    }   
    public float GetBias()
    {
        return bias;
    }
    public float GetActivatedValue()
    {
        return result;
    }
    public void SetActivatedValue(float val)
    {
        result = val;
    }

}

public class RotateSlightRightAction : ActionNeuron
{
    private const int id = 14;
    public RotateSlightRightAction(GameObject parent) : base(parent)
    {
        animal = parent.GetComponent<Animal>();
        Id = id;
    }

    public override void DoAction()
    {
        parent.transform.Rotate(0, 45f, 0);
    }
}

public class RotateSlightLeftAction : ActionNeuron
{
    private const int id = 15;
    public RotateSlightLeftAction(GameObject parent) : base(parent)
    {
        animal = parent.GetComponent<Animal>();
        Id = id;
    }

    public override void DoAction()
    {
        parent.transform.Rotate(0, -45f, 0);
    }
}


public class RotateQuarterRightAction : ActionNeuron
{
    private const int id = 9;
    public RotateQuarterRightAction(GameObject parent) : base(parent)
    {
        animal = parent.GetComponent<Animal>();
        Id = id;
    }

    public override void DoAction()
    {
        parent.transform.Rotate(0, 90f, 0);
    }
}
public class RotateQuarterLeftAction : ActionNeuron
{
    private const int id = 10;
    public RotateQuarterLeftAction(GameObject parent) : base(parent)
    {
        animal = parent.GetComponent<Animal>();
        Id = id;
    }

    public override void DoAction()
    {
        parent.transform.Rotate(0, -90f, 0);
    }
}

public class TurnAroundAction : ActionNeuron
{
    private const int id = 11;
    public TurnAroundAction(GameObject parent) : base(parent)
    {
        animal = parent.GetComponent<Animal>();
        Id = id;
    }

    public override void DoAction()
    {
        parent.transform.Rotate(0, 180f, 0);
    }
}


public class DoNothingAction : ActionNeuron
{
    private const int id = 13;
    Body body;
    public DoNothingAction(GameObject parent) : base(parent)
    {
        animal = parent.GetComponent<Animal>();
        Id = id;
    }

    public override void DoAction()
    {
        
    }
}
public class TargetFood : ActionNeuron
{
    private  int id = lastInputNeuron + 1;

    public TargetFood(GameObject parent) : base(parent)
    {
        animal = parent.GetComponent<Animal>();
        Id = id;
       
    }

    public override void DoAction() { 

        Vector3 foodDirection =brain.nearestFood- parent.transform.position;
        Quaternion foodRotation = Quaternion.LookRotation(foodDirection);
        parent.transform.rotation = foodRotation;
        if (brain.nearestFood != Vector3.zero & animal.MoveCloser(brain.nearestFood))
        {
            animal.Eat();
            animal.eatTimer = Engine.numEatTicks;
            animal.currentEyeSight = animal.baseEyeSight;
        }
    }
}

public class TargetWater : ActionNeuron
{
    private int id = (lastInputNeuron + 2);
    public TargetWater(GameObject parent) : base(parent)
    {
        animal = parent.GetComponent<Animal>();
        Id = id;

    }
    public override void DoAction()
    {
        Vector3 waterDirection = brain.nearestWater - parent.transform.position;
        Quaternion waterRotation = Quaternion.LookRotation(waterDirection);
        parent.transform.rotation = waterRotation;
        if (brain.nearestWater != Vector3.zero & animal.MoveCloser(brain.nearestWater))
        {
            animal.Drink();
            animal.drinkTimer = Engine.numDrinkTicks;
            animal.currentEyeSight= animal.baseEyeSight;
        }
    } 
}
public class RotateRandomAction : ActionNeuron
{
    private int id = lastInputNeuron + 4;
    public RotateRandomAction(GameObject parent) : base(parent)
    {
        animal = parent.GetComponent<Animal>();
        Id = id;
    }

    public override void DoAction()
    {
        var randX = UnityEngine.Random.Range(-1, 2);
        var randY = UnityEngine.Random.Range(-1, 2);
        Vector3 pos = parent.transform.position;
        parent.transform.position = new Vector3(pos.x + randX, 0, pos.z + randY);


    }
}
    public class Scout : ActionNeuron
    {
        private int id = lastInputNeuron + 3;
        public Scout(GameObject parent) : base(parent)
        {
            animal = parent.GetComponent<Animal>();
            Id = id;
        }

        public override void DoAction()
        {
            animal.scountTimer = Engine.numScoutTicks;
            animal.currentEyeSight = animal.baseEyeSight* 3;
        }
    }


//public class Breed : ActionNeuron
//{

//}
//public class Kill : ActionNeuron
//{
//}
////scout/call?
//public class Call : ActionNeuron
//{
//}
