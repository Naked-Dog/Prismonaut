public interface IPlatform
{
    PlatformType PlatformType { get; set; }

    public void PlatformAction(Player2DController player2DController);
}