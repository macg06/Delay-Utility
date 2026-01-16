using MACG.Utility;
using UnityEngine;

public class UsageExample : MonoBehaviour
{
    private Transform ExampleTransform;
    private void Start()
    {
        Delays.RunInNextFrame(() => {Debug.Log("Initialized something in the second frame!");});
        Delays.RunInNextFrame(() => {Debug.Log("Initialized something in the 5th frame!");}, 5);
        
        // Checking distance between this transform, and another for exactly 10 seconds.
        Delays.RunForSeconds(() =>
        {
            float dist = Vector3.Distance(transform.position, ExampleTransform.position);
            if(dist <= 1) Debug.Log("Within 1 meter!");
        }, 10);

        // Changing volume to 0 for 1 second. This will persist between scenes. If the scene is changed while this is running, the audio will keep turning down.
        Delays.RunForSeconds((() =>
        {
            while (AudioListener.volume >= 0)
            {
                AudioListener.volume = Mathf.MoveTowards(AudioListener.volume, 0, Time.deltaTime);
            }
        }), 1, true);
    }
}
