using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Network : MonoBehaviour, IPunObservable
{

    const string xVelKey = "XV", yVelKey = "YV";
    const string stateKey = "ST";
    const string xPosKey = "XP", yPosKey = "YP";
    const string yRotKey = "YR";

    private PhotonView photonView;
    private Player_Movement movement;

    private int stateSent;
    private float lastXVelocitySent, lastYVelocitySent;
    private float lastXPosSent, lastYPosSent;
    private float lastYRotSent;

    private int stateReceived;
    private float lastXVelocityReceived, lastYVelocityReceived;
    private float lastXPosReceived, lastYPosReceived;
    private float lastYRotReceived;

    [HideInInspector]
    public bool updated = false;
    [HideInInspector]
    public List<bool> updatedList = new List<bool>();

    [HideInInspector]
    public string stringToSend = "";
    [HideInInspector]
    public string stringReceived;

    [SerializeField]
    private float lerpTransformTime = 0;
    [SerializeField]
    private float lerpRotationTime = 0;

    Rigidbody2D rb;

    [HideInInspector]
    public List<float> stringsToJson = new List<float>();
    [HideInInspector]
    public string serializedString;

    public bool sharePosition;
    public bool shareRotation;
    public bool shareState;
    public bool shareVelocity;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>();
        movement = GetComponent<Player_Movement>();
    }

    public virtual void Start()
    {
        lastXPosSent = transform.position.x;
        lastYPosSent = transform.position.y;
        lastYRotSent = transform.eulerAngles.y;

        lastXPosReceived = transform.position.x;
        lastYPosReceived = transform.position.y;
        lastYRotReceived = transform.eulerAngles.y;

        transform.position = new Vector2(lastXPosSent, lastYPosSent);
        transform.eulerAngles = new Vector2(transform.eulerAngles.x, lastYRotSent);
    }

    public virtual void Update()
    {
        if (!photonView.IsMine)
        {
            if (!updated)
            {
                movement.StatesMoveManager();
                movement.AnimationManager();
                UpdatePlayer();
            }

            movement.CollisionsCheck();
        }
    }

    private void FixedUpdate()
    {
        if(!photonView.IsMine && updated)
            FixedSimulatePlayer();
    }

    private void FixedSimulatePlayer()
    {
        switch(movement.state)
        {
            case PlayerStates.IDLE:
            case PlayerStates.JUMP:

                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, movement.idleFriction * Time.deltaTime), rb.velocity.y);

                break;

            case PlayerStates.RUN:

                Move(movement.groundAcceleration);

                break;

            case PlayerStates.JUMPMOVE:

                Move(movement.airAcceleration);

                break;

            case PlayerStates.AIR:

                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, movement.idleFriction * Time.deltaTime), rb.velocity.y);
                rb.velocity += Vector2.up * movement.gravity * movement.fallMultiplier * Time.deltaTime;

                break;

            case PlayerStates.AIRMOVE:

                Move(movement.airAcceleration);
                rb.velocity += Vector2.up * movement.gravity * movement.fallMultiplier * Time.deltaTime;

                break;
        }

    }

    private void Move(float acceleration)
    {
        int hor = transform.eulerAngles.y == 0 ? 1 : -1;

        if (hor * rb.velocity.x > 0)
            rb.AddForce(Vector2.right * hor * acceleration, ForceMode2D.Force);
        else
            rb.AddForce(Vector2.right * hor * acceleration * movement.turnAroundSpeed, ForceMode2D.Force);
    }

    #region SerializeVelocity

    public void StoreVelocity()
    {
        if (rb.velocity.x != lastXVelocitySent)
        {
            stringToSend += xVelKey + rb.velocity.x + ";";
            lastXVelocitySent = rb.velocity.x;
        }

        if (rb.velocity.y != lastYVelocitySent)
        {
            stringToSend += yVelKey + rb.velocity.y + ";";
            lastYVelocitySent = rb.velocity.y;
        }
    }

    public void ReadVelocity()
    {
        if (stringReceived.Contains(xVelKey))
        {
            int xVelIndex = stringReceived.IndexOf(xVelKey);
            string xNewVel = "";

            for (int x = xVelIndex + xVelKey.Length; x < stringReceived.Length; x++)
            {
                if (stringReceived[x] != ';')
                    xNewVel += stringReceived[x];
                else
                    break;
            }

            lastXVelocityReceived = float.Parse(xNewVel);
        }

        if (stringReceived.Contains(yVelKey))
        {
            int yVelIndex = stringReceived.IndexOf(yVelKey);
            string yNewVel = "";

            for (int x = yVelIndex + yVelKey.Length; x < stringReceived.Length; x++)
            {
                if (stringReceived[x] != ';')
                    yNewVel += stringReceived[x];
                else
                    break;
            }

            lastYVelocityReceived = float.Parse(yNewVel);
        }
    }

    public bool UpdateVelocity()
    {
        float xVel = rb.velocity.x;
        float yVel = rb.velocity.y;

        if (rb.velocity.x != lastXVelocityReceived)
        {
            xVel = lastXVelocityReceived;

        }

        if (transform.position.y != lastYVelocityReceived)
        {

            yVel = lastYVelocityReceived;

        }

        rb.velocity = new Vector2(xVel, yVel);

        return true;
    }

    #endregion
    #region SerializeState

    public void StoreState()
    {
        if((int)movement.state != stateSent)
        {
            stringToSend += stateKey + (int)movement.state + ";";
            stateSent = (int)movement.state;
        }
    }

    public void ReadState()
    {
        if(stringReceived.Contains(stateKey))
        {
            int stateIndex = stringReceived.IndexOf(stateKey);
            string newState = "";

            for(int x = stateIndex + stateKey.Length; x < stringReceived.Length; x++)
            {
                if (stringReceived[x] != ';')
                    newState += stringReceived[x];
                else
                    break;
            }

            stateReceived = int.Parse(newState);
        }
    }

    public bool UpdateState()
    {
        if((int)movement.state != stateReceived)
        {
            movement.state = (PlayerStates)stateReceived;
        }

        return true;
    }

    #endregion
    #region SerializePosition
    public void StorePosition()
    {
        if (transform.position.x != lastXPosSent)
        {
            stringToSend += xPosKey + transform.position.x + ";";
            lastXPosSent = transform.position.x;
        }
        if (transform.position.y != lastYPosSent)
        {
            stringToSend += yPosKey + transform.position.y + ";";
            lastYPosSent = transform.position.y;
        }
    }

    public void ReadPosition()
    {
        if (stringReceived.Contains(xPosKey))
        {
            int xPosIndex = stringReceived.IndexOf(xPosKey);
            string xNewPos = "";

            for (int x = xPosIndex + xPosKey.Length; x < stringReceived.Length; x++)
            {
                if (stringReceived[x] != ';')
                    xNewPos += stringReceived[x];
                else
                    break;
            }

            lastXPosReceived = float.Parse(xNewPos);
        }

        if (stringReceived.Contains(yPosKey))
        {
            int yPosIndex = stringReceived.IndexOf(yPosKey);
            string yNewPos = "";

            for (int x = yPosIndex + yPosKey.Length; x < stringReceived.Length; x++)
            {
                if (stringReceived[x] != ';')
                    yNewPos += stringReceived[x];
                else
                    break;
            }

            lastYPosReceived = float.Parse(yNewPos);
        }
    }

    public bool UpdatePosition()
    {
        float xPos = transform.position.x;
        float yPos = transform.position.y;

        if (transform.position.x != lastXPosReceived)
        {
            xPos = lastXPosReceived;

        }

        if (transform.position.y != lastYPosReceived)
        {
            yPos = lastYPosReceived;
        }

        if (Vector2.Distance(transform.position, new Vector2(xPos, yPos)) < 5)
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(xPos, yPos), lerpTransformTime * Time.deltaTime);
        else
            transform.position = new Vector3(xPos, yPos);

        return transform.position == new Vector3(xPos, yPos);
    }
    #endregion
    #region SerializeRotation
    public void StoreRotation()
    {
        if (transform.eulerAngles.y != lastYPosSent)
        {
            stringToSend += yRotKey + transform.eulerAngles.y + ";";
            lastYRotSent = transform.eulerAngles.y;
        }
    }

    public void ReadRotation()
    {
        if (stringReceived.Contains(yRotKey))
        {
            int yRotIndex = stringReceived.IndexOf(yRotKey);
            string yNewRot = "";

            for (int x = yRotIndex + yRotKey.Length; x < stringReceived.Length; x++)
            {
                if (stringReceived[x] != ';')
                    yNewRot += stringReceived[x];
                else
                    break;
            }

            lastYRotReceived = float.Parse(yNewRot);
        }
    }

    public bool UpdateRotation()
    {
        float yRot = transform.eulerAngles.y;
        if (transform.eulerAngles.y != lastYRotReceived)
        {
            if (lerpRotationTime == 0)
                yRot = lastYRotReceived;
            else
                yRot = Mathf.Lerp(yRot, lastYRotReceived, lerpRotationTime);
        }

        transform.eulerAngles = new Vector2(transform.eulerAngles.x, yRot);

        return transform.eulerAngles.y == yRot;
    }
    #endregion
    #region SerializeManager
    public virtual void PrepareToSerialize()
    {
        if (sharePosition)
            StorePosition();
        if (shareRotation)
            StoreRotation();
        if (shareVelocity)
            StoreVelocity();
        if (shareState)
            StoreState();
    }

    public virtual void ReadString()
    {
        if (sharePosition)
            ReadPosition();
        if (shareRotation)
            ReadRotation();
        if (shareVelocity)
            ReadVelocity();
        if (shareState)
            ReadState();
    }

    public virtual void UpdatePlayer()
    {
        updatedList = new List<bool>();

        if (sharePosition)
            updatedList.Add(UpdatePosition());
        if (shareRotation)
            updatedList.Add(UpdateRotation());
        if (shareVelocity)
            updatedList.Add(UpdateVelocity());
        if (shareState)
            updatedList.Add(UpdateState());

        if(updatedList.Count > 0)
            updated = !updatedList.Contains(false);
    }
    #endregion

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            PrepareToSerialize();

            serializedString = stringToSend;
            stringToSend = "";

            stream.SendNext(serializedString);
        }
        else
        {
            stringReceived = (string)stream.ReceiveNext();
            ReadString();

            updated = false;
        }
    }
}
