using MakeEmulator.Graph;

if (args.Length < 2) {
    Console.WriteLine("You must provide path for makefile and name of the task to run");
    return;
}

var parseResult = TaskGraph.ParseMakefile(args[0]);
if (!parseResult.IsSuccess) {
    Console.WriteLine(parseResult.Error);
    return;
}

var buildResult = parseResult.Value!.Build(args[1]);
if (!buildResult.IsSuccess) {
    Console.WriteLine(buildResult.Error);
    return;
}

foreach (var task in buildResult.Value!) {
    Console.WriteLine($"Task {task.Name}");
    foreach(var action in task.Actions) {
        Console.WriteLine($"\t{action}");
    }
}
