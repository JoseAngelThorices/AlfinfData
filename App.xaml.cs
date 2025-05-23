﻿using AlfinfData.Services.BdLocal;
using Microsoft.Maui.Storage;
using AlfinfData.Models.SQLITE;


namespace AlfinfData
{
    public partial class App : Application
    {

        public App(DatabaseService dbService)
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Light;

            RegistrarAñoInstalacion(); // Guardamos el año si es la primera vez

            var fichajeRepo = new FichajeRepository(dbService);
        }

        private void RegistrarAñoInstalacion()
        {
            if (!Preferences.ContainsKey("AñoInicio"))
            {
                Preferences.Set("AñoInicio", DateTime.Now.Year);
            }
        }

       


        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        

    }
}
