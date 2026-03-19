// ================================================================
// Copyright (c) 2026 Nicolette Mashaba. All rights reserved.
// Student Number : 20232990
// Module         : MDB622 - Database Manipulation
// Assessment     : Formative Assessment 2
// Campus         : Polokwane (Online) - CTU Training Solutions
// Date           : March 2026
//
// File           : Form1.cs
// Description    : Main application form - FNB TellerApp
//                  Q6: Connection | Q7: CRUD | Q8: Stored Proc
//                  Q9: Error Handling
//
// Unauthorised copying, sharing or distribution of this file
// outside of academic submission is strictly prohibited.
// ================================================================

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace FNB_TellerApp
{
    public partial class Form1 : Form
    {
        // Colours
        private readonly Color CPurpleDark = Color.FromArgb(45, 27, 78);
        private readonly Color CPurpleMid = Color.FromArgb(106, 63, 160);
        private readonly Color CPurpleLight = Color.FromArgb(237, 232, 245);
        private readonly Color CSilver = Color.FromArgb(192, 192, 192);
        private readonly Color CSuccess = Color.FromArgb(39, 174, 96);
        private readonly Color CDanger = Color.FromArgb(192, 57, 43);
        private readonly Color CText = Color.FromArgb(26, 10, 46);
        private readonly Color CGridAlt = Color.FromArgb(245, 240, 255);

        // Fonts
        private readonly Font FTitle = new Font("Segoe UI", 17f, FontStyle.Bold);
        private readonly Font FSub = new Font("Segoe UI", 9f, FontStyle.Regular);
        private readonly Font FLabel = new Font("Segoe UI", 9f, FontStyle.Bold);
        private readonly Font FInput = new Font("Segoe UI", 10f, FontStyle.Regular);
        private readonly Font FBtn = new Font("Segoe UI", 9f, FontStyle.Bold);
        private readonly Font FSmall = new Font("Segoe UI", 8f, FontStyle.Regular);
        private readonly Font FGrid = new Font("Segoe UI", 9f, FontStyle.Regular);
        private readonly Font FCard = new Font("Segoe UI", 9f, FontStyle.Bold);
        private readonly Font FPage = new Font("Segoe UI", 15f, FontStyle.Bold);

        // Controls
        private Label lblConnStatus, lblStatus, lblLastAccount;
        private DataGridView dgvCustomers;
        private Panel pnlSidebar, pnlCRUD, pnlRegister, pnlNavIndicator;
        private TextBox txtFullName, txtIDNumber, txtCustomerID, txtUpdateName;
        private TextBox txtRegFullName, txtRegIDNumber, txtRegDeposit;
        private DateTimePicker dtpDOB, dtpRegDOB;
        private Button btnAdd, btnLoad, btnUpdate, btnDelete, btnTestConn;
        private Button btnRegister, btnNavCRUD, btnNavRegister;

        public Form1()
        {
            InitializeComponent();
            BuildUI();
            LoadCustomers();
        }

        private void BuildUI()
        {
            this.Text = "FNB TellerApp - FNB Student Connect";
            this.Size = new Size(1100, 720);
            this.MinimumSize = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = CPurpleDark;
            this.DoubleBuffered = true;

            BuildHeader();
            BuildStatusBar();

            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.FromArgb(250, 248, 255),
                Padding = new Padding(0)
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 195f));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            // Sidebar
            pnlSidebar = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(30, 15, 55) };
            tbl.Controls.Add(pnlSidebar, 0, 0);
            BuildSidebar();

            // Content
            var pnlContent = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(250, 248, 255) };
            tbl.Controls.Add(pnlContent, 1, 0);

            pnlCRUD = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(250, 248, 255), Visible = true, AutoScroll = true };
            pnlRegister = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(250, 248, 255), Visible = false, AutoScroll = true };
            pnlContent.Controls.Add(pnlRegister);
            pnlContent.Controls.Add(pnlCRUD);

            BuildCRUDPanel();
            BuildRegisterPanel();

            // Add tbl LAST so Fill works correctly after Top/Bottom docked controls
            this.Controls.Add(tbl);
            tbl.BringToFront();
        }

        private void BuildHeader()
        {
            var h = new Panel { Dock = DockStyle.Top, Height = 76, BackColor = Color.FromArgb(40, 20, 70) };
            h.Paint += (s, e) => { using (var pen = new Pen(CPurpleMid, 2)) e.Graphics.DrawLine(pen, 0, h.Height - 1, h.Width, h.Height - 1); };

            h.Controls.Add(new Label { Text = "FNB TellerApp", Font = FTitle, ForeColor = Color.White, AutoSize = true, Location = new Point(20, 10), BackColor = Color.Transparent });
            h.Controls.Add(new Label { Text = "FNB Student Connect  |  Customer Management System  |  MDB622 FA2", Font = FSub, ForeColor = CSilver, AutoSize = true, Location = new Point(21, 46), BackColor = Color.Transparent });

            lblConnStatus = new Label { Text = "o  Not tested", Font = new Font("Segoe UI", 9f), ForeColor = CSilver, AutoSize = true, BackColor = Color.Transparent, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            lblConnStatus.Location = new Point(h.Width - 150, 32);
            h.Resize += (s, e) => lblConnStatus.Location = new Point(h.Width - lblConnStatus.Width - 16, 32);
            h.Controls.Add(lblConnStatus);
            this.Controls.Add(h);
        }

        private void BuildStatusBar()
        {
            var s = new Panel { Dock = DockStyle.Bottom, Height = 28, BackColor = Color.FromArgb(30, 15, 55) };
            btnTestConn = new Button { Text = "Test Connection", Font = FSmall, FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, ForeColor = CSilver, Size = new Size(110, 22), Location = new Point(8, 3), Cursor = Cursors.Hand };
            btnTestConn.FlatAppearance.BorderColor = Color.FromArgb(80, 55, 120);
            btnTestConn.FlatAppearance.BorderSize = 1;
            btnTestConn.Click += BtnTestConn_Click;
            lblStatus = new Label { Text = "Ready", Font = FSmall, ForeColor = CSilver, AutoSize = true, Location = new Point(130, 7), BackColor = Color.Transparent };
            s.Controls.AddRange(new Control[] { btnTestConn, lblStatus });
            this.Controls.Add(s);
        }

        private void BuildSidebar()
        {
            // Footer must be added FIRST when using DockStyle.Bottom
            var lblFooter = new Label
            {
                Text = "Nicolette Mashaba\n20232990  |  MDB622 FA2",
                Font = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(110, 85, 150),
                AutoSize = false,
                Size = new Size(195, 44),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Bottom,
                BackColor = Color.Transparent
            };
            pnlSidebar.Controls.Add(lblFooter);

            var lblNav = new Label
            {
                Text = "NAVIGATION",
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(140, 110, 180),
                AutoSize = false,
                Size = new Size(195, 22),
                Location = new Point(0, 16),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(14, 0, 0, 0),
                BackColor = Color.Transparent
            };
            pnlSidebar.Controls.Add(lblNav);

            // Indicator bar sits on left edge — 4px wide only
            pnlNavIndicator = new Panel
            {
                Size = new Size(4, 38),
                Location = new Point(0, 44),
                BackColor = CPurpleMid
            };
            pnlSidebar.Controls.Add(pnlNavIndicator);

            // Buttons start AFTER indicator bar
            btnNavCRUD = MakeSideBtn("  Customer Management", 44, true);
            btnNavRegister = MakeSideBtn("  Register via Proc", 88, false);
            btnNavCRUD.Click += (s, e) => ShowPanel(true);
            btnNavRegister.Click += (s, e) => ShowPanel(false);
            pnlSidebar.Controls.Add(btnNavCRUD);
            pnlSidebar.Controls.Add(btnNavRegister);
        }

        private Button MakeSideBtn(string text, int top, bool active)
        {
            var btn = new Button { Text = "  " + text, Size = new Size(195, 38), Location = new Point(0, top), FlatStyle = FlatStyle.Flat, BackColor = active ? Color.FromArgb(55, 30, 90) : Color.Transparent, ForeColor = active ? Color.White : Color.FromArgb(170, 145, 205), Font = new Font("Segoe UI", 9.5f, active ? FontStyle.Bold : FontStyle.Regular), TextAlign = ContentAlignment.MiddleLeft, Cursor = Cursors.Hand };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(55, 30, 90);
            return btn;
        }

        private void ShowPanel(bool showCRUD)
        {
            pnlCRUD.Visible = showCRUD; pnlRegister.Visible = !showCRUD;
            btnNavCRUD.BackColor = showCRUD ? Color.FromArgb(55, 30, 90) : Color.Transparent;
            btnNavCRUD.ForeColor = showCRUD ? Color.White : Color.FromArgb(170, 145, 205);
            btnNavCRUD.Font = new Font("Segoe UI", 9.5f, showCRUD ? FontStyle.Bold : FontStyle.Regular);
            btnNavRegister.BackColor = !showCRUD ? Color.FromArgb(55, 30, 90) : Color.Transparent;
            btnNavRegister.ForeColor = !showCRUD ? Color.White : Color.FromArgb(170, 145, 205);
            btnNavRegister.Font = new Font("Segoe UI", 9.5f, !showCRUD ? FontStyle.Bold : FontStyle.Regular);
            pnlNavIndicator.Top = showCRUD ? 44 : 88;
        }

        private void BuildCRUDPanel()
        {
            int p = 16;
            pnlCRUD.Controls.Add(new Label { Text = "Customer Management", Font = FPage, ForeColor = CPurpleDark, AutoSize = true, Location = new Point(p, 12), BackColor = Color.Transparent });
            pnlCRUD.Controls.Add(new Label { Text = "View, add, update and delete customers  (Q7: CRUD Operations)", Font = new Font("Segoe UI", 8.5f), ForeColor = Color.FromArgb(130, 100, 170), AutoSize = true, Location = new Point(p, 46), BackColor = Color.Transparent });

            var pnlGrid = new Panel { Location = new Point(p, p + 68), Size = new Size(840, 300), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            pnlGrid.Paint += PaintCard;
            dgvCustomers = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, RowHeadersVisible = false, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AllowUserToResizeRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, Font = FGrid, GridColor = Color.FromArgb(225, 215, 240), CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal, EnableHeadersVisualStyles = false };
            dgvCustomers.DefaultCellStyle.BackColor = Color.White; dgvCustomers.DefaultCellStyle.ForeColor = CText; dgvCustomers.DefaultCellStyle.SelectionBackColor = CPurpleMid; dgvCustomers.DefaultCellStyle.SelectionForeColor = Color.White; dgvCustomers.DefaultCellStyle.Padding = new Padding(4, 5, 4, 5);
            dgvCustomers.AlternatingRowsDefaultCellStyle.BackColor = CGridAlt;
            dgvCustomers.ColumnHeadersDefaultCellStyle.BackColor = CPurpleDark; dgvCustomers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White; dgvCustomers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold); dgvCustomers.ColumnHeadersDefaultCellStyle.Padding = new Padding(4, 7, 4, 7);
            dgvCustomers.ColumnHeadersHeight = 34; dgvCustomers.RowTemplate.Height = 30;
            dgvCustomers.SelectionChanged += DgvCustomers_SelectionChanged;
            pnlGrid.Controls.Add(dgvCustomers);
            pnlCRUD.Controls.Add(pnlGrid);

            btnLoad = MakeBtn("Refresh", CPurpleMid, new Point(p, p + 380), new Size(110, 32));
            btnLoad.Click += (s, e) => LoadCustomers();
            pnlCRUD.Controls.Add(btnLoad);

            // Add card
            var pA = new Panel { Location = new Point(p, p + 424), Size = new Size(405, 196), Anchor = AnchorStyles.Top | AnchorStyles.Left };
            pA.Paint += PaintCard;
            pA.Controls.Add(MakeCardLbl("Add New Customer", new Point(12, 10)));
            pA.Controls.Add(MakeLbl("Full Name:", new Point(12, 38)));
            txtFullName = MakeTxt(new Point(12, 56), 378); pA.Controls.Add(txtFullName);
            pA.Controls.Add(MakeLbl("SA ID Number (13 digits):", new Point(12, 90)));
            txtIDNumber = MakeTxt(new Point(12, 108), 378); txtIDNumber.MaxLength = 13; pA.Controls.Add(txtIDNumber);
            pA.Controls.Add(MakeLbl("Date of Birth:", new Point(12, 142)));
            dtpDOB = MakeDtp(new Point(12, 160)); pA.Controls.Add(dtpDOB);
            btnAdd = MakeBtn("Add Customer", CSuccess, new Point(220, 158), new Size(168, 32));
            btnAdd.Click += BtnAdd_Click; pA.Controls.Add(btnAdd);
            pnlCRUD.Controls.Add(pA);

            // Edit card
            var pE = new Panel { Location = new Point(p + 425, p + 424), Size = new Size(430, 196), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            pE.Paint += PaintCard;
            pE.Controls.Add(MakeCardLbl("Update / Delete Customer", new Point(12, 10)));
            pE.Controls.Add(MakeLbl("Customer ID (auto-filled on row click):", new Point(12, 38)));
            txtCustomerID = MakeTxt(new Point(12, 56), 200); pE.Controls.Add(txtCustomerID);
            pE.Controls.Add(MakeLbl("New Full Name:", new Point(12, 90)));
            txtUpdateName = MakeTxt(new Point(12, 108), 400); pE.Controls.Add(txtUpdateName);
            btnUpdate = MakeBtn("Update Name", CPurpleMid, new Point(12, 155), new Size(140, 32)); btnUpdate.Click += BtnUpdate_Click;
            btnDelete = MakeBtn("Delete", CDanger, new Point(162, 155), new Size(110, 32)); btnDelete.Click += BtnDelete_Click;
            pE.Controls.Add(btnUpdate); pE.Controls.Add(btnDelete);
            pnlCRUD.Controls.Add(pE);
        }

        private void BuildRegisterPanel()
        {
            int p = 16;
            pnlRegister.Controls.Add(new Label { Text = "Register New Customer", Font = FPage, ForeColor = CPurpleDark, AutoSize = true, Location = new Point(p, p), BackColor = Color.Transparent });
            pnlRegister.Controls.Add(new Label { Text = "Calls usp_RegisterNewCustomer via CommandType.StoredProcedure  (Q8)", Font = new Font("Segoe UI", 8.5f), ForeColor = Color.FromArgb(130, 100, 170), AutoSize = true, Location = new Point(p, p + 36), BackColor = Color.Transparent });

            var pF = new Panel { Location = new Point(p, p + 68), Size = new Size(555, 310) };
            pF.Paint += PaintCard;
            pF.Controls.Add(MakeCardLbl("New Customer Registration - via Stored Procedure", new Point(12, 10)));
            pF.Controls.Add(MakeLbl("Full Name:", new Point(12, 38)));
            txtRegFullName = MakeTxt(new Point(12, 56), 526); pF.Controls.Add(txtRegFullName);
            pF.Controls.Add(MakeLbl("SA ID Number (13 digits):", new Point(12, 94)));
            txtRegIDNumber = MakeTxt(new Point(12, 112), 254); txtRegIDNumber.MaxLength = 13; pF.Controls.Add(txtRegIDNumber);
            pF.Controls.Add(MakeLbl("Date of Birth:", new Point(276, 94)));
            dtpRegDOB = MakeDtp(new Point(276, 112)); pF.Controls.Add(dtpRegDOB);
            pF.Controls.Add(MakeLbl("Initial Deposit (Rands):", new Point(12, 150)));
            txtRegDeposit = MakeTxt(new Point(12, 168), 254); pF.Controls.Add(txtRegDeposit);

            var pI = new Panel { Location = new Point(12, 212), Size = new Size(526, 46), BackColor = CPurpleLight };
            pI.Paint += (s, e) => { using (var pen = new Pen(CPurpleMid, 3)) e.Graphics.DrawLine(pen, 0, 0, 0, pI.Height); };
            pI.Controls.Add(new Label { Text = "Uses CommandType.StoredProcedure - not inline SQL.\nAccount number returned via OUTPUT parameter (@AccountNumber).", Font = new Font("Segoe UI", 8.5f, FontStyle.Italic), ForeColor = CPurpleMid, AutoSize = false, Size = new Size(516, 42), Location = new Point(10, 2), BackColor = Color.Transparent });
            pF.Controls.Add(pI);

            btnRegister = MakeBtn("Register Customer", CPurpleMid, new Point(12, 270), new Size(175, 34));
            btnRegister.Click += BtnRegister_Click;
            pF.Controls.Add(btnRegister);
            pnlRegister.Controls.Add(pF);

            var pR = new Panel { Location = new Point(p + 575, p + 68), Size = new Size(260, 160) };
            pR.Paint += PaintCard;
            pR.Controls.Add(MakeCardLbl("Generated Account Number", new Point(12, 10)));
            lblLastAccount = new Label { Text = "---", Font = new Font("Segoe UI", 20f, FontStyle.Bold), ForeColor = CPurpleMid, AutoSize = false, Size = new Size(236, 50), Location = new Point(12, 36), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleCenter };
            pR.Controls.Add(lblLastAccount);
            pR.Controls.Add(new Label { Text = "Auto-generated by the stored procedure", Font = new Font("Segoe UI", 8f), ForeColor = Color.FromArgb(140, 110, 170), AutoSize = false, Size = new Size(236, 28), Location = new Point(12, 98), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleCenter });
            pnlRegister.Controls.Add(pR);

            var pQ = new Panel { Location = new Point(p + 575, p + 244), Size = new Size(260, 144) };
            pQ.Paint += PaintCard;
            pQ.Controls.Add(MakeCardLbl("Q8: How It Works", new Point(12, 10)));
            pQ.Controls.Add(new Label { Text = "1. CommandType.StoredProcedure is set\n2. Parameters added via AddWithValue()\n3. @AccountNumber set as Output\n4. Value read after ExecuteNonQuery()", Font = new Font("Segoe UI", 8.5f), ForeColor = CText, AutoSize = false, Size = new Size(236, 118), Location = new Point(12, 30), BackColor = Color.Transparent });
            pnlRegister.Controls.Add(pQ);
        }

        // Event handlers
        private void BtnTestConn_Click(object sender, EventArgs e)
        {
            SetStatus("Testing connection...", CSilver);
            bool ok = DatabaseHelper.TestConnection();
            lblConnStatus.Text = ok ? "Connected" : "Failed"; lblConnStatus.ForeColor = ok ? CSuccess : CDanger;
            if (ok) { SetStatus("Connection successful.", CSuccess); MessageBox.Show("Connection Successful!\n\nFNB_StudentDB is reachable.", "Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            else SetStatus("Connection failed.", CDanger);
        }

        private void LoadCustomers()
        {
            SetStatus("Loading...", CSilver);
            CustomerService.LoadCustomers(dgvCustomers);

            // Set specific column widths so Date of Birth is not cut off
            if (dgvCustomers.Columns.Count >= 4)
            {
                dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dgvCustomers.Columns[0].Width = 60;   // ID
                dgvCustomers.Columns[1].Width = 220;  // Full Name
                dgvCustomers.Columns[2].Width = 160;  // ID Number
                dgvCustomers.Columns[3].Width = 120;  // Date of Birth
            }

            SetStatus("Loaded " + dgvCustomers.Rows.Count + " customer(s).", CSuccess);
        }

        private void DgvCustomers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count == 0) return;
            var row = dgvCustomers.SelectedRows[0];
            txtCustomerID.Text = row.Cells["ID"].Value != null ? row.Cells["ID"].Value.ToString() : "";
            txtUpdateName.Text = row.Cells["Full Name"].Value != null ? row.Cells["Full Name"].Value.ToString() : "";
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text)) { Warn("Please enter Full Name."); return; }
            if (txtIDNumber.Text.Trim().Length != 13) { Warn("ID Number must be exactly 13 digits."); return; }
            SetStatus("Adding...", CSilver);
            bool ok = CustomerService.AddCustomer(txtFullName.Text.Trim(), txtIDNumber.Text.Trim(), dtpDOB.Value.Date);
            if (ok) { MessageBox.Show("Customer added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); txtFullName.Clear(); txtIDNumber.Clear(); LoadCustomers(); SetStatus("Customer added.", CSuccess); }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            int cid; if (!int.TryParse(txtCustomerID.Text.Trim(), out cid)) { Warn("Please select a customer from the grid first."); return; }
            if (string.IsNullOrWhiteSpace(txtUpdateName.Text)) { Warn("Please enter the new Full Name."); return; }
            if (MessageBox.Show("Update customer ID " + cid + " to:\n\"" + txtUpdateName.Text.Trim() + "\"?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            SetStatus("Updating...", CSilver);
            bool ok = CustomerService.UpdateCustomer(cid, txtUpdateName.Text.Trim());
            if (ok) { MessageBox.Show("Customer updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); LoadCustomers(); SetStatus("Updated.", CSuccess); }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            int cid; if (!int.TryParse(txtCustomerID.Text.Trim(), out cid)) { Warn("Please select a customer from the grid first."); return; }
            if (MessageBox.Show("Permanently delete customer ID " + cid + "?\nThis cannot be undone.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            SetStatus("Deleting...", CSilver);
            bool ok = CustomerService.DeleteCustomer(cid);
            if (ok) { MessageBox.Show("Customer deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); txtCustomerID.Clear(); txtUpdateName.Clear(); LoadCustomers(); SetStatus("Deleted.", CSuccess); }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRegFullName.Text)) { Warn("Please enter Full Name."); return; }
            if (txtRegIDNumber.Text.Trim().Length != 13) { Warn("ID Number must be 13 digits."); return; }
            decimal dep; if (!decimal.TryParse(txtRegDeposit.Text.Trim(), out dep) || dep < 0) { Warn("Please enter a valid deposit amount."); return; }
            SetStatus("Calling stored procedure...", CSilver);
            string acc = CustomerService.RegisterNewCustomerViaProcedure(txtRegFullName.Text.Trim(), txtRegIDNumber.Text.Trim(), dtpRegDOB.Value.Date, dep);
            if (acc != null) { lblLastAccount.Text = acc; MessageBox.Show("Registered!\n\nAccount: " + acc, "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information); SetStatus("Account: " + acc, CSuccess); txtRegFullName.Clear(); txtRegIDNumber.Clear(); txtRegDeposit.Clear(); LoadCustomers(); }
        }

        private void SetStatus(string msg, Color col) { lblStatus.Text = msg; lblStatus.ForeColor = col; }
        private void Warn(string msg) { MessageBox.Show(msg, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

        private void PaintCard(object sender, PaintEventArgs e)
        {
            var p = (Panel)sender;
            using (var br = new SolidBrush(Color.White)) e.Graphics.FillRectangle(br, p.ClientRectangle);
            using (var pen = new Pen(Color.FromArgb(220, 210, 240), 1)) e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
            using (var br = new SolidBrush(CPurpleMid)) e.Graphics.FillRectangle(br, 0, 0, p.Width, 3);
        }

        private Label MakeCardLbl(string t, Point loc) => new Label { Text = t, Font = FCard, ForeColor = CPurpleMid, AutoSize = true, Location = loc, BackColor = Color.Transparent };
        private Label MakeLbl(string t, Point loc) => new Label { Text = t, Font = FLabel, ForeColor = CText, AutoSize = true, Location = loc, BackColor = Color.Transparent };
        private TextBox MakeTxt(Point loc, int w) => new TextBox { Location = loc, Size = new Size(w, 26), Font = FInput, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(250, 248, 255) };
        private DateTimePicker MakeDtp(Point loc) => new DateTimePicker { Location = loc, Size = new Size(200, 26), Format = DateTimePickerFormat.Short, Font = FInput, Value = new DateTime(1995, 1, 1) };

        private Button MakeBtn(string text, Color back, Point loc, Size size)
        {
            var btn = new Button { Text = text, Location = loc, Size = size, FlatStyle = FlatStyle.Flat, BackColor = back, ForeColor = Color.White, Font = FBtn, Cursor = Cursors.Hand };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(back, 0.1f);
            return btn;
        }
    }
}