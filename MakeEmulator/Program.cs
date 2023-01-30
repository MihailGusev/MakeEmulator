using MakeEmulator.Graph;

if (args.Length < 2) {
    Console.WriteLine("You must provide path for makefile and Name of the task to run");
    return;
}

if (TaskGraph.TryParseMakefile(args[0], out var graph, out var errorMessage)) {
    Console.WriteLine(graph!.Build(args[1]));
}
else {
    Console.WriteLine(errorMessage);
}