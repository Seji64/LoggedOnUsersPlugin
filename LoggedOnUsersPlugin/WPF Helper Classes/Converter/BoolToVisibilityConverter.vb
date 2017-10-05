Imports System.Globalization
Imports System.Windows
Imports System.Windows.Data

Namespace LoggedOnUsersPlugin

    <ValueConversion(GetType(Boolean), GetType(Visibility))>
    Public Class BoolToVisibilityConverter
        Implements IValueConverter
        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            Try
                Dim v = CBool(value)
                Return If(v, Visibility.Visible, Visibility.Hidden)
            Catch generatedExceptionName As InvalidCastException
                Return Visibility.Hidden
            End Try
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class

End Namespace