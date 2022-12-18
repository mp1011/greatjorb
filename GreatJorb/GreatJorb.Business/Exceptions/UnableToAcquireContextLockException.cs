namespace GreatJorb.Business.Exceptions
{
    public class UnableToAcquireContextLockException : Exception
    {
        public UnableToAcquireContextLockException(Exception? innerException = null) : base("Unable to acquire a Context Lock", innerException) { }
    }
}
