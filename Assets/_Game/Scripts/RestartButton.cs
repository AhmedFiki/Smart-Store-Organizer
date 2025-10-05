using UnityEngine;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    [SerializeField] private StorageAreaType currentArea;
    
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }
    private void OnEnable()
    {
        if (button != null)
            button.onClick.AddListener(OnButtonClick);
    }
    private void OnDisable()
    {
        if (button != null)
            button.onClick.RemoveListener(OnButtonClick);
    }
    private void OnButtonClick()
    {
        SaveManager.ClearAreaSaveData(currentArea);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
