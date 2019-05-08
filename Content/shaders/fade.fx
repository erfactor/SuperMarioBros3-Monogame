texture2D ScreenTexture;
sampler TextureSampler = sampler_state
{
   Texture = <ScreenTexture>;
};

// Fade
float fade;//<0,1>
float4 Fade(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
 float4 Color = ScreenTexture.Sample(TextureSampler, texCoord.xy)*(1-fade); 
 Color.a = 1;
 return Color;
}

technique PostProcess
{
 pass P0
 {
  PixelShader = compile ps_4_0 Fade();
 }
}