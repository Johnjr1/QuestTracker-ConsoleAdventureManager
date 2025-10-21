//Klass som representerar en quest i spelet
public class Quest
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime DueDate { get; set; }
    public string Priority { get; set; } = "Medium";
    public bool IsCompleted { get; set; } = false;
}