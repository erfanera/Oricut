using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using System.IO;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.InteropServices;


using Rhino.Collections;
using GH_IO;
using GH_IO.Serialization;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace Oricut
{
   
    public class Chevron_pattern : GH_Component
    {

        
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Chevron_pattern()
          : base("Chevron", "Chevron",
              "draw chevron pattern from a grid",
              "Oricut", "Patterns")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        /// 

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Grid", "G", "grid input", GH_ParamAccess.tree);
            pManager.AddBooleanParameter("Flip", "F", "True to flip the grid", GH_ParamAccess.item , false);
            pManager.AddBooleanParameter("Radial", "R", "True to ", GH_ParamAccess.item , false);
           
            

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Report", "Re", "guid", GH_ParamAccess.item );
            pManager.AddCurveParameter("Border", "B", "Border Lines", GH_ParamAccess.list);
            pManager.AddCurveParameter("Mountain", "M", "Mountain lines", GH_ParamAccess.list);
            pManager.AddCurveParameter("Valley", "V", "Valley Lines", GH_ParamAccess.list);
            
            
            

        }

        //Global variables 
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
            
            if (!DA.GetDataTree<Grasshopper.Kernel.Types.GH_Point>(0, out ptGrid)) return;


            ptGridsafe = ptGrid.Duplicate();



            Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Point> tr_out = ptGrid;
            var test = ptGrid.Branches[0];
            var situtation = 0;                //if there is no change just a simple grid
            string report = "every thing works great";
            

            //--------------------------------------------------------------------First two functions


            if (flip) { situtation = 4;  }

                if (radial)
            {
                ptGrid = ptGridsafe.Duplicate();
                if (flip) { situtation = 1;  }
                if (!flip) { situtation = 2; }
                for (int kk = 0; kk < ptGrid.PathCount; kk++)
                {

                    ptGrid[kk].Add(ptGrid[kk][0]);
                    if (situtation == 1)
                    {
                        ptGrid[kk].Add(ptGrid[kk][1]);
                       
                    }

                }
                ptgridd = ptGrid.Duplicate();
                
            }
            if (ptGrid.Branches[0].Count % 2 == 0 & (situtation ==1 || situtation==2))
            {
                report = "try to supply even number for both of columns and rows ";
            }


                if (situtation ==4) {
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
                situtation =0;
            }
            //-------------------------------------------------------------------MAIN LOOP

            for (int p = 0; p < ptGrid.PathCount; p++)
                {


                    for (int i = 0; i < ptGrid.Branches[0].Count; i++)
                    {
                        var val = 1;
                        if (i % 2 == 0) { val = -1; }



                        try
                        {

                            //generate borders

                            if (i == 0 || i == ptGrid.Branches[0].Count - 1)
                            {
                                if (situtation == 2)
                                {
                                    if (p % 2 == 0 & i == 0)
                                    {
                                        if (p != ptGrid.PathCount - 1)
                                        {


                                            valley.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + val][i].Value));
                                        }

                                    }
                                    else
                                    {

                                    if (p != ptGrid.PathCount - 1)
                                    {
                                        if (p == ptGrid.PathCount - 2 & ptGrid.Branches[0].Count%2 == 0)
                                        {
                                            border.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + val][i].Value));
                                        }
                                        else
                                        {


                                           mountain.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + val][i].Value));
                                        }
                                    }
                                    }
                                }
                                if (p == 0 || p == ptGrid.Branches.Count - 1)
                                {
                                    //border.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + val][i].Value));
                                }
                                else
                                {
                                    if (situtation != 2)
                                    {
                                        //don't forget to turn it on!
                                        border.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + val][i].Value));
                                    }
                                }
                            }
                            else
                            {
                                if (p == 0 || p == ptGrid.Branches.Count - 1)
                                {

                                }
                                else
                                {
                                if (i % 2 == 0)
                                {
                                    if (p % 2 == 0)
                                    {

                                        mountain.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + val][i].Value));

                                    }
                                    else
                                    {
                                        valley.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + val][i].Value));

                                    }
                                }
                                else
                                {
                                    if (p % 2 == 0)
                                    {

                                        valley.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + val][i].Value));

                                    }
                                    else
                                    {
                                        mountain.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + val][i].Value));

                                    }


                                }

                                }
                            }



                        }

                        catch { }



                        if (i % 2 == 0)
                        {
                            //----------------------------------------------------------border line
                            if (!(situtation == 1) & (p == 0 || p == ptGrid.Branches.Count - 2)) //----for square and radial two sit
                            {

                                if (p % 2 == 0 || p == ptGrid.Branches.Count - 2)
                                {

                                    try
                                    {
                                        if ((situtation == 2 & (!(p == 0))) || situtation == 0)
                                        {
                                            border.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 1][i + 1].Value));
                                        }

                                    }
                                    catch { }
                                    try
                                    {
                                        if (((situtation == 2 & !(p == 0))) || situtation == 0)
                                        {
                                            border.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 1][i - 1].Value));
                                        }

                                    }
                                    catch { }
                                }
                                else
                                {
                                    if (situtation != 2)
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
                                    }
                                }

                            }
                            //-----------------------chevi path----------------------------------
                            if (p % 2 == 0 & !(p == ptGrid.Branches.Count - 2))
                            {
                                if (situtation == 0 & !(p == 0))

                                {
                                    if ((p != ptGrid.Branches.Count - 1) & p != 0)
                                    {
                                        try
                                        {

                                            mountain.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 1][i + 1].Value));

                                        }
                                        catch { }
                                        try
                                        {


                                           mountain.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 1][i - 1].Value));

                                        }
                                        catch { }
                                    }
                                }
                                else
                                {
                                    if ((p != ptGrid.Branches.Count - 1))
                                    {
                                        try
                                        {

                                           mountain.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 1][i + 1].Value));

                                        }
                                        catch { }
                                        try
                                        {

                                            mountain.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 1][i - 1].Value));

                                        }
                                        catch { }

                                    }
                                }
                            }
                            else
                            {
                                if (situtation == 0 & !(p == 0))
                                {
                                    try
                                    {
                                        if ((p != ptGrid.Branches.Count - 2))
                                        {
                                            valley.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 1][i + 1].Value));
                                        }
                                    }
                                    catch { }
                                    try
                                    {
                                        if ((p != ptGrid.Branches.Count - 2))
                                        {
                                            valley.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 1][i - 1].Value));
                                        }
                                    }
                                    catch { }
                                }
                                else
                                {
                                    try
                                    {
                                        if ((p != ptGrid.Branches.Count - 2))
                                        {
                                            valley.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 1][i + 1].Value));
                                        }
                                    }
                                    catch { }
                                    try
                                    {
                                        if ((p != ptGrid.Branches.Count - 2))
                                        {
                                            valley.Add(new Line(ptGrid.Branches[p][i].Value, ptGrid.Branches[p + 1][i - 1].Value));
                                        }
                                    }
                                    catch { }
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
            
            args.Display.DrawLines(mountain , System.Drawing.Color.Blue, 2);
            args.Display.DrawLines(valley, System.Drawing.Color.Red, 2);
            args.Display.DrawLines(border, System.Drawing.Color.Black, 2);

        }








        //List<IGH_PreviewData> _items;
        //public override void DrawViewportWires(IGH_PreviewArgs args)
        //{

        //    if (_items == null) return;
        //    foreach (var item in _items)
        //    {
        //        GH_PreviewWireArgs args1 = new GH_PreviewWireArgs(args.Viewport, args.Display, Color.ForestGreen, args.DefaultCurveThickness);
        //        if (!(item is GH_Mesh) || CentralSettings.PreviewMeshEdges)
        //            item.DrawViewportWires(args1);
        //    }
        //}








        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Resource1.chevron_2;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        /// 
        public override Guid ComponentGuid
        {
            get { return new Guid("6efc1ac8-38c5-4a85-9663-e617a8c91c6a"); }
        }
        List<Point3d> yellowWiresPts = new List<Point3d>();

        //public override BoundingBox ClippingBox
        //{
        //    get
        //    {
        //        return new BoundingBox(new Point3d(-100, -100, -100), new Point3d(100, 100, 100));
        //    }
        //}

        ////Draw all wires and points in this method.
        //public override void DrawViewportWires(IGH_PreviewArgs args)
        //{
        //    // Obviously I think the problem is coming from here but don't know where exactely since there is just one line ^^
        //    args.Display.DrawPolyline(yellowWiresPts, Color.Yellow);
        //}

        //public override void BeforeRunScript()
        //{
        //    yellowWiresPts.Clear();
        //}
    }

}

