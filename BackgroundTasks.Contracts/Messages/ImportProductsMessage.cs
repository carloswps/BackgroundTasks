namespace BackgroundTasks.Contracts.Messages;

public sealed class ImportProductMessage
{
    public Guid TaskId { get; set; }
    public int Limit { get; set; }
}
