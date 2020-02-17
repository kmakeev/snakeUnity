using UnityEngine;
using MLAgents;
using UnityEngine.Profiling;
using MLAgents.Sensor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HeadAgent : Agent
{

    // Start is called before the first frame update
    public Vector2 direction = Vector2.right;
    public GameObject tailPrefab;
    public GameObject grid;
    public GameObject foodController;

    public float timeBetweenDecisionsAtInference;
    public float moveSpeed = 0.3f;
    float m_TimeSinceDecision;
    float deltaMoveTime;

    List<Transform> tail = new List<Transform>();

    bool ate = false;
    bool lost = false;

    // Start is called before the first frame update
    public override void InitializeAgent()
    {
    }


    // Start is called before the first frame update
    void Start()
    {
        

    }

    public override void AgentReset()
    {
        for (int i = 0; i < tail.Count; i++)
            {
                Destroy(tail[i].gameObject);
            }
        tail.Clear();
        foodController.GetComponent<SpawnFood>().DestroyAllFood();
        transform.localPosition = Vector2.zero;
        // Debug.Log("You Lose " + tail.Count);
    }

    public override void CollectObservations()
    {
        // AddVectorObs(transform.position);
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
            lost = true;
        }
    }

    public bool CheckFoodPosition(Vector3 position)
    {
        // Debug.Log("Check position for new food " + position);
        bool result = true;

        for (int i = 0; i < tail.Count; i++) {
            if (tail[i].gameObject.transform.position == position)
            {
                result = false;
                break;
            }

        }
        return result;
    }

    public override void AgentAction(float[] vectorAction)
    {        
        var movement = (int)vectorAction[0];
        Vector2 v = transform.position;
        // Debug.Log("Action " + direction);
        switch (movement) {
            case 1:
                direction = Vector2.right;
                break;
            case 2:
                direction = -Vector2.up;    // '-up' means 'down'
                break;
            case 3:
                direction = -Vector2.right; // '-right' means 'left'
                break;
            case 4:
                direction = Vector2.up;
                break;
            default:
                break;
        }
        

        if (Time.time > deltaMoveTime + moveSpeed) {                 
            
            // Debug.Log("Move time begin " + direction);
            transform.Translate(direction);
            if (ate) {
                // Load Prefab into the world
                GameObject g = (GameObject)Instantiate(tailPrefab,
                                                      v,
                                                      Quaternion.identity);
                g.transform.SetParent(grid.transform);
                // Keep track of it in our tail list
                tail.Insert(0, g.transform);

                // Reset the flag
                ate = false;
                AddReward(1f);
            }
            else if (tail.Count > 0) {
                tail.Last().position = v;
                tail.Insert(0, tail.Last());
                tail.RemoveAt(tail.Count - 1);
            }
            if (lost) {
                AddReward(-1f);
                lost = false;
                Done();
            }
            deltaMoveTime = Time.time;            
            
            AddReward(-0.002f);
        }        

    }

    public override float[] Heuristic()
    {

        if (Input.GetKey(KeyCode.RightArrow) && direction != Vector2.left)
            return new float[] { 1 };
        else if (Input.GetKey(KeyCode.DownArrow) && direction != Vector2.up)
            return new float[] { 2 };    
        else if (Input.GetKey(KeyCode.LeftArrow) && direction != Vector2.right)
            return new float[] { 3 }; 
        else if (Input.GetKey(KeyCode.UpArrow) && direction != Vector2.down)
            return new float[] { 4 };
        
        return new float[] { 0 };

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
