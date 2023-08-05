# FVM-Demo

This Windows Forms/C#/VB/.NET 7 application uses the cell-centered finite volume method (FVM) to generate fluid flow simulations by solving the
incompressible Navier-Stokes equations for a lid-cavity problem. 

This application allows grids of different types to be generated (e.g. irregular triangles, equilateral triangles, rectangular) and uses the
same grid-agnostic FVM solver for all grid types. Stable solutions are much easier to find on regular grids (play around with mesh granularity, lid velocity,
density and viscosity) than on irregular grids.

The code is well commented, so it can be read easily if you are looking for help with building your own FVM methods. The solution to the pressure equation,
in particular, will be useful as it is not often well covered in online materials.

Linear equations are solved step-by-step using the SIMPLE predictor-corrector method. No matrix methods are used, but parallelization is used throughout.

Sample plots at Reynolds Number of 13 on a rectangular grid:

![U Velocities in Lid Cavity](https://github.com/Nikkinoodl/FVM-Demo/assets/17559271/55ee06b7-09a9-4526-bea6-4aaf07aa5d4a)
![V Velocities in Lid Cavity](https://github.com/Nikkinoodl/FVM-Demo/assets/17559271/1dbb9406-d9cd-4190-ac24-79baf3a1748b)
![Pressure P in Lid Cavity](https://github.com/Nikkinoodl/FVM-Demo/assets/17559271/67d74387-dee0-4cfe-9a21-8a02535ea52d)
