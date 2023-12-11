using DotRecast.Stride.Code.Extensions;
using Stride.Engine;
using Stride.Graphics;
using Stride.Physics;

namespace DotRecast.Stride;
public class MeshReader : SyncScript
{
	public ModelComponent Model { get; set; }

	private int[] _indices;
	private float[] _vertices;

	//https://discord.com/channels/500285081265635328/500292370923913222/905396615190306816
	public override void Start()
	{
		//var test = Model.ColliderShape.Description;
		
		_indices = Model.Model.Meshes[0].GetIndices(Game);
		_vertices = Model.Model.Meshes[0].GetVertices(Game);

		//_vertices[0].Buffer.
	}

	public override void Update()
	{
		
	}
}
