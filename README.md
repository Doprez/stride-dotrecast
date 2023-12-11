# stride-dotrecast
Integrating Dotrcast with Stride

Im having a hard time with the coordinate system (I think due to the left vs right handed design but not too sure). Either way, I have a very beginner demo of using the new DotRecast library to generate a mesh as you can see with the cubes at each vertex of the mesh.

I had to invert the X and Y axis of the mesh before importing into the library as you can see with the rainbow mesh I am using for debugging and then invert the navmesh on the X and Y axis to get this final result.

![recast-example](https://github.com/Doprez/stride-dotrecast/assets/73259914/8071b988-463b-4b36-b3ba-79184471b93b)
