using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateRoundSprite : MonoBehaviour
{
    public Transform roundOne;
    public Transform roundTwo;
    public Transform roundThree;

    public Sprite win;
    public Sprite draw;
    public Sprite lose;

    private int currentRound = 1;
    // Start is called before the first frame update
    void Awake()
    {
        // maybe hardcode round object access, currently done via editor
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRoundOutcomeObject();
    }

    void UpdateRoundOutcomeObject()
    {
        if (currentRound != GlobalVariables.round)
        {
            currentRound = GlobalVariables.round;
            SpriteRenderer spriteRenderer;
            switch (currentRound)
            {
                case 2: // means round 1 has ended
                    spriteRenderer = roundOne.GetComponent<SpriteRenderer>();
                    roundOne.GetComponent<Spinner>().enabled = false;
                    break;
                case 3:
                    spriteRenderer = roundTwo.GetComponent<SpriteRenderer>();
                    roundTwo.GetComponent<Spinner>().enabled = false;
                    break;
                case 4:
                    spriteRenderer = roundThree.GetComponent<SpriteRenderer>();
                    roundThree.GetComponent<Spinner>().enabled = false;
                    break;
                default:
                    spriteRenderer = null;
                    break;
            }
            switch (GlobalVariables.roundWinners[currentRound - 2])
            { // -2 because starts at 0 and round already updated
                case BoardSide.Top:
                    spriteRenderer.sprite = lose;
                    break;
                case BoardSide.Bottom:
                    spriteRenderer.sprite = win;
                    break;
                case BoardSide.None:
                    spriteRenderer.sprite = draw;
                    break;
            }

        }
    }
}
