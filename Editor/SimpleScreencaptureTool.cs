
#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;


namespace TemplateTools
{

    enum CaptureStatus
    {
        Idle,
        PrepareForCaptureArea,
        CapturingOnArea
    }


    [EditorTool("Simple Screen Capture Tool")]
    class SimpleScreencaptureTool : EditorTool
    {
        
        [SerializeField]
        public Texture2D m_ToolIcon;

        [SerializeField]
        public Texture2D captureIcon;

        [SerializeField]
        public Texture2D multiCaptureIcon;

        [SerializeField]
        public Texture2D selCaptureIcon;

        [SerializeField]
        public Texture2D exploreIcon;

        //[SerializeField]
        //public Texture2D settingIcon;

        GUIContent m_IconContent;

        GUIContent captureIconContent;
        GUIContent exploreIconContent;
        GUIContent multiCaptureIconContent;
        GUIContent settingIconContent;

        static float hSliderValue;

        bool status = false;

        bool cap = false;

        Rect mouseRect = new Rect(0, 0, 0, 0);

        CaptureStatus currentCaptureStatus = CaptureStatus.Idle;

        void OnEnable()
        {
            m_IconContent = new GUIContent()
            {
                image = m_ToolIcon,
                text = "Simple Screen Capture Tool",
                tooltip = "Screenshot"
            };

            captureIconContent = new GUIContent()
            {
                image = captureIcon,
                tooltip = "Capture Single"

            };

            exploreIconContent = new GUIContent()
            {
                image = exploreIcon,
                tooltip = "Explore the screenshot repository"
            };

            multiCaptureIconContent = new GUIContent()
            {
                image = multiCaptureIcon,
                tooltip = "Capture Multiple"
            };

            /*
            settingIconContent = new GUIContent()
            {
                image = settingIcon,
                tooltip = "Setting"
            };
            */

            hSliderValue = 1.0f;
        }

        public override GUIContent toolbarIcon
        {
            get { return m_IconContent; }
        }

       
        
        
        public override void OnToolGUI(EditorWindow window)
        {

            //window.wantsMouseMove = true;
            

            Event current = Event.current;


            if (current.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(0);
            }


            if (current.type == EventType.MouseDown && currentCaptureStatus != CaptureStatus.Idle)
            {
                Vector2 p = HandleUtility.GUIPointToScreenPixelCoordinate(current.mousePosition);

                //Debug.Log(Screen.height);
                //Debug.Log(Camera.current.pixelHeight);
                
                mouseRect.x = current.mousePosition.x;
                mouseRect.y = current.mousePosition.y;
                
                //mouseRect.x = p.x;
                //mouseRect.y = Camera.current.pixelHeight - p.y;

                current.Use();

                Debug.Log(mouseRect);
                //Debug.Log(Camera.current.pixelWidth);
                //Debug.Log(Camera.current.scaledPixelWidth);

                //Debug.Log(GUIUtility.GUIToScreenPoint(current.mousePosition));

                //Debug.Log("Wat : " + HandleUtility.GUIPointToScreenPixelCoordinate(current.mousePosition));



                //Debug.Log(GUIUtility.GUIToScreenPoint(current.mousePosition) - new Vector2(SceneView.currentDrawingSceneView.position.x, SceneView.currentDrawingSceneView.position.y));

                //Debug.Log("SceneView Position : " );

                //Debug.Log("Window Pos : " + window.position);
                //Debug.Log("Window Max Size : " + window.maxSize);



            }

            if (current.type == EventType.MouseDrag && currentCaptureStatus != CaptureStatus.Idle)
            {

                Vector2 p = HandleUtility.GUIPointToScreenPixelCoordinate(current.mousePosition);


                mouseRect.width = Mathf.Abs(current.mousePosition.x - mouseRect.x);
                mouseRect.height = Mathf.Abs(current.mousePosition.y - mouseRect.y);

                currentCaptureStatus = CaptureStatus.CapturingOnArea;

                current.Use();
            }

            if (current.type == EventType.MouseUp && currentCaptureStatus != CaptureStatus.Idle)
            {
                current.Use();

                
                currentCaptureStatus = CaptureStatus.Idle;

                SimpleScreencapture.DoCaptureOnSelection(mouseRect);
                Debug.Log(mouseRect + " : Captured.");
                mouseRect = Rect.zero;
            }

            
            if (current.type == EventType.MouseLeaveWindow && currentCaptureStatus == CaptureStatus.CapturingOnArea)
            {
                //Debug.Log("out!");
                current.Use();

                SimpleScreencapture.DoCaptureOnSelection(mouseRect);
                Debug.Log(mouseRect + " : Captured.");

                mouseRect = Rect.zero;
                currentCaptureStatus = CaptureStatus.Idle;
            }
            

            Handles.BeginGUI();

            if (currentCaptureStatus != CaptureStatus.Idle)
                Handles.DrawSolidRectangleWithOutline(mouseRect, new Color(0.5f, 0.5f, 0.5f, 0.25f), Color.yellow);


            GUILayout.Window(1234, new Rect(window.position.width - 170, window.position.height - 125, 160, 115), (id) =>
            {

                GUILayout.BeginVertical("Box");

                status = EditorGUILayout.BeginFoldoutHeaderGroup(status, new GUIContent("Resolution"));

                if (status)
                {
                    SimpleScreencapture.selectedResolution = EditorGUILayout.Popup(SimpleScreencapture.selectedResolution, SimpleScreencapture.GetRenderResolutions());

                    if (SimpleScreencapture.selectedResolution == 4)
                        hSliderValue = EditorGUILayout.Slider(hSliderValue, 0.05f, 8f);
                }

                EditorGUILayout.EndFoldoutHeaderGroup();

                GUILayout.EndVertical();

                GUILayout.BeginHorizontal("Box");

                GUIStyle style = new GUIStyle(GUI.skin.button);

                if (GUILayout.Button(captureIconContent, style, GUILayout.Width(32), GUILayout.Height(32)))
                {
                    currentCaptureStatus = CaptureStatus.Idle; 
                    SimpleScreencapture.DoCaptureOnCurrentSceneBySize(SimpleScreencapture.selectedResolution, hSliderValue); 
                }

                if (GUILayout.Button(new GUIContent(selCaptureIcon, "Capture Selected"), GUILayout.Width(32), GUILayout.Height(32)))
                {
                    currentCaptureStatus = CaptureStatus.PrepareForCaptureArea;
                    SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Select an area for capturing."), 3);
                }

                if (GUILayout.Button(multiCaptureIconContent, style, GUILayout.Width(32), GUILayout.Height(32)))
                {
                    currentCaptureStatus = CaptureStatus.Idle;
                    SimpleScreencapture.DoCaptureOnActiveCameras(hSliderValue);
                }

                if (GUILayout.Button(exploreIconContent, style, GUILayout.Width(32), GUILayout.Height(32)))
                {
                    currentCaptureStatus = CaptureStatus.Idle;
                    SimpleScreencapture.DoExploreScreenshotDepot();
                }

                /*
                if (GUILayout.Button(settingIconContent, style, GUILayout.Width(32), GUILayout.Height(32)))
                {
                    currentCaptureStatus = CaptureStatus.Idle;
                }
                */

                GUILayout.EndHorizontal();

                //GUILayout.BeginHorizontal("Box");

                
                
                //GUILayout.EndHorizontal();


                GUI.DragWindow();

            }, new GUIContent("Screen Capture"), GUILayout.Width(150));

            Handles.EndGUI();

            
        }
    }
}

#endif