# FNB TellerApp

**MDB622 — Database Manipulation | Formative Assessment 2**  
CTU Training Solutions | NQF Level 6 | SAQA ID 119458

---

## Overview

FNB TellerApp is a Windows Forms desktop application built in C# (.NET Framework). It connects to the **FNB Student Connect** banking database (`FNB_StudentDB`) on SQL Server and demonstrates full CRUD operations, stored procedure execution, and professional error handling aligned to the MDB622 FA2 outcomes.

---

## Tech Stack

![C#](https://img.shields.io/badge/C%23-.NET%204.7.2-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![WinForms](https://img.shields.io/badge/Windows%20Forms-Desktop-0078D6?style=for-the-badge&logo=windows&logoColor=white)
![ADO.NET](https://img.shields.io/badge/ADO.NET-Data%20Access-2F74C0?style=for-the-badge)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![Visual Studio](https://img.shields.io/badge/Visual%20Studio-2022-5C2D91?style=for-the-badge&logo=visualstudio&logoColor=white)

---

## Scope Highlights

- **Q6** — Database connection via `App.config` and `DatabaseHelper.cs` with `TestConnection()`
- **Q7** — Full CRUD: load, add, update, delete customers using parameterized queries
- **Q8** — Stored procedure execution using `CommandType.StoredProcedure` and an `OUTPUT` parameter
- **Q9** — Error handling: `try-catch-finally`, `using` statements, FK constraint handling (Msg 547), duplicate detection (Msg 2627)

---

## Database Setup

Run `Solution.sql` in SSMS against `FNB_StudentDB`. It creates:

- View: `vw_CustomerAccountSummary`
- Stored Procedure: `usp_RegisterNewCustomer`
- Scalar Function: `ufn_CalculateMonthlyFee`
- Audit Table: `TransactionAudit`
- Trigger: `trg_AuditTransaction`

---

## How To Run

1. Open `FNB_TellerApp.sln` in Visual Studio 2022
2. Update `App.config` — replace `HACKER17\SQLEXPRESS` with your SQL Server instance name if different
3. Add reference: `System.Configuration`
4. Press F5 to build and run

---

## Notes

This project is intended for academic submission purposes only.
