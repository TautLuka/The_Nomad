using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class WristScreen : MonoBehaviour
{
    //Transforms
    public TMP_Text tempBox;
    public Image batteryBox;

    //current values
    int currentTemp;
    int displayTemp;

    //max tenp values
    int tempMax = 99;

    //runtime changing values (day / night)
    int idleTemp;
    int walkingTemp;

    //day values
    int dayIdleTemp = 35;
    int dayWalkingTemp = 50;

    //night values
    int nightIdleTemp;
    int nightWalkingTemp;

    //day / night bool
    bool isDay = true;

    //timers
    private float timer = 0f;
    private float timerWait = 0f;

    //delay amounts
    float tempToIdleDelayAmount = 0.5f;
    float tempToRunningDelayAmount = 0.5f;
    float tempFromIdleToWalkingDelayAmount = 0.5f;
    float tempFromRunningToWalkingDelayAmount = 0.5f;
    float waitingTimeTillRegen = 1f;

    //fluctuation array
    int[] fluctuationArray = { 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1 };
    int i = 0;

    //random values
    float randomValue = 0f;

    //bools for wait timers
    bool timerDone = false;

    //batterie State Sprites
    public Sprite FullBatterie;
    public Sprite EmptyBattery;
    public Sprite OneBar;
    public Sprite TwoBars;
    public Sprite ThreeBars;
    public Sprite FourBars;
    public Sprite FiveBars;


    public Text batterybox;

    float BatteryCapacity = 80f;
    float BatteryLevel;
    float BatteryBars = 6f;
    float BatteryBarSize;
    float CycleTime = 60f;
    float BatteryDrainTimer = 0f;
    float BatteryDrain;

    public Text coolingBox;

    float CoolerBank = 100f;
    float CoolerLevel;
    float CoolerDrain = 1f;
    float CoolerDrainTimer;


    float nigga;

    void Start()
    {
        // temp

        if (isDay)
        {
            idleTemp = dayIdleTemp;
            walkingTemp = dayWalkingTemp;
        }
        else
        {
            idleTemp = nightIdleTemp;
            walkingTemp = nightWalkingTemp;
        }

        randomValue = Random.Range(0f, 2f);
        currentTemp = idleTemp;
        displayTemp = currentTemp;

        // charging and shit

        Charge();
        BatteryDrain = BatteryCapacity / CycleTime;
        BatteryBarSize = BatteryCapacity / BatteryBars;
        CoolerLevel = CoolerBank;
    }

    // Update is called once per frame
    void Update()
    {
        BatteryUpdate();

        CoolingUpdate();

        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.LeftShift))
        {
            if (currentTemp != walkingTemp)
            {
                Walking();
                timerDone = false;
                timerWait = 0f;
            }

            else
            {
                Fluctuation();
            }

        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            if (currentTemp != tempMax)
            {
                Running();
                timerDone = false;
                timerWait = 0f;
            }
            /*
            else
            {
                Fluctuation();
            }
            */
        }
        else
        {
            if (currentTemp > idleTemp)
            {
                if (timerDone)
                {
                    Idle();
                }
                else
                {
                    WaitTimer();
                }

            }
            else
            {
                Fluctuation();
            }
        }

        tempBox.text = "" + displayTemp;

    }

    void Idle()
    {
        if (currentTemp > idleTemp)
        {
            timer += Time.deltaTime;

            if (timer >= tempToIdleDelayAmount)
            {
                timer = 0f;
                currentTemp--;
                displayTemp = currentTemp;
                i = 0;
            }
        }
    }

    void Fluctuation()
    {
        timer += Time.deltaTime;

        if (timer > randomValue)
        {
            timer = 0f;

            displayTemp += fluctuationArray[i];

            i++;

            randomValue = Random.Range(0f, 2f);

            if (i > fluctuationArray.Length - 1)
            {
                i = 0;
            }
        }
    }

    void Walking()
    {
        if (currentTemp != walkingTemp)
        {
            timer += Time.deltaTime;

            if (currentTemp < walkingTemp)
            {
                if (timer > tempFromIdleToWalkingDelayAmount)
                {
                    timer = 0f;
                    currentTemp++;
                    displayTemp = currentTemp;
                    i = 0;
                }
            }
            else
            {
                if (timer > tempFromRunningToWalkingDelayAmount)
                {
                    timer = 0f;
                    currentTemp--;
                    displayTemp = currentTemp;
                    i = 0;
                }
            }
        }
    }

    void Running()
    {
        if (currentTemp < tempMax)
        {
            timer += Time.deltaTime;

            if (timer >= tempToRunningDelayAmount)
            {
                timer = 0f;
                currentTemp++;
                displayTemp = currentTemp;
                i = 0;
            }
        }
    }

    void WaitTimer()
    {
        timerWait += Time.deltaTime;

        if (timerWait > waitingTimeTillRegen)
        {
            timerDone = true;
            timerWait = 0f;
        }
    }




    //junk code//

    void BatteryUpdate()
    {
        batterybox.text = "" + Mathf.Ceil(BatteryLevel / BatteryBarSize) + "/" + BatteryBars;
        nigga = Mathf.Ceil(BatteryLevel / BatteryBarSize);
        BatteryDrainTimer += Time.deltaTime;
        if (BatteryDrainTimer >= 1f)
        {
            if (BatteryLevel <= 0)
            {
                batteryBox.sprite = FullBatterie;
                print("dead");
                Charge();
            }
            BatteryDrainTimer = 0f;
            BatteryLevel -= BatteryDrain;


            switch (nigga)
            {
                case 5:
                    batteryBox.sprite = FiveBars;
                    return;
                case 4:
                    batteryBox.sprite = FourBars;
                    return;
                case 3:
                    batteryBox.sprite = ThreeBars;
                    return;
                case 2:
                    batteryBox.sprite = TwoBars;
                    return;
                case 1:
                    batteryBox.sprite = OneBar;
                    return;
            }
        }
    }

    public void Charge()
    {
        BatteryLevel = BatteryCapacity;
        batteryBox.sprite = FullBatterie;
    }

    //Cooling//

    void CoolingUpdate()
    {
        CoolerLevel = Mathf.Clamp(CoolerLevel, 0f, 100f);
        coolingBox.text = "" + CoolerLevel + "/" + CoolerBank;
        CoolerDrainTimer += Time.deltaTime;

        if (CoolerDrainTimer >= 0.75f)
        {
            if (CoolerLevel <= 0f)
            {
                //smth to do with temp
            }

            if (currentTemp >= 95f)
            {
                CoolerLevel -= CoolerDrain * 5f;
            }
            else
            {
                CoolerLevel -= CoolerDrain;
            }
            CoolerDrainTimer = 0f;
        }
    }

    public void Sip(string LiquidType)
    {
        if (LiquidType == "Water") { CoolerLevel += 40f; }
        if (LiquidType == "Antifreeze") { CoolerLevel += 100f; }
    }

}


