#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414

using System;
using UnityEngine;

public class TouchModel : ApplicationElement
{
    // static states
    public static byte Idle = 0;
    public static byte Swiping = 1;
    public static byte Pinching = 2;
    public static byte Elevating = 4;

    // By how much?
    public float PinchAmount { get; set; }
    public byte CurrentState { get; set; }
    public Vector2 SwipeAmount { get; set; }

    // Location at the screen where the user is long pressing at
    public Vector2 TouchPosition { get; set; }
}