using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Core.Domain;
using Core.DataCollections;

namespace UI
{
    public class PMCollection
    {
        public PlotModel uModel;
        public PlotModel vModel;
        public PlotModel pModel;
        public PlotModel tModel;

        public PMCollection()
        {

            //u, v and p plots are all based on scatter series plots so they can be set up together
            ScatterSeries seriesU = new();
            ScatterSeries seriesV = new();
            ScatterSeries seriesP = new();
            ScatterSeries seriesT = new();

            foreach (Cell p in Repository.CellList)
            {
                var ucolorValue = p.Vel.X;
                seriesU.Points.Add(new ScatterPoint(p.R.X, p.R.Y, 15, ucolorValue));

                var vcolorValue = p.Vel.Y;
                seriesV.Points.Add(new ScatterPoint(p.R.X, p.R.Y, 15, vcolorValue));

                var pcolorValue = p.P;
                seriesP.Points.Add(new ScatterPoint(p.R.X, p.R.Y, 15, pcolorValue));

                var bcolorValue = p.Test;
                seriesT.Points.Add(new ScatterPoint(p.R.X, p.R.Y, 15, bcolorValue));
            }

            //Add plot models for u, v velocities and pressure p
            PlotModel _uModel = new() { Title = "U Velocities in Lid Cavity", Background = OxyColors.White };
            PlotModel _vModel = new() { Title = "V Velocities in Lid Cavity", Background = OxyColors.White };
            PlotModel _pModel = new() { Title = "Pressure P in Lid Cavity", Background = OxyColors.White };
            PlotModel _tModel = new() { Title = "Testing and Debug", Background = OxyColors.White };

            //Add axes and assign heat map and contour series
            List<PlotModel> pmList = new() { _uModel, _vModel, _pModel, _tModel };

            foreach (var m in pmList)
            {
                m.Axes.Add(new LinearColorAxis
                {
                    Position = AxisPosition.Right,
                    Palette = OxyPalettes.Jet(256),
                    HighColor = OxyColors.Yellow,
                    LowColor = OxyColors.Black,
                    UseSuperExponentialFormat = true,
                    TextColor = OxyColors.Black
                });

                m.Axes.Add(new LinearAxis()
                {
                    MajorGridlineStyle = LineStyle.Solid,
                    Position = AxisPosition.Left,
                    Title = "Y position"
                });

                m.Axes.Add(new LinearAxis()
                {
                    MajorGridlineStyle = LineStyle.Solid,
                    Position = AxisPosition.Bottom,
                    Title = "X Position"
                });
            }

            _uModel.Series.Add(seriesU);
            _vModel.Series.Add(seriesV);
            _pModel.Series.Add(seriesP);
            _tModel.Series.Add(seriesT);

            uModel = _uModel;
            vModel = _vModel;
            pModel = _pModel;
            tModel = _tModel;
        }
    }
}
