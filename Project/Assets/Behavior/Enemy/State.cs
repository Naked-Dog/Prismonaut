using System;
using Unity.Behavior;

[BlackboardEnum]
public enum State
{
    Idle,
	Chase,
	Charge,
	Rush,
	StopRush,
	Flinch,
	Die
}
