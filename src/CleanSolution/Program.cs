using System;
using System.IO;
using MSPro.CLArgs;



const string SEARCH_PATTERN = "CleanSolution.Command*.dll";

var builder = CommandBuilder.Create();
builder.ConfigureCommands(commands =>
{
    commands.AddAssemblies(new AssemblyCommandResolver2(
                               Directory.GetFiles(Environment.CurrentDirectory, SEARCH_PATTERN, SearchOption.AllDirectories)));
});
Commander2 commander = builder.Build();
commander.Execute();