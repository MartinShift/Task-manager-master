using cal;
using menus;
using Tasks;
using Xmls;
using CSharpLesson;


namespace Task_manager
{

    public class TaskManager
    {
        public const string SaveFile = "D:\\Mein progectos\\Task manager";
        public TaskList Tasks { get; set; } = new();
        public DateTime Current { get; set; }
        public int CurrentPos { get; set; }
        public TaskPrinter Tprinter { get; set; } = duty =>
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(duty.Info());
            Console.ForegroundColor = ConsoleColor.White;
        };
        public EventPrinter Eprinter { get; set; } = e =>
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(e.Info());
            Console.ForegroundColor = ConsoleColor.White;
        };
        public void Save()
        {
            XmlHelper<List<Event>>.Serialize(Tasks.Eventsonly(), SaveFile + "\\Events.xml");
            XmlHelper<List<Duty>>.Serialize(Tasks.Dutiesonly(), SaveFile + "\\Duties.xml");
        }
        public void Load()
        {
            if(File.Exists(SaveFile + "\\Events.xml"))
                Tasks.Tasklist.AddRange(XmlHelper<List<Event>>.Deserialize(SaveFile + "\\Events.xml"));
            if (File.Exists(SaveFile + "\\Duties.xml"))
                Tasks.Tasklist.AddRange(XmlHelper<List<Duty>>.Deserialize(SaveFile + "\\Duties.xml"));
        }
        public void EditYear()
        {
            int year = Current.Year;
            int month = Current.Month;
            string mes;
            while (true)
            {
                Console.Clear();
                Current = new DateTime(year, month, Current.Day);
                mes = "< " + ((Months)Current.Month).ToString() + $" {Current.Year} >";
                Menu.Frame(85, 11, 36, 8, ConsoleColor.Magenta);
                Console.SetCursorPosition(60 - mes.Length / 2, 9);
                Console.Write(mes);
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.LeftArrow:
                        month--;
                        if (month == 0)
                        {
                            year--;
                            month = 12;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        month++;
                        if (month == 13)
                        {
                            year++;
                            month = 1;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        year--;
                        break;
                    case ConsoleKey.DownArrow:
                        year++;
                        break;
                    case ConsoleKey.Enter:
                    case ConsoleKey.Escape:
                        return;
                }
            }
        }
        public void ClearDayTasks()
        {
            if (Tasks.Tasklist.Count == 0) return;
            DateTime prevday = Menu.MinusOne(Current);
            DateTime nextday = Menu.PlusOne(Current);
            int count = Tasks.DayTasks(prevday).Count > Tasks.DayTasks(nextday).Count ? Tasks.DayTasks(prevday).Count : Tasks.DayTasks(nextday).Count;
            for (int i = 0; i < count + 2; i++)
            {
                Console.SetCursorPosition(1, 15 + i);
                Console.Write(new string(' ', 50));
            }
        }
        public void DrawDayTasks()
        {

            List<BaseTask> temp = Tasks.DayTasks(Current);
            
            if (temp.Count == 0) { return; }
            int y = 11 + Calendar.MonthWeek(new DateTime(Current.Year, Current.Month, DateTime.DaysInMonth(Current.Year, Current.Month)));
            Menu.Frame(temp.MaxBy(x => x.Info().Length).Info().Length + 5, y + temp.Count + 2, 1, y, ConsoleColor.DarkBlue);
            for (int i = 0; i < temp.Count; i++)
            {
                Console.SetCursorPosition(2, y + 1 + i);
                if (temp[i] is Duty) Tprinter.Invoke(temp[i] as Duty);
                else Eprinter.Invoke(temp[i] as Event);
            }
        }

        public static void Run()
        {
            TaskManager manager = new()
            {

                Current = DateTime.Now,
                CurrentPos = DateTime.Now.Day
            };
            manager.Load();
            while (true)
            {
                manager.Current = new DateTime(manager.Current.Year, manager.Current.Month, manager.CurrentPos);
                manager.ClearDayTasks();
                manager.DrawDayTasks();
                Calendar.DrawControls(new string[] { "Up", "Down", "Left", "Right", "F1", "F2", "Esc" }, new string[] { "Up", "Down", "Left", "Right", "Add duty", "Add event", "Leave" }, 30);
                Calendar.DrawMonth(manager.Current, manager.Tasks);
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.UpArrow: //Вверх
                        {

                            manager.CurrentPos -= 7;
                            if (manager.CurrentPos <= 0 && Calendar.MonthWeek(manager.Current) != 0) manager.CurrentPos = 1;
                            else if (Calendar.MonthWeek(manager.Current) == 0)
                            {
                                manager.CurrentPos = 1;
                                ConsoleHelper.SetColor(ConsoleColor.Black, ConsoleColor.White);
                                Calendar.DrawYear(manager.Current);
                                ConsoleHelper.SetColor(ConsoleColor.White, ConsoleColor.Black);
                                switch (Console.ReadKey().Key)
                                {
                                    case ConsoleKey.Enter:
                                        manager.EditYear();
                                        Console.Clear();
                                        break;
                                }
                            }
                            break;
                        }
                    case ConsoleKey.DownArrow: //Вниз
                        {
                            manager.CurrentPos += 7;
                            if (manager.CurrentPos > DateTime.DaysInMonth(manager.Current.Year, manager.Current.Month) && Calendar.MonthWeek(manager.Current) != Calendar.MonthWeek(new DateTime(manager.Current.Year, manager.Current.Month, DateTime.DaysInMonth(manager.Current.Year, manager.Current.Month))))
                            {
                                manager.CurrentPos = DateTime.DaysInMonth(manager.Current.Year, manager.Current.Month);
                                Console.Clear();
                            }
                            else if (manager.CurrentPos > DateTime.DaysInMonth(manager.Current.Year, manager.Current.Month) && Calendar.MonthWeek(manager.Current) != (Calendar.MonthWeek(new DateTime(manager.Current.Year, manager.Current.Month, DateTime.DaysInMonth(manager.Current.Year, manager.Current.Month))) - 1))
                            {
                                int months = manager.Current.Month + 1;
                                manager.Current = new DateTime(months == 13 ? manager.Current.Year + 1 : manager.Current.Year, months == 13 ? 1 : months, 1);
                                manager.CurrentPos = 1;
                                Console.Clear();
                            }
                            break;
                        }
                    case ConsoleKey.LeftArrow: //Вліво
                        {

                            manager.CurrentPos--;
                            if (manager.CurrentPos == 0)
                            {
                                int year = manager.Current.Month == 1 ? manager.Current.Year - 1 : manager.Current.Year;
                                int months = manager.Current.Month == 1 ? 12 : manager.Current.Month - 1;
                                manager.Current = new DateTime(year, months, DateTime.DaysInMonth(year, months));
                                manager.CurrentPos = DateTime.DaysInMonth(year, months);
                                Console.Clear();
                            }
                            break;
                        }
                    case ConsoleKey.RightArrow: //Вправо
                        {

                            manager.CurrentPos++;
                            if (manager.CurrentPos > DateTime.DaysInMonth(manager.Current.Year, manager.Current.Month))
                            {
                                int year = manager.Current.Month == 12 ? manager.Current.Year + 1 : manager.Current.Year;
                                int months = manager.Current.Month == 12 ? 1 : manager.Current.Month + 1;
                                manager.Current = new DateTime(year, months, 1);
                                manager.CurrentPos = 1;
                                Console.Clear();
                            }
                            break;
                        }
                    case ConsoleKey.F1: //Додати завдання
                        {
                            switch (Menu.MainMenu(new List<string> { "This date", "Enter date" }, "Select duty date"))
                            {
                                case 0:
                                    manager.Tasks.Add(Duty.Create(manager.Current));
                                    break;
                                case 1:
                                    manager.Tasks.Add(Duty.Create());
                                    break;
                            }
                            break;
                        }
                    case ConsoleKey.F2: //Додати подію
                        {
                            switch (Menu.MainMenu(new List<string> { "This date", "Enter date" }, "Select event date"))
                            {
                                case 0:
                                    manager.Tasks.Add(Event.Create(manager.Current));
                                    break;
                                case 1:
                                    manager.Tasks.Add(Event.Create());
                                    break;
                            }
                            manager.Tasks.UpdateTo(manager.Tasks.Tasklist.Count - 1, new DateTime(manager.Current.Year + 1, manager.Current.Month, manager.Current.Day));
                            break;
                        }
                    case ConsoleKey.Escape: // Вийти
                        manager.Save();
                        Console.Clear();
                        return;

                }
            }
        }
    }
}
