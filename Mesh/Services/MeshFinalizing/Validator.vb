Imports System.Windows.Forms.VisualStyles
Imports Core.Common
Imports Core.Domain
Imports Core.Interfaces
Imports Mesh.Factories

Namespace Services
    Public Class Validator : Implements IValidator

        Private ReadOnly data As IDataAccessService
        Private ReadOnly factory As IFinalizingFactory

        Public Sub New(data As IDataAccessService, factory As IFinalizingFactory)

            Me.data = data
            Me.factory = factory

        End Sub

        ''' <summary>
        ''' Validates the edge of each rectangular cell to ensure that it is properly aligned
        ''' </summary>
        Public Sub InspectEdges() Implements IValidator.InspectEdges

            For Each c As Cell In data.CalcCells

                Dim left As Edge = (From e As Edge In c.Edges
                                    Order By e.R.X Ascending
                                    Select e).First()

                Dim top As Edge = (From e As Edge In c.Edges
                                   Order By e.R.Y Descending
                                   Select e).First()

                Dim right As Edge = (From e As Edge In c.Edges
                                     Order By e.R.X Descending
                                     Select e).First()

                Dim bottom As Edge = (From e As Edge In c.Edges
                                      Order By e.R.Y Ascending
                                      Select e).First()

                Debug.Assert(left.SideName = SideName.S1)

                Debug.Assert(top.SideName = SideName.S2)

                Debug.Assert(right.SideName = SideName.S3)

                Debug.Assert(bottom.SideName = SideName.S4)

                For Each e In c.Edges

                    If e.R.Y = 0 Then

                        Debug.Assert(e.SideType = SideType.boundary)

                    End If


                Next

            Next

        End Sub

    End Class
End Namespace
