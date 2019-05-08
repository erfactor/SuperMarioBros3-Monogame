texture2D ScreenTexture;
sampler TextureSampler = sampler_state
{
   Texture = <ScreenTexture>;
};

float power;

float2 Distort(float2 p)
{
    float theta  = atan2(p.y, p.x);
    float radius = length(p);
    radius = pow(radius, power);
    p.x = radius * cos(theta);
    p.y = radius * sin(theta);
    return 0.5 * (p + 1.0);
}

// Barrel
const float PI = 3.1415926535;

float4 Barrel(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	float2 xy = 2.0 * texCoord - 1.0;
	float2 uv;
	float d = length(xy);
	if (d < 1.0)
	{
	uv = Distort(xy);
	}
	else
	{
	uv = texCoord;
	}
	return ScreenTexture.Sample(TextureSampler, uv);
}


technique PostProcess
{
  pass P0
 {
  PixelShader = compile ps_4_0 Barrel();
 }
}