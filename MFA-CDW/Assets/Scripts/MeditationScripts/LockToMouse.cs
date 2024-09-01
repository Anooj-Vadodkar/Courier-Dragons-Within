using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class LockToMouse : MonoBehaviour
{
    public static LockToMouse instance;

    [SerializeField]
    private float controllerMoveSpeed;
    [SerializeField]
    private float mouseMoveSpeed;

    [SerializeField]
    private bool useMouse;

    [SerializeField]
    private float xBounds;
    [SerializeField]
    private float upperYBound;
    [SerializeField]
    private float lowerYBound;

    RingBreathe breathe;

    private List<ObjectMove> objectsToMove = new List<ObjectMove>();

    private Vector2 controllerTilt;

    private float hitTimer;

    [SerializeField]
    private float maxHitTime = 0.75f;

    [SerializeField]
    private AudioSource hatefulSounds;

    private float defaultVolume;

    private bool locked = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        breathe = GetComponentInChildren<RingBreathe>();
        defaultVolume = hatefulSounds.volume;
        hatefulSounds.volume = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(!locked)
        {
            Vector3 offset;
            if (useMouse)
            {
                Vector3 targetPosition = Mouse.current.position.ReadValue();
                targetPosition.z = 99.0f;
                targetPosition = Camera.main.ScreenToWorldPoint(targetPosition);
                targetPosition.z = 0.0f;
                targetPosition.x = Mathf.Clamp(targetPosition.x, -xBounds, xBounds);
                targetPosition.y = Mathf.Clamp(targetPosition.y, lowerYBound, upperYBound);
                targetPosition = transform.position - targetPosition;
                targetPosition.Normalize();
                offset = targetPosition * mouseMoveSpeed * Time.deltaTime;
            }
            else
            {
                //Vector3 targetPosition = controllerTilt;
                Vector3 targetPosition = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

                targetPosition = transform.position - targetPosition;
                targetPosition.Normalize();
                offset = targetPosition * mouseMoveSpeed * Time.deltaTime;
            }
            foreach (ObjectMove obj in objectsToMove)
            {
                obj.MoveObject(offset);
            }
        }
        

        if(hitTimer > 0.0f)
        {
            hitTimer -= Time.deltaTime;
            if(hitTimer <= 0.0f)
            {
                hatefulSounds.volume = 0.0f;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet") || collision.CompareTag("ObstacleBreakable"))
        {
            Destroy(collision.gameObject);
            hitTimer = maxHitTime;
            hatefulSounds.volume = defaultVolume;
        }
        else if(collision.CompareTag("Obstacle"))
        {
            Time.timeScale = 0.0f;
        }
    }

   
    public void Lock()
    {
        locked = true;
    }
    public void Unlock()
    {
        locked = false;
    }

    public void AddObject(ObjectMove obj)
    {
        objectsToMove.Add(obj);
    }

    public void RemoveObject(ObjectMove obj)
    {
        objectsToMove.Remove(obj);
    }
}
