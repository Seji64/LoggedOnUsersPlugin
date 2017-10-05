'Taken from https://blogs.msdn.microsoft.com/atc_avalon_team/2006/04/10/fixed-width-column-in-listview-a-column-that-cannot-be-resized/

Imports System.Windows
Imports System.Windows.Controls

Namespace LoggedOnUsersPlugin
    Public Class FixedWidthColumn
        Inherits GridViewColumn
#Region "Constructor"

        Shared Sub New()
            WidthProperty.OverrideMetadata(GetType(FixedWidthColumn), New FrameworkPropertyMetadata(Nothing, New CoerceValueCallback(AddressOf OnCoerceWidth)))
        End Sub

        Private Shared Function OnCoerceWidth(o As DependencyObject, baseValue As Object) As Object
            Dim fwc As FixedWidthColumn = TryCast(o, FixedWidthColumn)
            If fwc IsNot Nothing Then
                Return fwc.FixedWidth
            End If
            Return 0.0
        End Function

#End Region

#Region "FixedWidth"

        Public Property FixedWidth() As Double
            Get
                Return CDbl(GetValue(FixedWidthProperty))
            End Get
            Set
                SetValue(FixedWidthProperty, Value)
            End Set
        End Property

        Public Shared ReadOnly FixedWidthProperty As DependencyProperty = DependencyProperty.Register("FixedWidth", GetType(Double), GetType(FixedWidthColumn), New FrameworkPropertyMetadata(Double.NaN, New PropertyChangedCallback(AddressOf OnFixedWidthChanged)))

        Private Shared Sub OnFixedWidthChanged(o As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim fwc As FixedWidthColumn = TryCast(o, FixedWidthColumn)
            If fwc IsNot Nothing Then
                fwc.CoerceValue(WidthProperty)
            End If
        End Sub

#End Region
    End Class

End Namespace


