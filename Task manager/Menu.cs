namespace menus;

public static class ConsoleHelper
{
    public static void SetColor(ConsoleColor a, ConsoleColor b)
    {
        Console.ForegroundColor = a;
        Console.BackgroundColor = b;
    }
}
public class Menu
{
    public long ActiveOption { get; set; } = 0;
    public List<string> Options { get; set; }
    public string Question { get; set; }
    public Menu(List<string> options, string question)
    {
        ActiveOption = 0;
        Options = options;
        Question = question;
    }
    public void DrawQuestion()
    {
        long width = Question.Length + 4 > GetFrameWidth() ? Question.Length + 4 : GetFrameWidth();
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == 2)
                {
                    ConsoleHelper.SetColor(ConsoleColor.White, ConsoleColor.Blue);
                    Console.SetCursorPosition(x, y);
                    Console.Write(' ');
                }
            }
        }
        ConsoleHelper.SetColor(ConsoleColor.White, ConsoleColor.Black);
        Console.SetCursorPosition(2, 1);
        Console.Write(Question);
    }
    public void DrawFrame()
    {
        long width = GetFrameWidth() > Question.Length + 4 ? GetFrameWidth() : Question.Length + 4;
        long height = GetFrameHeight() + 2;
        DrawQuestion();
        for (short y = 0; y < height; y++)
        {
            for (short x = 0; x < width; x++)
            {
                if (x == 0 || x == width - 1 || y == 2 || y == height - 1)
                {
                    Console.SetCursorPosition(x, y);
                    ConsoleHelper.SetColor(ConsoleColor.White, ConsoleColor.Blue);
                    Console.Write(' ');
                }
            }
        }
        ConsoleHelper.SetColor(ConsoleColor.White, ConsoleColor.Black);
    }
    public void DrawControls(string[] Keys, string[] Means)
    {
        for (int i = 0; i < Keys.Length; i++)
        {
            Console.SetCursorPosition((int)GetFrameWidth() > Question.Length + 4 ? (int)GetFrameWidth() + 7 : Question.Length + 7, 1 + i * 2);
            ConsoleHelper.SetColor(ConsoleColor.Green, ConsoleColor.Black);
            Console.Write(Keys[i] + ":");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" " + Means[i]);
        }
    }
    public long GetFrameWidth()
    {
        return !Options.Any() ? 5 : Options.Max(x => x.Length) + 6;
    }
    public long GetFrameHeight()
    {
        return Options.Count + 4;
    }
    public void Down()
    {
        ActiveOption++;
        if (ActiveOption >= Options.Count)
        {
            ActiveOption = 0;
        }
    }
    public void Up()
    {
        ActiveOption--;
        if (ActiveOption < 0)
        {
            ActiveOption = Options.Count - 1;
        }
    }
    public void DrawOptions()
    {
        DrawFrame();
        short startX = 3;
        short startY = 4;
        for (int i = 0; i < Options.Count; i++)
        {
            Console.SetCursorPosition(startX, startY + i);
            ConsoleHelper.SetColor(ActiveOption == i ? ConsoleColor.Red : ConsoleColor.White, ActiveOption == i ? ConsoleColor.White : ConsoleColor.Black);
            Console.Write(Options[i]);
        }
    }
    public static void Frame(int width, int height, int startX, int startY, ConsoleColor color)
    {
        ConsoleHelper.SetColor(ConsoleColor.White, color);
        for (int y = startY; y < height; y++)
        {
            for (int x = startX; x < width; x++)
            {
                if (x == startX || x == width - 1 || y == startY || y == height - 1)
                {
                    Console.SetCursorPosition(x, y);
                    Console.WriteLine(' ');
                }
            }
        }
        Console.SetCursorPosition(startX + 1, startY + 1);
        ConsoleHelper.SetColor(ConsoleColor.White, ConsoleColor.Black);
    }
    public static int MainMenu(List<string> mes, string question)
    {
        Console.Clear();
        Menu menu = new(mes, question);
        while (true)
        {
            menu.DrawOptions();
            menu.DrawControls(new string[] { "Up", "Down", "Enter" }, new string[] { "Up", "Down", "Select" });
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.UpArrow:
                    menu.Up();
                    break;
                case ConsoleKey.DownArrow:
                    menu.Down();
                    break;
                case ConsoleKey.Enter:
                    {
                        Console.Clear();
                        return (int)menu.ActiveOption;
                    }
            }
        }
    }
    public static void Message(string mes, ConsoleColor color)
    {
        Frame(85, 11, 36, 8, color);
        Console.SetCursorPosition(60 - mes.Length / 2, 9);
        Console.Write(mes);
        Console.ReadKey();
        Console.Clear();
    }
    public static string InputBox(string mes, ConsoleColor color)
    {
        ConsoleHelper.SetColor(ConsoleColor.White, ConsoleColor.Black);
        Console.Clear();
        Frame(85, 12, 30, 9, color);
        Console.Write(mes);
        string res = Console.ReadLine();
        Console.Clear();
        return res;
    }
    public static void WriteNice(List<string> mes, ConsoleColor color)
    {
        Console.Clear();
        Frame(mes.Max(x => x.Length) + 4, mes.Count + 2, 0, 0, color);
        for (int i = 0; i < mes.Count; i++)
        {
            Console.SetCursorPosition(1, 1 + i);
            Console.Write(mes[i]);
        }
        Console.ReadKey();
        Console.Clear();
    }
    public static DateTime Fulltime(string str)
    {
        DateTime time = SelectTime(str);
        string[] strs = InputBox("Enter time in 24h format(in HH:MM): ", ConsoleColor.Green).Split(new char[] { ':', '.', ' ' });
        TimeSpan ts = new(int.Parse(strs[0]), int.Parse(strs[1]), 0);
        time += ts;
        return time;
    }
    public static DateTime Fulltime(DateTime time)
    {
        string[] strs = InputBox("Enter time in 24h format(in HH:MM): ", ConsoleColor.Green).Split(new char[] { ':', '.', ' ' });
        TimeSpan ts = new(int.Parse(strs[0]), int.Parse(strs[1]), 0);
        time += ts;
        return time;
    }
    public static DateTime PlusOne(DateTime time)
    {
        int day = time.Day == DateTime.DaysInMonth(time.Year, time.Month) ? 1 : time.Day + 1;
        int month = time.Day == DateTime.DaysInMonth(time.Year, time.Month) ? time.Month + 1 : time.Month;
        int year = month == 13 ? time.Year + 1 : time.Year;
        if(month == 13) { month = 1; }
        return new DateTime(year, month, day);
    }
    public static DateTime MinusOne(DateTime time)
    {
        int year = time.Month == 1 && time.Day == 1 ? time.Year - 1 : time.Year;
        int month = time.Month;
        if (time.Month == 1 && time.Day == 1) month = 12;
        else if (time.Day == 1) month -= 1;
        int day = time.Day == 1 ? DateTime.DaysInMonth(year, month) : time.Day - 1;
        if (month == 13) { month = 1; }
        return new DateTime(year, month, day);
    }
    public static DateTime SelectTime(string str)
    {
        DateTime res = new();
        switch (MainMenu(new List<string> { "Current time", "Enter time" }, str))
        {
            case 0:
                res = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                break;
            case 1:
                res = DateTime.Parse(InputBox("Enter time(in dd.mm.yyyy): ", ConsoleColor.Green));
                break;
        }
        return res;
    }
    public static int EnterInterval(DateTime date)
    {
        int res = 0;
        switch (MainMenu(new List<string> { "Every Year", "Every Month", "Every Week", "Every Day", "No Interval", "Enter Interval" }, "Select time method"))
        {
            case 0:
                res = DateTime.IsLeapYear(date.Year) ? 366 : 365;
                break;
            case 1:
                res = DateTime.DaysInMonth(date.Year, date.Month);
                break;
            case 2:
                res = 7;
                break;
            case 3:
                res = 1;
                break;
            case 4:
                res = 0;
                break;
            case 5:
                res = int.Parse(InputBox("Enter interval(in days): ", ConsoleColor.Magenta));
                break;
        }
        return res;
    }
}
