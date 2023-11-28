using UnityEngine;

namespace UpdateSystem.CoroutineSystem
{
    public class WaitForSeconds : IWait
    {
        private float timer;

        public WaitForSeconds(float seconds)
        {
            timer = seconds;
        }
            
        public bool CanPerform()
        {
            timer -= Time.deltaTime;
            return timer <= 0;
        }
    }
}