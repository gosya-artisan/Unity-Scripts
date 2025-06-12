using UnityEngine;
using UnityEngine.AI;

public class NewMonoBehaviourScript : MonoBehaviour
{

    // idle - 0
    // flair - -1
    //walk - 1
    //run - 2

    private Animator animator;
    private int state = 2;
    private GameObject player;
    private NavMeshAgent agent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("state", 2);
        player = GameObject.FindGameObjectWithTag("Player");  
        agent = GetComponent<NavMeshAgent>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //agent.SetDestination(player.transform.position); 
    }

    private void TestAnimations()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Space key was pressed");
            if (state < 2)
            {
                state += 1; 
            }
            else 
            {
                state = -1;
            }
            
        }

        animator.SetInteger("state", state);
    }
}
