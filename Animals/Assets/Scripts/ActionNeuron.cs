using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ActionNeuron: Neuron, Destination
{
    private List<Tuple<int, float>> m_weights;

    public static int actionNeuronCount = 0;

    private float result;

    private float bias;

    protected int id;
    public ActionNeuron(GameObject parent)
    {
        this.parent = parent;
        m_weights = new List<Tuple<int, float>>();
        animal = parent.GetComponent<Animal>();
        id = ++actionNeuronCount;
        totalNeuronsAvailable++;
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
    public abstract float GiveValue();
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

    public override int GetId()
    {
        return Neuron.lastInputNeuron + id;
    }

}

public class MoveUp : ActionNeuron
{
    public MoveUp(GameObject parent) : base(parent)
    {

    }

    public override void DoAction()
    {
        var x = parent.transform.position.x;
        var z = parent.transform.position.z+1;
        parent.transform.position= new Vector3(x, 0, z);
        parent.transform.rotation = Quaternion.Euler(0, 0, 0);

    }
    public override float GiveValue()
    {
        return 0;
    }

}

public class MoveDown : ActionNeuron
{
    public MoveDown(GameObject parent) : base(parent)
    {
    }

    public override void DoAction()
    {

        var x = parent.transform.position.x;
        var z = parent.transform.position.z - 1;
        parent.transform.position = new Vector3(x, 0, z);
        parent.transform.rotation = Quaternion.Euler(0, 180, 0);
    }
    public override float GiveValue()
    {
        return 0;
    }
}


public class MoveLeft : ActionNeuron
{

    public MoveLeft(GameObject parent) : base(parent)
    {
    }

    public override void DoAction()
    {
        var x = parent.transform.position.x-1;
        var z = parent.transform.position.z;
        parent.transform.position = new Vector3(x, 0, z);
        parent.transform.rotation = Quaternion.Euler(0, -90, 0);
    }
    public override float GiveValue()
    {
        return 0;
    }
}
public class MoveRight : ActionNeuron
{

    public MoveRight(GameObject parent) : base(parent)
    {
    }

    public override void DoAction()
    {
        var x = parent.transform.position.x + 1;
        var z = parent.transform.position.z;
        parent.transform.position = new Vector3(x, 0, z);
        parent.transform.rotation = Quaternion.Euler(0, 90, 0);
    }
    public override float GiveValue()
    {
        return 0;
    }
}
public class MoveX : ActionNeuron
{

    public MoveX(GameObject parent) : base(parent)
    {
    }

    public override void DoAction()
    {
        var x = parent.transform.position.x + 1;
        var z = parent.transform.position.z;
        parent.transform.position = new Vector3(x, 0, z);
        parent.transform.rotation = Quaternion.Euler(0, 90, 0);
    }
    public override float GiveValue()
    {
        //var tot;
        //foreach (var weight in GetWeights())
        //{
        //    tot+=weight.Item2;
        //}
        //var activatedValue = (float)Math.Tanh(sum + ((Destination)n).GetBias());
        //destinationNeuron.SetActivatedValue(activatedValue);
        //TODO change way computer, negled action itself and do in brain
        return 0;
    }
}
public class MoveY : ActionNeuron
{

    public MoveY(GameObject parent) : base(parent)
    {
    }

    public override void DoAction()
    {
        var x = parent.transform.position.x + 1;
        var z = parent.transform.position.z;
        parent.transform.position = new Vector3(x, 0, z);
        parent.transform.rotation = Quaternion.Euler(0, 90, 0);
    }
    public override float GiveValue()
    {
        return 0;
    }
}
//public class TurnAroundAction : ActionNeuron
//{
//    private const int id = 11;
//    public TurnAroundAction(GameObject parent) : base(parent)
//    {
//        animal = parent.GetComponent<Animal>();
//        Id = id;
//    }

//    public override void DoAction()
//    {
//        parent.transform.Rotate(0, 180f, 0);
//    }
//}


//public class DoNothingAction : ActionNeuron
//{
//    private const int id = 13;
//    Body body;
//    public DoNothingAction(GameObject parent) : base(parent)
//    {
//        animal = parent.GetComponent<Animal>();
//        Id = id;
//    }

//    public override void DoAction()
//    {

//    }
//}
public class TargetFood : ActionNeuron
{ 

    public TargetFood(GameObject parent) : base(parent)
    {

    }

    public override void DoAction() { 

        Vector3 foodDirection =brain.nearestFood- parent.transform.position;
        Quaternion foodRotation = Quaternion.LookRotation(foodDirection);
        parent.transform.rotation = foodRotation;
        if (brain.nearestFood != Vector3.zero & animal.MoveCloser(brain.nearestFood))
        {
            animal.UpdatePosition();
            animal.CheckActions();
            animal.eatTimer = Engine.numEatTicks;
            animal.currentEyesight = animal.baseEyesight;
            Debug.Log("Should eat");
       }
    }
    public override float GiveValue()
    {
        return 0;
    }
}

public class TargetWater : ActionNeuron
{
    public TargetWater(GameObject parent) : base(parent)
    {

    }
    public override void DoAction()
    {
        Vector3 waterDirection = brain.nearestWater - parent.transform.position;
        Quaternion waterRotation = Quaternion.LookRotation(waterDirection);
        parent.transform.rotation = waterRotation;
        if (brain.nearestWater != Vector3.zero & animal.MoveCloser(brain.nearestWater))
        {
            animal.UpdatePosition();
            animal.CheckActions();
            animal.drinkTimer = Engine.numDrinkTicks;
            animal.currentEyesight = animal.baseEyesight;
            Debug.Log("should drink");
        }
    }
    public override float GiveValue()
    {
        return 0;
    }
}

public class RotateRandomAction : ActionNeuron
{
    public RotateRandomAction(GameObject parent) : base(parent)
    {
    }

    public override void DoAction()
    {
        int randX = UnityEngine.Random.Range(-2, 3);
        int randY = UnityEngine.Random.Range(-2, 3);
        Vector3 pos = parent.transform.position;
        parent.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
        parent.transform.position = new Vector3(pos.x + randX, 0, pos.z + randY);


    }
    public override float GiveValue()
    {
        return 0;
    }
}
public class Scout : ActionNeuron
{
    public Scout(GameObject parent) : base(parent)
    {
    }

    public override void DoAction()
    {
        animal.scountTimer = Engine.numScoutTicks;
        animal.currentEyesight = animal.baseEyesight * 3;
    }
    public override float GiveValue()
    {
        return 0;
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
