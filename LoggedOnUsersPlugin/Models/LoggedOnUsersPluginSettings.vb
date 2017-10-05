Imports System.Collections.ObjectModel
Imports System.ComponentModel

Namespace LoggedOnUsersPlugin

    <Serializable>
    Public Class LoggedOnUsersPluginSettings

        Private m_userlist As New List(Of User)

        <Xml.Serialization.XmlElement>
        Public Property UserList As List(Of User)
            Set(value As List(Of User))
                m_userlist = value
            End Set
            Get
                Return m_userlist
            End Get
        End Property

    End Class

End Namespace


