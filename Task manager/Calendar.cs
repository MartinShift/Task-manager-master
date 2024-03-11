using menus;
using Tasks;
enum Months
{
    January = 1, February, March, April, May, June, July, August, September, October, November, December
}
namespace cal
{
    public static class Calendar
    {
        public static void DrawControls(string[] Keys, string[] Means, int start)
        {
            for (int i = 0; i < Keys.Length; i++)
            {
                Console.SetCursorPosition(start,1+i*2);
                ConsoleHelper.SetColor(ConsoleColor.Green, ConsoleColor.Black);
                Console.Write(Keys[i] + ":");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" " + Means[i]);
            }
        }
        public static int MonthWeek(DateTime date)
        {
            int res = 0;
            for (int i = 1; i <= date.Day; i++)
            {
                if (new DateTime(date.Year, date.Month, i).DayOfWeek == DayOfWeek.Sunday) { res++; }
            }
            if (new DateTime(date.Year, date.Month, 1).DayOfWeek == DayOfWeek.Sunday) res--;
            return res;
        }
        public static void DrawWeekDays()
        {
            Console.SetCursorPosition(3, 2);
            Console.Write("Su Mo Tu We Th Fr Sa");
            Console.SetCursorPosition(3, 3);
            Console.Write("--------------------");
        }
        public static void DrawYear(DateTime date)
        {
            string month = "< " + ((Months)date.Month).ToString() + $" {date.Year} >";
            Console.SetCursorPosition(9 - (((Months)date.Month).ToString().Length / 2), 1);
            Console.Write(month);
        }
        public static void DrawDay(DateTime date, TaskList tasks, DateTime current)
        {
            Console.SetCursorPosition(((int)date.DayOfWeek) * 3 + 3, MonthWeek(date) + 4);
            if (tasks.IsEvent(date)) { Console.BackgroundColor = ConsoleColor.Magenta; }
            if (tasks.IsBusy(date)) { Console.BackgroundColor = ConsoleColor.Red; }
            if (date.ToShortDateString() == current.ToShortDateString()) { ConsoleHelper.SetColor(ConsoleColor.Black, ConsoleColor.White); }
            Console.Write(date.Day);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void DrawMonth(DateTime date, TaskList tasks)
        {
            DrawYear(date);
            DrawWeekDays();
            Menu.Frame(25, 6+MonthWeek(new DateTime(date.Year,date.Month,DateTime.DaysInMonth(date.Year,date.Month))), 1, 0, ConsoleColor.Blue);
            for (int i = 1; i <= DateTime.DaysInMonth(date.Year, date.Month); i++)
            {
                DrawDay(new DateTime(date.Year, date.Month, i), tasks, date);
            }
            Console.SetCursorPosition(0, 10);
        }
    }
}
