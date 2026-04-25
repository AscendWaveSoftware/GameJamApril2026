using UnityEngine;

public class RandomAsset : MonoBehaviour
{
    [SerializeField] GameObject[] m_objects;


    void Start()
    {
        if (m_objects != null)
        {
            Instantiate(m_objects[Random.Range(0, m_objects.Length)],this.transform);
        }
    }
}
