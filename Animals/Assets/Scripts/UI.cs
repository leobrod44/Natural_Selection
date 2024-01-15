using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class UI : MonoBehaviour
{
    public Transform columnContainer;
    public GameObject imagePrefab;
    public Camera captureCamera;
    public Slider slider1;
    public Slider slider2;
    public Text nameText;


    // Assuming you have a list of animals
    private List<GameObject> displayed = new List<GameObject>();
    private Generation generation;

    void Start()
    {
        generation = gameObject.GetComponentInParent<Engine>().generation;

    }
    private void Update()
    {
        // Create UI elements for each animal
        foreach (GameObject animal in generation.currentPopulation)
        {
            if (animal.activeInHierarchy && !displayed.Contains(animal))
            {
                CreateAnimalUIElement(animal);
            }
            
        }
    }

    void CreateAnimalUIElement(GameObject animalObj)
    {
        // Create name text
        nameText = CreateText("Your Name", new Vector2(10, 90));
    }
    //public byte[] Capture(GameObject obj)
    //{
    //    // Get the screen position of the 3D object
    //    Vector3 screenPosition = captureCamera.WorldToScreenPoint(obj.transform.position);

    //    // Create a texture to render to
    //    int width = Screen.width;
    //    int height = Screen.height;
    //    RenderTexture renderTexture = new RenderTexture(width, height, 24);
    //    captureCamera.targetTexture = renderTexture;

    //    // Set the camera to the object's position and render
    //    captureCamera.transform.position = obj.transform.position;
    //    GameObject clone = Instantiate(obj, captureCamera.transform);
    //    clone.transform.localPosition = screenPosition;
    //    clone.transform.localRotation = Quaternion.identity;
        
    //    captureCamera.Render();

    //    // Read the pixels from the render texture
    //    Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
    //    RenderTexture.active = renderTexture;
    //    tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
    //    tex.Apply();

    //    // Encode the texture to PNG
    //    byte[] encodeToPNG = tex.EncodeToPNG();

    //    // Save the PNG to a file
    //    File.WriteAllBytes(obj.name + ".png", encodeToPNG);

    //    // Clean up
    //    captureCamera.targetTexture = null;
    //    RenderTexture.active = null;
    //    GameObject imageObject = new GameObject("DynamicRawImage");

    //    // Add a RawImage component to the GameObject
    //    RawImage rawImage = imageObject.AddComponent<RawImage>();

    //    // Create a Texture2D and load the image bytes
    //    Texture2D texture = new Texture2D(2, 2); // You may need to adjust the dimensions
    //    bool success = texture.LoadImage(encodeToPNG);

    //    // Check if the image is loaded successfully
    //    if (success)
    //    {
    //        // Assign the texture to the RawImage component
    //        rawImage.texture = texture;

    //        // Set the RectTransform properties for positioning and sizing
    //        RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
    //        rectTransform.SetParent(CanvasManager.Instance.Canvas.transform); // You may need to adjust this based on your canvas hierarchy
    //        rectTransform.anchoredPosition = new Vector2(0, 0); // Adjust the position as needed
    //        rectTransform.sizeDelta = new Vector2(100, 100); // Adjust the size as needed
    //    }
    //    else
    //    {
    //        Debug.LogError("Failed to load the image from bytes.");
    //    }
    //    Destroy(tex);
    //    Destroy(renderTexture);

    //    return encodeToPNG;
    
 
    //}
    // Function to create a slider
    Slider CreateSlider(string label, Vector2 position)
    {
        GameObject sliderGO = new GameObject(label);
        sliderGO.transform.SetParent(transform);

        Slider slider = sliderGO.AddComponent<Slider>();
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 0.5f;

        RectTransform rectTransform = slider.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;

        return slider;
    }

    // Function to create text
    Text CreateText(string labelText, Vector2 position)
    {
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(transform);

        Text text = textGO.AddComponent<Text>();
        text.text = labelText;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 16;

        RectTransform rectTransform = text.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;

        return text;
    }
}

