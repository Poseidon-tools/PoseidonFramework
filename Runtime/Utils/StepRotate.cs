using UnityEngine;

namespace Utils
{
    public class StepRotate : MonoBehaviour
    {
        [SerializeField] private Vector3 axis = new Vector3(0, 0, 360);
        [SerializeField] private float angle = 30.0f;
        [SerializeField] private float timeStep = 0.133f;

        private float currentTime;

        private void Update()
        {
            currentTime += Time.deltaTime;

            if (!(currentTime >= timeStep)) return;
        
            transform.Rotate(axis, angle);
            currentTime -= timeStep;
        }
    }
}
