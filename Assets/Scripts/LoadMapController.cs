using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using easyar;

using SpatialMap_SparseSpatialMap;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMapController : MonoBehaviour
{
    public RawImage PreviewImage;
    public List<Color32> color32s = new List<Color32>();

    public SparseSpatialMapController mapTemp;
    private MapSession mapSession;
    private ARSession session;
    private SparseSpatialMapWorkerFrameFilter mapWorker;
    private Texture2D capturedImage;

    List<SparseSpatialMapController> MapControllers = new List<SparseSpatialMapController>();

    private VideoCameraDevice videoCamera;

    public AudioSource aS;

    private void Awake()
    {
#if UNITY_EDITOR
        GameObject.FindObjectOfType<VIOCameraDeviceUnion>().enabled = false;
#endif
        session = FindObjectOfType<ARSession>();
        mapWorker = FindObjectOfType<SparseSpatialMapWorkerFrameFilter>();
        videoCamera = session.GetComponentInChildren<VideoCameraDevice>();

    }


    private void Start()
    {

        mapSession = new MapSession(mapWorker, MapMetaManager.LoadAll());
        mapSession.LoadMapMeta(mapTemp, false);
        mapSession.CurrentMapLocalized += (MapData) =>
        {
            if (aS.isPlaying == false)
            {
                aS.PlayOneShot(aS.clip);
            }
        };
        videoCamera.DeviceOpened += () =>
        {
            if (videoCamera == null)
            {
                return;
            }
            videoCamera.FocusMode = CameraDeviceFocusMode.Continousauto;
        };

    }

    private void LoadMeta(List<MapMeta> mapMetas)
    {

        int i = 0;
        foreach (var item in mapMetas)
        {
            var tempController = Instantiate(mapTemp).GetComponent<SparseSpatialMapController>();

            tempController.SourceType = SparseSpatialMapController.DataSource.MapManager;
            tempController.MapManagerSource.ID = item.Map.ID;
            tempController.MapManagerSource.Name = item.Map.Name;
            tempController.MapWorker = mapWorker;

            tempController.MapLoad += (arg1, arg2, arg3) =>
            {
                Debug.Log("\t下载信息：" + (arg2 ? "成功" : "失败：" + arg3) + Environment.NewLine +
                    "\t名称" + arg1.Name + Environment.NewLine +
                    "\tID" + arg1.ID + Environment.NewLine);
                mapName = arg1.Name;
            };
            tempController.MapLocalized += () => { Debug.Log("\t定位成功: " + mapName); };
            tempController.MapStopLocalize += () => { Debug.Log("\t定位停止 " + mapName); };
            tempController.PointCloudParticleParameter.StartColor = color32s[i];
            MapControllers.Add(tempController);
            i++;
        }
        mapWorker.Localizer.startLocalization();
    }

    private string mapName;

    public void BackMain()
    {
        SceneManager.LoadScene("MainMapScene");
    }


    public void OpenPoint(bool flag)
    {
        foreach (var item in mapSession.Maps)
        {
            item.Controller.ShowPointCloud = flag;
        }
    }

    private void DestroySession()
    {
        if (mapSession != null)
        {
            mapSession.Dispose();
            mapSession = null;
        }
    }

    private void OnDestroy()
    {
        DestroySession();
    }


    public void Snapshot()
    {
        var oneShot = Camera.main.gameObject.AddComponent<OneShot>();
        oneShot.Shot(false, (texture) =>
        {
            if (capturedImage)
            {
                Destroy(capturedImage);
            }
            capturedImage = texture;
            PreviewImage.texture = capturedImage;
            Destroy(oneShot);
        });
    }


    public void SavePic(RawImage texture)
    {

        Texture texture1 = texture.texture;
        Texture2D texture2D = new Texture2D(texture1.width, texture1.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture1.width, texture1.height, 32);
        Graphics.Blit(texture1, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);

        string destination;
#if UNITY_EDITOR
        destination = Application.persistentDataPath + "/Screenshots";
#elif UNITY_ANDROID
        destination = "/mnt/sdcard/DCIM/Screenshots";
#endif

        if (!System.IO.Directory.Exists(destination))
        {
            System.IO.Directory.CreateDirectory(destination);
        }
        destination = destination + "/" + System.DateTime.Now.ToFileTime() + ".PNG";
        //保存文件  
        Debug.Log("路径：" + destination);
        File.WriteAllBytes(destination, texture2D.EncodeToPNG());
    }



}
