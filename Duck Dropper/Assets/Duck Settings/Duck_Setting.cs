using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new duck setting", menuName = "Duck Setting")]
public class Duck_Setting : ScriptableObject
{
    public int dynamicHighDuckCount;
    public int dynamicDuckCount;

    [Space]

    public int staticHighDuckCount;
    public int staticDuckCount;
    public int staticBoxDuckCount;
    public int staticFlatDuckCount;
}
