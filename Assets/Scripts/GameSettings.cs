public enum PlayerMode
{
    Player1,   // Игрок выбрал только Левую платформу
    Player2,  // Игрок выбрал только Правую платформу
}

public class GameSettings
{
    // Храним текущий выбор игрока (по умолчанию, например, на двоих)
    public PlayerMode SelectedMode { get; set; } = PlayerMode.Player1;
}