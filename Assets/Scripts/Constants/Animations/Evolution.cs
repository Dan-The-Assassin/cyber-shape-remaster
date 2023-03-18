using UnityEngine;

namespace Constants.Animations
{
    public static class Evolution
    {
        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int Evolve = Animator.StringToHash("Evolve");

        public static class Triggers
        {
            public static readonly int Evolution = Animator.StringToHash("Evolution");
        } 
    }
}