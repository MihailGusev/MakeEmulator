using MakeEmulator.Parsers;

if (args.Length < 1) {
    Console.WriteLine("You must provide name of the task to run");
    return;
}

var path = Path.Combine(Directory.GetCurrentDirectory(), "makefile");

var parseResult = new TaskGraphParser().Parse(path);
if (!parseResult.IsSuccess) {
    Console.WriteLine(parseResult.Error);
    return;
}

var buildResult = parseResult.Value!.Build(args[0]);
if (!buildResult.IsSuccess) {
    Console.WriteLine(buildResult.Error);
    return;
}

foreach (var task in buildResult.Value!) {
    Console.WriteLine($"Task {task.Name}");
    foreach (var action in task.Actions) {
        Console.WriteLine($"\t{action}");
    }
}
