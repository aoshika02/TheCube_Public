using UnityEngine;

public class TitleRoute : MonoBehaviour
{
    public Transform[] Transforms => _transforms;
    [SerializeField] private Transform[] _transforms;
}
