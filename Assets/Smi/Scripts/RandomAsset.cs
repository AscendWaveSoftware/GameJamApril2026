using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RandomAsset : MonoBehaviour
{
    [SerializeField] Mesh[] m_meshes;
    private MeshFilter m_meshFilter;

    void Start()
    {
        m_meshFilter = GetComponent<MeshFilter>();
        if(m_meshes != null)
        m_meshFilter.mesh = m_meshes[Random.Range(0, m_meshes.Length)];
    }
}
