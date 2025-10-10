using System;
using CleanSolution.Services;



public class ConsolePrinter : IPrinter
{
    public void Write(string message) => Console.Write(message);
    public void WriteLine(string? message = null) => Console.WriteLine(message);
}