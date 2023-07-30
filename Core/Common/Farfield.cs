namespace Core.Common
{
    /// <summary>
    /// The space in which the grid is constructed and the CFD is performed. This class is used to 
    /// propogate settings throughout the grid construction and CFD processes.
    /// </summary>
    public class Farfield : Settings
    {

        // Properties that are calculated from other properties in the space
        public int NodesOnBoundary(string side, int numnodes)
        {
            sbyte s = 1;
            double perimeter = 2 * (Height + Width);
            double linearDimension = 0;

            switch (side)
            {
                case "width":
                    {
                        s = 1;     // add to the horizontals
                        linearDimension = Width;
                        break;
                    }

                case "height":
                    {
                        s = -1;    // subtract from the verticals
                        linearDimension = Height;
                        break;
                    }
            }

            // nodetrade lets distribution of nodes be changed at build time
            // we take nodes from the vertical boundary edges and give them to the horizontal boundary edges
            return (int)Math.Round(((numnodes - 4) * linearDimension / perimeter) + s * Nodetrade);
        }

        public double CellHeight(int layer)
        {
            // Calculates the height of the layer (orthogonal distance relative to the line connecting nextn, n)

            double h = Cellheight * Cellfactor * Math.Pow(layer, Expansionpower);

            return h;
        }

        public void ValidateNodeTrade(int numnodes)
        {
            if (Nodetrade > (numnodes - 4) / (double)4)
            {
                Nodetrade = 0;
                //MsgBox(MeshConstants.MSGNUMNODES);
            }
        }

        public void ValidateOffset(int numnodes)
        {
            // offset must be less than numnodes to avoid out of range exceptions
            if (Offset >= numnodes)
            {
                Offset = (short)(numnodes - 1);
                //MsgBox(MeshConstants.MSGOFFSET);
            }
        }
    }
}
