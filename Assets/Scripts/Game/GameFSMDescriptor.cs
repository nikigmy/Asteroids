using Utils;

namespace Game
{
    public sealed class GameFSMDescriptor : FSMDescriptor
    {
        public enum Action
        {
            ViewMainMenu,
            StartGame,
            StartGameOverCeremony,
            StartGameCompletedCeremony
        }

        public enum State
        {
            Initialising,
            MainMenu,
            Game,
            GameOver,
            GameCompleted
        }

        public GameFSMDescriptor()
        {
            AddTransitionInfo(Action.ViewMainMenu, State.Initialising, State.MainMenu);
            AddTransitionInfo(Action.ViewMainMenu, State.Game, State.MainMenu);
            AddTransitionInfo(Action.ViewMainMenu, State.GameOver, State.MainMenu);
            AddTransitionInfo(Action.ViewMainMenu, State.GameCompleted, State.MainMenu);

            AddTransitionInfo(Action.StartGame, State.MainMenu, State.Game);
            AddTransitionInfo(Action.StartGame, State.GameOver, State.Game);
            AddTransitionInfo(Action.StartGame, State.GameCompleted, State.Game);

            AddTransitionInfo(Action.StartGameOverCeremony, State.Game, State.GameOver);
            AddTransitionInfo(Action.StartGameCompletedCeremony, State.Game, State.GameCompleted);
        }
    }
}