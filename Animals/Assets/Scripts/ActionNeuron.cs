using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionNeuron
{
    public GameObject parent;
    public ActionNeuron(GameObject parent)
    {
        this.parent = parent;
    }
    public abstract void DoAction();
}

public class RotateSlightRightAction : ActionNeuron
{
    public Animal brain;
    public RotateSlightRightAction(GameObject parent) : base(parent)
    {
        brain = parent.GetComponent<Animal>();
    }

    public override void DoAction()
    {
        parent.transform.Rotate(0, 45f, 0);
    }
}

public class RotateSlightLeftAction : ActionNeuron
{
    public Animal brain;
    public RotateSlightLeftAction(GameObject parent) : base(parent)
    {
        brain = parent.GetComponent<Animal>();
    }

    public override void DoAction()
    {
        parent.transform.Rotate(0, -45f, 0);
    }
}


public class RotateQuarterRightAction : ActionNeuron
{
    public Animal brain;
    public RotateQuarterRightAction(GameObject parent) : base(parent)
    {
        brain = parent.GetComponent<Animal>();
    }

    public override void DoAction()
    {
        parent.transform.Rotate(0, 90f, 0);
    }
}
public class RotateQuarterLeftAction : ActionNeuron
{
    public Animal brain;
    public RotateQuarterLeftAction(GameObject parent) : base(parent)
    {
        brain = parent.GetComponent<Animal>();
    }

    public override void DoAction()
    {
        parent.transform.Rotate(0, -90f, 0);
    }
}

public class TurnAroundAction : ActionNeuron
{
    public Animal brain;
    public TurnAroundAction(GameObject parent) : base(parent)
    {
        brain = parent.GetComponent<Animal>();
    }

    public override void DoAction()
    {
        parent.transform.Rotate(0, 180f, 0);
    }
}


public class RotateRandomAction : ActionNeuron
{
    public Animal brain;
    public RotateRandomAction(GameObject parent) : base(parent)
    {
        brain = parent.GetComponent<Animal>();
    }

    public override void DoAction()
    {
        var rand = Random.Range(0, 360);
        parent.transform.Rotate(0, rand, 0);
    }
}
