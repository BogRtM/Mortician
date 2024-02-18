using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCurve : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
}
