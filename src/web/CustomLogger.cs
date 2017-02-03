namespace WebApplication
{
    public static class CustomLogger
    {
        public static void Log(string fmt, params object[] args)
        {
            var msg = string.Format("[Docker-Test] {0}\r\n", string.Format(fmt, args));
            System.Console.WriteLine(msg);
            System.IO.File.AppendAllText(@"c:\temp\web.txt", msg);
        }
    }
}
