using Stride.Games;
using Stride.Rendering;
using Stride.Extensions;
using Stride.Graphics;
using Stride.Graphics.Data;

namespace DotRecast.Stride.Code.Extensions;
public static class MeshExtensions
{
	public static int[] GetIndices(this Mesh mesh, IGame game)
	{
		return mesh.Draw.IndexBuffer.Buffer.GetData<int>(game.GraphicsContext.CommandList);
	}

	public static float[] GetVertices(this Mesh mesh, IGame game)
	{
		//var test = mesh.Draw.GetVertexBufferData<VertexPositionNormalTexture>("POSITION");
		//var test2 = mesh.Draw.VertexBuffers[0].Buffer.GetData<byte>(game.GraphicsContext.CommandList);
		var test3 = mesh.Draw.VertexBuffers[0].Buffer.GetSerializationData();
		return mesh.Draw.VertexBuffers[0].Buffer.GetData<float>(game.GraphicsContext.CommandList);
	}
}
