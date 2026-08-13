using UnityEngine;

public class OutLineShader
{
	private SpriteRenderer renderer;
	private MaterialPropertyBlock materialPropertyBlock;
	private readonly int shader_sprite_size = Shader.PropertyToID("_Sprite_Size_Vector");
	private Sprite last_sprite;

	public OutLineShader(SpriteRenderer render, MaterialPropertyBlock materialBlock)
	{
		renderer = render;
		materialPropertyBlock = materialBlock;
	}

	public void LateUpdate()
	{
		if (renderer == null || materialPropertyBlock == null) return;
		if (last_sprite != null && last_sprite == renderer.sprite) return;

		Texture2D tex = renderer.sprite.texture;
		if (tex == null || last_sprite == null) return;

		last_sprite = renderer.sprite;
		Vector2 texelSize = new Vector2(tex.width, tex.height);

		renderer.GetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetVector(shader_sprite_size, texelSize);
		renderer.SetPropertyBlock(materialPropertyBlock);
	}
}
