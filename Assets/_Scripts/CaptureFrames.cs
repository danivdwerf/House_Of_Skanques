using UnityEngine;
using System.Collections;
using System.IO;

public class CaptureFrames : MonoBehaviour
{
    [SerializeField]private int preferedWidth;
    [SerializeField]private string folderToUse;
    [SerializeField]private string imageName;
    [SerializeField]private float fps;

    static string GetFullPath(string aAssetPath)
    {
        var projectPath = System.IO.Path.GetDirectoryName(Application.dataPath);
        return System.IO.Path.Combine(projectPath, aAssetPath);
    }
        
    void Start ()
    {
        StartCoroutine(TakeSequence());
    }

    IEnumerator TakeSingleScreenshot()
    {
        string basePath = GetFullPath(folderToUse);
        if (!System.IO.Directory.Exists(basePath))
            System.IO.Directory.CreateDirectory(basePath);

        int width = (int)preferedWidth;
        int height = (int)(preferedWidth / Camera.main.aspect);

        RenderTexture renderTexture = new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32);

        var prevCameraTexture = Camera.main.targetTexture;
        Camera.main.targetTexture = renderTexture;

        yield return new WaitForEndOfFrame();

        var prevRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;
        Texture2D screenshotTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        screenshotTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshotTexture.Apply();

        byte[] bytes = screenshotTexture.EncodeToPNG();
        string screenshotPathname = System.IO.Path.Combine(basePath, imageName) + ".png";
        File.WriteAllBytes(screenshotPathname, bytes);

        Destroy(screenshotTexture);
        Camera.main.targetTexture = prevCameraTexture;
        RenderTexture.active = prevRenderTexture;

        Debug.Log("Screenshot Captured!");
    }

    IEnumerator TakeSequence()
    {
        yield return new WaitForEndOfFrame();

        var animator = GetComponent<Animator>();
        var idleStateHash = animator.GetCurrentAnimatorStateInfo(0).nameHash;

        animator.SetTrigger("test");
        animator.StartRecording(0);

        int stateHash = 0;
        do
        {
            yield return null;
            stateHash = animator.GetCurrentAnimatorStateInfo(0).nameHash;
        }
        while (stateHash == idleStateHash);
        do
        {
            yield return null;
            stateHash = animator.GetCurrentAnimatorStateInfo(0).nameHash;
        }
        while (stateHash != idleStateHash);

        animator.StopRecording();
        var clipLength = animator.recorderStopTime - animator.recorderStartTime;

        var basePath = GetFullPath(folderToUse);
        if (!System.IO.Directory.Exists(basePath))
            System.IO.Directory.CreateDirectory(basePath);

        var width = (int)preferedWidth;
        var height = (int)(preferedWidth / Camera.main.aspect);

        var renderTexture = new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32);

        var prevCameraTexture = Camera.main.targetTexture;
        Camera.main.targetTexture = renderTexture;

        var screenshotTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);

        animator.StartPlayback();

        var frameDelta = 1.0f / fps;
        var currentTime = 0.0f;
        var screenshotNumber = 1;

        while (currentTime < clipLength)
        {
            animator.playbackTime = animator.recorderStartTime + currentTime;
            yield return new WaitForEndOfFrame();

            var prevRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;
            screenshotTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenshotTexture.Apply();
            RenderTexture.active = prevRenderTexture;

            byte[] bytes = screenshotTexture.EncodeToPNG();
            var screenshotPathname = System.IO.Path.Combine(basePath, imageName) + screenshotNumber.ToString("D3") + ".png";
            File.WriteAllBytes(screenshotPathname, bytes);

            screenshotNumber++;
            currentTime += frameDelta;
        }

        animator.StopPlayback();
        Destroy(screenshotTexture);
        Camera.main.targetTexture = prevCameraTexture;

        Debug.Log("Screenshot Captured!");
    }
}
