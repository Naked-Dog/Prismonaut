using UnityEngine;

public class EnemyStrikingDistanceCheck : MonoBehaviour
{
    private Enemy _enemy;

    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_enemy.StateMachine.CurrentEnemyState == _enemy.StunState)
        {
            _enemy.SetAggroStatus(false);
            return;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            _enemy.SetStrikingDistanceBool(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _enemy.SetStrikingDistanceBool(false);
        }
    }
}
