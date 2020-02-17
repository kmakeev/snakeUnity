using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnFood : MonoBehaviour
{
    public GameObject foodPrefab;
    public GameObject wall;
    private CompositeCollider2D _collider2D;
    public GameObject grid;
    private Bounds bounds;
    private GameObject newFood;
    GameObject head;
    List<Transform> foods = new List<Transform>();


    private void Awake()
    {
        _collider2D = wall.GetComponent<CompositeCollider2D>();
        if (_collider2D != null)
        {
            bounds = _collider2D.bounds;
        }
        head = GameObject.FindWithTag("Head");
        if (head != null)
        {
            Debug.Log("Found head " + head);            
        }
        else
        {
            Debug.Log("Game object Head not found");
        }
    }
    // Borders
    // public int x, y;

    //public Transform borderTop;
    //public Transform borderBottom;
    //public Transform borderLeft;
    //public Transform borderRight;

    public void DestroyAllFood()
    {
        for (int i = 0; i < foods.Count; i++)
        {
            if (foods[i] != null)
                Destroy(foods[i].gameObject);
        }
        foods.Clear();
    }

    void Spawn()
    {
        // Debug.Log("Info of bounds Composite collaider on scene" + bounds);
        bool isFoodNotSpawned = true;
        // x position between left & right border
        while (isFoodNotSpawned) {
            int x = (int)Random.Range(bounds.center.x - bounds.extents.x + 1,
                                              bounds.center.x + bounds.extents.x - 1);
                            // y position between top & bottom border
        
            int y = (int)Random.Range(bounds.center.y - bounds.extents.y,
                                      bounds.center.y + bounds.extents.y - 2 );

            // Instantiate the food at (x, y)
            if (head.GetComponent<HeadAgent>().CheckFoodPosition(new Vector3(x, y, 0))) {
                newFood =Instantiate(foodPrefab, new Vector2(x, y), Quaternion.identity); // default rotation
                // Debug.Log("Info of food counts" + food.Count);
                newFood.transform.SetParent(grid.transform);
                foods.Insert(0, newFood.transform);
                isFoodNotSpawned = false;
            }
        }
        
        
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Spawn", 3, 4);
    }
        
}
