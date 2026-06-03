namespace TaskManager.Application.Common.Types.CustomExceptions
{
    public class SaveChangesException : Exception
    {
        public SaveChangesException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }




}
