using DotRecast.Stride.Code.Extensions;
using Stride.CommunityToolkit.Rendering.Utilities;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering.Materials.ComputeColors;
using Stride.Rendering.Materials;
using Stride.Rendering;
using System.Collections.Generic;
using DotRecast.Stride.Code.DotRecast;
using Stride.Core.Diagnostics;
using System.Diagnostics;

namespace DotRecast.Stride;
public class MeshReader : StartupScript
{
	public ModelComponent Model { get; set; }

	private List<int> _indices;
	private List<Vector3> _vertices;

	public override void Start()
	{
		Log.ActivateLog(LogMessageType.Debug);
		(_vertices, _indices) = Model.Model.GetMeshVerticesAndIndices(Game);
		for(int i = 0; i < _vertices.Count; i++)
		{
			_vertices[i] *= new Vector3(-Entity.Transform.Scale.X, -Entity.Transform.Scale.Y, Entity.Transform.Scale.Z);
		}
		CreateMesh(_vertices, _indices);

		NavMeshManager navMeshManager = new NavMeshManager(Game, this, SceneSystem, Log);
		var stopWatch = Stopwatch.StartNew();
		navMeshManager.BuildNavMesh();
		stopWatch.Stop();
		Log.Debug(stopWatch.ElapsedMilliseconds.ToString());
	}

	public List<int> GetIndices()
	{ 
		return _indices; 
	}

	public List<Vector3> GetVertices()
	{
		return _vertices;
	}

	private void CreateMesh(List<Vector3> vertices, List<int> indices)
	{
		MeshBuilder meshBuilder = new MeshBuilder();

		meshBuilder.WithPrimitiveType(PrimitiveType.TriangleList);
		meshBuilder.WithIndexType(IndexingType.Int32);

		var position = meshBuilder.WithPosition<Vector3>();
		var color = meshBuilder.WithColor<Color>();

		for (int i = 0; i < vertices.Count; i++)
		{
			meshBuilder.AddVertex();
			meshBuilder.SetElement(position, vertices[i]);
			if (i % 2 == 0)
				meshBuilder.SetElement(color, Color.Red);
			else if (i % 3 == 0)
				meshBuilder.SetElement(color, Color.DarkGreen);
			else
				meshBuilder.SetElement(color, Color.Blue);
		}

		foreach (var index in indices)
		{
			meshBuilder.AddIndex(index);
		}

		var meshDraw = meshBuilder.ToMeshDraw(Game.GraphicsDevice);

		AddMesh(Game.GraphicsDevice, SceneSystem.SceneInstance.RootScene, new Vector3(20, 0, 0), meshDraw);
	}

	Entity AddMesh(GraphicsDevice graphicsDevice, Scene rootScene, Vector3 position, MeshDraw meshDraw)
	{
		var entity = new Entity { Scene = rootScene, Transform = { Position = position } };
		var model = new Model
		{
		new MaterialInstance
		{
			Material = Material.New(graphicsDevice, new MaterialDescriptor
			{
				Attributes = new MaterialAttributes
				{
					DiffuseModel = new MaterialDiffuseLambertModelFeature(),
					Diffuse = new MaterialDiffuseMapFeature
					{
						DiffuseMap = new ComputeVertexStreamColor()
					},
				}
			})
		},
		new Mesh
		{
			Draw = meshDraw,
			MaterialIndex = 0
		}
		};
		entity.Add(new ModelComponent { Model = model });
		return entity;
	}
}
