namespace CleanSolution.Services
{
    public interface IPrinter
    {
        /// <summary>
        /// Print a message without logging it.
        /// </summary>
        public void Write(string message);
    
        /// <summary>
        /// Print a message without logging it.
        /// </summary>
        public void WriteLine(string? message=null);
    }
}