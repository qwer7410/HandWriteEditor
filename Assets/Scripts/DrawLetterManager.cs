﻿using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DrawLetterManager
{
    #region Instance
    private static DrawLetterManager _instance = null;
    public static DrawLetterManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new DrawLetterManager();
            }
            return _instance;
        }
    }
    #endregion

    internal const string ExportPath = "/Data/mesh_data.json";
    
    private Dictionary<string, Letter> _letterDict = null;
    internal Dictionary<string, Letter> LetterDict
    {
        get
        {
            if(_letterDict == null)
            {
                _letterDict = new Dictionary<string, Letter>();
            }
            return _letterDict;
        }
        private set
        {
            _letterDict = value;
        }
    }
    
    /// <summary>
    /// 更新字典某字母某笔画数据
    /// </summary>
    /// <param name="charactor">字母字串</param>
    /// <param name="letter">字母信息</param>
    internal void UpdateLetterDict(string charactor, Stroke stroke)
    {
        if(string.IsNullOrEmpty(charactor) || stroke == null)
        {
            Debug.LogError(string.Format("非法数据.charactor={0},stroke={1}", charactor, stroke));
            return;
        }
        if (LetterDict.ContainsKey(charactor))
        {
            //Dictionary<byte, Stroke> dict = LetterDict[charactor].StrokeDict;
            //if(dict!=null)
            //{
            //    if(dict.ContainsKey(stroke.index))
            //    {
            //        dict[stroke.index] = stroke;
            //    }
            //    else
            //    {
            //        dict.Add(stroke.index, stroke);
            //    }
            //}

            List<Stroke> list = LetterDict[charactor].StrokeList;
            bool isContain = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].index == stroke.index)
                {
                    list[i] = stroke;
                    isContain = true;
                    break;
                }
            }
            if (!isContain)
            {
                list.Add(stroke);
            }
        }
        else
        {
            Letter letter = new Letter();
            letter.Charactor = charactor;
            //letter.StrokeDict.Add(1, stroke);
            letter.StrokeList.Add(stroke);
            LetterDict.Add(charactor, letter);
        }
    }
    /// <summary>
    /// 根据json字串更新LetterDict
    /// </summary>
    /// <param name="json"></param>
    internal void UpdateLetterDictFromJson(string json)
    {
        LetterDict = JsonMapper.ToObject<Dictionary<string, Letter>>(json);
        if (LetterDict == null || LetterDict.Count <= 0)
        {
            Debug.LogError(string.Format("LetterDict无数据"));
        }
    }
    internal Stroke GetStroke(string charactor, byte idx)
    {
        if (LetterDict != null && LetterDict.ContainsKey(charactor))
        {
            Letter tmpLetter = LetterDict[charactor];
            //if(tmpLetter != null && tmpLetter.StrokeDict != null && tmpLetter.StrokeDict.ContainsKey(idx))
            //{
            //    return tmpLetter.StrokeDict[idx];
            //}
            if (tmpLetter != null && tmpLetter.StrokeList != null && tmpLetter.StrokeList.Count > 0)
            {
                for (int i = 0; i < tmpLetter.StrokeList.Count; i++)
                {
                    if (tmpLetter.StrokeList[i].index == idx)
                    {
                        return tmpLetter.StrokeList[i];
                    }
                }
            }
        }
        return null;
    }
}

[Serializable]
public class Letter
{//字母类
    public string Charactor;
    public List<Stroke> StrokeList = new List<Stroke>();  //该字母笔画列表
    //public Dictionary<byte, Stroke> StrokeDict = new Dictionary<byte, Stroke>();  //该字母笔画列表
}
[Serializable]
public class Stroke
{
    public byte index = 0;  //该mesh所对应字符的第几划;1基
    public int smooth = 0;  //相邻关键结点之间生成曲线结点个数，越多越平滑
    public float width = 0; //该mesh的宽度
    //public Vector2 goLclPos;    //该mesh的位置
    //public Quaternion goLclRot; //该mesh的旋转
    public List<Vector2> nodeList;    //曲线控制点列表

    //反序列化时根据nodeList算法生成↓
    public List<Vector2> curvePoints;   //曲线上的点列表
    public List<Quaternion> tangentQuaternions; //曲线上的点切线列表
    public Vector3[] vertices;  //mesh的顶点列表，保存经过mesh所在的transform旋转后的位置而非localPosition
    public int[] triangles; //mesh的三角形索引
    public Vector2[] uv;    //mesh的uv列表
    //反序列化时根据nodeList算法生成↑
}