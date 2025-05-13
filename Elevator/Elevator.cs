namespace ElevatorSimulation;

class Elevator : IElevator
{
    public int CurrentFloor { get; private set; }
    public Direction CurrentDirection { get; private set; }
    public int BasementsCount { get; }
    public int FloorsCount { get; }

    private readonly SortedSet<int> _upRequests = new();
    private readonly SortedSet<int> _downRequests = new();
    private readonly int _totalFloors;
    private bool _isRunning;
    private Task? _elevatorTask;
    private readonly CancellationTokenSource _cts = new();

    public Elevator(int floorsCount, int basementsCount)
    {
        BasementsCount = basementsCount;
        FloorsCount = floorsCount;
        _totalFloors = floorsCount + basementsCount;

        CurrentFloor = 0;
        CurrentDirection = Direction.Up;
    }

    public void AddRequest(int floor)
    {
        int lowestFloor = -BasementsCount;
        int highestFloor = FloorsCount - 1;

        if (floor < lowestFloor || floor > highestFloor)
        {
            Console.WriteLine($"Invalid floor request: {floor}. Valid range is {lowestFloor} to {highestFloor}");
            return;
        }

        if (floor == CurrentFloor)
        {
            Console.WriteLine($"Elevator is already at this floor");
            return;
        }

        if (floor > CurrentFloor)
        {
            _upRequests.Add(floor);
        }
        else
        {
            _downRequests.Add(floor);
        }
    }

    public Task StartAsync()
    {
        if (!_isRunning)
        {
            _isRunning = true;
            _elevatorTask = RunAsync(_cts.Token);
            Console.WriteLine("Elevator service started");
        }

        return Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        if (_isRunning)
        {
            _isRunning = false;
            _cts.Cancel();

            if (_elevatorTask is not null)
            {
                try
                {
                    await _elevatorTask;
                }
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    _cts.Dispose();
                }
            }

            Console.WriteLine("Elevator service stopped");
        }
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await ProcessRequestsAsync(cancellationToken);

            try
            {
                await Task.Delay(1000, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    private async Task ProcessRequestsAsync(CancellationToken cancellationToken)
    {
        if (!_upRequests.Any() && !_downRequests.Any())
        {
            return;
        }

        if (CurrentDirection == Direction.Up)
        {
            if (_upRequests.Any())
            {
                await MoveToFloorAsync(_upRequests.Min(), cancellationToken);
                _upRequests.Remove(CurrentFloor);
            }
            else if (_downRequests.Any())
            {
                CurrentDirection = Direction.Down;
            }
        }
        else
        {
            if (_downRequests.Any())
            {
                await MoveToFloorAsync(_downRequests.Max(), cancellationToken);
                _downRequests.Remove(CurrentFloor);
            }
            else if (_upRequests.Any())
            {
                CurrentDirection = Direction.Up;
            }
        }

        if (CurrentDirection == Direction.Up && !_upRequests.Any() && _downRequests.Any())
        {
            CurrentDirection = Direction.Down;
        }
        else if (CurrentDirection == Direction.Down && !_downRequests.Any() && _upRequests.Any())
        {
            CurrentDirection = Direction.Up;
        }
    }

    private async Task MoveToFloorAsync(int targetFloor, CancellationToken cancellationToken)
    {
        if (targetFloor == CurrentFloor) return;

        Direction moveDirection = targetFloor > CurrentFloor ? Direction.Up : Direction.Down;

        Console.WriteLine($"\nElevator moving {moveDirection} from {FormatFloorName(CurrentFloor)} to {FormatFloorName(targetFloor)}");

        while (CurrentFloor != targetFloor)
        {
            try
            {
                await Task.Delay(3000, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }

            CurrentFloor = moveDirection == Direction.Up ? CurrentFloor + 1 : CurrentFloor - 1;

            string floorName = FormatFloorName(CurrentFloor);
            Console.WriteLine($"Passing {floorName}");
        }

        Console.WriteLine($"Elevator arrived at {FormatFloorName(CurrentFloor)}");
        Console.WriteLine("Doors opening... Please enter or exit");

        try
        {
            await Task.Delay(2000, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }

        Console.WriteLine("Doors closing");
    }

    private string FormatFloorName(int internalFloor)
    {
        if (internalFloor < 0)
        {
            return $"Floor B{Math.Abs(internalFloor)}";
        }
        else
        {
            return $"Floor {internalFloor + 1}";
        }
    }
}