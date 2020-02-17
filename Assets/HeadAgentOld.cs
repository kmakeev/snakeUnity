using System.Collections;
using MLAgents;
using UnityEngine;
using UnityEngine.Profiling;
using System.Collections.Generic;
using MLAgents.Sensor;
using System.Linq;


public class SnakeHead : HeadAgent
{
    // Start is called before the first frame update
    public Vector2 direction = Vector2.right;
    public GameObject tailPrefab;
    public GameObject grid;

    public float timeBetweenDecisionsAtInference;
    float m_TimeSinceDecision;

    public List<Transform> tail = new List<Transform>();

    bool ate = false;

    public override void InitializeAgent()
    {
    }

    void Start()
    {
       // InvokeRepeating("Move", 0.3f, 0.3f);
    }

    public override void AgentReset()
    {

    }

    public override void CollectObservations()
    {
        // Target and Agent positions
        AddVectorObs(transform.position);
        
    }

    public override void AgentAction(float[] vectorAction)
    {

        AddReward(-0.002f);
        
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        // Debug.Log("In OnTrigerEnter2D" + coll);
        // Food?
        if (coll.gameObject.CompareTag("Food"))
        {
            // Get longer in next Move call
            ate = true;

            // Remove the Food
            Destroy(coll.gameObject);
        }
        // Collided with Tail or Border
        else
        {
            // ToDo 'You lose' screen
            // Debug.Log("You Lose" + coll);
            for (int i = 0; i < tail.Count; i++)
            {
                Destroy(tail[i].gameObject);
            }
            tail.Clear();
            transform.localPosition = Vector2.zero;
            // Debug.Log("You Lose " + tail.Count);
            
        }
    }

    public bool CheckFoodPosition (Vector3 position)
    {
        // Debug.Log("Check position for new food " + position);
        bool result = true;

        for (int i = 0; i < tail.Count; i++)
        {
            if (tail[i].gameObject.transform.position == position)
            {
                result = false;
                break;
            }
                
        }
        return result;
    }

    // Update is called once per frame
    /*
    void Move()
    {
        Vector2 v = transform.position;

        transform.Translate(direction);

        if (ate)
        {
            // Load Prefab into the world
            GameObject g = (GameObject)Instantiate(tailPrefab,
                                                  v,
                                                  Quaternion.identity);
            g.transform.SetParent(grid.transform);
            // Keep track of it in our tail list
            tail.Insert(0, g.transform);

            // Reset the flag
            ate = false;
        }
        else if (tail.Count > 0)
        {
            tail.Last().position = v;
            tail.Insert(0, tail.Last());
            tail.RemoveAt(tail.Count - 1);
        }
        
    }
    */

    public override float[] Heuristic()
    {
        var action = new float[2];
        // Move in a new Direction?
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        /*
        if (Input.GetKey(KeyCode.RightArrow) && direction != Vector2.left)
            direction = Vector2.right;
        else if (Input.GetKey(KeyCode.DownArrow) && direction != Vector2.up)
            direction = -Vector2.up;    // '-up' means 'down'
        else if (Input.GetKey(KeyCode.LeftArrow) && direction != Vector2.right)
            direction = -Vector2.right; // '-right' means 'left'
        else if (Input.GetKey(KeyCode.UpArrow) && direction != Vector2.down)
            direction = Vector2.up;
        */    
    return action;

    }
    public void FixedUpdate()
    {
        WaitTimeInference();
    }

    void WaitTimeInference()
    {
        if (!Academy.Instance.IsCommunicatorOn)
        {
            RequestDecision();
        }
        else
        {
            if (m_TimeSinceDecision >= timeBetweenDecisionsAtInference)
            {
                m_TimeSinceDecision = 0f;
                RequestDecision();
            }
            else
            {
                m_TimeSinceDecision += Time.fixedDeltaTime;
            }
        }
    }
}
