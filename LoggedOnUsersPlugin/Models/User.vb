
Imports System.ComponentModel

Namespace LoggedOnUsersPlugin
    <Serializable>
    Public Class User
        Inherits PropertyChangedBase

        Private m_username As String = String.Empty
        Public Property Username As String
            Set(value As String)
                m_username = value
                RaisePropertyChanged("Username")
            End Set
            Get
                Return m_username
            End Get
        End Property

        Private m_is_selected As Boolean = False


        Public Property isSelected As Boolean
            Set(value As Boolean)
                m_is_selected = value
                RaisePropertyChanged("isSelected")
            End Set
            Get
                Return m_is_selected
            End Get
        End Property

    End Class

End Namespace

