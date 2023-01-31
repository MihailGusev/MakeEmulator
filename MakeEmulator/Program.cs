using MakeEmulator.Graph;

if (args.Length < 1) {
    Console.WriteLine("You must provide name of the task to run");
    return;
}

var path = Path.Combine(Directory.GetCurrentDirectory(), "makefile");

var parseResult = TaskGraph.ParseMakefile(path);
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
    // According to the document, we need to display task name as well
    // but there are only actions displayed in the example
    Console.WriteLine($"Task {task.Name}");
    foreach(var action in task.Actions) {
        Console.WriteLine($"\t{action}");
    }
}
