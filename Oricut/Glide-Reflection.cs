using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Oricut
{
    public class MyComponent1 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public MyComponent1()
          : base("Glide Reflection", "glide reflection",
              "draw glide reflection pattern from grid of points",
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
            pManager.AddTextParameter("Report", "Report", "Report", GH_ParamAccess.item);
            pManager.AddCurveParameter("Border", "Border", "Border Lines", GH_ParamAccess.list);
            pManager.AddCurveParameter("Mountain","Mountain", "Mountain lines", GH_ParamAccess.list);
            pManager.AddCurveParameter("Valley", "Valley", "Valley Lines", GH_ParamAccess.list);
            
        }

        List<Line> border = new List<Line>();
        List<Line> mountain = new List<Line>();
        List<Line> valley = new List<Line>();
        protected override void SolveInstance(IGH_DataAccess DA)
        {


            //-----------------------------------------------INPUT MANAGMENT
            bool flip = false;
            bool radial = false;

            mountain.Clear();
            valley.Clear();
            border.Clear();


            DA.GetData(1, ref flip);
            DA.GetData(2, ref radial);
            Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Point> ptGridsafe = new Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Point>();
            Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Point> ptGrid = new Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Point>();
            Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Point> ptGridEx = new Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Point>();
            Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Point> ptgridd = new Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Point>();

            var situtation = 0;
            if (flip) { situtation = 4; }
            if (flip & radial) { situtation = 1; }
            if (!flip & radial) { situtation = 2; }

            if (!DA.GetDataTree<Grasshopper.Kernel.Types.GH_Point>(0, out ptGrid)) return;
            ptGridsafe = ptGrid.Duplicate();
            //you have to make it better later

            if (radial) {
                //ptGrid.RemovePath(ptGrid.get_Path(0));

                ptGridsafe.RemovePath(ptGrid.get_Path(0));
            }

            



            Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Point> tr_out = ptGrid;
            var test = ptGrid.Branches[0];
                           //if there is no change just a simple grid
            string report = "every thing works great";


            //--------------------------------------------------------------------First two functions

            if (ptGrid.Branches[0].Count % 2 == 0 & (situtation == 1 || situtation == 2))
            {
                report = "try to supply even number for both of columns and rows ";
            }


            if (radial)
            {
                
                ptGrid = ptGridsafe.Duplicate();

                for (int kk = 0; kk < ptGrid.PathCount; kk++)
                {

                    ptGrid[kk].Add(ptGrid[kk][0]);


                }
                ptgridd = ptGrid.Duplicate();

            }
            if (ptGrid.Branches[0].Count % 2 == 0 & (situtation == 1 || situtation == 2))
            {
                report = "try to supply even number for both of columns and rows ";
            }


            if (situtation == 4)
            {
                ptGrid = ptGridsafe.Duplicate();
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
            if (situtation == 1)
            {
                ptGrid = ptgridd.Duplicate();
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


            if (situtation == 4)
            {
                situtation = 0;
            }
            //-------------------------------------------------------------------MAIN LOOP

            for (int p = 0; p < ptGrid.PathCount; p++)
            {


                for (int i = 0; i < ptGrid.Branches[0].Count; i++)
                {
                    if (p % 2 == 0)
                    {
                        int checker = 0;
                        if (p % 4 == 0) { checker = 1; }
                        //----------------------------------VALLEY LINES
                        if (!(p == 0 || p == ptGrid.PathCount - 1) ||(situtation==1 &p==0))
                        {

                            try
                            {
                               mountain.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p][i + 1].Value));
                            }
                            catch { }
                        }
                        ///----------------------------------------------
                        if (i % 2 == checker )
                        {
                            if (situtation != 1 &(p == 0 | p == ptGrid.PathCount - 2))
                            {
                                try
                                {
                                    border.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 1][i + 1].Value));

                                }
                                catch { }
                                try
                                {
                                    border.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 1][i - 1].Value));

                                }
                                catch { }
                                //-----------add conditional value
                                if((i==0 || i == ptGrid[0].Count - 1)&!(situtation==1))
                                {
                                    try
                                    {
                                        border.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 2][i].Value));
                                    }
                                    catch { }
                                }
                                try
                                {
                                    mountain.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 2][i].Value));
                                }
                                catch { }



                            }
 

                                else
                                {

                                    try
                                    {
                                        valley.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 1][i + 1].Value));

                                    }
                                    catch { }
                                    try
                                    {
                                        valley.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 1][i - 1].Value));

                                    }
                                    catch { }
                                if (i == 0 || i == ptGrid[0].Count - 1)
                                {
                                    try
                                    {

                                        if (situtation == 2)
                                        {
                                            mountain.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 2][i].Value));
                                        }
                                        else
                                        {
                                           border.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 2][i].Value));
                                        }
                                    }
                                    catch
                                    {

                                        try
                                        {
                                            //no difference
                                         border.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 1][i].Value));
                                        }
                                        catch { }


                                    }
                                }
                                else
                                {

                                        try
                                        {
                                           mountain.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 2][i].Value));
                                        }
                                        catch
                                        {

                                            try
                                            {
                                                mountain.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 1][i].Value));
                                            }
                                            catch { }


                                        }
                                    }
                                } 

                        }

                        else
                        {
                            if (situtation !=1 &( p == 0 || p == ptGrid.PathCount - 1))
                            {

                                //adding border mountain and valley lines
                                if (p == 0)
                                {
                                    if(i==0 || i==ptGrid[0].Count - 1)
                                    {
                                        if (situtation != 2)
                                        {
                                            try
                                            {
                                                border.Add(new Line(ptGrid.Branches[p + 1][i].Value, ptGrid.Branches[p + 2][i].Value));
                                            }
                                            catch { }
                                        }
                                    }
                                    try
                                    {
                                        mountain.Add(new Line(ptGrid.Branches[p + 1][i].Value, ptGrid.Branches[p + 2][i].Value));
                                    }
                                    catch { }
                                }

                                try
                                {
                                    border.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p - 1][i + 1].Value));

                                }
                                catch { }
                                try
                                {
                                    border.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p - 1][i - 1].Value));

                                }
                                catch { }
                            }

                            //additional line

  
                            else
                            {
                                
                                try
                                {
                                    valley.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p - 1][i + 1].Value));

                                }
                                catch { }
                                try
                                {
                                    valley.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p - 1][i - 1].Value));

                                }
                                catch { }



                                if ((i != ptGrid[0].Count -1 & i != 0))
                                {

                                    try
                                    {
                                        mountain.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 1][i].Value));
                                    }

                                    catch { }
                                    if (p != ptGrid.PathCount - 3 || situtation == 1)
                                    {
                                        try
                                        {
                                            mountain.Add(new Line(ptGrid.Branches[p + 1][i].Value, ptGrid.Branches[p + 2][i].Value));
                                        }
                                        catch { }
                                    }
                                }
                                if(i==0 || i == ptGrid[0].Count - 1)
                                {

                                    if (situtation !=2)
                                    {
                                        try
                                        {
                                            border.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 1][i].Value));
                                        }

                                        catch { }
                                        if (p != ptGrid.PathCount - 3 || situtation == 1)
                                        {
                                            try
                                            {
                                                 border.Add(new Line(ptGrid.Branches[p + 1][i].Value, ptGrid.Branches[p + 2][i].Value));
                                            }
                                            catch { }
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            mountain.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 1][i].Value));
                                        }

                                        catch { }
                                        if (p != ptGrid.PathCount - 3 || situtation == 1)
                                        {
                                            try
                                            {
                                                mountain.Add(new Line(ptGrid.Branches[p + 1][i].Value, ptGrid.Branches[p + 2][i].Value));
                                            }
                                            catch { }
                                        }

                                    }
                                 
                                }
                            }




                            


                            
                        }
                    }


                }
            }
            DA.SetData(0, report);
            DA.SetDataList(1, border);
            DA.SetDataList(2, mountain);
            DA.SetDataList(3, valley);
        }
        public override void DrawViewportWires(IGH_PreviewArgs args)
        {

            args.Display.DrawLines(mountain, System.Drawing.Color.Blue, 2);
            args.Display.DrawLines(valley, System.Drawing.Color.Red, 2);
            args.Display.DrawLines(border, System.Drawing.Color.Black, 2);

        }
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Resource1.glide_reflection_2;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("351f65ad-46a4-4cad-896f-c1905223f86e"); }
        }
    }
}