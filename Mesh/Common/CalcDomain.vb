Namespace AppSettings
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

        Public ReadOnly Property NodesOnBoundary(side As String, numnodes As Integer) As Integer
            Get
                Dim s As SByte
                Dim perimeter As Double = 2 * (Height + Width)
                Dim linearDimension As Double

                Select Case side
                    Case "width"
                        s = 1     'add to the horizontals
                        linearDimension = Width
                    Case "height"
                        s = -1    'subtract from the verticals
                        linearDimension = Height
                End Select

                'nodetrade lets distribution of nodes be changed at build time
                'we take nodes from the vertical boundary edges and give them to the horizontal boundary edges
                Return (numnodes - 4) * linearDimension / perimeter + s * Nodetrade

            End Get
        End Property

        Public ReadOnly Property TriangleHeight(layer As Integer) As Double
            Get
                'Calculates the height of the layer (orthogonal distance relative to the line connecting nextn, n)

                Dim h As Double
                h = Cellheight * Cellfactor * System.Math.Pow(layer, Expansionpower)

                Return h

            End Get
        End Property

#End Region

#Region "Public Methods"
        Public Sub ValidateNodeTrade(numnodes As Integer)
            If Nodetrade > (numnodes - 4) / 4 Then
                Nodetrade = 0
                MsgBox(MeshConstants.MSGNUMNODES)
            End If
        End Sub

        Public Sub ValidateOffset(numnodes As Integer)
            'offset must be less than numnodes to avoid out of range exceptions
            If Offset >= numnodes Then
                Offset = numnodes - 1
                MsgBox(MeshConstants.MSGOFFSET)
            End If
        End Sub

#End Region

    End Class
End Namespace