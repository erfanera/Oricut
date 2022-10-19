using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Oricut
{
    public class Diamond : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent2 class.
        /// </summary>
        public Diamond()
          : base("Diamond", "Diamond",
              "Draw diamon pattern from point grid",
              "Oricut", "Patterns")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("grid", "grid", "grid input", GH_ParamAccess.tree);
            pManager.AddBooleanParameter("Flip", "Flip", "True to flip the grid", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Radial", "Radial", "True to ", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Mountain", "Mountain", "Mountain lines", GH_ParamAccess.list);
            pManager.AddCurveParameter("Valley", "Valley", "Valley Lines", GH_ParamAccess.list);
            pManager.AddCurveParameter("Border", "Border", "Border Lines", GH_ParamAccess.list);
         
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool flip = false;
            bool radial = false;

            DA.GetData(1, ref flip);
            DA.GetData(2, ref radial);

            Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Point> ptGrid = new Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Point>();
            Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Point> ptGridEx = new Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Point>();
            Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Point> ptgridd = new Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Point>();
            List<Line> mountain = new List<Line>();
            List<Line> valley = new List<Line>();
            List<Line> border = new List<Line>();



            if (!DA.GetDataTree<Grasshopper.Kernel.Types.GH_Point>(0, out ptGrid)) return;


            Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Point> tr_out = ptGrid;
            var test = ptGrid.Branches[0];
            var situtation = 0;                //if there is no change just a simple grid


            //--------------------------------------------------------------------First two functions


            if (radial)
            {
                if (flip) { situtation = 1; }
                if (flip == false) { situtation = 2; }
                for (int kk = 0; kk < ptGrid.PathCount; kk++)
                {
                    ptGrid[kk].Add(ptGrid[kk][0]);
                    if (situtation == 1)
                    {
                        ptGrid[kk].Add(ptGrid[kk][1]);

                    }

                }
            }

            if (flip)
            {
                for (int i = 0; i < ptGrid.Branches[0].Count; i++)
                {
                    var newpath = new Grasshopper.Kernel.Data.GH_Path(i);
                    for (int p = 0; p < ptGrid.PathCount; p++)
                    {

                        ptGridEx.Insert(ptGrid[p][i], newpath, p);

                    }
                }
                ptGrid = ptGridEx;
            }
            DA.SetDataTree(0, ptGrid);
        }
            //----------------------------------------------------------------------------MAin Loop
            //for (int p = 0; p < ptGrid.PathCount; p++)
            //{


            //    for (int i = 0; i < ptGrid.Branches[0].Count; i++)
            //    {

            //    }
            //}
        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Resource1.diamond_2;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("69271757-d739-4114-9353-2c1ba4497b19"); }
        }
    }
}