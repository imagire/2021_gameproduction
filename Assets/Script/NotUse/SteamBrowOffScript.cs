using UnityEngine;

public class SteamBrowOffScript : MonoBehaviour
{
    enum Direction
    {
        None,
        Up,
        Right,
        Down,
        Left,
    }

    public ParticleSystem steamParticle;

    float rightVertical;
    float rightHorizontal;

    Direction stickDirection = Direction.None;
    Direction stickDirectionOld;
    Direction stickRotateDirection = Direction.None;

    public float steamPower = 20f;
    public float defaultSize = 1f;
    public float maxScale = 7f;
    float size = 0f;

    Vector3 scale;

    bool sizeUpFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        size = defaultSize;
        scale = new Vector3(defaultSize, 0.1f, defaultSize);

        steamParticle = steamParticle.GetComponent<ParticleSystem>();

    }

    // Update is called once per frame
    void Update()
    {
        rightHorizontal = Input.GetAxis("R_Horizontal");//左右
        rightVertical = Input.GetAxis("R_Vertical");//上下

        stickDirectionOld = stickDirection;

        if (rightHorizontal <= 0.3f && rightVertical <= 0.3f && rightHorizontal >= -0.3f && rightVertical >= -0.3f)
        {
            stickDirection = Direction.None;
        }
        else if (rightHorizontal == 1 && rightVertical <= 0.3f && rightVertical >= -0.3f)
        {
            stickDirection = Direction.Right;
        }
        else if (rightHorizontal <= 0.3f && rightHorizontal >= -0.3f && rightVertical == 1)
        {
            stickDirection = Direction.Down;
            if (stickDirectionOld == Direction.None)
            {
                sizeUpFlag = true;
            }
        }
        else if (rightHorizontal == -1 && rightVertical <= 0.3f && rightVertical >= -0.3f)
        {
            stickDirection = Direction.Left;
        }
        else if (rightHorizontal <= 0.3f && rightHorizontal >= -0.3f && rightVertical == -1)
        {
            stickDirection = Direction.Up;
        }


        if (sizeUpFlag)
        {
            scale = gameObject.transform.localScale;

            //回転方向が決まってない場合
            if (stickRotateDirection == Direction.None)
            {
                //回転方向を取得する
                if (stickDirection == Direction.Right)
                {
                    stickRotateDirection = Direction.Left;
                }
                else if (stickDirection == Direction.Left)
                {
                    stickRotateDirection = Direction.Right;
                }
            }
            //右回転の時
            else if (stickRotateDirection == Direction.Right)
            {
                //回転の向きが一致していた場合パワーをためる
                if ((int)stickDirection == (int)stickDirectionOld % 4 + 1)
                {
                    if (size <= maxScale - defaultSize)
                    {
                        size += 0.5f;
                        print("up1");
                    }
                }
            }
            //左回転の時
            else
            {
                //回転の向きが一致していた場合パワーをためる
                if ((int)stickDirection % 4 == (int)stickDirectionOld - 1)
                {
                    if (size <= maxScale - defaultSize)
                    {
                        size += 0.5f;
                        print("up1");
                    }
                }
            }

            if (stickDirection == Direction.None)
            {
                while (true)
                {
                    if (scale.x >= size + defaultSize)
                    {
                        sizeUpFlag = false;
                        size = 0f;

                        var ep = new ParticleSystem.EmitParams();
                        steamParticle.Emit(ep, 1000);

                        print("hit");
                        break;
                    }
                    
                    scale += new Vector3(0.125f, 0, 0.125f) * 0.125f * 0.125f;
                    gameObject.transform.localScale = scale;
                }
            }
        }
        else
        {
            if(scale.x > defaultSize)
            scale -= new Vector3(0.125f, 0, 0.125f) * 0.5f;
            gameObject.transform.localScale = scale;
        }

        //print(size);
    }

    private void FixedUpdate()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        //吹き飛ばし
        if (other.tag == "Enemy")
        {
            var rb = other.GetComponent<Rigidbody>();

            Vector3 vector = other.transform.position - gameObject.transform.position;
            rb.AddForce(vector.normalized * steamPower,ForceMode.Impulse);

            //print("hit");
        }
    }
}
