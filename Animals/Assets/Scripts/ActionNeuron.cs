using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ActionNeuron: Neuron, Destination
{
    private List<Tuple<int, float>> m_weights;

    private float result;
    public ActionNeuron(GameObject parent)
    {
        this.parent = parent;
        m_weights = new List<Tuple<int, float>>();
    }
    public abstract void DoAction();
    public List<Tuple<int, float>> GetWeights()
    {
        return m_weights;
    }
    public void AddWeight(int sourceId, float val)
    {
        m_weights.Add(new Tuple<int, float>(sourceId, val));
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
    private const int id = 7;
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
    private const int id = 8;
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


public class RotateRandomAction : ActionNeuron
{
    private const int id = 12;
    public RotateRandomAction(GameObject parent) : base(parent)
    {
        animal = parent.GetComponent<Animal>();
        Id = id;
    }

    public override void DoAction()
    {
        var rand = UnityEngine.Random.Range(0, 360);
        parent.transform.Rotate(0, rand, 0);
    }
}

public class DoNothingAction : ActionNeuron
{
    private const int id = 13;
    public DoNothingAction(GameObject parent) : base(parent)
    {
        animal = parent.GetComponent<Animal>();
        Id = id;
    }

    public override void DoAction()
    {
        
    }
}
