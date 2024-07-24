namespace GMF
{
    public static class GameInitializer
    {
        public static void Initialize()
        {
            var manager = Services.GetService<IGameStateManager>();
            manager.ChangeState(new InitializationState());
        }
    }
}

