using ICities;
using UnityEngine;

namespace ForceLevelUp
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private static GameObject gameObject;
        
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (mode != LoadMode.NewGame && mode != LoadMode.LoadGame && mode != LoadMode.NewGameFromScenario)
            {
                return;
            }

            if (gameObject != null)
            {
                return;
            }
            gameObject = new GameObject("ForceLevelupPanelExtender");
            gameObject.AddComponent<GamePanelExtender>();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            if (gameObject == null)
            {
                return;
            }
            Object.Destroy(gameObject);
            gameObject = null;
        }


    }
}
