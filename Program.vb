
Imports System
Imports System.Windows.Forms
Imports MySqlConnector
Imports StayPointHMS.StayPointHMS

Friend Module Program

    <STAThread()>
    Public Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Try
            If Not AdministratorExists() Then
                Using setup As New StayPointHMS.FirstAdminSetupForm()
                    setup.ShowDialog()
                End Using

                ' Recheck after setup closes.
                If Not AdministratorExists() Then
                    Return
                End If
            End If

            Using login As New StayPointHMS.LoginForm()
                If login.ShowDialog() <> DialogResult.OK Then
                    Return
                End If
            End Using

            If String.Equals(CurrentUser.Role, "ADMIN",
                             StringComparison.OrdinalIgnoreCase) Then

                Application.Run(
                    New StayPointHMS.AdminDashboardForm())

            ElseIf String.Equals(CurrentUser.Role, "STAFF",
                                 StringComparison.OrdinalIgnoreCase) Then

                Application.Run(
                    New StayPointHMS.StaffMainDashboardForm())

            Else
                MessageBox.Show(
                    "Your account role is invalid. Contact the administrator.",
                    "StayPoint HMS",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            MessageBox.Show(
                "StayPoint HMS could not start." &
                Environment.NewLine & Environment.NewLine &
                ex.Message,
                "Startup Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function AdministratorExists() As Boolean
        Using conn = DbConnectionFactory.Create()
            conn.Open()

            Using cmd As New MySqlCommand(
                "SELECT COUNT(*) FROM users WHERE role = 'ADMIN' AND is_active = 1",
                conn)

                Return Convert.ToInt32(cmd.ExecuteScalar()) > 0
            End Using
        End Using
    End Function

End Module
