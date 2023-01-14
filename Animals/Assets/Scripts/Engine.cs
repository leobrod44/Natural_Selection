using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    public int animalCount;

    private List<Body> animals = new List<Body>();

    public  List<GameObject> skeletons;
    void Start()
    {
        //
        for (int i = 0; i < animalCount; i++)
        {
            var x = Random.Range(0, Layout.MAPSIZE);
            var y = Random.Range(0, Layout.MAPSIZE);
            var bodySize = Random.Range(0.2f,1);
            var eyeSize = Random.Range(0.1f, 0.5f); 
            var legSize = Random.Range(0.2f, 0.8f);
            CreateAnimal(AnimalType.Quadruped, x, y);

        }
    }

    public void CreateAnimal(AnimalType type, int x, int y)
    {
        GameObject skeleton = Instantiate(skeletons[0]);
        skeleton.transform.position = new Vector3(x, 1f, y);
        //Determine body characteristics 
        var bodySize = Random.Range(-0.2f, 0.5f);
        var eyeSize = Random.Range(-0.2f, 0.5f);
        var legSize = Random.Range(-0.2f, 0.5f);

        Body newAnimalBody = new Body(skeleton, bodySize, eyeSize, legSize, x, y);
        animals.Add(newAnimalBody);
    }

    public enum AnimalType
    {
        Quadruped,
        Biped,
        Insect
    }
}
