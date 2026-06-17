using UnityEngine;

public class Controls : MonoBehaviour
{
    public GameObject controlsPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // se estiver aberto, fecha
            if (controlsPanel.activeSelf)
            {
                controlsPanel.SetActive(false);
            }
        }
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
    }
}