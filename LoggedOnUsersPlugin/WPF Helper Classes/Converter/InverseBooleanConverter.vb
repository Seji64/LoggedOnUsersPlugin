
Imports System.Globalization
Imports System.Windows.Data

Namespace LoggedOnUsersPlugin

    <ValueConversion(GetType(Boolean), GetType(Boolean))>
    Public Class InverseBooleanConverter
        Implements IValueConverter
#Region "IValueConverter Members"

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
            If targetType <> GetType(Boolean) Then
                Throw New InvalidOperationException("The target must be a boolean")
            End If

            Return Not CBool(value)
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException()
        End Function

#End Region
    End Class

End Namespace
