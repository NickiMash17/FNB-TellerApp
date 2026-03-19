// ================================================================
// Copyright (c) 2026 Nicolette Mashaba. All rights reserved.
// Student Number : 20232990
// Module         : MDB622 - Database Manipulation
// Assessment     : Formative Assessment 2
// Campus         : Polokwane (Online) - CTU Training Solutions
// Date           : March 2026
//
// File           : DatabaseHelper.cs
// Description    : Q6 - Database connection and configuration
//                  Reads connection string from App.config
//                  TestConnection() verifies database reachability
//
// Unauthorised copying, sharing or distribution of this file
// outside of academic submission is strictly prohibited.
// ================================================================

using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FNB_TellerApp
{
    public static class DatabaseHelper
    {
        // Q6: Read connection string from App.config
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["FNBStudentDB"].ConnectionString;
            }
        }

        // Q6: TestConnection - returns true on success, false on failure
        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Connection Failed:\n\n" + ex.Message,
                    "Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
