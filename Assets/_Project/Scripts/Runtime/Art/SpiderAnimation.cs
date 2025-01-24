using UnityEngine;
using UnityEngine.Rendering;

public class SpiderAnimation : MonoBehaviour
{

    [SerializeField] private GameObject legTopPart;
    [SerializeField] private GameObject legBotPart;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField, Range(0, 15)] private float legLenght;
    [SerializeField, Range(1, 20)] private int legsAmount;
    [SerializeField, Range(0.1f, 3)] private float height;
    [SerializeField] private float legMoveRange;
    [SerializeField] private float legSpeed;
    [SerializeField, Range(0, 5)] private float legBendAmount;
    [SerializeField] private float moveSpeed;

    [SerializeField] private bool affectedByVelocity;

    
    private Vector3 moveDir;
    private Vector3 lastPos;
    private spiderLeg[] legs;
    private float standTimer = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        legs = new spiderLeg[legsAmount];

        for (int i = 0; i < legsAmount; i++)
        {
            GameObject newTopLeg = Instantiate(legTopPart);
            legs[i].TopLeg = newTopLeg;
            GameObject newBotLeg = Instantiate(legBotPart);
            legs[i].BotLeg = newBotLeg;

            legs[i].InUse = false;

            legs[i].TopLeg.transform.position = transform.position;
            legs[i].BotLeg.transform.position = transform.position;

        }
    }

    // Update is called once per frame
    void Update()
    {

        Movement();

        if(transform.position == lastPos)
        {
            standTimer -= Time.deltaTime;

            if(standTimer < 0)
            {
                legs[Random.Range(0, legsAmount-1)].InUse = false;
                standTimer = 0.1f;
            }

        }
        else
        {
            standTimer = 0.1f;
        }

        if (affectedByVelocity)
        {
            moveDir = (transform.position - lastPos).normalized;
        }

        RaycastHit belowHit;
        if (Physics.Raycast(transform.position, Vector3.down, out belowHit, height * 2, groundLayer))
        {
            Debug.DrawRay(transform.position, Vector3.down * belowHit.distance, Color.green);

            if(belowHit.distance <= height * 2)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 5 * Time.deltaTime, transform.position.z);
            }
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 10 * Time.deltaTime, transform.position.z);
        }


        lastPos = transform.position;

        for(int i = 0; i < legsAmount; i++)
        {
            Vector3 rayDir = new Vector3 (Mathf.Sin(i) + moveDir.x /2, height / -1, Mathf.Cos(i) + moveDir.z/2);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, rayDir, out hit, legLenght, groundLayer))
            {
                Debug.DrawRay(transform.position, rayDir * hit.distance, Color.green);

                if (!legs[i].InUse)
                {
                    legs[i].legTopTarget = ((hit.point + new Vector3(transform.position.x, transform.position.y + legBendAmount, transform.position.z)) / 2);
                    legs[i].legBotTarget = hit.point;
                    legs[i].lastFootPos = hit.point;
                    legs[i].InUse = true;
                }
            }
            else
            {
                Debug.DrawRay(transform.position, rayDir * legLenght, Color.red);
            }

            if (Vector3.Distance(hit.point, legs[i].lastFootPos) > legMoveRange - (i%2))
            {
                legs[i].InUse = false;
            }

            if (!legs[i].InUse)
            {
                legs[i].legTopTarget = new Vector3(transform.position.x + Mathf.Sin(i), transform.position.y + 0.5f, transform.position.z + Mathf.Cos(i));
                legs[i].legBotTarget = new Vector3(transform.position.x + Mathf.Sin(i), transform.position.y - 1   , transform.position.z + Mathf.Cos(i));
            }

            legs[i].TopLeg.transform.position = Vector3.MoveTowards(legs[i].TopLeg.transform.position, legs[i].legTopTarget, legSpeed * Time.deltaTime);
            legs[i].BotLeg.transform.position = Vector3.MoveTowards(legs[i].BotLeg.transform.position, legs[i].legBotTarget, legSpeed * Time.deltaTime);

            legs[i].TopLeg.transform.up = legs[i].TopLeg.transform.position - transform.position;
            legs[i].BotLeg.transform.up =  legs[i].BotLeg.transform.position - legs[i].TopLeg.transform.position;
        }

    }



    private void Movement()
    {
        int moveY = 0;
        int moveX = 0;

        if (Input.GetKey(KeyCode.W))
        {
            moveY = moveY += 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveX = moveX -= 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveY = moveY -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveX = moveX += 1;
        }

        Vector3 moveDir = new Vector3(moveX, 0, moveY).normalized;

        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }


}

public struct spiderLeg
{
    public GameObject TopLeg;
    public GameObject BotLeg;
    public Vector3 legTopTarget;
    public Vector3 legBotTarget;
    public Vector3 lastFootPos;

    public bool InUse;
}
