using UnityEngine;

namespace PlayerSystem
{
    // General Events
    public class OnUpdate { }
    public struct OnFixedUpdate { }
    public struct OnLateUpdate { }
    public struct OnCollisionEnter2D
    {
        public Collision2D collision;
        public OnCollisionEnter2D(Collision2D collision) { this.collision = collision; }
    }
    public struct OnCollisionStay2D
    {
        public Collision2D collision;
        public OnCollisionStay2D(Collision2D collision) { this.collision = collision; }
    }
    public struct CollisionExit2D
    {
        public Collision2D collision;
        public CollisionExit2D(Collision2D collision) { this.collision = collision; }
    }

    public struct PlayPlayerSounEffect
    {
        public string clipName;
        public float volume;
        public PlayPlayerSounEffect(string clipName, float volume = 1)
        {
            this.clipName = clipName;
            this.volume = volume;
        }
    }
}