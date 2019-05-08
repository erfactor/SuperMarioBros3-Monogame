texture2D ScreenTexture;
sampler TextureSampler = sampler_state
{
   Texture = <ScreenTexture>;
};

// Circle
float circleSize;
float2 circlePosition;
float2 coord;
float2 tempCoord = { 0.,0. };
static const float4 black = {0., 0., 0., 1.0};
float4 Circle(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
tempCoord.x = ((texCoord.x - circlePosition.x) / 0.6) + circlePosition.x;
tempCoord.y = texCoord.y;
if(distance(circlePosition, tempCoord) > circleSize){
	 return black;
 }
 else return ScreenTexture.Sample(TextureSampler, texCoord.xy); 
}

technique PostProcess
{
 pass P0
 {
  PixelShader = compile ps_4_0 Circle();
 }
}