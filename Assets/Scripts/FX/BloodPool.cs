using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodPool : MonoBehaviour
{
    public enum BloodColor { Red, Green, Yellow, Brown }

    public static GameObject BloodPoolObject;

    public int PoolSize = 1000;
    public GameObject Module;
    public static GameObject _Module;
    public Material _Red;
    public Material _Green;
    public Material _Yellow;
    public Material _Brown;

    public static Material Red;
    public static Material Green;
    public static Material Yellow;
    public static Material Brown;

    [ReadOnlyField] public static List<BloodModule> bloodModules = new List<BloodModule>();

	void Start ()
    {
        _Module = Module;
        Red = _Red;
        Green = _Green;
        Yellow = _Yellow;
        Brown = _Brown;

        bloodModules.Clear();
        BloodPoolObject = gameObject;
        
        for (int i = 0; i < PoolSize; ++i)
        {
            var module = Instantiate(Module);
            bloodModules.Add(module.GetComponent<BloodModule>());
        }
    }

    public static void Splatter(Vector3 Point, int Amount, BloodColor Color)
    {
        //I hate C#
        for (int i = 0; i < bloodModules.Count; i++)//BloodModule module in bloodModules)
        {
            if (!bloodModules[i])
            {
                var module1 = Instantiate(_Module);
                bloodModules[i] = module1.GetComponent<BloodModule>();
            }
        }

        for (int i = 0; i < Amount; ++i)
        {
            foreach (BloodModule module in bloodModules)
            {
                if(!module.gameObject.activeSelf)
                {
                    if (Color == BloodColor.Green)
                        module.MatObj.material = Green;
                    else if (Color == BloodColor.Red)
                        module.MatObj.material = Red;
                    else if (Color == BloodColor.Yellow)
                        module.MatObj.material = Yellow;
                    else if (Color == BloodColor.Brown)
                        module.MatObj.material = Brown;

                    module.transform.position = Point;
                    module.gameObject.SetActive(true);
                    module.Instantiate();
                    break;
                }
            }
        }
    }

    public void FixAllBlood()
    {
        foreach(BloodModule module in bloodModules)
        {
            module.transform.parent = gameObject.transform;
            module.gameObject.SetActive(false);
        }
    }
}