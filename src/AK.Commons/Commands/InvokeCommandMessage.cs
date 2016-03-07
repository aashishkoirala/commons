namespace AK.Commons.Commands
{
    internal class InvokeCommandRequest
    {
        public string Id { get; }
        public int Attempt { get; set; }

        public InvokeCommandRequest(string id, int attempt = 1)
        {
            this.Id = id;
            this.Attempt = 1;
        }
    }
}