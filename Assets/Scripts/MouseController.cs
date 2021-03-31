using UnityEngine;
public class MouseController : MonoBehaviour
{   //Vector3 pos = Input.mousePosition;
    //float vertical = Input.GetAxis("Mouse Y");
    //float horizontal = Input.GetAxis("Mouse X");
    Ray ray;
    RaycastHit hit;
    GameObject selectionQuad;
    [SerializeField]
    Vector3 mousePosition1;
    [SerializeField]
    Vector3 mousePosition2;
    bool isClicked = false;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            unitController.beginSelection();
            isClicked = true;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                mousePosition1 = hit.point;
            }
            selectionQuad = GameObject.CreatePrimitive(PrimitiveType.Cube);
            selectionQuad.name = "Selection";
            selectionQuad.transform.localScale = Vector3.zero;
            selectionQuad.GetComponent<BoxCollider>().isTrigger = true;
            //Destroy(selecRect.GetComponent<BoxCollider>());
            MeshRenderer cRenderer = selectionQuad.GetComponent<MeshRenderer>();
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
        if (Input.GetMouseButtonUp(0))
        {
            unitController.endSelection();
            isClicked = false;
            Destroy(selectionQuad);
        }
    }
    private void FixedUpdate()
    {
        if (isClicked)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                mousePosition2 = hit.point;
            }
            Debug.DrawLine(mousePosition1, mousePosition2, Color.red);
            selectionQuad.transform.position = (mousePosition2 + mousePosition1) * 0.5f + Vector3.up * 0.01f;
            selectionQuad.transform.localScale = new Vector3(
                Mathf.Abs(mousePosition2.x - mousePosition1.x),
                Mathf.Abs(mousePosition2.y - mousePosition1.y) + 0.5f,
                Mathf.Abs(mousePosition2.z - mousePosition1.z));
        }
    }
}