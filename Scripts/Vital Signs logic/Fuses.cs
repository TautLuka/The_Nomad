using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuses : MonoBehaviour
{
    private bool[] fuses = { true, true, true, true, true };
    private int[] health = { 100, 100, 100, 100, 100 };

    private float firstStrike;
    private float secondStrike;

    private bool firstStrikeDone = false;
    private bool secondStrikeDone = false;

    private float time = 0;
    private int i = 0;

    private bool effectOneActive = false;
    private bool effectTwoActive = false;
    private bool effectThreeActive = false;
    private bool effectFourActive = false;
    private bool effectFiveActive = false;



    void Start()
    {
        DecideStrikingTime();
    }

    
    void Update()
    {
        time += Time.deltaTime;

        if(time > firstStrike && !firstStrikeDone)
        {
            print("first strike");

            Strike();

            firstStrikeDone = true;
        }

        if(time > secondStrike && !secondStrikeDone)
        {

            print("second strike");

            Strike();

            secondStrikeDone = true;
        }

        if(time > 12 && firstStrikeDone && secondStrikeDone)
        {
            DecideStrikingTime();

            time = 0f;
            i = 0;

            firstStrikeDone = false;
            secondStrikeDone = false;

        }

        if(time > i)
        {
            for (int i = 0; i < fuses.Length; i++)
            {
                if(health[i] != 0)
                {
                    health[i] -= 1;
                }
            }

            i++;
        }
    }


    public void DecideStrikingTime()
    {
        firstStrike = Random.Range(0, 5);
        secondStrike = Random.Range(7, 12);
    }

    public void Strike()
    {
        for (int i = 0; i < health.Length; i++)
        {
            if (health[i] != 0)
            {

                if (Random.Range(0, 100) > health[i])
                {
                    health[i] = 0;

                    print("fuse " + (i + 1) + " dead");
                    break;
                }
            }
        }
    }

    public void InWater()
    {
        
    }
}
