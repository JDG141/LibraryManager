namespace LibraryTask
{
    class Program
    {
        public static void Main()
        {
            DbManager db = new DbManager(); // Create new db connection and keep it open for efficiency

            UiManager ui = new UiManager(db);
            ui.Run();
        }
    }
}