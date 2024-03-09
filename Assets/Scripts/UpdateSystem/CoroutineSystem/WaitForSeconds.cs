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
            
        public bool CanPerformNow()
        {
            timer -= Time.deltaTime;
            return timer <= 0;
        }
    }
}