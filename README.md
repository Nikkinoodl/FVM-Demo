# FVM-Demo

This Windows Forms/C#/VB application uses the cell-centered finite volume method (FVM) to generate fluid flow simulations by solving the
incompressible Navier-Stokes equations. 

This is an extension of earlier projects that developed triangular mesh generation and fluid flow solutions using the finite difference method.

Although it is still a project in development, lid cavity solutions using both triangular and rectangular meshes can be obtained (numerical instability may 
be encountered). The code is well commented, so it can be read easily if you are looking for help with building your own FVM methods.

Linear equations are solved step-by-step using the SIMPLE method. No matrix methods are used, but parallization is used throughout.

Some things are still in development, particularly setting boundary conditions for airfoil sections, so any solutions that include airfoil sections 
do not produce meaningful or convergent solutions at present.
