﻿Namespace AppSettings
    Public Class Calcdomain : Inherits BaseSettings

#Region "constructor"
        Public Sub New()
        End Sub
#End Region

#Region "fields"
        'Inherits protected variables from basesettings.vb
#End Region

#Region "public properties"
        'Properties that are calculated from other properties in the calc domain

        Public ReadOnly Property NodesOnBoundary(ByVal side As String, ByVal numnodes As Integer) As Integer
            Get
                Dim s As SByte
                Dim perimeter As Double = 2 * (Me.Height + Me.Width)
                Dim linearDimension As Double

                Select Case side
                    Case "width"
                        s = 1     'add to the horizontals
                        linearDimension = Me.Width
                    Case "height"
                        s = -1    'subtract from the verticals
                        linearDimension = Me.Height
                End Select

                'nodetrade lets distribution of nodes be changed at build time
                'we take nodes from the vertical boundary edges and give them to the horizontal boundary edges
                Return (numnodes - 4) * linearDimension / perimeter + s * Me.Nodetrade

            End Get
        End Property

        Public ReadOnly Property TriangleHeight(ByVal layer As Integer) As Double
            Get
                'Calculates the height of the layer (orthogonal distance relative to the line connecting nextn, n)

                Dim h As Double
                h = Me.Cellheight * Me.Cellfactor * System.Math.Pow(layer, Me.Expansionpower)

                Return h

            End Get
        End Property

#End Region

#Region "Public Methods"
        Public Sub ValidateNodeTrade(ByVal numnodes As Integer)
            If Me.Nodetrade > (numnodes - 4) / 4 Then
                Me.Nodetrade = 0
                MsgBox(Constants.MsgNumNodes)
            End If
        End Sub

        Public Sub ValidateOffset(ByVal numnodes As Integer)
            'offset must be less than numnodes to avoid out of range exceptions
            If Me.Offset >= numnodes Then
                Me.Offset = numnodes - 1
                MsgBox(Constants.MsgOffset)
            End If
        End Sub

#End Region

    End Class
End Namespace