using System;
using System.Windows.Forms;
namespace KontrastIyilestirmeProgrami
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormKontrastIyilestirme());
        }
    }
}
