# FVM-Demo

This Windows Forms/C#/VB/.NET8 application uses the cell-centered finite volume method (FVM) to generate fluid flow simulations by solving the
incompressible Navier-Stokes equations for a 2D lid-cavity problem. 

The application allows grids of different types to be generated (e.g. irregular triangles, equilateral triangles, rectangular) and uses the
same grid-agnostic FVM solver for all grid types. What makes this project unique is a feature that allows grids to be transformed via 2D Conway tiling operations: kis, join, kis+join and trunc.

In the code, boundary conditions are enforced by lining the edges of the cavity with zero-thickness cells, essentially enabling parameters to be set on a 1D cell with a single edge.  

Stable solutions are much easier to find on regular grids (play around with mesh granularity, lid velocity, density and viscosity) than on irregular grids, and also on grids which tend to have consistent cell sizes. In its present state the solution does not account for numerical diffusion perpendicular to face normals, so solutions are difficult to achieve on grids that have cells with non-regular geometries.

The code is well commented, so it can be read easily if you are looking for help with building your own FVM methods. The solution to the pressure equation,
in particular, will be useful as it is not often well covered in online materials.

Linear equations are solved step-by-step using the SIMPLE predictor-corrector method and central differencing. No matrix methods are used, but parallelization is used throughout.

Details of the FVM method and its implementation in this project are documented in the following project file:

[Finite-Volume-Method-Overview.md](https://github.com/Nikkinoodl/FVM-Demo/blob/master/Finite-Volume-Method-Overview.md)

## Running the Application

To get started, open the solution in Visual Studio and run the UI project (press the unfilled arrow button on the menu bar, or select from the menu options):

![FVM-start](https://github.com/user-attachments/assets/abbbd6dd-4494-47f6-8b02-ad10760f9713)

You will be presented with a form that has a display area to the left and an input form on the right hand side. This form is used to construct the mesh or grid on which the FVM calculations are made. The white box on the left hand side represents the cavity in which the fluid flow is being modelled.

To start, we only have three things to input: the cavity height, the cavity width and the type of grid we want to build.

![Fullscreen-start](https://github.com/user-attachments/assets/d665ea31-eb83-4389-834b-d45df6ec990e)

## Building the Grid

To build a grid, simply enter the cavity dimensions (in meters) in the Height and Width boxes and select the base grid type from the dropdown menu. You are presented with an option for
different types of grid options: triangular, equilateral or square. I would suggest starting with an equilateral or square grid.

Go the form section labelled **1.Start Build** and click **[Build Grid]**. For a first try at building a grid, you
should keep the grid simple and regular. The starting grid will be drawn and this usually consists of a single figure--in this case, a triangle.

![Initial-build](https://github.com/user-attachments/assets/6f8ef458-aacf-483b-bab6-09a94dbef6ea)


Now go to the form section labelled **2.Optimize** and click **[Refine]**. The grid will be redrawn with a finer mesh. The other buttons in this area, plus the smoothing cycles input box are used only for
irregular triangle grids.

![Form-start](https://github.com/user-attachments/assets/5717d433-e4c5-4e8c-bcee-79e5556981c6)


The resulting grid will look something like this:

![Refine-1](https://github.com/user-attachments/assets/d8044139-82d0-44c2-958c-b219894d8de8)


Each time you click **[Refine]**, the grid will be redrawn and will appear more granular. You can repeat the **[Refine]** operation as many times as you like, but to start I'd recommend making the grid like the example below. Skip **3.Extended Tiling** for now.

When you are satisfied with the grid, go to **4.Commit** and click **[Finalize]**.  This performs some pre-processing of the grid geometry and prepares for the CFD calculations. Once the grid is finalized, you can proceed with CFD calculations.

NOTE: A finalized grid can be only re-built by clicking on the **[Reset]** button, otherwise you can re-start the grid at anytime by clicking on **[Build]**.

![Refine-3](https://github.com/user-attachments/assets/9f5c97d2-3271-4b35-b9be-400433f77b89)


## Starting the CFD Simulation

When the finalization process is finished, hit the **[CFD]** button and a new screen will open:

![CFD-1](https://github.com/user-attachments/assets/821db714-ce8c-45f0-a19a-aa0a8e3722d4)


You are now ready to perform some computational fluid dynamics. Before you doing anything else, click the **[Precalc]** button. This does some preprocessing which makes the 
subsequent calculations much faster.

For a first run, I recommend keeping the parameters in the boxes unchanged, except for **Calc Time**. If you have kept all the default values unchanged and the grid is the same granularity as shown, you should be able to get the soution to converge. However, I think it is always a good idea to do a trial run first and so I would suggest setting **Calc Time** to, say, 0.1s. The program execution time should be fast (in the order of a few milliseconds).

To perform a calculation simply click the **[Run]** button.

![CFD-2](https://github.com/user-attachments/assets/39c671a8-7ec0-4e17-97b1-73d2c12970e6)


The result should look something like this:

![CFD-3](https://github.com/user-attachments/assets/daacffa6-86a7-48b1-bc47-6ef2bcc1a0da)


From here, you can play around with the parameters at will and run new CFD solutions without the need to rebuild the grid each time. Extending the execution time will often result in a smoother
solution. Sometimes this will cause the CDF solution to "blow up"--that is, it can become numerically unstable and the CFD run will terminate prematurely. You can sometimes find a stable solution with different parameters, but it often indicates that the grid doesn't work for these particular fluid characteristics.

![CDF-4](https://github.com/user-attachments/assets/d7581d38-887e-45b2-827b-9bbc8e5a2f0b)


Clicking the **U**, **V** and **P** radio buttons will display different plots without running the code. **U** shows the horizontal velocities, **V** shows the vertical velocities and **P** shows the pressures in
the cavity. **Test** is reserved for debugging purposes and can be used for special plots if you need to modify the code.

![CFD-5](https://github.com/user-attachments/assets/fabee2f1-06b0-4274-8feb-5b79cd32a377)


## Saving the Plots

Save a plot directly to file by clicking **[Save Plot]**. A dialog box will open up from where you can set the folder and file name.

![Save-Plot](https://github.com/user-attachments/assets/1ba6439a-d650-4314-871b-743e70cf95b1)


## Return to the Grid Building Screen

To refine the grid, simply close the CFD window. As noted earlier, the **[Reset]** button must be clicked before the grid can be rebuilt: it
is not possible to refine an existing grid at this stage.
