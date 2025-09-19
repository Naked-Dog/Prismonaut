using System;
using Unity.Behavior;

[BlackboardEnum]
public enum State
{
	Awake,
	Walk,
	Chase,
	Charge,
	Rush,
	StopRush,
	Flinch,
	Restore
}
