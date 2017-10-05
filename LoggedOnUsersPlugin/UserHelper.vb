Imports System.DirectoryServices

Namespace LoggedOnUsersPlugin
    Public Class UserHelper


        Private Shared m_localMachine As New DirectoryEntry("WinNT://" + Environment.MachineName)
        Private Const ACCOUNTDISABLE As Integer = &H2

        Private Shared Function isUserActive(ByVal m_user_entry As DirectoryEntry) As Boolean

            Dim m_userflags As Integer

            m_userflags = CInt(m_user_entry.Properties("UserFlags").Value)

            Return Not Convert.ToBoolean(m_userflags And ACCOUNTDISABLE)

        End Function
        Public Shared Function GetLocalUsers() As List(Of String)

            Dim m_rt_userlist As New List(Of String)

            For Each m_user As DirectoryEntry In m_localMachine.Children

                If (m_user.SchemaClassName = "User") Then

                    If isUserActive(m_user) = True Then
                        m_rt_userlist.Add(m_user.Name)
                    End If

                End If

            Next

            Return m_rt_userlist

        End Function


    End Class

End Namespace