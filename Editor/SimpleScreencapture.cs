



#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.IO;

using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

using UnityEditor.EditorTools;

namespace TemplateTools
{
    public class SimpleScreencapture
    {

        static float captureSizeMultiplier = 1.0f;

        //static LocalizationHelper currentHelper;


        static string[] renderResolution =
        {
            "720p - 1280x720",
            "1080p - 1920x1080",
            "4K - 3840x2160",
            "8K - 7680x4320",
            "Custom - Screen"
        };

        public static int selectedResolution = 1;


        public static string[] GetRenderResolutions()
        {
            return renderResolution;
        }


        /*
        [InitializeOnLoadMethod]
        public static void InitLocalizationModule()
        {
            currentHelper = new LocalizationHelper(typeof(SimpleScreencapture).ToString());
            currentHelper.RefreshDictionary();
        }
        */


        static void DoCaptureOnCamera(Camera cam)
        {
            if (cam == null)
                return;

            Debug.Log("Scene cameras : " + cam.name);

            string currentScreenshot = Path.Combine(Path.GetTempPath(), "Capture_" + SceneManager.GetActiveScene().name + "_" + cam.name.Replace(" ", "_") + "_" + System.DateTime.Now.ToLongTimeString().Replace(" ", "_").Replace(":", "_") + ".png");

            Debug.Log("Capture file : " + currentScreenshot);

            OnCaptureScreenshot(cam, captureSizeMultiplier, cam.pixelWidth, cam.pixelHeight, currentScreenshot);

            
        }


        public static void DoCaptureOnCurrentSceneBySize(int selectedResIndex, float sizeMultiplier)
        {
            captureSizeMultiplier = sizeMultiplier;
            DoCaptureOnCurrentScene(selectedResIndex);
        }

        public static void DoCaptureOnActiveCameras(float sizeMultiplier)
        {
            captureSizeMultiplier = sizeMultiplier;
            DoCaptureOnActiveCameras();
        }


        static Vector2Int GetCaptureResolution(int selectedResIndex)
        {
            Vector2Int outVector = Vector2Int.one;

            if (selectedResIndex == renderResolution.Length - 1)
                return outVector;

            string[] tokens = renderResolution[selectedResIndex].Split(' ');

            string temp = tokens[tokens.Length - 1];
            tokens.Initialize();
            tokens = temp.Split('x');

            outVector = new Vector2Int(int.Parse(tokens[0]), int.Parse(tokens[1]));

            return outVector;
        }


        public static void DoCaptureOnSelection(Rect rect)
        {
            Camera currentSceneCamera = SceneView.GetAllSceneCameras()[0];

            Debug.Log("Scene cameras : " + SceneView.GetAllSceneCameras().Length.ToString());

            string currentScreenshot = Path.Combine(Path.GetTempPath(), "Capture_" + SceneManager.GetActiveScene().name + "_" + System.DateTime.Now.ToLongTimeString().Replace(" ", "_").Replace(":", "_") + ".png");

            Debug.Log("Capture file : " + currentScreenshot);

            Vector2 pos  = HandleUtility.GUIPointToScreenPixelCoordinate(new Vector2(rect.x, rect.y));
            Vector2 pos1 = HandleUtility.GUIPointToScreenPixelCoordinate(new Vector2(rect.x + rect.width, rect.y + rect.height));

            rect.x = pos.x;
            rect.y = currentSceneCamera.pixelHeight - pos.y;
            rect.width = Mathf.Abs(pos1.x - pos.x);
            rect.height = Mathf.Abs(pos1.y - pos.y);

            OnCaptureScreenshot(currentSceneCamera, rect, currentScreenshot);
        }

        
        [MenuItem("Template/Capture Screenshot/Run \"Screen Capturer\"", priority = 10)]
        
        public static void ActivateScreencaptureTool()
        {
            EditorTools.SetActiveTool<SimpleScreencaptureTool>();
        }


        [MenuItem("Template/Capture Screenshot/Capture current scene &_p", priority = 30)]
        public static void DoCaptureOnCurrentScene()
        {
            DoCaptureOnCurrentScene(selectedResolution);
        }


        //[MenuItem("Template/Capture Screenshot/Capture current scene &_p", priority = 20)]
        public static void DoCaptureOnCurrentScene(int selectedResIndex)
        {
            Camera currentSceneCamera = SceneView.GetAllSceneCameras()[0];

            Debug.Log("Scene cameras : " + SceneView.GetAllSceneCameras().Length.ToString());

            string currentScreenshot = Path.Combine(Path.GetTempPath(), "Capture_" + SceneManager.GetActiveScene().name + "_" + System.DateTime.Now.ToLongTimeString().Replace(" ", "_").Replace(":", "_") + ".png");

            Debug.Log("Capture file : " + currentScreenshot);

            Vector2Int captureRes = GetCaptureResolution(selectedResIndex);

            if (selectedResIndex == renderResolution.Length - 1)
                OnCaptureScreenshot(currentSceneCamera, captureSizeMultiplier, currentSceneCamera.pixelWidth, currentSceneCamera.pixelHeight, currentScreenshot);
            else
                OnCaptureScreenshot(currentSceneCamera, 1.0f, captureRes.x, captureRes.y, currentScreenshot);
        }

        [MenuItem("Template/Capture Screenshot/Capture current scene &_p", validate = true)]
        static bool IsEnabledCapture()
        {
            if (SceneView.GetAllSceneCameras().Length == 0)
                return false;

            if (SceneManager.GetActiveScene() == null)
                return false;

            return true;
        }


        [MenuItem("Template/Capture Screenshot/Capture by active cameras", priority = 40)]
        public static void DoCaptureOnActiveCameras()
        {
            Camera[] cameras = Component.FindObjectsOfType<Camera>();

            foreach (Camera currentCam in cameras)
            {
                if (currentCam.isActiveAndEnabled)
                    DoCaptureOnCamera(currentCam);
            }

        }


        [MenuItem("Template/Capture Screenshot/Explore screenshot depot &_o", priority = 70)]
        public static void DoExploreScreenshotDepot()
        {
            Application.OpenURL(Path.GetTempPath());
        }

        [MenuItem("Template/Capture Screenshot/Explore screenshot depot &_o", validate = true)]
        static bool IsThereScreenshotFilesAtDepot()
        {
            if (Directory.GetFiles(Path.GetTempPath(), "Capture_*.png").Length == 0)
                return false;
             
            return true;
        }


        static void OnCaptureScreenshot(Camera camera, float sizeMultiplier, int width, int height, string savepath)
        {

            Debug.Log("SceneView name : " + SceneManager.GetActiveScene().name);
            Debug.Log("Allowed dynamic resolution ? : " + camera.allowDynamicResolution.ToString());
            Debug.Log("Camera Pixel width : " + camera.pixelWidth.ToString());
            Debug.Log("Camera Pixel height : " + camera.pixelHeight.ToString());



            int renderWidth;
            int renderHeight;

            //renderWidth = Mathf.RoundToInt(camera.pixelWidth * sizeMultiplier);
            //renderHeight = Mathf.RoundToInt(camera.pixelHeight * sizeMultiplier);
            renderWidth = Mathf.RoundToInt(width * sizeMultiplier);
            renderHeight = Mathf.RoundToInt(height * sizeMultiplier);


            

            RenderTexture renderTexture = new RenderTexture(renderWidth, renderHeight, 16, RenderTextureFormat.ARGB32);

            float originalAspect = camera.aspect;
            bool originalMSAA = camera.allowMSAA;

            camera.aspect = (float)renderWidth / (float)renderHeight;
            camera.allowMSAA = true;


            camera.targetTexture = renderTexture;


            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);


            camera.Render();
            RenderTexture.active = renderTexture;


            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] byteArray = renderResult.EncodeToPNG();
            System.IO.File.WriteAllBytes(savepath, byteArray);

            camera.targetTexture = null;


            camera.aspect = originalAspect;
            camera.allowMSAA = originalMSAA;



            RenderTexture.active = null;

            camera.ResetAspect();


            SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Screen Captured."), 4);

        }


        static void OnCaptureScreenshot(Camera camera, Rect rect, string savepath)
        {
            float sizeMultiplier = 1.0f;

            int renderWidth = camera.pixelWidth;
            int renderHeight = camera.pixelHeight;


            RenderTexture renderTexture = new RenderTexture(renderWidth, renderHeight, 16, RenderTextureFormat.ARGB32);

            float originalAspect = camera.aspect;
            camera.aspect = (float)renderWidth / (float)renderHeight;


            camera.targetTexture = renderTexture;


            //Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Texture2D renderResult = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);


            camera.Render();
            RenderTexture.active = renderTexture;

            Debug.Log(rect);
            renderResult.ReadPixels(rect, 0, 0, false);
            

            byte[] byteArray = renderResult.EncodeToPNG();
            System.IO.File.WriteAllBytes(savepath, byteArray);

            camera.targetTexture = null;
            camera.aspect = originalAspect;
            RenderTexture.active = null;

            camera.ResetAspect();


            SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Screen Captured."), 4);


        }
    }

}
#endif