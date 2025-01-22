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
    [SerializeField] private float legSpeed;

    private Vector3 lastPos;
    private spiderLeg[] legs;
    private float standTimer = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        legs = new spiderLeg[legsAmount];

        for (int i = 0; i < legsAmount; i++)
        {
            GameObject newLeg = Instantiate(legTopPart);
            legs[i].TopLeg = newLeg;
            GameObject newBotLeg = Instantiate(legBotPart);
            legs[i].BotLeg = newBotLeg;

            legs[i].InUse = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

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

        lastPos = transform.position;

        for(int i = 0; i < legsAmount; i++)
        {
            Vector3 rayDir = new Vector3 (Mathf.Sin(i), height / -1, Mathf.Cos(i));

            RaycastHit hit;
            if (Physics.Raycast(transform.position, rayDir, out hit, legLenght, groundLayer))
            {
                Debug.DrawRay(transform.position, rayDir * hit.distance, Color.green);
                legs[i].BotLeg.transform.up = hit.point;

                if (!legs[i].InUse)
                {
                    legs[i].legTopTarget = ((hit.point + transform.position) / 2);
                    legs[i].legBotTarget = ((hit.point + transform.position) / 2);
                    legs[i].InUse = true;
                }
            }
            else
            {
                Debug.DrawRay(transform.position, rayDir * legLenght, Color.red);
            }

            if (Vector3.Distance(legs[i].TopLeg.transform.position, transform.position) > legLenght/3)
            {
                legs[i].InUse = false;
            }

            legs[i].TopLeg.transform.position = Vector3.MoveTowards(legs[i].TopLeg.transform.position, legs[i].legTopTarget, legSpeed * Time.deltaTime);
            legs[i].BotLeg.transform.position = Vector3.MoveTowards(legs[i].BotLeg.transform.position, legs[i].legBotTarget, legSpeed * Time.deltaTime);

            legs[i].TopLeg.transform.up = -(transform.position - legs[i].TopLeg.transform.position);
            //legs[i].BotLeg.transform.up = hit.point;

            Debug.Log(hit.point);
        }

    }

}

public struct spiderLeg
{
    public GameObject TopLeg;
    public GameObject BotLeg;
    public Vector3 legTopTarget;
    public Vector3 legBotTarget;
    public bool InUse;
}
