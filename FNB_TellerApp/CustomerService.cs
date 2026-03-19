// ================================================================
// Copyright (c) 2026 Nicolette Mashaba. All rights reserved.
// Student Number : 20232990
// Module         : MDB622 - Database Manipulation
// Assessment     : Formative Assessment 2
// Campus         : Polokwane (Online) - CTU Training Solutions
// Date           : March 2026
//
// File           : CustomerService.cs
// Description    : Q7: CRUD operations (Add, Read, Update, Delete)
//                  Q8: Stored procedure execution with OUTPUT param
//                  Q9: Error handling - try/catch/finally, FK handling
//                  All queries use parameterized SQL (SQL injection safe)
//
// Unauthorised copying, sharing or distribution of this file
// outside of academic submission is strictly prohibited.
// ================================================================

using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FNB_TellerApp
{
    public static class CustomerService
    {
        // Q7.1: READ - load all customers into DataGridView
        public static void LoadCustomers(DataGridView dgv)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string sql = "SELECT CustomerID AS [ID]," +
                                 "       FullName    AS [Full Name]," +
                                 "       IDNumber    AS [ID Number]," +
                                 "       DateOfBirth AS [Date of Birth]" +
                                 " FROM  dbo.Customers" +
                                 " ORDER BY CustomerID;";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        dgv.DataSource = dt;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Database error loading customers:\n\n" + sqlEx.Message,
                    "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers:\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Q7.2: CREATE - add new customer with parameterized INSERT
        public static bool AddCustomer(string fullName, string idNumber, DateTime dob)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string sql = "INSERT INTO dbo.Customers (IDNumber, FullName, DateOfBirth)" +
                                 " VALUES (@IDNumber, @FullName, @DateOfBirth);";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@IDNumber",    idNumber);
                        cmd.Parameters.AddWithValue("@FullName",    fullName);
                        cmd.Parameters.AddWithValue("@DateOfBirth", dob);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                {
                    MessageBox.Show(
                        "A customer with this ID Number already exists.\n\nPlease check the ID Number and try again.",
                        "Duplicate Record", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Database error adding customer:\n\n" + sqlEx.Message,
                        "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding customer:\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Q7.3: UPDATE - update customer FullName by CustomerID
        public static bool UpdateCustomer(int customerID, string newFullName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string sql = "UPDATE dbo.Customers" +
                                 " SET   FullName   = @FullName" +
                                 " WHERE CustomerID = @CustomerID;";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@FullName",   newFullName);
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        int rows = cmd.ExecuteNonQuery();

                        if (rows == 0)
                        {
                            MessageBox.Show("No customer found with ID: " + customerID,
                                "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating customer:\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Q7.4: DELETE - remove customer, handle FK constraint (error 547)
        public static bool DeleteCustomer(int customerID)
        {
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.ConnectionString);
                conn.Open();
                string sql = "DELETE FROM dbo.Customers WHERE CustomerID = @CustomerID;";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerID);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 547)
                {
                    // FK violation - customer has linked accounts
                    MessageBox.Show(
                        "This customer cannot be deleted because they still have linked accounts.\n\n" +
                        "Please delete all associated accounts before removing the customer.",
                        "Cannot Delete - Linked Accounts",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Database error deleting customer:\n\n" + sqlEx.Message,
                        "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting customer:\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                // Q9.1: finally block always closes the connection
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        // Q8: Execute stored procedure with OUTPUT parameter
        public static string RegisterNewCustomerViaProcedure(
            string fullName, string idNumber, DateTime dob, decimal initialDeposit)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("dbo.usp_RegisterNewCustomer", conn))
                    {
                        // Q8: Must use CommandType.StoredProcedure
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Input parameters
                        cmd.Parameters.AddWithValue("@FullName",       fullName);
                        cmd.Parameters.AddWithValue("@IDNumber",       idNumber);
                        cmd.Parameters.AddWithValue("@DateOfBirth",    dob);
                        cmd.Parameters.AddWithValue("@InitialDeposit", initialDeposit);

                        // Q8: OUTPUT parameter to receive account number
                        SqlParameter outputParam = new SqlParameter();
                        outputParam.ParameterName = "@AccountNumber";
                        outputParam.SqlDbType     = SqlDbType.VarChar;
                        outputParam.Size          = 20;
                        outputParam.Direction     = ParameterDirection.Output;
                        cmd.Parameters.Add(outputParam);

                        cmd.ExecuteNonQuery();

                        // Q8: Read the OUTPUT value after execution
                        if (outputParam.Value != null && outputParam.Value != DBNull.Value)
                            return outputParam.Value.ToString();

                        return null;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                {
                    MessageBox.Show(
                        "A customer with this ID Number is already registered.",
                        "Duplicate Customer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Database error registering customer:\n\n" + sqlEx.Message,
                        "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error registering customer:\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}
