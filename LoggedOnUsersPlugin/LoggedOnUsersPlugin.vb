Imports System.ComponentModel.Composition
Imports Wsapm.Extensions

Namespace LoggedOnUsersPlugin

    <Export(GetType(WsapmPluginBase))>
    <WsapmPlugin("LoggedOnUsers", "v1.0.2", "{C7DB166B-FBA9-4D99-A5ED-CFFFCDAE2715}")>
    Public Class LoggedOnUsersPlugin
        Inherits WsapmPluginAdvancedBase

        Private m_settingsControl As LoggedOnUsersPluginControl

        Public Sub New()
            MyBase.New(GetType(LoggedOnUsersPluginSettings))
        End Sub

        Public Overrides ReadOnly Property SettingsControl As Object
            Get

                If IsNothing(m_settingsControl) Then
                    m_settingsControl = New LoggedOnUsersPluginControl
                End If

                Return m_settingsControl

            End Get
        End Property

        Protected Overrides Function CheckPluginPolicy() As PluginCheckSuspendResult

            Dim m_pluginSettings As LoggedOnUsersPluginSettings = CType(Me.CurrentSettings, LoggedOnUsersPluginSettings)

            Try

                If IsNothing(m_pluginSettings) Or IsNothing(m_pluginSettings.UserList) Then
                    Return New PluginCheckSuspendResult(False, String.Empty)
                End If

                For Each m_session In TermServicesManager.ListSessions(Environment.MachineName)

                    Dim m_session_info As TerminalSessionInfo

                    m_session_info = TermServicesManager.GetSessionInfo(Environment.MachineName, m_session.SessionId)

                    If Not IsNothing(m_session_info) AndAlso Not String.IsNullOrWhiteSpace(m_session_info.UserName) Then

                        'No specific Users defined - All active Sessions are matching
                        If m_pluginSettings.UserList.Count = 0 Then

                            If m_session_info.ConnectState = TermServicesManager.WTS_CONNECTSTATE_CLASS.Active Then
                                Return New PluginCheckSuspendResult(True, String.Format(Resources.Strings.PluginCheckSuspendResult_Message, m_session_info.UserName))
                            End If

                        Else

                            'Check if current user is configured to keep this server alive
                            If m_session_info.ConnectState = TermServicesManager.WTS_CONNECTSTATE_CLASS.Active And m_pluginSettings.UserList.Exists(Function(x) x.Username.ToLower.Equals(m_session_info.UserName.ToLower)) Then
                                Return New PluginCheckSuspendResult(True, String.Format(Resources.Strings.PluginCheckSuspendResult_Message, m_session_info.UserName))
                            End If

                        End If

                    End If

                Next

                'No User Session has matched
                Return New PluginCheckSuspendResult(False, String.Empty)

            Catch ex As Exception
                Return New PluginCheckSuspendResult(False, String.Empty)
            End Try

        End Function

        Protected Overrides Function Initialize() As Boolean
            Return True
        End Function

        Protected Overrides Function LoadDefaultSettings() As Object
            Return New LoggedOnUsersPluginSettings
        End Function

        Protected Overrides Function Prepare() As Boolean
            Return True
        End Function

        Protected Overrides Function TearDown() As Boolean
            Return True
        End Function
    End Class

End Namespace