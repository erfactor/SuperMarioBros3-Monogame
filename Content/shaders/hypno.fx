texture2D ScreenTexture;
sampler TextureSampler = sampler_state
{
	Texture = <ScreenTexture>;
};

float a;
float b;
float c;
float d;
int x;
float fill;
static const float4 black = { 0., 0., 0., 1.0 };
float4 Hypno(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
if (texCoord.y < a) { return black; }
if (texCoord.y > c) { return black; }
if (texCoord.x > b) { return black; }
if (texCoord.x < d) { return black; }
if (x == 1)
	{
	if (texCoord.y < a + 0.1 && texCoord.x < fill) return black;
	}
else if (x == 2)
	{
	if (texCoord.x > b - 0.1 && texCoord.y < fill) return black;
	}
else if (x == 3)
	{
	if (texCoord.y > c - 0.1 && texCoord.x > fill) return black;
	}
else if (x == 4) 
	{
	if (texCoord.x < d + 0.1 && texCoord.y > fill) return black;
	}

return ScreenTexture.Sample(TextureSampler, texCoord.xy);
}


technique PostProcess
{
	pass P0
	{
		PixelShader = compile ps_4_0 Hypno();
	}
}