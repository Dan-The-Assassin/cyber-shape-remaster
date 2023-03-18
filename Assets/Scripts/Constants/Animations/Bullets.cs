using UnityEngine;

namespace Constants.Animations
{
    public static class Bullets
    {
        public static readonly int ChangeType = Animator.StringToHash("ChangeType");
        public static readonly int Idle = Animator.StringToHash("Idle");
    
        public static class Triggers
        {
            public static readonly int ChangeType = Animator.StringToHash("ChangeType");
        }
    }
}