using UnityEngine;

namespace Enemy
{
    public class EnemyProjectile : MonoBehaviour
    {
        private Vector3 _direction;
        private float _speed;
        private float _remainingLifetime;

        public void Initialize(Vector3 direction, float speed, float lifetime)
        {
            _direction = direction;
            _speed = speed;
            _remainingLifetime = lifetime;
        }

        private void Update()
        {
            transform.position += _direction * (_speed * Time.deltaTime);

            _remainingLifetime -= Time.deltaTime;
            if (_remainingLifetime <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}

