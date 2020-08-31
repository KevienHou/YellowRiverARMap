//================================================================================================================================
//
//  Copyright (c) 2015-2020 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

using UnityEngine;
using UnityEngine.Networking;

namespace SpatialMap_SparseSpatialMap
{
    public class MapMetaManager
    {
        public enum FileNameType
        {
            ID, Name
        }


        private static readonly string root = Application.persistentDataPath + "/SparseSpatialMap";
        private static readonly string rootUrl = @"http://152.136.221.124:8012/YellowRiverARMap/SparseSpatialMap";



        public static List<MapMeta> LoadAll()
        {
            Debug.Log(root);
            var metas = new List<MapMeta>();
            var dirRoot = GetRootPath();
            try
            {
                foreach (var path in Directory.GetFiles(dirRoot, "*.meta"))
                {
                    try
                    {
                        metas.Add(JsonUtility.FromJson<MapMeta>(File.ReadAllText(path)));
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
            return metas;
        }


        public static bool Save(MapMeta meta, FileNameType fileNameType = FileNameType.ID)
        {
            try
            {
                switch (fileNameType)
                {
                    case FileNameType.ID:
                        File.WriteAllText(GetPath(meta.Map.ID), JsonUtility.ToJson(meta, true));
                        break;
                    case FileNameType.Name:
                        File.WriteAllText(GetPath(meta.Map.Name), JsonUtility.ToJson(meta, true));
                        break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
            return true;
        }


        public static bool Delete(MapMeta meta)
        {
            if (!File.Exists(GetPath(meta.Map.ID)))
            {
                return false;
            }
            try
            {
                File.Delete(GetPath(meta.Map.ID));
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
            return true;
        }

        private static string GetRootPath()
        {
            var path = root;
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        private static string GetPath(string id)
        {
            return GetRootPath() + "/" + id + ".meta";
        }
    }
}
