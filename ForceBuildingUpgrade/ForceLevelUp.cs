using ICities;
using UnityEngine;

namespace ForceLevelUp
{
    public class ForceLevelUp : LoadingExtensionBase, IUserMod
    {
        private static GameObject gameObject;
        
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (mode != LoadMode.NewGame && mode != LoadMode.LoadGame)
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

        public string Name
        {
            get { return "Force Level Up"; }
        }

        public string Description
        {
            get { return "Adds button to force buildings to level up"; }
        }
    }
}
