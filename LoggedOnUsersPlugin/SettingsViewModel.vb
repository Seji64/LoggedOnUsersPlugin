
Imports System.Collections.ObjectModel
Imports System.Windows.Data

Namespace LoggedOnUsersPlugin
    Public Class SettingsViewModel
        Inherits PropertyChangedBase

        Private m_lock_userlist As New Object

        Public Sub New()

            BindingOperations.EnableCollectionSynchronization(Me.UserList, m_lock_userlist)

        End Sub

        Public Async Function Init() As Threading.Tasks.Task(Of Boolean)

            Dim m_local_users As New List(Of String)

            Me.isBusy = True

            Me.UserList.Clear()

            m_local_users = Await System.Threading.Tasks.Task.Run(Function() UserHelper.GetLocalUsers)

            For Each m_user_object In m_local_users

                Dim m_user As New User

                m_user.Username = m_user_object

                Me.UserList.Add(m_user)

            Next

            Me.isBusy = False

            Return True

        End Function

        Private m_settings As New LoggedOnUsersPluginSettings
        Public Property Settings As LoggedOnUsersPluginSettings
            Set(value As LoggedOnUsersPluginSettings)
                m_settings = value
                RaisePropertyChanged("Settings")
            End Set
            Get
                Return m_settings
            End Get
        End Property


        Private m_userlist As New ObservableCollection(Of User)
        Public Property UserList As ObservableCollection(Of User)
            Set(value As ObservableCollection(Of User))
                m_userlist = value
                RaisePropertyChanged("UserList")
            End Set
            Get
                Return m_userlist
            End Get
        End Property

        Private m_is_busy As Boolean = False
        Public Property isBusy As Boolean
            Set(value As Boolean)
                m_is_busy = value
                RaisePropertyChanged("isBusy")
            End Set
            Get
                Return m_is_busy
            End Get
        End Property


    End Class

End Namespace

