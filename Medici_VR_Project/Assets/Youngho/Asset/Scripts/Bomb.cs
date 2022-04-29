using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Bomb : MonoBehaviour, BombInterface
{

    // button push and pull position
    Vector3 defaultPosition;
    Vector3 pushedPosition;
    [SerializeField]
    float pushedZDistance;


    // button push and pull time
    [SerializeField]
    float buttonPushTime = 2f;
    [SerializeField]
    float buttonPullTime = 1f;

    // light GameObjects
    [SerializeField]
    GameObject button;
    [SerializeField]
    GameObject light;
    [SerializeField]
    GameObject head;



    // Patterns about Bomb
    [SerializeField]
    List<ButtonBombPattern> patterns;
    ButtonBombPattern thisGamePattern;

    // timer relate
    string nowSecond;


    //light
    public Lights buttonBoxLight;

    //Bomb State
    enum State
    {
        Normal,
        Warnning,
        Wrong,
        Success
    }

    State state;

    private void Start()
    {

        // ButtonPosition Setting
        defaultPosition = button.transform.position;
        pushedPosition = defaultPosition + button.transform.up * pushedZDistance;
        //pullBack = new ButtonDeligate(PullButtonAndLightOff);

        //thisGamePattern = patterns[Random.Range(0, patterns.Count)];
        thisGamePattern = patterns[0];
        SetBomb(thisGamePattern);

        state = State.Normal;
    }


    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Normal:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    PushButton();
                    NormalStateTryDefuse();
                }
                break;
            case State.Warnning:
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    PullButton();
                    WarnningStateKeyUp();
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    PushButton();

                    WarnningStateKeyDown();
                }
                break;
            case State.Wrong:
                break;
            case State.Success:
                break;
        }
    }

    private void WarnningStateKeyDown()
    {
        bool isSuccess = false;
        string second = (/*Timer.instance.curTime*/ 50 % 60).ToString();

        switch (thisGamePattern.patternNum)
        {
            case 1:
            case 2:
                if( second == thisGamePattern.endTime)
                {
                    isSuccess = true;
                }
                CheckWarnningSuccessAndChangeState(isSuccess);
                break;
            default:
                isSuccess = true;
                break;
        }

        if (!isSuccess)
        {
            Fail();
        }
    }

    private void WarnningStateKeyUp()
    {
        bool isSuccess = false;
        string second = "50";

        switch (thisGamePattern.patternNum)
        {
            case 0:
            case 3:
            case 4:
                if (second == thisGamePattern.endTime)
                {
                    isSuccess = true;
                }
                CheckWarnningSuccessAndChangeState(isSuccess);
                break;
            case 1:
            case 2:
                isSuccess = true;
                break;
        }

        if (!isSuccess)
        {
            Fail();
        }

    }

    public bool NormalStateTryDefuse()
    {
        bool isSuccess = false;
        //Timer �� ��.
        string second = "55";

        isSuccess = FirstTimeCheck(second);

        CheckNormalSuccessAndChangeState(isSuccess);

        return isSuccess;
    }


    private bool FirstTimeCheck(string second)
    {
        bool isSuccess = false;
        if (second == thisGamePattern.startTime)
        {
            isSuccess = true;
        }
        return isSuccess;

    }

    private void CheckNormalSuccessAndChangeState(bool isSuccess)
    {
        if (isSuccess)
        {
            state = State.Warnning;
            ChangeLightColor(thisGamePattern.lightColor);
        }
        else
        {
            state = State.Wrong;
            ChangeLightColor(Color.red);
            Fail();
        }
    }
    
    private void CheckWarnningSuccessAndChangeState(bool isSuccess)
    {
        if (isSuccess)
        {
            state = State.Success;
            ChangeLightColor(Color.green);
        }
        else
        {
            state = State.Wrong;
            ChangeLightColor(Color.red);
        }
    }

    private void SetBomb(ButtonBombPattern thisGamePattern)
    {
        head.GetComponent<MeshRenderer>().material.color = thisGamePattern.buttonColor;

        button.GetComponentInChildren<Text>().text = thisGamePattern.buttonText;
    }

    void ChangeLightColor(Color color)
    {
        light.GetComponent<MeshRenderer>().material.color = color;
    }



    private void PushButton()
    {
        isButtonPush = true;
        StartCoroutine(IEPushButton());

    }

    IEnumerator IEPushButton()
    {
        float timer = 0;
        while (timer < buttonPushTime && isButtonPush)
        {
            timer += Time.deltaTime;
            button.transform.position = Vector3.Lerp(button.transform.position, pushedPosition, 0.05f);
            yield return 0;
        }
       
    }

    bool isButtonPush;

    private void PullButton()
    {
        isButtonPush = false;
        StopCoroutine(IEPushButton());
        StartCoroutine(IEPullButton());
    }


    IEnumerator IEPullButton()
    {
        float timer = 0;
        while (timer < buttonPullTime)
        {
            timer += Time.deltaTime;
            button.transform.position = Vector3.Lerp(button.transform.position, defaultPosition, 0.05f);
            yield return 0;
        }

    }




    public bool Fail()
    {
        print("���н� ���� �ڸ��ϴ�.");
        buttonBoxLight.OnFail(() =>
        {
            BombManager.instance.OnFailButtonBox();
        });
        return false;
    }

    public bool Success()
    {
        print("������ ���� �ڸ��ϴ�.");
        return true;
    }


}


/*
[Serializable]
struct ButtonBombPattern
{
    public int patternNum;
    public Color buttonColor;
    public Color lightColor;
    public string buttonText;
    public string startTime;
    public string endTime;
    public bool isKeepButtonPush;

    public ButtonBombPattern(int patternNum, Color buttonColor, Color lightColor, string buttonText, string startTime, string endTime, bool isKeepButtonPush)
    {
        this.patternNum = patternNum;
        this.buttonColor = buttonColor;
        this.lightColor = lightColor;
        this.buttonText = buttonText;
        this.startTime = startTime;
        this.endTime = endTime;
        this.isKeepButtonPush = isKeepButtonPush;
    }

}
*/

