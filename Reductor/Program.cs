using System;
using System.Windows.Forms;
using ModelLibrary;

namespace Reductor
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm mainForm = new MainForm();
            IModel model = new Model();
            new Presenter(mainForm, model);
            Application.Run(mainForm);
        }
    }
}
