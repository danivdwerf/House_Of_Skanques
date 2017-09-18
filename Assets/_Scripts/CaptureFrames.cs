using UnityEngine;
using System.Collections;
using System.IO;

public class CaptureFrames : MonoBehaviour
{
    public int ScreenshotWidth = 512;
    public string ScreenshotDir = "PNGsForGIF";
    public string ScreenshotPrefix = "Capture_";
    public float FramesPerSecond = 60.0f;

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
        string basePath = GetFullPath(ScreenshotDir);
        if (!System.IO.Directory.Exists(basePath))
            System.IO.Directory.CreateDirectory(basePath);

        int width = (int)ScreenshotWidth;
        int height = (int)(ScreenshotWidth / Camera.main.aspect);

        RenderTexture renderTexture = new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32);

        // Tell Unity that it needs to use it
        var prevCameraTexture = Camera.main.targetTexture;
        Camera.main.targetTexture = renderTexture;

        // Wait until rendering has happened!
        yield return new WaitForEndOfFrame();

        // Copy the content of the render texture into a Texture2D (so we can access the texture buffer)
        var prevRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;
        Texture2D screenshotTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        screenshotTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshotTexture.Apply();

        // Encode the buffer to PNG and write it to disk!
        byte[] bytes = screenshotTexture.EncodeToPNG();
        string screenshotPathname = System.IO.Path.Combine(basePath, ScreenshotPrefix) + ".png";
        File.WriteAllBytes(screenshotPathname, bytes);

        // Clean up
        Destroy(screenshotTexture);
        Camera.main.targetTexture = prevCameraTexture;
        RenderTexture.active = prevRenderTexture;

        Debug.Log("Screenshot Captured!");
    }

    IEnumerator TakeSequence()
    {
        yield return new WaitForEndOfFrame();

        var animator = GetComponent<Animator>();
        int idleStateHash = animator.GetCurrentAnimatorStateInfo(0).nameHash;

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
        float clipLength = animator.recorderStopTime - animator.recorderStartTime;

        string basePath = GetFullPath(ScreenshotDir);
        if (!System.IO.Directory.Exists(basePath))
            System.IO.Directory.CreateDirectory(basePath);

        int width = (int)ScreenshotWidth;
        int height = (int)(ScreenshotWidth / Camera.main.aspect);

        RenderTexture renderTexture = new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32);

        var prevCameraTexture = Camera.main.targetTexture;
        Camera.main.targetTexture = renderTexture;

        Texture2D screenshotTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);

        animator.StartPlayback();

        float frameDelta = 1.0f / FramesPerSecond;
        float currentTime = 0.0f;
        int screenshotNumber = 1;
        while (currentTime < clipLength)
        {
            // Manually set the playback time on the Animator
            animator.playbackTime = animator.recorderStartTime + currentTime;

            // Do the capture
            yield return new WaitForEndOfFrame();

            // Copy the content of the render texture into a Texture2D (so we can access the texture buffer)
            var prevRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;
            screenshotTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenshotTexture.Apply();
            RenderTexture.active = prevRenderTexture;

            // Encode the buffer to PNG and write it to disk!
            byte[] bytes = screenshotTexture.EncodeToPNG();
            string screenshotPathname = System.IO.Path.Combine(basePath, ScreenshotPrefix) + screenshotNumber.ToString("D3") + ".png";
            File.WriteAllBytes(screenshotPathname, bytes);

            // Increment the PNG number, so we don't overwrite previous files
            ++screenshotNumber;

            // Next frame
            currentTime += frameDelta;
        }

        // Clean up
        animator.StopPlayback();
        Destroy(screenshotTexture);
        Camera.main.targetTexture = prevCameraTexture;

        Debug.Log("Screenshot Captured!");
    }
}
