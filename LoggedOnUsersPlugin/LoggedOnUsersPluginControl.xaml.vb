Imports System.Windows.Controls
Imports Wsapm.Extensions

Namespace LoggedOnUsersPlugin
    Public Class LoggedOnUsersPluginControl
        Inherits UserControl
        Implements IWsapmPluginSettingsControl

        Dim _settingsviewmodel As SettingsViewModel
        Public Sub New()

            InitializeComponent()

            _settingsviewmodel = New SettingsViewModel

            Me.DataContext = _settingsviewmodel

        End Sub

        Public Async Sub SetSettings(settings As Object) Implements IWsapmPluginSettingsControl.SetSettings

            Dim mysettings As LoggedOnUsersPluginSettings = CType(settings, LoggedOnUsersPluginSettings)

            Await _settingsviewmodel.Init()

            For Each m_user In _settingsviewmodel.UserList

                If mysettings.UserList.Exists(Function(x) x.Username.ToLower.Equals(m_user.Username.ToLower)) Then
                    m_user.isSelected = True
                End If

            Next

        End Sub

        Public Function GetSettingsBeforeSave() As Object Implements IWsapmPluginSettingsControl.GetSettingsBeforeSave

            _settingsviewmodel.Settings.UserList.Clear()

            For Each m_user In _settingsviewmodel.UserList

                If m_user.isSelected = True Then
                    _settingsviewmodel.Settings.UserList.Add(m_user)
                End If

            Next

            Return _settingsviewmodel.Settings

        End Function
    End Class

End Namespace