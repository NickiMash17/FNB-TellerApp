// ================================================================
// Copyright (c) 2026 Nicolette Mashaba. All rights reserved.
// Student Number : 20232990
// Module         : MDB622 - Database Manipulation
// Assessment     : Formative Assessment 2
// Campus         : Polokwane (Online) - CTU Training Solutions
// Date           : March 2026
//
// File           : Program.cs
// Description    : Application entry point - FNB TellerApp
//
// Unauthorised copying, sharing or distribution of this file
// outside of academic submission is strictly prohibited.
// ================================================================
using System;
using System.Windows.Forms;

namespace FNB_TellerApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
