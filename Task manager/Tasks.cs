using menus;
using CSharpLesson;
namespace Tasks;
public struct Coord
{
    public int X { get; set; }
    public int Y { get; set; }
    public void Set() { Console.SetCursorPosition(X, Y); }
}

public abstract class BaseTask
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Name { get; set; } = "";
    public BaseTask(DateTime start, DateTime end, string name)
    {
        Start = start;
        End = end;
        Name = name;
    }
    public BaseTask() { }
    public abstract string Info();
}
public class Duty : BaseTask
{
    public Duty(DateTime start, DateTime end, string name) : base(start, end, name) { }
    public Duty() {  }
    public static Duty Create()
    {
        return new Duty(Menu.Fulltime("Select start time"), Menu.Fulltime("Select end time"), Menu.InputBox("Enter name: ", ConsoleColor.Blue));
    }
    public static Duty Create(DateTime time)
    {
        return new Duty(Menu.Fulltime(time), Menu.Fulltime(time), Menu.InputBox("Enter name: ", ConsoleColor.Blue));
    }
    public override string Info()
    {
        return $"{Name}: {Start:HH:mm} to {End:HH:mm}";
    }
}
public class Event : BaseTask
{
    public int Interval { get; set; }
    public Event(DateTime start, DateTime end, string name, int interval) : base(start, end, name)
    {
        Interval = interval;
    }
    public Event() { }
    public static Event Create()
    {
        DateTime date = Menu.Fulltime("Select start time");
        return new Event(date, Menu.Fulltime("Select end time"), Menu.InputBox("Enter name: ", ConsoleColor.Blue), Menu.EnterInterval(date));
    }
    public static Event Create(DateTime time)
    {
        return new Event(Menu.Fulltime(time), Menu.Fulltime(time), Menu.InputBox("Enter name: ", ConsoleColor.Blue),Menu.EnterInterval(time));
    }
    public override string Info()
    {
        return $"{Name}: {Start:HH:mm} to {End:HH:mm} every {Interval} days";
    }
}
public class TaskList
{
    public List<BaseTask> Tasklist { get; set; } = new();
    public void UpdateTo(int idx, DateTime time)
    {
        if (idx < 0 || idx > Tasklist.Count || Tasklist[idx] is not Event || (Tasklist[idx] as Event).Interval <= 0) return;
        for (Event temp = new(Tasklist[idx].Start, Tasklist[idx].End, Tasklist[idx].Name, (Tasklist[idx] as Event).Interval); temp.Start < time;)
        {
            temp = new(temp.Start, temp.End, temp.Name, temp.Interval);
            temp.Start += new TimeSpan(temp.Interval, 0, 0, 0);
            temp.End += new TimeSpan(temp.Interval, 0, 0, 0);
            if (!Tasklist.Contains(temp)) { Tasklist.Add(temp); }
        }
    }
    public bool IsBusy(DateTime date)
    {
        foreach (var task in Tasklist)
        {
            if ((task.Start.ToShortDateString() == date.ToShortDateString() || task.End.ToShortDateString() == date.ToShortDateString()) && task is Duty)
            {
                return true;
            }
        }
        return false;
    }
    public bool IsEvent(DateTime date)
    {
        foreach (var task in Tasklist)
        {
            if ((task.Start.ToShortDateString() == date.ToShortDateString() || task.End.ToShortDateString() == date.ToShortDateString()) && task is Event)
            {
                return true;
            }
        }
        return false;
    }
    public List<BaseTask> DayTasks(DateTime date)
    {
        List<BaseTask> tasks = new();
        foreach (var task in Tasklist)
        {
            if (task.Start.ToShortDateString() == date.ToShortDateString() || task.End.ToShortDateString() == date.ToShortDateString())
            {
                tasks.Add(task);
            }
        }
        return tasks;
    }
    public void Add(BaseTask task)
    {
        Tasklist.Add(task);
    }
    public List<Event> Eventsonly()
    {
        List<Event> events = new();
        foreach(var task in Tasklist)
        {
        if(task is Event) { events.Add(task as Event); }
        }
        return events;
    }
    public List<Duty> Dutiesonly()
    {
        List<Duty> duties = new();
        foreach (var task in Tasklist)
        {
            if (task is Duty) { duties.Add(item: task as Duty); }
        }
        return duties;
    }
}