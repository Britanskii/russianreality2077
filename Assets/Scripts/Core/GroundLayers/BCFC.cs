using System;
using System.Collections.Generic;
using Core.GlobalFunctions;
using Core.GroundLayers;
using UnityEngine;
using UnityEngine.UI;

public class Bcfc : MonoBehaviour
{
    public static Bcfc Instance;

    public VideoLayer cinematic;
    public Layer background;
    public Layer foreground;

    private void Awake()
    {
        Instance = this;
    }
}