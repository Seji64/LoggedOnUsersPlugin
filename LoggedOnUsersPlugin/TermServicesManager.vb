Imports System.Runtime.InteropServices

Namespace LoggedOnUsersPlugin

    ''' <summary>
    ''' Taken from https://stackoverflow.com/a/13987558
    ''' THX!
    ''' </summary>
    ''' 
    Public Class TermServicesManager

        <DllImport("wtsapi32.dll")>
        Private Shared Function WTSOpenServer(<MarshalAs(UnmanagedType.LPStr)> pServerName As [String]) As IntPtr
        End Function

        <DllImport("wtsapi32.dll")>
        Private Shared Sub WTSCloseServer(hServer As IntPtr)
        End Sub

        <DllImport("Wtsapi32.dll")>
        Public Shared Function WTSQuerySessionInformation(hServer As IntPtr, sessionId As Integer, wtsInfoClass As WTS_INFO_CLASS, ByRef ppBuffer As System.IntPtr, ByRef pBytesReturned As UInteger) As Boolean
        End Function

        <DllImport("wtsapi32.dll")>
        Private Shared Function WTSEnumerateSessions(hServer As IntPtr, <MarshalAs(UnmanagedType.U4)> Reserved As Int32, <MarshalAs(UnmanagedType.U4)> Version As Int32, ByRef ppSessionInfo As IntPtr, <MarshalAs(UnmanagedType.U4)> ByRef pCount As Int32) As Int32
        End Function

        <DllImport("wtsapi32.dll")>
        Private Shared Sub WTSFreeMemory(pMemory As IntPtr)
        End Sub

        <StructLayout(LayoutKind.Sequential)>
        Private Structure WTS_SESSION_INFO
            Public SessionID As Int32
            <MarshalAs(UnmanagedType.LPStr)>
            Public pWinStationName As [String]
            Public State As WTS_CONNECTSTATE_CLASS
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure WTS_CLIENT_ADDRESS
            Public AddressFamily As UInteger
            <MarshalAs(UnmanagedType.ByValArray, SizeConst:=20)>
            Public Address As Byte()
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure WTS_CLIENT_DISPLAY
            Public HorizontalResolution As UInteger
            Public VerticalResolution As UInteger
            Public ColorDepth As UInteger
        End Structure

        Public Enum WTS_CONNECTSTATE_CLASS
            Active = 0
            Connected = 1
            ConnectQuery = 2
            Shadow = 3
            Disconnected = 4
            Idle = 5
            Listen = 6
            Reset = 7
            Down = 8
            Init = 9
        End Enum

        Public Enum WTS_INFO_CLASS
            InitialProgram = 0
            ApplicationName = 1
            WorkingDirectory = 2
            OEMId = 3
            SessionId = 4
            UserName = 5
            WinStationName = 6
            DomainName = 7
            ConnectState = 8
            ClientBuildNumber = 9
            ClientName = 10
            ClientDirectory = 11
            ClientProductId = 12
            ClientHardwareId = 13
            ClientAddress = 14
            ClientDisplay = 15
            ClientProtocolType = 16
        End Enum

        Private Shared Function OpenServer(Name As String) As IntPtr
            Dim server As IntPtr = WTSOpenServer(Name)
            Return server
        End Function

        Private Shared Sub CloseServer(ServerHandle As IntPtr)
            WTSCloseServer(ServerHandle)
        End Sub

        Public Shared Function ListSessions(ServerName As String) As List(Of TerminalSessionData)
            Dim server As IntPtr = IntPtr.Zero
            Dim ret As New List(Of TerminalSessionData)()
            server = OpenServer(ServerName)

            Try
                Dim ppSessionInfo As IntPtr = IntPtr.Zero

                Dim count As Int32 = 0
                Dim retval As Int32 = WTSEnumerateSessions(server, 0, 1, ppSessionInfo, count)
                Dim dataSize As Int32 = Marshal.SizeOf(GetType(WTS_SESSION_INFO))

                Dim current As Int64 = CInt(ppSessionInfo)

                If retval <> 0 Then
                    For i As Integer = 0 To count - 1
                        Dim temp As IntPtr = New IntPtr(current)
                        Dim si As WTS_SESSION_INFO = CType(Marshal.PtrToStructure(temp, GetType(WTS_SESSION_INFO)), WTS_SESSION_INFO)
                        current += dataSize

                        ret.Add(New TerminalSessionData(si.SessionID, si.State, si.pWinStationName))
                    Next

                    WTSFreeMemory(ppSessionInfo)
                End If
            Finally
                CloseServer(server)
            End Try

            Return ret
        End Function

        Public Shared Function GetSessionInfo(ServerName As String, SessionId As Integer) As TerminalSessionInfo
            Dim server As IntPtr = IntPtr.Zero
            server = OpenServer(ServerName)
            Dim buffer As System.IntPtr = IntPtr.Zero
            Dim bytesReturned As UInteger
            Dim data As New TerminalSessionInfo()

            Try
                Dim worked As Boolean = WTSQuerySessionInformation(server, SessionId, WTS_INFO_CLASS.ApplicationName, buffer, bytesReturned)

                If Not worked Then
                    Return data
                End If

                Dim strData As String = Marshal.PtrToStringAnsi(buffer)
                data.ApplicationName = strData

                worked = WTSQuerySessionInformation(server, SessionId, WTS_INFO_CLASS.ClientAddress, buffer, bytesReturned)

                If Not worked Then
                    Return data
                End If

                Dim si As WTS_CLIENT_ADDRESS = CType(Marshal.PtrToStructure(buffer, GetType(WTS_CLIENT_ADDRESS)), WTS_CLIENT_ADDRESS)
                data.ClientAddress = si

                worked = WTSQuerySessionInformation(server, SessionId, WTS_INFO_CLASS.ClientBuildNumber, buffer, bytesReturned)

                If Not worked Then
                    Return data
                End If

                Dim lData As Integer = Marshal.ReadInt32(buffer)
                data.ClientBuildNumber = lData

                worked = WTSQuerySessionInformation(server, SessionId, WTS_INFO_CLASS.ClientDirectory, buffer, bytesReturned)

                If Not worked Then
                    Return data
                End If

                strData = Marshal.PtrToStringAnsi(buffer)
                data.ClientDirectory = strData

                worked = WTSQuerySessionInformation(server, SessionId, WTS_INFO_CLASS.ClientDisplay, buffer, bytesReturned)

                If Not worked Then
                    Return data
                End If

                Dim cd As WTS_CLIENT_DISPLAY = CType(Marshal.PtrToStructure(buffer, GetType(WTS_CLIENT_DISPLAY)), WTS_CLIENT_DISPLAY)
                data.ClientDisplay = cd

                worked = WTSQuerySessionInformation(server, SessionId, WTS_INFO_CLASS.ClientHardwareId, buffer, bytesReturned)

                If Not worked Then
                    Return data
                End If

                lData = Marshal.ReadInt32(buffer)
                data.ClientHardwareId = lData

                worked = WTSQuerySessionInformation(server, SessionId, WTS_INFO_CLASS.ClientName, buffer, bytesReturned)
                strData = Marshal.PtrToStringAnsi(buffer)
                data.ClientName = strData

                worked = WTSQuerySessionInformation(server, SessionId, WTS_INFO_CLASS.ClientProductId, buffer, bytesReturned)
                Dim intData As Int16 = Marshal.ReadInt16(buffer)
                data.ClientProductId = intData

                worked = WTSQuerySessionInformation(server, SessionId, WTS_INFO_CLASS.ClientProtocolType, buffer, bytesReturned)
                intData = Marshal.ReadInt16(buffer)
                data.ClientProtocolType = intData

                worked = WTSQuerySessionInformation(server, SessionId, WTS_INFO_CLASS.ConnectState, buffer, bytesReturned)
                lData = Marshal.ReadInt32(buffer)
                data.ConnectState = CType([Enum].ToObject(GetType(WTS_CONNECTSTATE_CLASS), lData), WTS_CONNECTSTATE_CLASS)

                worked = WTSQuerySessionInformation(server, SessionId, WTS_INFO_CLASS.DomainName, buffer, bytesReturned)
                strData = Marshal.PtrToStringAnsi(buffer)
                data.DomainName = strData

                worked = WTSQuerySessionInformation(server, SessionId, WTS_INFO_CLASS.InitialProgram, buffer, bytesReturned)
                strData = Marshal.PtrToStringAnsi(buffer)
                data.InitialProgram = strData

                worked = WTSQuerySessionInformation(server, SessionId, WTS_INFO_CLASS.OEMId, buffer, bytesReturned)
                strData = Marshal.PtrToStringAnsi(buffer)
                data.OEMId = strData

                worked = WTSQuerySessionInformation(server, SessionId, WTS_INFO_CLASS.SessionId, buffer, bytesReturned)
                lData = Marshal.ReadInt32(buffer)
                data.SessionId = lData

                worked = WTSQuerySessionInformation(server, SessionId, WTS_INFO_CLASS.UserName, buffer, bytesReturned)
                strData = Marshal.PtrToStringAnsi(buffer)
                data.UserName = strData

                worked = WTSQuerySessionInformation(server, SessionId, WTS_INFO_CLASS.WinStationName, buffer, bytesReturned)
                strData = Marshal.PtrToStringAnsi(buffer)
                data.WinStationName = strData

                worked = WTSQuerySessionInformation(server, SessionId, WTS_INFO_CLASS.WorkingDirectory, buffer, bytesReturned)
                strData = Marshal.PtrToStringAnsi(buffer)
                data.WorkingDirectory = strData
            Finally
                WTSFreeMemory(buffer)
                buffer = IntPtr.Zero
                CloseServer(server)
            End Try

            Return data
        End Function

    End Class

    Public Class TerminalSessionData
        Public SessionId As Integer
        Public ConnectionState As TermServicesManager.WTS_CONNECTSTATE_CLASS
        Public StationName As String

        Public Sub New(sessionId__1 As Integer, connState As TermServicesManager.WTS_CONNECTSTATE_CLASS, stationName__2 As String)
            SessionId = sessionId__1
            ConnectionState = connState
            StationName = stationName__2
        End Sub

        Public Overrides Function ToString() As String
            Return [String].Format("{0} {1} {2}", SessionId, ConnectionState, StationName)
        End Function
    End Class

    Public Class TerminalSessionInfo
        Public InitialProgram As String
        Public ApplicationName As String
        Public WorkingDirectory As String
        Public OEMId As String
        Public SessionId As Integer
        Public UserName As String
        Public WinStationName As String
        Public DomainName As String
        Public ConnectState As TermServicesManager.WTS_CONNECTSTATE_CLASS
        Public ClientBuildNumber As Integer
        Public ClientName As String
        Public ClientDirectory As String
        Public ClientProductId As Integer
        Public ClientHardwareId As Integer
        Public ClientAddress As TermServicesManager.WTS_CLIENT_ADDRESS
        Public ClientDisplay As TermServicesManager.WTS_CLIENT_DISPLAY
        Public ClientProtocolType As Integer
    End Class

End Namespace
