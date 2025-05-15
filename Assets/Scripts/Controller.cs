using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public DevelopManager developManager;
    bool isShowDevPanel = false;
    void Start(){
        Cursor.visible = false;
        developManager.gameObject.SetActive(isShowDevPanel);
    }
    void Update()
    {
        // Check if Escape key was pressed this frame
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Quit the application
            #if UNITY_EDITOR
                // If running in the editor
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                // If running in a build
                Application.Quit();
            #endif
            
            Debug.Log("Application Quit");
        }

        if(Input.GetKeyDown(KeyCode.Q)){
            isShowDevPanel = !isShowDevPanel;
            developManager.gameObject.SetActive(isShowDevPanel);
            Cursor.visible = isShowDevPanel;
        }
    }
}
  