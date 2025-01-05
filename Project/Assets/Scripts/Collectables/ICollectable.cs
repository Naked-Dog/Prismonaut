using UnityEngine;
using UnityEngine.Events;

public interface ICollectable
{
    CollectableType CollectableType { get; set; }
    public void Collect();
    public void OnTriggerEnter2D(Collider2D collision);
}
