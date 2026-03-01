using System;
using System.Collections.Generic;
using System.Linq;
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
            DelayHandler.Instance = handler;
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

            handler.RunInNextFrames = handler.GetComponents<DEL_RunInNextFrame>().ToList();
            handler.RunInSeconds = handler.GetComponents<DEL_RunInSeconds>().ToList();
            handler.RunForFrames = handler.GetComponents<DEL_RunForFrames>().ToList();
            handler.RunForSeconds = handler.GetComponents<DEL_RunForSeconds>().ToList();
        }
        
        #region frames
        
        /// <param name="persist">Use this only for persistent things, like global managers or objects that use DontDestroyOnLoad. If this is used wrong, Unity will freak out and throw errors.</param>
        public static DEL_RunInNextFrame RunInNextFrame(Action action, int frames = 1, bool persist = false)
        {
            DEL_RunInNextFrame comp = DelayHandler.Instance.GetRunInNextFrame();
            if (comp == null) return null;
            comp.action = action;
            comp.frames = frames;
            comp.persist = persist;

            return comp;
        }

        /// <param name="persist">Use this only for persistent things, like global managers or objects that use DontDestroyOnLoad. If this is used wrong, Unity will freak out and throw errors.</param>
        public static DEL_RunForFrames RunForFrames(Action action, int frames = 1, bool persist = false)
        {
            DEL_RunForFrames comp = DelayHandler.Instance.GetRunForFrames();
            if (comp == null) return null;
            comp.action = action;
            comp.frames = frames;
            comp.persist = persist;

            return comp;
        }
        #endregion

        #region seconds
        
        /// <param name="persist">Use this only for persistent things, like global managers or objects that use DontDestroyOnLoad. If this is used wrong, Unity will freak out and throw errors.</param>
        public static DEL_RunInSeconds RunInSeconds(Action action, float seconds = 1, bool persist = false)
        {
            DEL_RunInSeconds comp = DelayHandler.Instance.GetRunInSeconds();
            if (comp == null) return null;
            comp.action = action;
            comp.seconds = seconds;
            comp.persist = persist;

            return comp;
        }
        
        /// <param name="persist">Use this only for persistent things, like global managers or objects that use DontDestroyOnLoad. If this is used wrong, Unity will freak out and throw errors.</param>
        public static DEL_RunForSeconds RunForSeconds(Action action, float seconds = 1, bool persist = false)
        {
            DEL_RunForSeconds comp = DelayHandler.Instance.GetRunForSeconds();
            if (comp == null) return null;
            comp.action = action;
            comp.seconds = seconds;
            comp.persist = persist;

            return comp;
        }
        #endregion
    }
    
    public class DelayHandler : MonoBehaviour
    {
        public List<DEL_RunInNextFrame> RunInNextFrames = new List<DEL_RunInNextFrame>();
        public List<DEL_RunInSeconds> RunInSeconds = new List<DEL_RunInSeconds>();
        public List<DEL_RunForFrames> RunForFrames = new List<DEL_RunForFrames>();
        public List<DEL_RunForSeconds> RunForSeconds = new List<DEL_RunForSeconds>();

        // This defines how many new components it adds when the limit is reached.
        // You *could* make this 1 if you want.
        // The reason I made it 5 is because it might stutter a bit less if it creates new components in advance, if you have a lot of delays planned at the same time.
        private const int AUTO_GROW_COUNT = 5;

        public static DelayHandler Instance;

        #region  get avaialble

        public DEL_RunInNextFrame GetRunInNextFrame()
        {
            for (int i = 0; i < RunInNextFrames.Count; i++)
            {
                if (RunInNextFrames[i].enabled) continue;

                RunInNextFrames[i].enabled = true; 
                return RunInNextFrames[i];
            }

            Debug.LogError($"Reached limit for ''DEL_RunInNextFrame''. We're adding {AUTO_GROW_COUNT} new components.");

            DEL_RunInNextFrame delayToReturn = null;
            for (int i = 0; i < AUTO_GROW_COUNT; i++)
            {
                DEL_RunInNextFrame newDelay = gameObject.AddComponent<DEL_RunInNextFrame>();
                RunInNextFrames.Add(newDelay);
                newDelay.enabled = false;
                if (i == 0) delayToReturn = newDelay;
            }

            delayToReturn.enabled = true;
            return delayToReturn;
        }

        public DEL_RunInSeconds GetRunInSeconds()
        {
            for (int i = 0; i < RunInSeconds.Count; i++)
            {
                if (RunInSeconds[i].enabled) continue;

                RunInSeconds[i].enabled = true;
                return RunInSeconds[i];
            }

            Debug.LogError($"Reached limit for ''DEL_RunInSeconds''. We're adding {AUTO_GROW_COUNT} new components.");

            DEL_RunInSeconds delayToReturn = null;
            for (int i = 0; i < AUTO_GROW_COUNT; i++)
            {
                DEL_RunInSeconds newDelay = gameObject.AddComponent<DEL_RunInSeconds>();
                RunInSeconds.Add(newDelay);
                newDelay.enabled = false;
                if(i == 0) delayToReturn = newDelay;
            }

            delayToReturn.enabled = true;
            return delayToReturn;
        }

        public DEL_RunForFrames GetRunForFrames()
        {
            for (int i = 0; i < RunForFrames.Count; i++)
            {
                if (RunForFrames[i].enabled) continue;

                RunForFrames[i].enabled = true;
                return RunForFrames[i];
            }

            Debug.LogError($"Reached limit for ''DEL_RunForFrames''. We're adding {AUTO_GROW_COUNT} new components.");

            DEL_RunForFrames delayToReturn = null;
            for (int i = 0; i < AUTO_GROW_COUNT; i++)
            {
                DEL_RunForFrames newDelay = gameObject.AddComponent<DEL_RunForFrames>();
                RunForFrames.Add(newDelay);
                newDelay.enabled = false;
                if(i == 0) delayToReturn = newDelay;
            }

            delayToReturn.enabled = true;
            return delayToReturn;
        }

        public DEL_RunForSeconds GetRunForSeconds()
        {
            for (int i = 0; i < RunForSeconds.Count; i++)
            {
                if (RunForSeconds[i].enabled) continue;

                RunForSeconds[i].enabled = true;
                return RunForSeconds[i];
            }

            Debug.LogError($"Reached limit for ''DEL_RunForSeconds''. We're adding {AUTO_GROW_COUNT} new components.");

            DEL_RunForSeconds delayToReturn = null;
            for (int i = 0; i < AUTO_GROW_COUNT; i++)
            {
                DEL_RunForSeconds newDelay = gameObject.AddComponent<DEL_RunForSeconds>();
                RunForSeconds.Add(newDelay);
                newDelay.enabled = false;
                if(i == 0) delayToReturn = newDelay;
            }

            delayToReturn.enabled = true;
            return delayToReturn;
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
            for (int i = 0; i < RunInNextFrames.Count; i++)
                if(!RunInNextFrames[i].persist) RunInNextFrames[i].enabled = false;
            
            for (int i = 0; i < RunInSeconds.Count; i++)
                if(!RunInSeconds[i].persist) RunInSeconds[i].enabled = false;
            
            for (int i = 0; i < RunForFrames.Count; i++)
                if(!RunForFrames[i].persist) RunForFrames[i].enabled = false;
            
            for (int i = 0; i < RunForSeconds.Count; i++)
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