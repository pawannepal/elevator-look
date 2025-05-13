using ElevatorSimulation;

Console.WriteLine("Elevator Simulation using LOOK Algorithm");
Console.WriteLine("----------------------------------------");

var elevator = new Elevator(10, 3);

await elevator.StartAsync();

while (true)
{
    Console.WriteLine($"\nCurrent elevator position: {Utilities.FormatFloorName(elevator.CurrentFloor)}");
    Console.WriteLine($"Current direction: {(elevator.CurrentDirection == Direction.Up ? "UP" : "DOWN")}");
    Console.WriteLine("\nOptions:");
    Console.WriteLine("1. Request elevator to a floor");
    Console.WriteLine("2. Add destination from inside elevator");
    Console.WriteLine("3. Exit simulation");
    Console.Write("\nEnter your choice: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.Write("Enter floor (B3-B1 for basements, 1-10 for floors): ");
            var floorInput = Console.ReadLine()?.Trim() ?? "";

            if (Utilities.TryParseFloorInput(floorInput, elevator.BasementsCount, elevator.FloorsCount, out int requestFloor))
            {
                elevator.AddRequest(requestFloor);
                Console.WriteLine($"Elevator requested to {Utilities.FormatFloorName(requestFloor)}");
            }
            else
            {
                Console.WriteLine($"Invalid floor. Valid range is B{elevator.BasementsCount}-B1 or 1-{elevator.FloorsCount}");
            }
            break;

        case "2":
            Console.Write("Enter destination floor (B3-B1 for basements, 1-10 for floors): ");
            var destInput = Console.ReadLine()?.Trim() ?? "";

            if (Utilities.TryParseFloorInput(destInput, elevator.BasementsCount, elevator.FloorsCount, out int destFloor))
            {
                elevator.AddRequest(destFloor);
                Console.WriteLine($"Added destination {Utilities.FormatFloorName(destFloor)}");
            }
            else
            {
                Console.WriteLine($"Invalid floor. Valid range is B{elevator.BasementsCount}-B1 or 1-{elevator.FloorsCount}");
            }
            break;

        case "3":
            Console.WriteLine("Exiting simulation. Thank you!");
            await elevator.StopAsync();
            return;

        default:
            Console.WriteLine("Invalid choice. Please try again.");
            break;
    }
}