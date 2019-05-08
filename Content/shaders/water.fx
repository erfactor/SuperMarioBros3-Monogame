texture2D ScreenTexture;
sampler TextureSampler = sampler_state
{
   Texture = <ScreenTexture>;
};

// Water
static const float4 waterColor = {64./255., 164./255., 223./255., 1.0};
float time;
float waterLevel;
float4 Water(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
if(texCoord.y + 0.01*sin(10*time+texCoord.x*10)+ 0.01*cos(4*time+texCoord.x*10) > 1.0 - waterLevel){
	 texCoord.x += sin(time+texCoord.x*10)*0.01f;
	 texCoord.y += cos(time+texCoord.y*10)*0.01f; 
	 float4 Color = ScreenTexture.Sample(TextureSampler, texCoord.xy); 
	 Color = lerp(Color, waterColor, 0.6);
	 return Color;
 }
 else return ScreenTexture.Sample(TextureSampler, texCoord.xy); 
}

technique PostProcess
{
  pass P0
 {
  PixelShader = compile ps_4_0 Water();
 }
}