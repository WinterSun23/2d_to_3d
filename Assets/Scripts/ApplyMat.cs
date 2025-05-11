using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ApplyMat : MonoBehaviour
{
    public RuntimeMaterialManager rmm;
    public Camera cam;
    public LayerMask matMask;
    // int index = 0;
    public Color curColor = Color.red;

    public GameObject currSelectGameobject = null;

    public Transform buttonGridParent;
    public GameObject colorButtonPrefab;

    public Transform colorSettingMenu;
    void Start()
    {
        // rmm.LoadMaterials();
        GenerateColorButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(0)){
            if(Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),out RaycastHit hit,Mathf.Infinity,matMask)){
                // hit.collider.gameObject.GetComponent<MeshRenderer>().material.color = rmm.generatedMaterials[index].color;
                // index+=1;index%=rmm.generatedMaterials.Count;
                // hit.collider.gameObject.GetComponent<MeshRenderer>().material.color = curColor;
                Debug.Log("hit");
                currSelectGameobject = hit.collider.gameObject;
                colorSettingMenu.gameObject.SetActive(true);
            }
        }
        if(Input.GetKeyDown(KeyCode.Escape)){
            colorSettingMenu.gameObject.SetActive(false);
        }
    }

    public void SetCurrentCol(){
        curColor = Color.red;
    }

    public void ApplyMaterial(Material mat){
        currSelectGameobject.GetComponent<MeshRenderer>().material.color = mat.color;
    }


    public void GenerateColorButtons()
    {
        // Clear previous buttons
        foreach (Transform child in buttonGridParent)
        {
            Destroy(child.gameObject);
        }

        rmm.LoadMaterials(); // Ensure this populates generatedMaterials

        foreach (Material mat in rmm.generatedMaterials)
        {
            GameObject buttonObj = Instantiate(colorButtonPrefab, buttonGridParent);
            Button btn = buttonObj.GetComponent<Button>();
            Image img = buttonObj.GetComponent<Image>();

            if (img != null && mat.HasProperty("_Color"))
            {
                img.color = mat.color;
            }

            // Optional: Add click behavior
            btn.onClick.AddListener(() => ApplyMaterial(mat));
        }
    }
}
