public class RobotTask
{
    public string Name { get; set; }
    public object[] Arguments { get; set; }

    public RobotTask(string name, params object[] args)
    {
        Name = name;
        Arguments = args;
    }
}