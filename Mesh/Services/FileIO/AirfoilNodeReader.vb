Imports Mesh.Factories

Namespace Services
    Public Class AirfoilNodeReader : Implements IAirfoilNodeReader

        Private ReadOnly factory As IGridFactory
        Public Sub New(factory As IGridFactory)

            Me.factory = factory

        End Sub

        Public Sub ReadAirfoilNodes(filename As String) Implements IAirfoilNodeReader.ReadAirfoilNodes

            'reads a list of nodes from a flat file that will be used to model an object
            'no error checking is performed during the read

            Dim n As Integer = 0 'start loop at zero so that list index will match .id
            Dim objReader As New IO.StreamReader(filename)
            Dim thisstr As String

            'read airfoil data and assign coordinates to nodes
            '
            While objReader.Peek <> -1

                thisstr = objReader.ReadLine()
                Dim coord As String() = Split(thisstr, ",")

                'create the node
                factory.AddAirfoilNode(n, coord(0), coord(1))

                n += 1

            End While

        End Sub
    End Class
End Namespace