using System.Linq;
using System.Collections.Generic;
using UnityEngine;
public class unitController : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> selectedVar;
    public static HashSet<GameObject> selected = new HashSet<GameObject>();
    public static bool isSelecting = false;
    public GameObject unit;
    GameObject selectionShape;
    CharacterController controller;
    Rigidbody rigidBody;
    Vector3 unitVelocity = Vector3.zero;
    bool groundedUnit;
    Ray ray;
    RaycastHit hit;
    Vector3 destination;
    Vector3 direction;
    public float unitSpeed = 2.0f;
    public float jumpHeight = 1.0f;
    public float gravityValue = -9.81f;
    bool isSelected = false;
    private void Awake()
    {
        selectedVar = unitController.selected.ToList();
    }
    static public void beginSelection()
    {
        isSelecting = true;
        foreach (GameObject item in unitController.selected)
        {
            item.GetComponent<unitController>().isSelected = false;
            Destroy(item.GetComponent<unitController>().selectionShape);
        }
        unitController.selected.Clear();
    }
    static public void endSelection()
    {
        isSelecting = false;
    }
    void Start()
    {
        unit = gameObject;
        if (!unit.TryGetComponent<CharacterController>(out controller))
            controller = unit.AddComponent<CharacterController>();
        if (!unit.TryGetComponent<Rigidbody>(out rigidBody))
            rigidBody = unit.AddComponent<Rigidbody>();
        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        destination = unit.transform.position;
    }
    public void leaveSelected()
    {
        Destroy(selectionShape);
    }
    public void becomeSelected()
    {
        selectionShape = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        selectionShape.name = "selectionShape";
        selectionShape.transform.localScale = transform.localScale * 2f;
        //selectionShape.transform.localScale =
        //    new Vector3(transform.localScale.x * 2f, transform.localScale.y * 0.2f, transform.localScale.z * 2f);
        selectionShape.transform.parent = transform;
        selectionShape.transform.position = transform.position;
        //selectionShape.GetComponent<BoxCollider>().isTrigger = true;
        Destroy(selectionShape.GetComponent<BoxCollider>());
        MeshRenderer cRenderer = selectionShape.GetComponent<MeshRenderer>();
        cRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        cRenderer.material.SetColor("_Color", new Color(0.25f, 0.25f, 1f, 0.2f));
        cRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        cRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        cRenderer.material.SetInt("_ZWrite", 0);
        //cRenderer.material.SetColor("_EmissionColor", Color.red);
        cRenderer.material.DisableKeyword("_ALPHATEST_ON");
        cRenderer.material.DisableKeyword("_ALPHABLEND_ON");
        cRenderer.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        //cRenderer.material.EnableKeyword("_EMISSION");
        cRenderer.material.renderQueue = 3000;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Selection" && isSelecting && !isSelected)
        {
            selected.Add(this.gameObject);
            isSelected = true;
            becomeSelected();
        }
    }
    private void OnTriggerExit(Collider other)

    {
        if (other.transform.name == "Selection" && isSelecting && isSelected)
        {
            selected.Remove(this.gameObject);
            isSelected = false;
            leaveSelected();
        }
    }
    private void FixedUpdate()
    {
        selectedVar = unitController.selected.ToList();
    }
    void Update()
    {
        groundedUnit = controller.isGrounded;
        if (groundedUnit && unitVelocity.y < 0)
        {
            unitVelocity.y = 0f;
        }
        if (Input.GetMouseButtonDown(1) && isSelected)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                destination = hit.point;
            }
        }
        direction = (destination - unit.transform.position).normalized;
        if (direction != Vector3.zero)
        {
            unit.transform.LookAt(destination, Vector3.up);
            controller.Move(direction * Time.deltaTime * unitSpeed);
        }
        if (Input.GetButtonDown("Jump") && groundedUnit && isSelected)
        {
            unitVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
        unitVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(unitVelocity * Time.deltaTime);
    }
}