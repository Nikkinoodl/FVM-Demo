Imports System.Xml
Imports Mesh.AppSettings

Namespace AppSettings
    Public Class Settings : Inherits BaseSettings
#Region "constructor"
        Public Sub New()
        End Sub
#End Region

#Region "fields"
        'Inherited from BaseSettings
#End Region

#Region "public properties"
        'on read takes the string and converts to specific type
        'on write takes a specific type and converts to a string
        '
        Public Overrides Property Width As Integer
            Get
                Return _width
            End Get
            Set(value As Integer)
                _width = CStr(value)
            End Set
        End Property
        Public Overrides Property Height As Integer
            Get
                Return _height
            End Get
            Set(value As Integer)
                _height = CStr(value)
            End Set
        End Property
        Public Overrides Property Scale As Short
            Get
                Return _scale
            End Get
            Set(value As Short)
                _scale = CStr(value)
            End Set
        End Property
        Public Overrides Property Layers As Short
            Get
                Return _layers
            End Get
            Set(value As Short)
                _layers = CStr(value)
            End Set
        End Property
        Public Overrides Property Cellheight As Short
            Get
                Return _cellheight
            End Get
            Set(value As Short)
                _cellheight = CStr(value)
            End Set
        End Property
        Public Overrides Property Cellfactor As Double
            Get
                Return _cellfactor
            End Get
            Set(value As Double)
                _cellfactor = value
            End Set
        End Property
        Public Overrides Property Nodetrade As Short
            Get
                Return _nodetrade
            End Get
            Set(value As Short)
                _nodetrade = CStr(value)
            End Set
        End Property
        Public Overrides Property Expansionpower As Double
            Get
                Return _expansionpower
            End Get
            Set(value As Double)
                _expansionpower = value
            End Set
        End Property
        Public Overrides Property Offset As Short
            Get
                Return _offset
            End Get
            Set(value As Short)
                If value < 2 Then
                    MessageBox.Show(MeshConstants.MSGMINOFFSET)
                    value = 2
                End If
                _offset = value
            End Set
        End Property
        Public Overrides Property Smoothingcycles As Short
            Get
                Return _smoothingcycles
            End Get
            Set(value As Short)
                If value < 1 Then
                    MessageBox.Show(MeshConstants.MSGSMOOTHINGCYCLES)
                    value = 1
                End If
                _smoothingcycles = value
            End Set
        End Property
        Public Overrides Property Airfoilnodes As Short
            Get
                Return _airfoilnodes
            End Get
            Set(value As Short)
                _airfoilnodes = CStr(value)
            End Set
        End Property
        Public Overrides Property Filename As String
            Get
                Return _filename
            End Get
            Set(value As String)
                _filename = value
            End Set
        End Property

#End Region

    End Class
End Namespace
