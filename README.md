# FVM-Demo

This Windows Forms/C#/VB/.NET 7 application uses the cell-centered finite volume method (FVM) to generate fluid flow simulations by solving the
incompressible Navier-Stokes equations for a lid-cavity problem. 

The application allows grids of different types to be generated (e.g. irregular triangles, equilateral triangles, rectangular) and uses the
same grid-agnostic FVM solver for all grid types. A newly added feature allows grids to be transformed via 2D Conway tiling operations: kis, join, kis+join and trunc.

Stable solutions are much easier to find on regular grids (play around with mesh granularity, lid velocity, density and viscosity) than on irregular grids, and also on grids which tend to have consistent cell sizes.

The code is well commented, so it can be read easily if you are looking for help with building your own FVM methods. The solution to the pressure equation,
in particular, will be useful as it is not often well covered in online materials.

Linear equations are solved step-by-step using the SIMPLE predictor-corrector method and central differencing. No matrix methods are used, but parallelization is used throughout.

The images show sample plots at Reynolds Number 80 on an equilateral triangle grid with kis and join tiling:

![Equilateral Kis Join](https://github.com/Nikkinoodl/FVM-Demo/assets/17559271/3ab80cbf-6973-4ede-828d-1c6fcf10ffec)
![U Equilateral Kis Join 80 Re](https://github.com/Nikkinoodl/FVM-Demo/assets/17559271/3d9c82cc-4c19-4c86-a081-9f69a8b4c248)
![V Equilateral Kis Join 80 Re](https://github.com/Nikkinoodl/FVM-Demo/assets/17559271/886ec398-cd6a-4df8-90db-687f8eb80151)
![P Equilateral Kis Join 80 Re](https://github.com/Nikkinoodl/FVM-Demo/assets/17559271/54cfe3c4-75b9-414b-a0bd-2f89859baae2)
