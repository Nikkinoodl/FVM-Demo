Imports System.Numerics
Imports Core.Common
Imports Core.Interfaces

Namespace Services
    Public Class Scaler : Implements IScaler

        Private ReadOnly data As IDataAccessService

        Public Sub New(data As IDataAccessService)

            Me.data = data

        End Sub

        Public Sub AirfoilScaler(farfield As Farfield) Implements IScaler.AirfoilScaler
            'scales the airfoil nodes to fit the chosen farfield parameters
            'and centers the airfoil in the farfield

            Dim offsetx, offsety, scale As Single

            offsetx = (farfield.Width - farfield.Scale) / 2
            offsety = farfield.Height / 2
            scale = farfield.Scale

            Dim offset As New Vector2(offsetx, offsety)

            For Each node In data.Nodelist
                If node.R.X = 1.0 And node.R.Y = 0.0 Then
                    node.Te_posn = True
                Else
                    node.Te_posn = False
                End If

                node.R *= scale
                node.R = Vector2.Add(node.R, offset)

            Next
        End Sub
    End Class
End Namespace