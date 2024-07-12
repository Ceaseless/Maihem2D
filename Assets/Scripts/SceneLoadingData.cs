namespace Maihem
{
    public static class SceneLoadingData
    {
        // These have to have the exact same name as the scene files
        public enum LoadableScene
        {
            MainMenu,
            GameScene,
            LoadingScene
        }

        public static LoadableScene SceneToLoad;
    }
}