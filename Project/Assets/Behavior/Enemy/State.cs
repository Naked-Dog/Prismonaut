using System;
using Unity.Behavior;

[BlackboardEnum]
public enum State
{
    Idle,
	Wander,
	Chase,
	Attack,
	Die
}
