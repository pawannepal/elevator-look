
namespace ElevatorSimulation;

interface IElevator
{
    int CurrentFloor { get; }
    Direction CurrentDirection { get; }
    int BasementsCount { get; }
    int FloorsCount { get; }

    void AddRequest(int floor);
    Task StartAsync();
    Task StopAsync();
}