using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MACG.Utility
{
    public static class Delays
    {
        // Creates object that handles all delays, and makes sure it persists between scenes.
        [RuntimeInitializeOnLoadMethod]
        public static void InitializeDelayHandler()
        {
            DelayHandler handler = new GameObject().AddComponent<DelayHandler>();
            GameObject.DontDestroyOnLoad(handler.gameObject);
            GameObject obj = handler.gameObject;
            
            // This creates 50 components per delay type. Feel free to modify the number for your own needs, if you feel like this is wasteful.
            // Just so you know, in total, this is 200 components.
            for (int i = 0; i < 50; i++)
            {
                obj.AddComponent<DEL_RunInNextFrame>().enabled = false;
                obj.AddComponent<DEL_RunInSeconds>().enabled = false;
                obj.AddComponent<DEL_RunForFrames>().enabled = false;
                obj.AddComponent<DEL_RunForSeconds>().enabled = false;
            }
            handler.RunInNextFrames = handler.GetComponents<DEL_RunInNextFrame>();
            handler.RunInSeconds = handler.GetComponents<DEL_RunInSeconds>();
            handler.RunForFrames = handler.GetComponents<DEL_RunForFrames>();
            handler.RunForSeconds = handler.GetComponents<DEL_RunForSeconds>();
        }
        
        #region frames
        
        /// <param name="persist">Use this only for persistent things, like global managers or objects that use DontDestroyOnLoad. If this is used wrong, Unity will freak out and throw errors.</param>
        public static void RunInNextFrame(Action action, int frames = 1, bool persist = false)
        {
            DEL_RunInNextFrame comp = DelayHandler.Instance.GetRunInNextFrame();
            if (comp == null) return;
            comp.action = action;
            comp.frames = frames;
            comp.persist = persist;
        }

        /// <param name="persist">Use this only for persistent things, like global managers or objects that use DontDestroyOnLoad. If this is used wrong, Unity will freak out and throw errors.</param>
        public static void RunForFrames(Action action, int frames, bool persist = false)
        {
            DEL_RunForFrames comp = DelayHandler.Instance.GetRunForFrames();
            if (comp == null) return;
            comp.action = action;
            comp.frames = frames;
            comp.persist = persist;
        }
        #endregion

        #region seconds
        
        /// <param name="persist">Use this only for persistent things, like global managers or objects that use DontDestroyOnLoad. If this is used wrong, Unity will freak out and throw errors.</param>
        public static void RunInSeconds(Action action, float seconds, bool persist = false)
        {
            DEL_RunInSeconds comp = DelayHandler.Instance.GetRunInSeconds();
            if (comp == null) return;
            comp.action = action;
            comp.seconds = seconds;
            comp.persist = persist;
        }
        
        /// <param name="persist">Use this only for persistent things, like global managers or objects that use DontDestroyOnLoad. If this is used wrong, Unity will freak out and throw errors.</param>
        public static void RunForSeconds(Action action, float seconds, bool persist = false)
        {
            DEL_RunForSeconds comp = DelayHandler.Instance.GetRunForSeconds();
            if (comp == null) return;
            comp.action = action;
            comp.seconds = seconds;
            comp.persist = persist;
        }
        #endregion
    }
    
    public class DelayHandler : MonoBehaviour
    {
        public DEL_RunInNextFrame[] RunInNextFrames;
        public DEL_RunInSeconds[] RunInSeconds;
        public DEL_RunForFrames[] RunForFrames;
        public DEL_RunForSeconds[] RunForSeconds;

        public static DelayHandler Instance;

        #region  get avaialble

        public DEL_RunInNextFrame GetRunInNextFrame()
        {
            for (int i = 0; i < RunInNextFrames.Length; i++)
            {
                if (RunInNextFrames[i].enabled) continue;

                RunInNextFrames[i].enabled = true; 
                return RunInNextFrames[i];
            }

            Debug.LogError("Can't return a 'RunInNextFrame'. All are used. ._.");
            return null;
        }

        public DEL_RunInSeconds GetRunInSeconds()
        {
            for (int i = 0; i < RunInSeconds.Length; i++)
            {
                if (RunInSeconds[i].enabled) continue;

                RunInSeconds[i].enabled = true;
                return RunInSeconds[i];
            }

            Debug.LogError("Can't return a 'RunInSeconds'. All are used. ._.");
            return null;
        }

        public DEL_RunForFrames GetRunForFrames()
        {
            for (int i = 0; i < RunForFrames.Length; i++)
            {
                if (RunForFrames[i].enabled) continue;

                RunForFrames[i].enabled = true;
                return RunForFrames[i];
            }

            Debug.LogError("Can't return a 'RunForFrames'. All are used. ._.");
            return null;
        }

        public DEL_RunForSeconds GetRunForSeconds()
        {
            for (int i = 0; i < RunForSeconds.Length; i++)
            {
                if (RunForSeconds[i].enabled) continue;

                RunForSeconds[i].enabled = true;
                return RunForSeconds[i];
            }

            Debug.LogError("Can't return a 'RunForSeconds'. All are used. ._.");
            return null;
        }

        #endregion

        void OnEnable()
        {
            Instance = this;
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        void OnDisable() => SceneManager.activeSceneChanged -= OnSceneChanged;
        
        // Main scene changed? we reset all delays. (as long as they're not set to persist!)
        // If we don't, the handler will try to set delays and run actions that don't exist anymore.
        // It's ideal this way.
        void OnSceneChanged(Scene oldScene, Scene newScene) => ResetAll();
        
        private void ResetAll()
        {
            for (int i = 0; i < RunInNextFrames.Length; i++)
                if(!RunInNextFrames[i].persist) RunInNextFrames[i].enabled = false;
            
            for (int i = 0; i < RunInSeconds.Length; i++)
                if(!RunInSeconds[i].persist) RunInSeconds[i].enabled = false;
            
            for (int i = 0; i < RunForFrames.Length; i++)
                if(!RunForFrames[i].persist) RunForFrames[i].enabled = false;
            
            for (int i = 0; i < RunForSeconds.Length; i++)
                if(!RunForSeconds[i].persist) RunForSeconds[i].enabled = false;
        }
    }
    
    public class DEL_RunInNextFrame : MonoBehaviour
    {
        public Action action;
        public int frames;
        public bool persist;

        private void Update()
        {
            frames--;

            if (frames <= 0)
            {
                action?.Invoke();
                enabled = false;
            }
        }
        
        private void OnDisable() => action = null;
    }
    public class DEL_RunForFrames : MonoBehaviour
    {
        public Action action;
        public int frames;
        public bool persist;

        private void Update()
        {
            action?.Invoke();
            frames--;

            if (frames <= 0) enabled = false;
        }
        
        private void OnDisable() => action = null;
    }


    public class DEL_RunInSeconds : MonoBehaviour
    {
        public Action action;
        public float seconds;
        public bool persist;
        
        private void Update()
        {
            seconds -= Time.deltaTime;
            if (seconds <= 0)
            {
                action?.Invoke();
                enabled = false;
            }
        }
        
        private void OnDisable() => action = null;
    }
    public class DEL_RunForSeconds : MonoBehaviour
    {
        public Action action;
        public float seconds;
        public bool persist;

        private void Update()
        {
            action?.Invoke();
            seconds -= Time.deltaTime;

            if (seconds <= 0) enabled = false;
        }

        private void OnDisable() => action = null;
    }
}
