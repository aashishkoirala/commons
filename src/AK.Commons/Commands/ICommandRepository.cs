namespace AK.Commons.Commands
{
    public interface ICommandRepository
    {
        string[] ListEligibleIds();
        ICommand Get(string id);
        void Put(string id, ICommand command);
        string NextId();
    }
}