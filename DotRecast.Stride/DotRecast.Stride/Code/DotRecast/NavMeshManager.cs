using DotRecast.Detour;
using DotRecast.Recast.Toolset;
using DotRecast.Recast.Toolset.Builder;
using DotRecast.Recast.Toolset.Geom;
using Stride.CommunityToolkit.Rendering.Utilities;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering.Materials.ComputeColors;
using Stride.Rendering.Materials;
using Stride.Rendering;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Stride.Games;
using Stride.Core.Mathematics;
using System;
using Stride.Core.Diagnostics;

namespace DotRecast.Stride.Code.DotRecast;
public class NavMeshManager
{
	private MeshReader _meshReader { get; set; }
	private IGame _game;
	private SceneSystem _sceneSystem;
	private Logger _log;

	public string FilePath = "C:\\dev\\GitControlledProjects\\DotRecast\\resources\\bridge.obj";

	public NavMeshManager(IGame game, MeshReader meshReader, SceneSystem sceneSystem, Logger log)
	{
		_meshReader = meshReader;
		_game = game;
		_sceneSystem = sceneSystem;
		_log = log;
	}

	public void BuildNavMesh()
	{
		var indices = _meshReader.GetIndices();
		List<float> verts = new List<float>();
		foreach (var v in _meshReader.GetVertices())
		{
			verts.Add(v.X);
			verts.Add(v.Y);
			verts.Add(v.Z);
		}
		//DemoInputGeomProvider geom = DemoInputGeomProvider.LoadFile(FilePath);
		DemoInputGeomProvider geom = new(verts, indices);

		RcNavMeshBuildSettings settings = new RcNavMeshBuildSettings();
		settings.vertsPerPoly = 12;
		SoloNavMeshBuilder tileNavMeshBuilder = new();
		var buildResult = tileNavMeshBuilder.Build(geom, settings);

		var telemetries = buildResult.RecastBuilderResults
			.Select(x => x.GetTelemetry())
			.SelectMany(x => x.ToList())
			.GroupBy(x => x.Key)
			.ToImmutableSortedDictionary(x => x.Key, x => x.Sum(y => y.Millis));

		foreach (var (key, millis) in telemetries)
		{
			var test = ($"{key}: {millis} ms");
			Console.WriteLine(test);
			_log.Debug(test);
		}

		var navMesh = buildResult.NavMesh;
		var tileCount = navMesh.GetTileCount();
		var tiles = new List<DtMeshTile>();
		for (int i = 0; i < tileCount; i++)
		{
			tiles.Add(navMesh.GetTile(i));
		}

		List<Vector3> strideVerts = new List<Vector3>();
		List<int> strideIndices = new();

		for(int i = 0; i < tiles.Count; i++)
		{
			for(int j = 0;j <  tiles[i].data.verts.Count();)
			{
				strideVerts.Add(
					new Vector3(-tiles[i].data.verts[j++], -tiles[i].data.verts[j++], tiles[i].data.verts[j++])
					);
			}
		}

		// This doesnt seem to work at all
		for (int i = 0; i < tiles.Count; i++)
		{
			//for (int j = 0; j < tiles[i].data.polys.Count(); j++)
			//{
				strideIndices.AddRange(tiles[i].data.detailTris);
			//}
		}
		//CreateMesh(strideVerts);
		SpawPrefabAtVerts(strideVerts);
	}

	private void SpawPrefabAtVerts(List<Vector3> verts)
	{
		// Make sure the cube is a root asset or else this wont load
		var cube = _game.Content.Load<Model>("Cube");

		foreach (var vert in verts)
		{
			AddMesh(_game.GraphicsDevice, _sceneSystem.SceneInstance.RootScene, vert, cube.Meshes[0].Draw);
		}
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
			if(index < vertices.Count && index > -1)
				meshBuilder.AddIndex(index);
		}

		var meshDraw = meshBuilder.ToMeshDraw(_game.GraphicsDevice);

		AddMesh(_game.GraphicsDevice, _sceneSystem.SceneInstance.RootScene, new Vector3(0, 1, 0), meshDraw);
	}

	private void CreateMesh(List<Vector3> vertices)
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

			meshBuilder.AddIndex(i);
		}

		var meshDraw = meshBuilder.ToMeshDraw(_game.GraphicsDevice);

		AddMesh(_game.GraphicsDevice, _sceneSystem.SceneInstance.RootScene, new Vector3(0, 0, 0), meshDraw);
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
