namespace PixelEngine
{
	public class Shader
	{
		public ShaderFunc Calculate { get; private set; }
		
		public Shader(ShaderFunc calculate) => Calculate = calculate;
	}

	public delegate Color ShaderFunc(int x, int y, Color prev, Color cur);
 }
