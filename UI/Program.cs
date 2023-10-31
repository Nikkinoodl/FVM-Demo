using Core.Interfaces;
using Core.Data;
using Mesh.Logic;
using Mesh.Services;
using Mesh.Factories;
using CFDCore;
using CFD;

namespace CFDSolv
{
    internal static class Program
    {
        public static readonly SimpleInjector.Container container = new();

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();
            Bootstrapper();
            Application.Run(container.GetInstance<Form2>());
        }
        private static void Bootstrapper()
        {
            container.Options.EnableAutoVerification = false;

            //Mesh Generation
            //UI Layer
            container.Register<Form1>();
            container.Register<Form2>();

            //logic layer
            container.Register<EmptySpace>();
            container.Register<DelaunayLogic>();
            container.Register<Redistribute>();
            container.Register<Smooth>();
            container.Register<Split>();
            container.Register<ResetData>();
            container.Register<Finalize>();
            container.Register<CFDLogic>();
            container.Register<TilingLogic>();

            //Mesh building services
            container.Register<IInitializer, Initializer>();
            container.Register<IStatusSetter, StatusSetter>();
            container.Register<ICellCalculator, CellCalculator>();
            container.Register<IBoundaryNodeChecker, BoundaryNodeChecker>();
            container.Register<ICellSplitter, CellSplitter>();
            container.Register<IGridSmoother, GridSmoother>();
            container.Register<IEmptySpaceBuilder, EmptySpaceBuilder>();
            container.Register<IDelaunay, Delaunay>();
            container.Register<IRedistributor, Redistributor>();
            container.Register<IBorderCellBuilder, BorderCellBuilder>();
            container.Register<IMeshPrecalc, MeshPrecalc>();

            //CFD services
            container.Register<IBCSetter, BCSetter>();
            container.Register<ICalcEngine, CalcEngine>();

            //factories
            container.Register<IGridFactory, GridFactory>();
            container.Register<IFinalizingFactory, FinalizingFactory>();

            //data layer
            container.Register<IDataPreparer, DataPreparer>();
            container.Register<IDataAccessService, DataAccessService>();

        }
    }
}