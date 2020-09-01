using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FindSomething : EditorWindow
{
    [MenuItem("ThursdayFrame/Tools/找东西")]
    static void AddWindow_FindSomething()
    {
        //EditorWindow window = new EditorWindow();
        EditorWindow.GetWindow<FindSomething>(false, "找东西");
    }


    string str;
    List<GameObject> objs = new List<GameObject>();



    private void OnGUI()
    {
        //文本框
        str = EditorGUILayout.TextField("输入文字", str);

        if (GUILayout.Button("搜索", GUILayout.Width(200)))
        {
            if (str.Equals(string.Empty)) 
            {
                //打开一个通知栏
                this.ShowNotification(new GUIContent("输入一些东西，例如组件名称？"));
            }
            objs.Clear();
            //GameObject[] objects = FindObjectsOfType<a>();
        }

        //EditorGUILayout.LabelField("鼠标在窗口的位置", Event.current.mousePosition.ToString());
        //texture = EditorGUILayout.ObjectField("添加贴图", texture, typeof(Texture), true) as Texture;
        if (GUILayout.Button("关闭窗口", GUILayout.Width(200)))
        {
            //关闭窗口
            this.Close();
        }


    }



}

