using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionNeuron: Neuron
{
    public ActionNeuron(GameObject parent)
    {
        this.parent = parent;
    }
    public abstract void DoAction();
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
        var rand = Random.Range(0, 360);
        parent.transform.Rotate(0, rand, 0);
    }
}
