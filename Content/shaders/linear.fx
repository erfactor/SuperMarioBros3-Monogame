texture2D ScreenTexture;
sampler TextureSampler = sampler_state
{
   Texture = <ScreenTexture>;
};

float setx;
float sety;
bool right;
static const float4 black = {0., 0., 0., 1.0};
float4 Linear(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
if(texCoord.y > sety){ return black;}
else if (texCoord.y <=sety && texCoord.y >= sety -0.1f){
	if(right)
	{
	 if (texCoord.x < setx) return ScreenTexture.Sample(TextureSampler, texCoord.xy); else return black;
	}
	else 
	{
	if (texCoord.x > setx) return ScreenTexture.Sample(TextureSampler, texCoord.xy); else return black;
	}
}
 else return ScreenTexture.Sample(TextureSampler, texCoord.xy); 
}


technique PostProcess
{
 pass P0
 {
  PixelShader = compile ps_4_0 Linear();
 }
}